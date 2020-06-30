using System;

namespace MotoProfessional.Models.Interfaces
{
    public interface IOrderTransaction
    {
        IOrder Order { get; set; }
        OrderTransactionType Type { get; set; }
        OrderTransaction.GoogleCheckoutNewOrder GcNewOrder { get; }
        OrderTransaction.GoogleCheckoutRiskInformation GcRiskInformation { get; }
        OrderTransaction.GoogleCheckoutOrderStateChange GcOrderStateChange { get; }
        OrderTransaction.GoogleCheckoutChargeAmount GcChargeAmount { get; }
        OrderTransaction.GoogleCheckoutRefundAmount GcRefundAmount { get; }
        OrderTransaction.GoogleCheckoutChargebackAmount GcChargebackAmount { get; }
        string GoogleOrderNumber { get; set; }

        /// <summary>
        /// If this is a Generic transaction then some text can be attributed to it. Max length 1000 chars.
        /// </summary>
        string Operation { get; set; }

        /// <summary>
        /// If this is a Generic transaction then a Member can be attributed with it.
        /// </summary>
        IMember Member { get; set; }

        /// <summary>
        /// If relevant, the IP address of the client initiating the transaction, i.e. the customer.
        /// </summary>
        string ClientIpAddress { get; set; }

        /// <summary>
        /// The identifier for the object.
        /// </summary>
        int Id { get; set; }

        /// <summary>
        /// If set, denotes the type of object the implementer is.
        /// </summary>
        DomainObject DomainObject { get; set; }

        DateTime Created { get; set; }
        DateTime LastUpdated { get; set; }

        /// <summary>
        /// Denotes whether or not the object has been persisted to the database.
        /// </summary>
        bool IsPersisted { get; set; }
    }
}