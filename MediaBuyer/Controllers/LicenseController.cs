using System;
using System.Collections.Generic;
using System.Linq;
using MotoProfessional.Models;
using MotoProfessional.Models.Interfaces;
using MPN.Framework.Caching;

namespace MotoProfessional.Controllers
{
	public class LicenseController
	{
		#region constructors
		internal LicenseController()
		{
		}
		#endregion

		#region public methods
        public ILicense NewLicense()
        {
            return new License(ClassMode.New);
        }

		/// <summary>
		/// Retrieves all the Licenses in the system.
		/// </summary>
		public List<ILicense> GetLicenses()
		{
			var licenses = CacheManager.RetrieveItem("GetLicenses()", 0, "Output") as List<ILicense>;
			if (licenses == null)
			{
				var db = new MotoProfessionalDataContext();
				var dbl = from l in db.DbLicenses
						  orderby l.PrimaryDimension descending
						  select l;

				licenses = dbl.Select(dbLicense => BuildLicenseObject(dbLicense)).ToList();
				CacheManager.AddItem(licenses, "GetLicenses()", 0, "Output");
			}

			return licenses;
		}

        /// <summary>
        /// A short-hand method for getting the active licenses in the system.
        /// </summary>
        public IEnumerable<ILicense> GetActiveLicenses()
        {
			return GetLicenses().Where(l => l.Status == GeneralStatus.Active).ToList();
        }

        /// <summary>
        /// Retrieves a single License.
        /// </summary>
        public ILicense GetLicense(int id)
        {
        	return id < 1 ? null : GetLicenses().Find(l => l.Id == id);
        }

		/// <summary>
        /// Persists any changes to a new or existing license.
        /// </summary>
        public void UpdateLicense(ILicense license)
        {
            if (license == null || !license.IsValid())
            {
                Controller.Instance.Logger.LogWarning("UpdateLicense() - Null or invalid License passed in.");
                return;
            }

            var db = new MotoProfessionalDataContext();
			var dbLicense = license.IsPersisted ? db.DbLicenses.Single(l => l.ID == license.Id) : new DbLicense();

            dbLicense.Name = license.Name;
			dbLicense.ShortDescription = license.ShortDescription;
            dbLicense.Description = license.Description;
            dbLicense.Status = (byte)license.Status;
			dbLicense.PrimaryDimension = license.PrimaryDimension;
            dbLicense.Created = license.Created;
            dbLicense.LastUpdated = DateTime.Now;

            if (!license.IsPersisted)
                db.DbLicenses.InsertOnSubmit(dbLicense);

            try
            {
                db.SubmitChanges();
            }
            catch (Exception ex)
            {
                Controller.Instance.Logger.LogError("UpdateLicense() - Main update failed.", ex);

                #if DEBUG
                throw;
                #endif

                return;
            }

            if (!license.IsPersisted)
            {
                license.Id = dbLicense.ID;
                license.IsPersisted = true;
            }

            // expire any licenses cache.
            CacheManager.RemoveItem("GetLicenses()", 0);
        }

		/// <summary>
		/// Retrieves a single RateCard.
		/// </summary>
		public IRateCard GetRateCard(int id)
		{
            var rc = CacheManager.RetrieveItem(RateCard.DomainObjectType.ToString(), id, String.Empty) as IRateCard;
            if (rc == null)
            {
                var db = new MotoProfessionalDataContext();
                var dbRateCard = db.DbRateCards.SingleOrDefault(qRc => qRc.ID == id);

                rc = BuildRateCardObject(dbRateCard);
                CacheManager.AddItem(rc, rc.DomainObject.ToString(), rc.Id, string.Empty);
            }

            return rc;
		}

		/// <summary>
		/// Returns a new Rate Card for use.
		/// </summary>
		public IRateCard NewRateCard()
		{
			var rc = new RateCard(ClassMode.New);
			return rc;
		}

