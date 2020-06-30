using System;
using System.Collections.Generic;

namespace MotoProfessional.Models.Interfaces
{
    public interface IOrder
    {
        IMember Customer { get; set; }
        ChargeMethod ChargeMethod { get; set; }
        ChargeStatus ChargeStatus { get; set; }
        decimal ChargeAmount { get; set; }
        IDigitalGood MasterDigitalGood { get; set; }

        /// <summary>
        /// It's useful to associate the originating basket with the order in case the order is not fulfuled and
        /// so a new order isn't created when this one can be re-used.
        /// </summary>
        List<IOrderItem> Items { get; }

        List<IOrderTransaction> Transactions { get; }

        /// <summary>
        /// Indicates whether or not the order-processing/payment process has begun. An order starts in an idle state which is when this will be false.
        /// </summary>
        bool HasTransactionBegun { get; }

        /// <summary>
        /// Indicates whether or not this order has had any digital-goods created (order or item level). The presence of a digital-good object
        /// doesn't guarantee the presence of files, query the Digital-Good for this information.
        /// </summary>
        bool HasDigitalGoods { get; }

        /// <summary>
        /// Indicates whether or not the application is currently generating DigitalGoods for this order.
        /// </summary>
        bool DigitalGoodsInProduction { get; set; }

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

        /// <summary>
        /// Returns a new OrderTransaction object associated with this order.
        /// </summary>
        IOrderTransaction NewOrderTransaction();

        void AddTransaction(IOrderTransaction orderTransaction);
    }
}