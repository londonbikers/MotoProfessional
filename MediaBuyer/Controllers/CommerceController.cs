using System;
using System.Linq;
using System.Text;
using System.Configuration;
using MotoProfessional.Exceptions;
using MotoProfessional.Models;
using GCheckout.Checkout;
using GCheckout.Util;
using MotoProfessional.Models.Interfaces;
using MPN.Framework.Caching;
using MPN.Framework.Communication;

namespace MotoProfessional.Controllers
{
    public class CommerceController
    {
		#region members
		private ILatestOrders _latestOrders;
		#endregion

		#region accessors
		public ILatestOrders LatestOrders
		{
			get
			{
                if (_latestOrders == null)
                    _latestOrders = new LatestOrders(100);

			    return _latestOrders;
			}
			internal set
			{
				_latestOrders = value;
			}
		}
		#endregion

		#region constructors
		internal CommerceController()
        {
        }
        #endregion

        #region basket methods
		/// <summary>
		/// Retrieves the current basket for a particular member.
		/// </summary>
		internal IBasket GetCurrentBasket(IMember member)
		{
			var db = new MotoProfessionalDataContext();
			var dbBasket = (from b in db.DbBaskets
								 where b.CustomerUID == (Guid)member.MembershipUser.ProviderUserKey &&
								 b.Status == (byte)BasketStatus.Current
								 select b).FirstOrDefault();

			if (dbBasket == null)
			{
				// new basket.
				var b = new Basket(ClassMode.New) {Member = member};
				return b;
			}

			// retrieve existing basket.
			var basket = new Basket(ClassMode.Existing)
         	{
         		Id = dbBasket.ID,
         		Name = dbBasket.Name,
         		Status = BasketStatus.Current,
         		Created = dbBasket.Created,
         		LastUpdated = dbBasket.LastUpdated,
         		Member = member
         	};

			if (dbBasket.OrderID.HasValue)
			{
				// attempt to find the order.
				basket.Order = member.Orders.FirstOrDefault(o => o.Id == dbBasket.OrderID);
			}

            foreach (var dbItem in dbBasket.DbBasketItems)
            {
                var item = new BasketItem(ClassMode.Existing);
                var photoProduct = new PhotoProduct
               	{
               		License = Controller.Instance.LicenseController.GetLicense(dbItem.LicenseID),
               		Photo = Controller.Instance.PhotoController.GetPhoto(dbItem.PhotoID)
               	};

            	item.Id = dbItem.ID;
                item.Created = dbItem.Created;
                item.PhotoProduct = photoProduct;

                // don't use Basket.AddPhotoProduct as that'll trigger persistence.
                basket.Items.Add(item);
            }

			return basket;
		}

		/// <summary>
		/// Persists any changes to a Basket object.
		/// </summary>
		internal void UpdateBasket(IBasket basket)
		{
			if (basket == null || !basket.IsValid())
			{
				Controller.Instance.Logger.LogWarning("UpdateBasket() - Null or invalid Basket passed in.");
				return;
			}

			var db = new MotoProfessionalDataContext();
			basket.LastUpdated = DateTime.Now;

			var dbBasket = basket.IsPersisted ? db.DbBaskets.Single(b => b.ID == basket.Id) : new DbBasket();
			dbBasket.CustomerUID = (Guid)basket.Member.MembershipUser.ProviderUserKey;
			dbBasket.Name = (!string.IsNullOrEmpty(basket.Name)) ? basket.Name.Trim() : null;
			dbBasket.Status = (byte)basket.Status;
			dbBasket.Created = basket.Created;
			dbBasket.LastUpdated = basket.LastUpdated;

            if (basket.Order != null)
                dbBasket.OrderID = basket.Order.Id;

			// new.
			if (!basket.IsPersisted)
				db.DbBaskets.InsertOnSubmit(dbBasket);

			try
			{
				db.SubmitChanges();
			}
			catch (Exception ex)
			{
				Controller.Instance.Logger.LogError("UpdateBasket() - Main update failed.", ex);
				return;
			}

			if (basket.IsPersisted) return;
			basket.Id = dbBasket.ID;
			basket.IsPersisted = true;
		}

