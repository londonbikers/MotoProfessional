using System;
using System.Collections.Generic;
using System.Linq;
using MotoProfessional.Models.Interfaces;

namespace MotoProfessional.Models
{
	public class Basket : CommonBase, IBasket
	{
		#region members
		private List<IBasketItem> _items;
		#endregion

		#region accessors
		public IMember Member { get; set; }
		public string Name { get; set; }
		public BasketStatus Status { get; set; }
		public List<IBasketItem> Items { get { return _items; } set { _items = value; } }
        public decimal TotalValue { get { return Items.Sum(i => i.PhotoProduct.Rate); } }
		/// <summary>
		/// It's useful to associate an order with a basket in case the order isn't fulfiled and then
		/// the user goes back to re-attempt the transaction. we don't want to create a new order in this situation.
		/// </summary>
		public IOrder Order { get; set; }
		#endregion

		#region static accessors
		public static DomainObject DomainObjectType { get { return DomainObject.Basket; } }
		#endregion

		#region constructors
		internal Basket(ClassMode mode)
        {
            if (mode == ClassMode.Existing)
                IsPersisted = true;

            DomainObject = DomainObjectType;
            _items = new List<IBasketItem>();
        }
		#endregion

		#region public methods
		public void AddPhotoProduct(IPhotoProduct pp)
		{
			lock (Items)
			{
				// don't add duplicates.
				if (Items.Exists(i => i.PhotoProduct.Id == pp.Id))
					return;

				// setup up basket-item.
				var bi = new BasketItem(ClassMode.New) {PhotoProduct = pp};

				// persist basket if necessary.
				if (!IsPersisted && IsValid())
					Controller.Instance.CommerceController.UpdateBasket(this);
				else if (!IsValid())
					return;

				// add to collection.
				Items.Add(bi);

				// persist to db.
				var db = new MotoProfessionalDataContext();
				var dbItem = new DbBasketItem
             	{
             		BasketID = Id,
             		LicenseID = pp.License.Id,
             		PhotoID = pp.Photo.Id,
             		Created = DateTime.Now
             	};

				db.DbBasketItems.InsertOnSubmit(dbItem);

				try
				{
					db.SubmitChanges();
					bi.Id = dbItem.ID;
					bi.IsPersisted = true;
				}
				catch (Exception ex)
				{
					Controller.Instance.Logger.LogError("Basket.AddPhotoProduct() - Persistence failed.", ex);
					Items.Remove(bi);
				}
			}
		}

		public void AddPhotoProduct(ILicense license, IPhoto photo)
		{
			var pp = new PhotoProduct {Photo = photo, License = license};
			AddPhotoProduct(pp);
		}

		public void RemovePhotoProduct(IPhotoProduct pp)
		{
			RemoveBasketItem(Items.Find(i => i.PhotoProduct != null && i.PhotoProduct.Id == pp.Id));
		}

		public void RemovePhotoProduct(ILicense license, IPhoto photo)
		{
			var pp = (from qPp in _items where qPp.PhotoProduct != null && qPp.PhotoProduct.License.Id == license.Id && qPp.PhotoProduct.Photo.Id == photo.Id select qPp.PhotoProduct).FirstOrDefault();
			if (pp != null)
				RemovePhotoProduct(pp);
		}

		public void RemoveBasketItem(IBasketItem item)
		{
			lock (Items)
			{
				// ensure it exists in the basket first.
				if (!Items.Exists(i => i.Id == item.Id))
					return;

				// remove from db.
				var db = new MotoProfessionalDataContext();
				db.DbBasketItems.DeleteOnSubmit(db.DbBasketItems.Single(bi => bi.ID == item.Id));

				try
				{
					db.SubmitChanges();

					// remove from collection.
					Items.Remove(Items.Find(i => i.Id == item.Id));
				}
				catch (Exception ex)
				{
					Controller.Instance.Logger.LogError("Basket.RemoveBasketItem() - Persistence failed.", ex);
				}
			}
		}

		public void RemoveBasketItem(int itemId)
		{
			RemoveBasketItem(Items.Find(i => i.Id == itemId));
		}

		public bool DoesBasketContainerPhotoProduct(ILicense license, IPhoto photo)
		{
			return _items.Exists(qBi => qBi.PhotoProduct != null && qBi.PhotoProduct.License.Id == license.Id && qBi.PhotoProduct.Photo.Id == photo.Id);
		}

		/// <summary>
		/// Determines if the Basket is valid for use and persistence.
		/// </summary>
		public bool IsValid()
		{
			return Member != null;
		}
		#endregion
	}
}