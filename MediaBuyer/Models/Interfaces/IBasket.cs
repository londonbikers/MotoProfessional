using System;
using System.Collections.Generic;

namespace MotoProfessional.Models.Interfaces
{
    public interface IBasket
    {
        IMember Member { get; set; }
        string Name { get; set; }
        BasketStatus Status { get; set; }
        List<IBasketItem> Items { get; set; }
        decimal TotalValue { get; }

        /// <summary>
        /// It's useful to associate an order with a basket in case the order isn't fulfiled and then
        /// the user goes back to re-attempt the transaction. we don't want to create a new order in this situation.
        /// </summary>
        IOrder Order { get; set; }

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

        void AddPhotoProduct(IPhotoProduct pp);
        void AddPhotoProduct(ILicense license, IPhoto photo);
        void RemovePhotoProduct(IPhotoProduct pp);
        void RemovePhotoProduct(ILicense license, IPhoto photo);
        void RemoveBasketItem(IBasketItem item);
        void RemoveBasketItem(int itemId);
        bool DoesBasketContainerPhotoProduct(ILicense license, IPhoto photo);

        /// <summary>
        /// Determines if the Basket is valid for use and persistence.
        /// </summary>
        bool IsValid();
    }
}