        /// <summary>
        /// Permenantly deletes a basket from the application/database.
        /// </summary>
        /// <returns>A bool indicating whether or not the delete succeeded.</returns>
        internal bool DeleteBasket(IBasket basket)
        {
            if (basket == null || !basket.IsPersisted)
            {
                Controller.Instance.Logger.LogWarning("DeleteBasket() - Null or unpersisted Basket passed in.");
                return false;
            }

            var db = new MotoProfessionalDataContext();
            var dbBasket = db.DbBaskets.SingleOrDefault(b => b.ID == basket.Id);
            if (dbBasket == null)
            {
                Controller.Instance.Logger.LogWarning("DeleteBasket() - Basket not found in database.");
                return false;
            }
            
            db.DbBasketItems.DeleteAllOnSubmit(dbBasket.DbBasketItems);
            db.DbBaskets.DeleteOnSubmit(dbBasket);

            try
            {
                db.SubmitChanges();
            }
            catch (Exception ex)
            {
                Controller.Instance.Logger.LogError("DeleteBasket() - Main persist failed.", ex);
                return false;
            }

            return true;
        }
        #endregion

        #region order methods
		/// <summary>
		/// Initiates the checkout process for a point-of-sale customer. Constructs an order and submits the request to Google Checkout. Will return
		/// a response object that will have a redirect URL or error code in.
		/// </summary>
		/// <param name="member">The Member who has a basket to use for the transaction.</param>
		/// <param name="request">The GCheckoutResponse object to use for the transaction. Can come from the Google Button web-control.</param>
		public GCheckoutResponse BeginPosTransaction(IMember member, CheckoutShoppingCartRequest request)
		{
			// all of the following examples are explained in the following document
			// http://code.google.com/apis/checkout/developer/Google_Checkout_Digital_Delivery.html

			var o = PrepareOrderForCustomer(member, false);

			// create a generic transaction just to record the client ip-address.
			var ot = o.NewOrderTransaction();
			ot.ClientIpAddress = member.IpAddress;
			ot.Type = OrderTransactionType.Generic;
			o.AddTransaction(ot);

			// tag on our order-id to the request so we can track it when it comes back via the Google Notification API.
			request.MerchantPrivateData = o.Id.ToString();

			foreach (var bi in member.Basket.Items)
			{
				// only photo-products are supported for now.
				if (bi.PhotoProduct == null) continue;
				var productName = "Digital Photo: " + bi.PhotoProduct.Photo.Name;
				var di = new DigitalItem(new Uri(Controller.Instance.PeripheralController.GetClientUrl(ClientUrlPage.OrderPage, o)), productName);
				request.AddItem(productName, bi.PhotoProduct.Photo.Comment, bi.PhotoProduct.Rate, 1, di);
			}

			var response = request.Send();
			if (!response.IsGood)
			{
				// log any error.
				var builder = new StringBuilder();
				builder.Append("Google Checkout post failed!\n");
				builder.Append("response.ResponseXml = " + response.ResponseXml + "\n");
				builder.Append("response.RedirectUrl = " + response.RedirectUrl + "\n");
				builder.Append("response.IsGood = " + response.IsGood + "\n");
				builder.Append("response.ErrorMessage = " + response.ErrorMessage + "\n");
				Controller.Instance.Logger.LogError(builder.ToString());
			}

			return response;
		}

		/// <summary>
		/// Builds the order from a members basket and executes it for non point-of-sale customers, i.e. those with invoicable accounts.
		/// </summary>
		/// <param name="member">The member who has a basket to use for the transaction.</param>
		/// <returns>The completed Order object.</returns>
		public IOrder ExecuteNonPosTransaction(IMember member)
		{
			if (member == null)
				throw new ArgumentException("Member cannot be null.");

			if (member.Company.ChargeMethod == ChargeMethod.PointOfSale)
				throw new TransactionException("Your company cannot make this type of purchase, sorry!");

			// create the order.
			var o = PrepareOrderForCustomer(member, true);
			
			// create a generic transaction to record the client ip-address.
			var ot = o.NewOrderTransaction();
			ot.Type = OrderTransactionType.Generic;
			ot.ClientIpAddress = member.IpAddress;
			ot.Member = member;
			o.AddTransaction(ot);

			if (member.Company.ChargeMethod == ChargeMethod.NoCharge)
			{
				o.ChargeStatus = ChargeStatus.Complete;
				UpdateOrder(o, true);
			}

			// orders for invoicable customers sit at outstanding status until account settled.
			// order can be downloaded still, as per biz rules.
			
			return o;
		}

