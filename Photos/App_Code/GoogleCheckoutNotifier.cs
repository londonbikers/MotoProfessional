using System;
using System.Web;
using System.IO;
using MotoProfessional;
using GCheckout.AutoGen;
using GCheckout.Util;
using MPN.Framework;

namespace App_Code
{
	/// <summary>
	/// Handles call-back messages from Google Checkout, alerting us of new orders, 
	/// changings in charge states, order-item states, etc.
	/// </summary>
	public class GoogleCheckoutNotifier : IHttpHandler
	{
		#region members
		private HttpContext _context;
		#endregion

        #region public methods
        /// <summary>
		/// Called by ASP.NET to process the http request. This is the equiv main function.
		/// </summary>
		public void ProcessRequest(HttpContext context)
		{
			_context = context;
			HandleRequest();
		}

		/// <summary>
		/// Determines is subsequent requests can reuse this handler.
		/// </summary>
		public bool IsReusable
		{
			get { return false; }
		}
		#endregion

		#region private methods
		private void HandleRequest()
		{
			// extract the XML from the request.
            var requestStream = _context.Request.InputStream;
            var requestStreamReader = new StreamReader(requestStream);
            var requestXml = requestStreamReader.ReadToEnd();
            requestStream.Close();

            if (string.IsNullOrEmpty(requestXml))
            {
                _context.Response.Write("Invalid notification.");
                Logger.LogError("GoogleCheckoutNotifier - Invalid notification:\n" + requestXml);
                return;
            }

			// REMOVE FOLLOWING WHEN HAPPY WITH NOTIFIER PERFORMANCE!
            Logger.LogDebug("GoogleCheckoutNotifier - Request:\n" + requestXml);

            #region retrieve order
            // there's always a google order number.
            var rawGoogleOrderNumber = EncodeHelper.GetElementValue(requestXml, "google-order-number");

            // if this is a new-order-notification then we'll find our own order-id in it.
            var rawOrderId = EncodeHelper.GetElementValue(requestXml, "MERCHANT_DATA_HIDDEN");
            if (string.IsNullOrEmpty(rawGoogleOrderNumber) && !Common.IsNumeric(rawOrderId))
            {
                // neither id found, so we can't continue!
                Logger.LogError("GoogleCheckoutNotifier - No OrderID or GoogleOrderNumber found in Notification XML!\nRequest XML:\n" + requestXml);
                return;
            }

            // prefer our own id over the google one for less lookup overhead.
			var order = Common.IsNumeric(rawOrderId) ? Controller.Instance.CommerceController.GetOrder(Convert.ToInt32(rawOrderId)) : Controller.Instance.CommerceController.GetOrder(rawGoogleOrderNumber);
            if (order == null)
            {
                Logger.LogError("GoogleCheckoutNotifier - No Order found for supplied ID!\nRequest XML:\n" + requestXml);
                return;
            }

            // create a new transaction for this notification.
            var ot = order.NewOrderTransaction();
            #endregion

            #region build transaction
			switch (EncodeHelper.GetTopElement(requestXml))
			{
				case "new-order-notification":
					var n1 = EncodeHelper.Deserialize(requestXml, typeof(NewOrderNotification)) as NewOrderNotification;
                    ot.Type = OrderTransactionType.GC_NewOrder;
			        if (n1 != null) ot.GoogleOrderNumber = n1.googleordernumber;

			        if (string.IsNullOrEmpty(ot.GoogleOrderNumber))
                    {
                        Logger.LogError("GoogleCheckoutNotifier - No GoogleOrderNumber specified for new-order-notification\nRequest XML:\n" + requestXml);
                        return;
                    }
					break;
				case "risk-information-notification":
                    //-- not used for now.
                    // required by the Google Checkout API.
                    Finish();
                    return;
					
                    // This notification tells us that Google has authorized the order and it has passed the fraud check.
					// Use the data below to determine if you want to accept the order, then start processing it.
                    //RiskInformationNotification n2 = EncodeHelper.Deserialize(requestXml, typeof(RiskInformationNotification)) as RiskInformationNotification;

                    //// for future use.
                    //ot.Type = OrderTransactionType.GC_RiskInformation;
                    //ot.GoogleOrderNumber = n2.googleordernumber;
                    //ot.GC_RiskInformation.AVS = n2.riskinformation.avsresponse;
                    //ot.GC_RiskInformation.CVN = n2.riskinformation.cvnresponse;
                    //ot.GC_RiskInformation.EligibleForSellerProtection = n2.riskinformation.eligibleforprotection;
					//break;
				case "order-state-change-notification":
                    // the order has changed either financial or fulfillment state in Google's system.
					var n3 = EncodeHelper.Deserialize(requestXml, typeof(OrderStateChangeNotification)) as OrderStateChangeNotification;
                    ot.Type = OrderTransactionType.GC_OrderStateChange;
			        if (n3 != null)
			        {
			            ot.GoogleOrderNumber = n3.googleordernumber;
			            ot.GcOrderStateChange.NewFinanceState = n3.newfinancialorderstate.ToString();
                        ot.GcOrderStateChange.NewFulfillmentState = n3.newfulfillmentorderstate.ToString();
                        ot.GcOrderStateChange.PrevFinanceState = n3.previousfinancialorderstate.ToString();
                        ot.GcOrderStateChange.PrevFulfillmentState = n3.previousfulfillmentorderstate.ToString();
			        }
			        break;
				case "charge-amount-notification":
                    // Google has successfully charged the customer's credit card.
					var n4 = EncodeHelper.Deserialize(requestXml, typeof(ChargeAmountNotification)) as ChargeAmountNotification;
                    ot.Type = OrderTransactionType.GC_ChargeAmount;
			        if (n4 != null)
			        {
			            ot.GoogleOrderNumber = n4.googleordernumber;
			            ot.GcChargeAmount.ChargedAmount = n4.latestchargeamount.Value;
			        }
			        break;
				case "refund-amount-notification":
                    // Google has successfully refunded the customer's credit card.
					var n5 = EncodeHelper.Deserialize(requestXml, typeof(RefundAmountNotification)) as RefundAmountNotification;
                    ot.Type = OrderTransactionType.GC_RefundAmount;
			        if (n5 != null)
			        {
			            ot.GoogleOrderNumber = n5.googleordernumber;
			            ot.GcRefundAmount.RefundedAmount = n5.latestrefundamount.Value;
			        }
			        break;
				case "chargeback-amount-notification":
                    // A customer initiated a chargeback with their credit card company to get their money back.
					var n6 = EncodeHelper.Deserialize(requestXml, typeof(ChargebackAmountNotification)) as ChargebackAmountNotification;
                    ot.Type = OrderTransactionType.GC_ChargebackAmount;
			        if (n6 != null)
			        {
			            ot.GoogleOrderNumber = n6.googleordernumber;
			            ot.GcChargebackAmount.ChargebackAmount = n6.latestchargebackamount.Value;
			        }
			        break;
				default:
                    Logger.LogError("GoogleCheckoutNotifier - Unrecognised notification type: " + EncodeHelper.GetTopElement(requestXml));
                    Finish();
                    return;
			}
			#endregion

			// adding the transaction will persist it all.
            order.AddTransaction(ot);

            // still perform an order update so order-related business rules can be applied.
            Controller.Instance.CommerceController.UpdateOrder(order, true);
            Finish();
		}

        /// <summary>
        /// Once processing is complete, this will send the correct response message to the client.
        /// </summary>
        private void Finish()
        {
            // required by the Google Checkout API.
            _context.Response.Write("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n");
            _context.Response.Write("<notification-acknowledgment xmlns=\"http://checkout.google.com/schema/2\"/>");
        }
		#endregion
	}
}