using System;
using System.Collections.Generic;
using System.Linq;
using MotoProfessional.Models;
using MotoProfessional.Models.Interfaces;
using MPN.Framework.Caching;

namespace MotoProfessional.Controllers
{
    public class CompanyController
    {
        #region members
        private ILatestCompanies _latestCompanies;
        #endregion

        #region accessors
        public ILatestCompanies LatestCompanies
        {
            get
            {
                if (_latestCompanies == null)
                    _latestCompanies = new LatestCompanies(100);

                return _latestCompanies;
            }
        	internal set
            {
                _latestCompanies = value;
            }
        }
        #endregion

        #region constructors
        internal CompanyController()
        {
        }
        #endregion

        #region public methods
        public ICompany NewCompany()
        {
			var c = new Company(ClassMode.New) {ChargeMethod = ChargeMethod.PointOfSale};
        	return c;
        }

        public ICompany GetCompany(int id)
        {
			if (id < 1)
				return null;

            var company = CacheManager.RetrieveItem(Company.DomainObjectType.ToString(), id, string.Empty) as ICompany;
            if (company == null)
            {
                var db = new MotoProfessionalDataContext();
                var dbCompany = db.DbCompanies.SingleOrDefault(c => c.ID == id);

                company = BuildCompanyObject(dbCompany);
                CacheManager.AddItem(company, company.DomainObject.ToString(), company.Id, string.Empty);
            }

            return company;
        }

        public void UpdateCompany(ICompany company)
        {
            if (company == null || !company.IsValid())
            {
                Controller.Instance.Logger.LogWarning("UpdateCompany() - Null or invalid Company passed in.");
                return;
            }

            var db = new MotoProfessionalDataContext();
        	company.LastUpdated = DateTime.Now;

            var dbCompany = company.IsPersisted ? db.DbCompanies.Single(c => c.ID == company.Id) : new DbCompany();
            dbCompany.Name = company.Name.Trim();
            dbCompany.Description = (!string.IsNullOrEmpty(company.Description)) ? company.Description.Trim() : null;
            dbCompany.Telephone = (!string.IsNullOrEmpty(company.Telephone)) ? company.Telephone.Trim() : null;
            dbCompany.Fax = (!string.IsNullOrEmpty(company.Fax)) ? company.Fax.Trim() : null;
            dbCompany.Address = (!string.IsNullOrEmpty(company.Address)) ? company.Address.Trim() : null;
            dbCompany.PostalCode = (!string.IsNullOrEmpty(company.PostalCode)) ? company.PostalCode.Trim() : null;

            if (company.Country != null)
                dbCompany.CountryID = company.Country.Id;
            else
                dbCompany.CountryID = null;
            
            dbCompany.Url = (company.Url != null) ? company.Url.AbsoluteUri : null;
			dbCompany.ChargeMethod = (byte)company.ChargeMethod;
            dbCompany.Status = (byte)company.Status;
            dbCompany.Created = company.Created;
            dbCompany.LastUpdated = company.LastUpdated;

            // new.
            if (!company.IsPersisted)
                db.DbCompanies.InsertOnSubmit(dbCompany);

            try
            {
                db.SubmitChanges();
            }
            catch (Exception ex)
            {
                Controller.Instance.Logger.LogError("UpdateCompany() - Main update failed.", ex);
                return;
            }

            if (!company.IsPersisted)
            {
                company.Id = dbCompany.ID;
                company.IsPersisted = true;

                // null this so the LatestCompanies cache gets rebuilt with it in on next use.
                LatestCompanies = null;
                CacheManager.AddItem(company, company.DomainObject.ToString(), company.Id, string.Empty);
            }

            // persist company staff.
            db.DbCompanyStaffs.DeleteAllOnSubmit(from cs in db.DbCompanyStaffs where cs.CompanyID == company.Id select cs);
            foreach (var ce in company.Employees)
                db.DbCompanyStaffs.InsertOnSubmit(new DbCompanyStaff { CompanyID = company.Id, PersonUID = (Guid)ce.Member.MembershipUser.ProviderUserKey, Status = (byte)ce.Status, Created = DateTime.Now });

            try
            {
                db.SubmitChanges();
            }
            catch (Exception ex)
            {
                Controller.Instance.Logger.LogError("UpdateCompany() - Persisting staff failed.", ex);
            }
        }
        #endregion

		#region search methods
		/// <summary>
		/// Searches for a company by its name. Attempts a loose search, i.e. for 'Ducati' in 'Airwaves Ducati'.
		/// </summary>
        public List<ICompany> GetCompaniesByLooseName(string name, int maxCompanies)
		{
			var db = new MotoProfessionalDataContext();
            var ids = (from c in db.DbCompanies
                       where c.Name.Contains(name)
                       select c.ID).Take(maxCompanies);

			return ids.Select(id => GetCompany(id)).ToList();
		}
		#endregion

		#region internal methods
		internal ICompanyEmployee BuildCompanyEmployeeObject(DbCompanyStaff dbStaffer)
        {
            var ce = new CompanyEmployee
         	{
         		Member = Controller.Instance.MemberController.GetMember(dbStaffer.PersonUID),
         		Status = (CompanyEmployeeStatus) dbStaffer.Status,
         		Company = GetCompany(dbStaffer.CompanyID)
         	};
			return ce;
        }
        #endregion

        #region private methods
        private ICompany BuildCompanyObject(DbCompany dbCompany)
        {
            var company = new Company(ClassMode.Existing)
          	{
          		Id = dbCompany.ID,
          		Name = dbCompany.Name,
          		Description = dbCompany.Description,
          		Telephone = dbCompany.Telephone,
          		Fax = dbCompany.Fax,
          		Address = dbCompany.Address,
          		PostalCode = dbCompany.PostalCode
          	};

        	if (dbCompany.CountryID.HasValue)
                company.Country = Controller.Instance.PeripheralController.GetCountry(dbCompany.CountryID.Value);

			company.ChargeMethod = (ChargeMethod)dbCompany.ChargeMethod;
            company.Status = (GeneralStatus)dbCompany.Status;

            if (!string.IsNullOrEmpty(dbCompany.Url))
                company.Url = new Uri(dbCompany.Url);

            // base properties.
            company.Created = dbCompany.Created;
            company.LastUpdated = dbCompany.LastUpdated;

            return company;
        }
        #endregion
    }
}