		public IOrder GetOrder(int orderId)
        {
            var order = CacheManager.RetrieveItem(Order.DomainObjectType.ToString(), orderId, string.Empty) as IOrder;
            if (order == null)
            {
                var db = new MotoProfessionalDataContext();
                var dbOrder = db.DbOrders.SingleOrDefault(o => o.ID == orderId);
                if (dbOrder == null)
                    return null;

                order = BuildOrderObject(dbOrder);
                CacheManager.AddItem(order, order.DomainObject.ToString(), order.Id, string.Empty);
            }

            return order;
        }

        /// <summary>
        /// Retrieves an order by looking for a matching GoogleOrderNumber transaction reference. Used by the Google Notification API.
        /// </summary>>
        public IOrder GetOrder(string googleOrderNumber)
        {
            // lookup our own order-id via the GoogleOrderNumber.
            var db = new MotoProfessionalDataContext();
            var dbOrderTransaction = db.DbOrderTransactions.FirstOrDefault(ot => ot.GC_OrderNumber == googleOrderNumber);
            return dbOrderTransaction == null ? null : GetOrder(dbOrderTransaction.OrderID);
        }

		/// <summary>
		/// Persists any changes to a new or existing Order object.
		/// </summary>
		public void UpdateOrder(IOrder order, bool fullUpdate)
		{
            if (order == null)
            {
                Controller.Instance.Logger.LogWarning("UpdateOrder() - Null Order passed in.");
                return;
            }

			var justBeenCompleted = false;
            var db = new MotoProfessionalDataContext();
            DbOrder dbOrder;
            order.LastUpdated = DateTime.Now;

			if (order.IsPersisted)
			{
				dbOrder = db.DbOrders.Single(o => o.ID == order.Id);
				if ((ChargeStatus)dbOrder.ChargeStatus == ChargeStatus.Outstanding && order.ChargeStatus == ChargeStatus.Complete)
					justBeenCompleted = true;
			}
			else
			{
				dbOrder = new DbOrder();
			}

            dbOrder.ChargeAmount = order.ChargeAmount;
            dbOrder.ChargeMethod = (byte)order.ChargeMethod;
            dbOrder.ChargeStatus = (byte)order.ChargeStatus;
			dbOrder.CustomerUID = order.Customer.Uid;
            dbOrder.LastUpdated = order.LastUpdated;
            dbOrder.Created = order.Created;

            if (!order.IsPersisted)
                db.DbOrders.InsertOnSubmit(dbOrder);

            try
            {
                db.SubmitChanges();
            }
            catch (Exception ex)
            {
                Controller.Instance.Logger.LogError("UpdateOrder() - Main update failed.", ex);
                return;
            }

            if (!order.IsPersisted)
            {
                order.Id = dbOrder.ID;
                order.IsPersisted = true;
                LatestOrders = null;
				CacheManager.AddItem(order, order.DomainObject.ToString(), order.Id, string.Empty);
            }

            // persist order items.
            foreach (var oi in order.Items)
                UpdateOrderItem(oi);

			// digital-goods to be created?
			if (fullUpdate && order.ChargeStatus == ChargeStatus.Complete && !order.HasDigitalGoods)
				Controller.Instance.DigitalGoodController.CreateDigitalGoodsAsync(order);

			// notify the customer of the order completion.
			if (!justBeenCompleted) return;
			SendOrderConfirmationEmail(order);

			// update statistics.
			foreach (var oi in order.Items)
				oi.PhotoProduct.Photo.Partner.Statistics.Reset();
		}

