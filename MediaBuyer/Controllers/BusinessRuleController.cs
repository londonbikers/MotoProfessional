using System.Linq;
using MotoProfessional.Models.Interfaces;

namespace MotoProfessional.Controllers
{
	public class BusinessRuleController
	{
		#region constructors
		internal BusinessRuleController()
		{
		}
		#endregion

		#region collection & photo rules
		public bool CanPhotosBeImported(IPartner partner)
		{
			// require an active and default rate-card.
			return partner.RateCards.Exists(rc => rc.Status == GeneralStatus.Active && rc.IsDefault);
		}

		public bool CanPhotoBeDeleted(IPhoto photo)
		{
			if (photo == null || !photo.IsPersisted)
				return false;

			// -- are there any orders for the photo?
			var db = new MotoProfessionalDataContext();
			return db.DbOrderItems.Count(oi => oi.PhotoID == photo.Id) <= 0;
		}

		public bool CanCollectionBeDeleted(ICollection collection)
		{
			if (collection == null || !collection.IsPersisted)
				return false;

			//-- deleting a collection would also delete all the photos as well.
			var db = new MotoProfessionalDataContext();

			//-- are there any photos?
			return collection.Photos.Count == 0 || collection.Photos.All(cp => db.DbOrderItems.Count(oi => oi.PhotoID == cp.Photo.Id) <= 0);
		}
		#endregion

		#region license & pricing rules
		public bool CanRateCardBeDeleted(IRateCard rateCard)
		{
			// todo.
			return true;
		}

        public decimal GetPhotoRate(IPhoto photo, ILicense license)
        {
            if (photo == null || license == null)
                return 0M;

            return photo.RateCard.Items.Where(rci => rci.License.Id == license.Id).First().Amount;
        }
		#endregion

		#region purchasing rules
		/// <summary>
		/// Determines whether or not a customer can add products to their basket and complete a transaction.
		/// </summary>
		public bool CanMemberMakeAPurchase(IMember member)
		{
			// we require a company.
			return member.Company != null;
		}
		#endregion
	}
}