		/// <summary>
		/// Persists changes to a new or existing rate-card and its rate-card-items.
		/// </summary>
		public void UpdateRateCard(IRateCard rc)
		{
			if (rc == null || !rc.IsValid())
			{
				Controller.Instance.Logger.LogWarning("UpdateRateCard(RateCard) - Null or Invalid RateCard passed in.");
				return;
			}

			var isNew = false;
			var db = new MotoProfessionalDataContext();
			DbRateCard dbRc;
			if (rc.IsPersisted)
			{
				dbRc = db.DbRateCards.Single(qRc => qRc.ID == rc.Id);
			}
			else
			{
				dbRc = new DbRateCard();
				isNew = true;
			}

			dbRc.Name = rc.Name;
			dbRc.Status = (byte)rc.Status;
			dbRc.IsDefault = rc.IsDefault;
			dbRc.PartnerID = rc.Partner.Id;
			dbRc.Created = rc.Created;
			dbRc.LastUpdated = DateTime.Now;

			if (!rc.IsPersisted)
				db.DbRateCards.InsertOnSubmit(dbRc);

			try
			{
				db.SubmitChanges();
				if (!rc.IsPersisted)
				{
					rc.Id = dbRc.ID;
					rc.IsPersisted = true;
				}
			}
			catch (Exception ex)
			{
				Controller.Instance.Logger.LogError("UpdateRateCard(RateCard) - Main update failed.", ex);

				#if DEBUG
				throw;
				#endif

				return;
			}

			#region update rate-card-items
			foreach (var rci in rc.Items)
			{
			    var rci1 = rci;
				var dbRci = rci.IsPersisted ? db.DbRateCardItems.Single(qRci => qRci.ID == rci1.Id) : new DbRateCardItem();

				dbRci.Amount = rci.Amount;
				dbRci.LicenseID = rci.License.Id;
				dbRci.RateCardID = rc.Id;
				dbRci.Created = rci.Created;
				dbRci.LastUpdated = DateTime.Now;

				if (!rci.IsPersisted)
					db.DbRateCardItems.InsertOnSubmit(dbRci);

				try
				{
					db.SubmitChanges();
					if (!rci.IsPersisted)
					{
						rci.Id = dbRci.ID;
						rci.IsPersisted = true;
					}
				}
				catch (Exception ex)
				{
					Controller.Instance.Logger.LogError("UpdateRateCard(RateCard) - RCI update failed.", ex);

					#if DEBUG
					throw;
					#endif

					return;
				}
			}
			#endregion

			// all done. cache if necessary.
			if (isNew)
				CacheManager.AddItem(rc, rc.DomainObject.ToString(), rc.Id, string.Empty);
		}
		#endregion

		#region internal methods
		/// <summary>
		/// Fleshes out a RateCardItem object from a SQL Linq one.
		/// </summary>
		internal IRateCardItem BuildRateCardItemObject(DbRateCardItem dbItem)
		{
			var item = new RateCardItem(ClassMode.Existing)
            {
                Id = dbItem.ID,
                License = GetLicense(dbItem.LicenseID),
                Amount = dbItem.Amount,
                Created = dbItem.Created,
                LastUpdated = dbItem.LastUpdated
            };

		    return item;
		}

		/// <summary>
		/// Fleshes out a RateCard object from a SQL Linq one.
		/// </summary>
		internal IRateCard BuildRateCardObject(DbRateCard dbRateCard)
		{
			var rateCard = new RateCard(ClassMode.Existing)
           	{
           		Id = dbRateCard.ID,
           		Partner = Controller.Instance.PartnerController.GetPartner(dbRateCard.PartnerID),
           		Name = dbRateCard.Name,
           		IsDefault = dbRateCard.IsDefault,
           		Created = dbRateCard.Created,
           		LastUpdated = dbRateCard.LastUpdated,
           		Status = (GeneralStatus) dbRateCard.Status
           	};

			return rateCard;
		}

		/// <summary>
		/// Fleshes out a License object from a SQL Linq one.
		/// </summary>
		internal ILicense BuildLicenseObject(DbLicense dbLicense)
		{
			var license = new License(ClassMode.Existing)
          	{
          		Id = dbLicense.ID,
          		Name = dbLicense.Name,
          		ShortDescription = dbLicense.ShortDescription,
          		Description = dbLicense.Description,
          		Status = (GeneralStatus) dbLicense.Status,
          		PrimaryDimension = dbLicense.PrimaryDimension,
          		Created = dbLicense.Created,
          		LastUpdated = dbLicense.LastUpdated
          	};

			return license;
		}
		#endregion
	}
}