        /// <summary>
        /// Persists any changes to a new or existing Order object.
        /// </summary>
        public void UpdateOrder(IOrder order)
        {
            UpdateOrder(order, false);
        }

        /// <summary>
        /// Persists any changes made to a new or existing OrderTransationObject.
        /// </summary>
        internal void UpdateOrderTransaction(IOrderTransaction orderTransaction)
        {
            var db = new MotoProfessionalDataContext();

            var dbTransaction = orderTransaction.IsPersisted ? db.DbOrderTransactions.Single(rOt => rOt.ID == orderTransaction.Id) : new DbOrderTransaction();
            dbTransaction.Created = orderTransaction.Created;
            dbTransaction.Type = (byte)orderTransaction.Type;
            dbTransaction.OrderID = orderTransaction.Order.Id;

			if (orderTransaction.Member != null)
				dbTransaction.MemberUID = orderTransaction.Member.Uid;

			if (!string.IsNullOrEmpty(orderTransaction.ClientIpAddress))
				dbTransaction.ClientIPAddress = orderTransaction.ClientIpAddress;
			else
				dbTransaction.ClientIPAddress = null;

            // if this is an existing transaction, let's reset the other properties.
            dbTransaction.GC_ChargebackAmount = null;
            dbTransaction.GC_ChargedAmount = null;
            dbTransaction.GC_NewFinanceState = null;
            dbTransaction.GC_NewFulfillmentState = null;
            dbTransaction.GC_OrderNumber = null;
            dbTransaction.GC_PrevFinanceState = null;
            dbTransaction.GC_PrevFulfillmentState = null;
            dbTransaction.GC_RefundedAmount = null;
			dbTransaction.Operation = null;

            switch (orderTransaction.Type)
            {
                case OrderTransactionType.GC_ChargebackAmount:
                    dbTransaction.GC_ChargebackAmount = orderTransaction.GcChargebackAmount.ChargebackAmount;
                    dbTransaction.GC_OrderNumber = orderTransaction.GoogleOrderNumber;
                    break;
                case OrderTransactionType.GC_NewOrder:
                    dbTransaction.GC_OrderNumber = orderTransaction.GoogleOrderNumber;
                    break;
                case OrderTransactionType.GC_OrderStateChange:
                    dbTransaction.GC_OrderNumber = orderTransaction.GoogleOrderNumber;
                    dbTransaction.GC_NewFinanceState = orderTransaction.GcOrderStateChange.NewFinanceState;
                    dbTransaction.GC_NewFulfillmentState = orderTransaction.GcOrderStateChange.NewFulfillmentState;
                    dbTransaction.GC_PrevFinanceState = orderTransaction.GcOrderStateChange.PrevFinanceState;
                    dbTransaction.GC_PrevFulfillmentState = orderTransaction.GcOrderStateChange.PrevFulfillmentState;
                    break;
                case OrderTransactionType.GC_RefundAmount:
                    dbTransaction.GC_OrderNumber = orderTransaction.GoogleOrderNumber;
                    dbTransaction.GC_RefundedAmount = orderTransaction.GcRefundAmount.RefundedAmount;
                    break;
                case OrderTransactionType.GC_RiskInformation:
                    dbTransaction.GC_OrderNumber = orderTransaction.GoogleOrderNumber;
                    break;
				case OrderTransactionType.Generic:
					dbTransaction.Operation = orderTransaction.Operation;
					break;
            }

            if (!orderTransaction.IsPersisted)
                db.DbOrderTransactions.InsertOnSubmit(dbTransaction);

            try
            {
                db.SubmitChanges();
            }
            catch (Exception ex)
            {
                Controller.Instance.Logger.LogError("UpdateOrderTransaction() - Main update failed.", ex);
                return;
            }

        	if (orderTransaction.IsPersisted) return;
        	orderTransaction.Id = dbTransaction.ID;
        	orderTransaction.IsPersisted = true;
        }

