using MotoProfessional.Models.Interfaces;

namespace MotoProfessional.Models
{
    public class OrderTransaction : CommonBase, IOrderTransaction
    {
        #region members
    	private readonly GoogleCheckoutNewOrder _gcNewOrder;
        private readonly GoogleCheckoutRiskInformation _gcRiskInformation;
        private readonly GoogleCheckoutOrderStateChange _gcOrderStateChange;
        private readonly GoogleCheckoutChargeAmount _gcChargeAmount;
        private readonly GoogleCheckoutRefundAmount _gcRefundAmount;
        private readonly GoogleCheckoutChargebackAmount _gcChargebackAmount;
    	private string _operation;
    	#endregion

        #region accessors
    	public IOrder Order { get; set; }
    	public OrderTransactionType Type { get; set; }
    	public GoogleCheckoutNewOrder GcNewOrder { get { return _gcNewOrder; } }
        public GoogleCheckoutRiskInformation GcRiskInformation { get { return _gcRiskInformation; } }
        public GoogleCheckoutOrderStateChange GcOrderStateChange { get { return _gcOrderStateChange; } }
        public GoogleCheckoutChargeAmount GcChargeAmount { get { return _gcChargeAmount; } }
        public GoogleCheckoutRefundAmount GcRefundAmount { get { return _gcRefundAmount; } }
        public GoogleCheckoutChargebackAmount GcChargebackAmount { get { return _gcChargebackAmount; } }
    	public string GoogleOrderNumber { get; set; }

    	/// <summary>
		/// If this is a Generic transaction then some text can be attributed to it. Max length 1000 chars.
		/// </summary>
		public string Operation 
		{ 
			get { return _operation; } 
			set 
			{ 
				_operation = value;
				if (!string.IsNullOrEmpty(_operation) && _operation.Length > 1000)
					_operation = _operation.Substring(0, 1000);
			} 
		}

    	/// <summary>
    	/// If this is a Generic transaction then a Member can be attributed with it.
    	/// </summary>
    	public IMember Member { get; set; }

    	/// <summary>
		/// If relevant, the IP address of the client initiating the transaction, i.e. the customer.
		/// </summary>
		public string ClientIpAddress { get; set; }
        #endregion

        #region static accessors
        public static DomainObject DomainObjectType { get { return DomainObject.OrderTransaction; } }
        #endregion

        #region constructors
        internal OrderTransaction(ClassMode mode)
        {
            if (mode == ClassMode.Existing)
                IsPersisted = true;

            DomainObject = DomainObjectType;
            _gcChargeAmount = new GoogleCheckoutChargeAmount();
            _gcNewOrder = new GoogleCheckoutNewOrder();
            _gcOrderStateChange = new GoogleCheckoutOrderStateChange();
            _gcRefundAmount = new GoogleCheckoutRefundAmount();
            _gcRiskInformation = new GoogleCheckoutRiskInformation();
            _gcChargebackAmount = new GoogleCheckoutChargebackAmount();
        }
        #endregion

        public class GoogleCheckoutNewOrder
        {
            // no content.
        }

        public class GoogleCheckoutRiskInformation
        {
            public string Avs { get; set; }
            public string Cvn { get; set; }
			/// <summary>
			/// Not currently in use.
			/// </summary>
			public bool EligibleForSellerProtection { get; set; }
        }

        public class GoogleCheckoutOrderStateChange
        {
            public string NewFinanceState { get; set; }
            public string NewFulfillmentState { get; set; }
            public string PrevFinanceState { get; set; }
            public string PrevFulfillmentState { get; set; }
        }

        public class GoogleCheckoutChargeAmount
        {
            public decimal ChargedAmount { get; set; }
        }

        public class GoogleCheckoutRefundAmount
        {
            public decimal RefundedAmount { get; set; }
        }

        public class GoogleCheckoutChargebackAmount
        {
            public decimal ChargebackAmount { get; set; }
        }
    }
}