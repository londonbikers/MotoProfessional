using System;
using System.Linq;
using MotoProfessional.Models;
using MotoProfessional.Models.Interfaces;
using MPN.Framework.Caching;

namespace MotoProfessional.Controllers
{
    public class PartnerController
    {
		#region members
		private ILatestPartners _latestPartners;
        private ITopPartners _topPartners;
		#endregion

		#region accessors
		public ILatestPartners LatestPartners
		{
			get
			{
                if (_latestPartners == null)
                    _latestPartners = new LatestPartners(100);

			    return _latestPartners;
			}
			internal set
			{
				_latestPartners = value;
			}
		}
        /// <summary>
        /// Provides access to the top-selling partners over the last month worth of orders.
        /// </summary>
        public ITopPartners TopPartners
        {
            get
            {
                if (_topPartners == null)
                    _topPartners = new TopPartners(6);

                return _topPartners;
            }
        	internal set
            {
                _topPartners = value;
            }
        }
		#endregion

        #region constructors
        internal PartnerController()
        {
        }
        #endregion

        #region public methods
        public IPartner NewPartner()
        {
            return new Partner(ClassMode.New);
        }

        public IPartner GetPartner(int id)
        {
			if (id < 1)
				return null;

            var partner = CacheManager.RetrieveItem(Partner.DomainObjectType.ToString(), id, String.Empty) as IPartner;
            if (partner == null)
            {
                var db = new MotoProfessionalDataContext();
                var dbPartner = db.DbPartners.SingleOrDefault(p => p.ID == id);
                if (dbPartner == null)
                    return null;

                partner = BuildPartnerObject(dbPartner);
                CacheManager.AddItem(partner, partner.DomainObject.ToString(), partner.Id, String.Empty);
            }

            return partner;
        }

        public void UpdatePartner(IPartner partner)
        {
            if (partner == null || !partner.IsValid())
            {
                Controller.Instance.Logger.LogWarning("UpdatePartner() - Null or invalid Partner passed in.");
                return;
            }

            var db = new MotoProfessionalDataContext();
        	var dbPartner = partner.IsPersisted ? db.DbPartners.Single(p => p.ID == partner.Id) : new DbPartner();

            dbPartner.Name = partner.Name.Trim();
            dbPartner.Description = (!string.IsNullOrEmpty(partner.Description)) ? partner.Description.Trim() : null;
            dbPartner.LogoFilename = (!string.IsNullOrEmpty(partner.LogoFilename)) ? partner.LogoFilename : null;

            if (partner.Company != null)
                dbPartner.CompanyID = partner.Company.Id;
            else
                dbPartner.CompanyID = null;            
            
            dbPartner.Status = (byte)partner.Status;
            dbPartner.Created = partner.Created;
            dbPartner.LastUpdated = partner.LastUpdated;

			if (!partner.IsPersisted)
				db.DbPartners.InsertOnSubmit(dbPartner);

            try
            {
                db.SubmitChanges();
            }
            catch (Exception ex)
            {
                Controller.Instance.Logger.LogError("UpdatePartner() - Main update failed.", ex);
                return;
            }

        	if (partner.IsPersisted) return;
        	partner.Id = dbPartner.ID;
        	partner.IsPersisted = true;
        	partner.CreateFileStore();

        	// null this so the LatestPartners cache gets rebuilt with it in on next use.
        	LatestPartners = null;
        	TopPartners.Reset();
        	CacheManager.AddItem(partner, partner.DomainObject.ToString(), partner.Id, String.Empty);
        }
        #endregion

        #region private methods
        private IPartner BuildPartnerObject(DbPartner dbPartner)
        {
            var partner = new Partner(ClassMode.Existing)
                          	{
                          		Id = dbPartner.ID,
                          		Name = dbPartner.Name,
                          		Description = dbPartner.Description,
                          		LogoFilename = dbPartner.LogoFilename,
                          		Status = (GeneralStatus) dbPartner.Status
                          	};

        	if (dbPartner.CompanyID.HasValue)
            {
                partner.Company = (Company) Controller.Instance.CompanyController.GetCompany(dbPartner.CompanyID.Value);

                // umm, I don't like 
                partner.Company.Partner = partner;
            }

            // base properties.
            partner.Created = dbPartner.Created;
            partner.LastUpdated = dbPartner.LastUpdated;

            return partner;
        }
        #endregion
    }
}