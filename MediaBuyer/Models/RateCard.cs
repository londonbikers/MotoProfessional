using System.Collections.Generic;
using System.Linq;
using MotoProfessional.Models.Interfaces;

namespace MotoProfessional.Models
{
	public class RateCard : CommonBase, IRateCard
	{
		#region members
		private List<IRateCardItem> _items;
		#endregion

		#region accessors
		public IPartner Partner { get; set; }
		public string Name { get; set; }
		/// <summary>
		/// A default and active rate-card is needed before any photos can be imported as a photo requires a rate-card.
		/// </summary>
		public bool IsDefault { get; set; }
		public GeneralStatus Status { get; set; }
		public List<IRateCardItem> Items 
		{ 
			get 
			{
				if (_items == null)
					RetrieveItems();

				return _items; 
			} 
			set
			{
				_items = value;
			} 
		}
		#endregion

		#region static accessors
		public static DomainObject DomainObjectType { get { return DomainObject.RateCard; } }
		#endregion

		#region constructors
		internal RateCard(ClassMode mode)
        {
            if (mode == ClassMode.Existing)
                IsPersisted = true;

            DomainObject = DomainObjectType;
			IsDefault = false;
			Status = GeneralStatus.New;
        }
		#endregion

		#region public methods
		/// <summary>
		/// If the RateCard is new, this will create place-holder items inside it for it to be filled out entirely.
		/// The items consists of all the active licenses in the system. The method can be used to fill in missing
		/// licenses as well.
		/// </summary>
		public void PopulateRateCardItems()
		{
			// instantiate here to avoid a db query on Items accessor.
			if (_items == null)
				_items = new List<IRateCardItem>();

			foreach (var rci in from lic in Controller.Instance.LicenseController.GetActiveLicenses()
			                    let lic1 = lic
			                    where !Items.Exists(qRci => qRci.License.Id == lic1.Id)
			                    select new RateCardItem(ClassMode.New) {License = lic, RateCard = this})
			{
				Items.Add(rci);
			}
		}

		public void AddRateCardItem(IRateCardItem rci)
		{
			if (Items.Exists(qRci => qRci.Id == rci.Id))
				return;

			rci.RateCard = this;
			Items.Add(rci);
		}

		public void RemoveRateCardItem(IRateCardItem rci)
		{
			Items.RemoveAll(qRci => qRci.Id == rci.Id);
		}

		/// <summary>
		/// Determines if the rate-card is valid for use and persistence.
		/// </summary>
		public bool IsValid()
		{
			if (string.IsNullOrEmpty(Name))
				return false;

			if (Items == null || Partner == null)
				return false;
			
			var licenses = Controller.Instance.LicenseController.GetActiveLicenses();

			// check we have all the licenses.
			if (licenses.Any(license => !Items.Exists(qRci => qRci.License.Id == license.Id)))
				return false;

			// check all the rci values.
			return Items.All(rci => rci.Amount > 0);
		}
		#endregion

		#region private methods
		private void RetrieveItems()
		{
			_items = new List<IRateCardItem>();
			var db = new MotoProfessionalDataContext();
			var dbItems = from rci in db.DbRateCardItems
						  where rci.RateCardID == Id
						  select rci;

			foreach (var dbItem in dbItems)
                _items.Add(Controller.Instance.LicenseController.BuildRateCardItemObject(dbItem));
		}
		#endregion
	}
}