        internal IOrder BuildOrderObject(DbOrder dbOrder)
        {
            var o = new Order(ClassMode.Existing)
        	{
        		Id = dbOrder.ID,
        		Customer = Controller.Instance.MemberController.GetMember(dbOrder.CustomerUID),
        		ChargeMethod = (ChargeMethod) dbOrder.ChargeMethod,
        		ChargeAmount = dbOrder.ChargeAmount,
        		ChargeStatus = (ChargeStatus) dbOrder.ChargeStatus,
        		Created = dbOrder.Created,
        		LastUpdated = dbOrder.LastUpdated
        	};

        	// is there a digital-good?
            if (dbOrder.DbDigitalGoods.Count > 0)
            {
                o.MasterDigitalGood = Controller.Instance.DigitalGoodController.BuildDigitalGoodObject(dbOrder.DbDigitalGoods[0]);
                o.MasterDigitalGood.Order = o;
            }

            return o;
        }

        internal IOrderItem BuildOrderItemObject(IOrder order, DbOrderItem dbOrderItem)
        {
            var oi = new OrderItem(ClassMode.Existing)
         	{
         		Id = dbOrderItem.ID,
         		Order = order,
         		Status = (OrderItemStatus) dbOrderItem.Status,
         		SaleAmount = dbOrderItem.SaleRate,
         		ProductType = ProductType.PhotoProduct,
         		PhotoProduct = new PhotoProduct(Controller.Instance.PhotoController.GetPhoto(dbOrderItem.PhotoID), Controller.Instance.LicenseController.GetLicense(dbOrderItem.LicenseID))
         	};

        	// no different types of products for now, so just assume it's a PhotoProduct.

        	// is there a digital-good?
            if (dbOrderItem.DbDigitalGoods.Count > 0)
            {
                oi.DigitalGood = Controller.Instance.DigitalGoodController.BuildDigitalGoodObject(dbOrderItem.DbDigitalGoods[0]);
                oi.DigitalGood.OrderItem = oi;
            }

            return oi;
        }

        internal IOrderTransaction BuildOrderTransactionObject(IOrder order, DbOrderTransaction dbTransaction)
        {
            var ot = new OrderTransaction(ClassMode.Existing)
         	{
         		Order = order,
         		Id = dbTransaction.ID,
         		Created = dbTransaction.Created,
         		Type = (OrderTransactionType) dbTransaction.Type,
         		GoogleOrderNumber = dbTransaction.GC_OrderNumber,
         		Operation = dbTransaction.Operation,
         		ClientIpAddress = dbTransaction.ClientIPAddress
         	};

        	if (dbTransaction.MemberUID.HasValue)
				ot.Member = Controller.Instance.MemberController.GetMember(dbTransaction.MemberUID.Value);

            switch (ot.Type)
            {
                case OrderTransactionType.GC_ChargebackAmount:
            		if (dbTransaction.GC_ChargebackAmount != null)
            			ot.GcChargebackAmount.ChargebackAmount = dbTransaction.GC_ChargebackAmount.Value;
            		break;

                case OrderTransactionType.GC_NewOrder:
                    // no content. for future use.
                    break;

                case OrderTransactionType.GC_RiskInformation:
                    // no content. for future use.
                    break;

                case OrderTransactionType.GC_OrderStateChange:
                    ot.GcOrderStateChange.NewFinanceState = dbTransaction.GC_NewFinanceState;
                    ot.GcOrderStateChange.NewFulfillmentState = dbTransaction.GC_NewFulfillmentState;
                    ot.GcOrderStateChange.PrevFinanceState = dbTransaction.GC_PrevFinanceState;
                    ot.GcOrderStateChange.PrevFulfillmentState = dbTransaction.GC_PrevFulfillmentState;
                    break;

                case OrderTransactionType.GC_RefundAmount:
                    if (dbTransaction.GC_RefundedAmount.HasValue)
                        ot.GcRefundAmount.RefundedAmount = dbTransaction.GC_RefundedAmount.Value;
                    break;
            }

            return ot;
        }
        #endregion

