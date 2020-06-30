using System;

namespace MotoProfessional.Models.Interfaces
{
    public interface IOrderItem
    {
        IOrder Order { get; set; }
        IPhotoProduct PhotoProduct { get; set; }
        decimal SaleAmount { get; set; }
        OrderItemStatus Status { get; set; }
        ProductType ProductType { get; set; }
        IDigitalGood DigitalGood { get; set; }

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