using System.Collections.Generic;
using System.Linq;
using MotoProfessional.Models.Interfaces;

namespace MotoProfessional.Models
{
    public class Order : CommonBase, IOrder
    {
        #region members
		private IMember _customer;
        private ChargeMethod _chargeMethod;
        private ChargeStatus _chargeStatus;
        private decimal _chargeAmount;
		private List<IOrderItem> _items;
        private List<IOrderTransaction> _transactions;
        private IDigitalGood _masterDigitalGood;
        #endregion

        #region accessors
		public IMember Customer { get { return _customer; } set { _customer = value; } }
        public ChargeMethod ChargeMethod { get { return _chargeMethod; } set { _chargeMethod = value; } }
        public ChargeStatus ChargeStatus { get { return _chargeStatus; } set { _chargeStatus = value; } }
        public decimal ChargeAmount { get { return _chargeAmount; } set { _chargeAmount = value; } }
        public IDigitalGood MasterDigitalGood { get { return _masterDigitalGood; } set { _masterDigitalGood = value; } }
        /// <summary>
        /// It's useful to associate the originating basket with the order in case the order is not fulfuled and
        /// so a new order isn't created when this one can be re-used.
        /// </summary>
		public List<IOrderItem> Items
		{
			get
			{
				if (_items == null)
					RetrieveItems();

				return _items;
			}
		}
        public List<IOrderTransaction> Transactions
        {
            get
            {
                if (_transactions == null)
                    RetrieveTransactions();

                return _transactions;
            }
        }
        /// <summary>
        /// Indicates whether or not the order-processing/payment process has begun. An order starts in an idle state which is when this will be false.
        /// </summary>
        public bool HasTransactionBegun
        {
            get
            {
            	// covers invoiced and no-charge orders.
				if (ChargeStatus == ChargeStatus.Complete)
					return true;

            	return ChargeMethod == ChargeMethod.PointOfSale && Transactions.Exists(ot => ot.Type == OrderTransactionType.GC_NewOrder);
            }
        }
        /// <summary>
        /// Indicates whether or not this order has had any digital-goods created (order or item level). The presence of a digital-good object
        /// doesn't guarantee the presence of files, query the Digital-Good for this information.
        /// </summary>
        public bool HasDigitalGoods { get { return MasterDigitalGood != null || Items.Exists(oi => oi.DigitalGood != null); } }
        /// <summary>
        /// Indicates whether or not the application is currently generating DigitalGoods for this order.
        /// </summary>
        public bool DigitalGoodsInProduction { get; set; }
        #endregion

        #region static accessors
        public static DomainObject DomainObjectType { get { return DomainObject.Order; } }
        #endregion

        #region constructors
        internal Order(ClassMode mode)
        {
            if (mode == ClassMode.Existing)
                IsPersisted = true;

            DomainObject = DomainObjectType;
            DigitalGoodsInProduction = false;
        }
        #endregion

        #region public methods

        /// <summary>
        /// Returns a new OrderTransaction object associated with this order.
        /// </summary>
        public IOrderTransaction NewOrderTransaction()
        {
            var ot = new OrderTransaction(ClassMode.New) {Order = this};
        	return ot;
        }

        public void AddTransaction(IOrderTransaction orderTransaction)
        {
            // make sure this isn't a duplicate.
            if (Transactions.Exists(ot => ot.Id == orderTransaction.Id))
                return;

            orderTransaction.Order = this;
            Controller.Instance.CommerceController.UpdateOrderTransaction(orderTransaction);
            Transactions.Add(orderTransaction);

			// ***************************************************************************** //
			// business rules to implement?                                                  //
			// ***************************************************************************** //

			if (orderTransaction.Type == OrderTransactionType.GC_ChargeAmount && ChargeStatus != ChargeStatus.Complete)
			{
				// the order is now complete, the customer has been charged.
				ChargeStatus = ChargeStatus.Complete;
			}
			else switch (orderTransaction.Type)
			{
				case OrderTransactionType.GC_RefundAmount:
					ChargeStatus = orderTransaction.GcRefundAmount.RefundedAmount == ChargeAmount ? ChargeStatus.Refunded : ChargeStatus.PartialRefund;
					break;
				case OrderTransactionType.GC_ChargebackAmount:
					ChargeStatus = ChargeStatus.ChargeBack;
					break;
			}

			Controller.Instance.CommerceController.UpdateOrder(this);
        }
        #endregion

        #region private methods
        private void RetrieveItems()
		{
			_items = new List<IOrderItem>();
			var db = new MotoProfessionalDataContext();
			var dbOrderItems = from oi in db.DbOrderItems
							   where oi.OrderID == Id
							   select oi;

            foreach (var dbOrderItem in dbOrderItems)
                _items.Add(Controller.Instance.CommerceController.BuildOrderItemObject(this, dbOrderItem));
		}

        private void RetrieveTransactions()
        {
            _transactions = new List<IOrderTransaction>();
            var db = new MotoProfessionalDataContext();
            var dbTransactions = from ot in db.DbOrderTransactions
                                 where ot.OrderID == Id
                                 orderby ot.Created ascending
                                 select ot;

            foreach (var dbTransaction in dbTransactions)
                _transactions.Add(Controller.Instance.CommerceController.BuildOrderTransactionObject(this, dbTransaction));
        }
		#endregion
	}
}