		#region private methods
		/// <summary>
		/// Creates a new Order object from a Basket.
		/// </summary>
		private IOrder CreateOrder(IBasket basket)
		{
			var total = 0M;
			var o = new Order(ClassMode.New) {ChargeStatus = ChargeStatus.Outstanding};

			foreach (var bi in basket.Items)
			{
				// the only supported product at this time.
				if (bi.PhotoProduct == null) continue;
				var oi = new OrderItem(ClassMode.New)
	         	{
	         		PhotoProduct = bi.PhotoProduct,
	         		SaleAmount = bi.PhotoProduct.Rate,
	         		Status = OrderItemStatus.Normal,
	         		Order = o
	         	};

				o.Items.Add(oi);
				total += bi.PhotoProduct.Rate;
			}

			o.ChargeAmount = total;
			return o;
		}

		/// <summary>
		/// Associates the order with the users basket.
		/// </summary>
		private IOrder PrepareOrderForCustomer(IMember member, bool preBuildDownloads)
		{
			// create the order.
			var o = CreateOrder(member.Basket);
			o.ChargeMethod = member.Company.ChargeMethod;
			o.Customer = member;

			// persist it.
			UpdateOrder(o, preBuildDownloads);

			// associate with basket.
			member.Basket.Order = o;
			UpdateBasket(member.Basket);

			// kill the member orders object so it gets reloaded afresh on next access.
			member.Orders = null;
			return o;
		}

        /// <summary>
        /// Persists any changes to an OrderItem object.
        /// </summary>
        private void UpdateOrderItem(IOrderItem item)
        {
            if (item == null || item.Order == null || !item.Order.IsPersisted)
            {
                Controller.Instance.Logger.LogWarning("UpdateOrderItem() - Null item, or item with null order or unpersisted order passed in.");
                return;
            }

            var db = new MotoProfessionalDataContext();
        	var dbOrderItem = item.IsPersisted ? db.DbOrderItems.Single(oi => oi.ID == item.Id) : new DbOrderItem();

            // the only product type for now.
            if (item.ProductType == ProductType.PhotoProduct)
            {
                dbOrderItem.PhotoID = item.PhotoProduct.Photo.Id;
                dbOrderItem.LicenseID = item.PhotoProduct.License.Id;
            }

            dbOrderItem.OrderID = item.Order.Id;
            dbOrderItem.Status = (byte)item.Status;
            dbOrderItem.SaleRate = item.SaleAmount;

            if (!item.IsPersisted)
                db.DbOrderItems.InsertOnSubmit(dbOrderItem);

            try
            {
                db.SubmitChanges();
            }
            catch (Exception ex)
            {
                Controller.Instance.Logger.LogError("UpdateOrderItem() - Main update failed.", ex);
                return;
            }

        	if (item.IsPersisted) return;
        	item.Id = dbOrderItem.ID;
        	item.IsPersisted = true;
        }

        private void SendOrderConfirmationEmail(IOrder order)
        {
            var args = new string[6];
            var items = new StringBuilder();

            // build the items arg.
            foreach (var item in order.Items)
            {
                items.AppendFormat("+ {0}\n", item.PhotoProduct.Photo.Name);
                items.AppendFormat("  {0} - £{1}\n\n", item.PhotoProduct.License.Name, item.SaleAmount.ToString("N2"));
            }

            args[0] = (string.IsNullOrEmpty(order.Customer.Profile.FirstName)) ? order.Customer.Profile.FirstName : order.Customer.MembershipUser.UserName;
            args[1] = order.Id.ToString();
            args[2] = order.Created.ToString(ConfigurationManager.AppSettings["ShortDateTimeFormatString"]);
            args[3] = items.ToString();
            args[4] = order.ChargeAmount.ToString("N2");

			switch (order.ChargeMethod)
			{
				case ChargeMethod.PointOfSale:
					args[5] = "Paid via Google Checkout.";
					break;
				case ChargeMethod.Invoiced:
					args[5] = "Invoiced to your account.";
					break;
				case ChargeMethod.NoCharge:
					args[5] = "No charge.";
					break;
			}

            EmailHelper.SendMail("OrderConfirmation", false, order.Customer.MembershipUser.Email, args);
        }
		#endregion
	}
}