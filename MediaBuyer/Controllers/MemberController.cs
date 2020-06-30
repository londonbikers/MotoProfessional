using System;
using System.Linq;
using System.Web.Security;
using MotoProfessional.Models;
using MotoProfessional.Models.Interfaces;
using MPN.Framework.Caching;

namespace MotoProfessional.Controllers
{
    public class MemberController
    {
        #region constructors
        internal MemberController()
        {
        }
        #endregion

        #region public methods
        /// <summary>
        /// Retrieves all the member-relevant objects associated with an ASPNET MebershipUser object.
        /// </summary>
        /// <param name="membershipUser">The ASPNET MembershipUser object provided by the presentation layer.</param>
        /// <remarks>Not cached, just fulfilled typically upon member login and cached in session.</remarks>
        public IMember GetMember(MembershipUser membershipUser)
        {
			var member = CacheManager.RetrieveItem(Member.DomainObjectType.ToString(), 0, membershipUser.ProviderUserKey.ToString()) as Member;
			if (member == null)
			{
				var db = new MotoProfessionalDataContext();
				member = new Member {MembershipUser = membershipUser};

				// retrieve optional profile.
				var dbProfile = db.DbProfiles.SingleOrDefault(p => p.MemberUID == (Guid)membershipUser.ProviderUserKey);
				if (dbProfile != null)
					member.Profile = BuildProfileObject(dbProfile);

				// associated with a company?
				var companyId = (from cs in db.DbCompanyStaffs
								 where cs.PersonUID == (Guid)membershipUser.ProviderUserKey
								 select cs.CompanyID).FirstOrDefault();

				if (companyId > 0)
					member.Company = Controller.Instance.CompanyController.GetCompany(companyId);

				CacheManager.AddItem(member, Member.DomainObjectType.ToString(), 0, member.MembershipUser.ProviderUserKey.ToString());
			}

            return member;
        }

        /// <summary>
        /// Retrieves all the member-relevant objects associated with an ASPNET MebershipUser object UID.
        /// </summary>
        /// <remarks>Not cached, just fulfilled typically upon member login and cached in session.</remarks>
        public IMember GetMember(Guid uid)
        {
            var mu = Membership.GetUser(uid);
            if (mu == null)
            {
                Controller.Instance.Logger.LogWarning(string.Format("MemberController.GetMember(Guid) - No ASPNET MembershipUser found for uid '{0}'.", uid));
                return null;
            }

            return GetMember(mu);
        }

        /// <summary>
        /// Persists any changes to a new or existing Member object.
        /// </summary>
        public void UpdateMember(IMember member)
        {
            var db = new MotoProfessionalDataContext();

			// persist MembershipUser changes.
			Membership.UpdateUser(member.MembershipUser);

            // persist profile.
            if (member.Profile.IsPersisted && member.Profile.IsEmpty())
            {
                // delete profile.
                db.DbProfiles.DeleteOnSubmit(db.DbProfiles.Single(p => p.MemberUID == (Guid)member.MembershipUser.ProviderUserKey));
                db.SubmitChanges();
            }
            else if (!member.Profile.IsEmpty())
            {
                // persist changes.
                var dbProfile = member.Profile.IsPersisted ? db.DbProfiles.Single(p => p.MemberUID == (Guid)member.MembershipUser.ProviderUserKey) : new DbProfile();
                dbProfile.MemberUID = (Guid)member.MembershipUser.ProviderUserKey;
                dbProfile.Title = (!string.IsNullOrEmpty(member.Profile.Title)) ? member.Profile.Title : null;
                dbProfile.Firstname = (!string.IsNullOrEmpty(member.Profile.FirstName)) ? member.Profile.FirstName : null;
                dbProfile.Middlename = (!string.IsNullOrEmpty(member.Profile.MiddleName)) ? member.Profile.MiddleName : null;
                dbProfile.Lastname = (!string.IsNullOrEmpty(member.Profile.LastName)) ? member.Profile.LastName : null;
                dbProfile.Sex = (byte)member.Profile.Sex;
                dbProfile.JobTitle = (!string.IsNullOrEmpty(member.Profile.JobTitle)) ? member.Profile.JobTitle : null;
                dbProfile.Telephone = (!string.IsNullOrEmpty(member.Profile.Telephone)) ? member.Profile.Telephone : null;
                dbProfile.BillingAddress = (!string.IsNullOrEmpty(member.Profile.BillingAddress)) ? member.Profile.BillingAddress : null;
                dbProfile.BillingPostalCode = (!string.IsNullOrEmpty(member.Profile.BillingPostalCode)) ? member.Profile.BillingPostalCode : null;

                if (member.Profile.BillingCountry != null)
                    dbProfile.BillingCountryID = member.Profile.BillingCountry.Id;
                else
                    dbProfile.BillingCountryID = null;

                dbProfile.Created = member.Profile.Created;
                dbProfile.LastUpdated = DateTime.Now;

                if (!member.Profile.IsPersisted)
                    db.DbProfiles.InsertOnSubmit(dbProfile);

                db.SubmitChanges();
                member.Profile.IsPersisted = true;
            }

			// the Member object is a little different to other domain objects in that it's not really created, just extended from the 
			// ASPNET MembershipUser object. Though we should still be checking if we need to put it in the cache.
			var cachedMember = CacheManager.RetrieveItem(Member.DomainObjectType.ToString(), 0, member.MembershipUser.ProviderUserKey.ToString()) as Member;
			if (cachedMember == null)
				CacheManager.AddItem(member, Member.DomainObjectType.ToString(), 0, member.MembershipUser.ProviderUserKey.ToString());
        }
        #endregion

        #region private methods
        private IProfile BuildProfileObject(DbProfile dbProfile)
        {
            var p = new Profile(ClassMode.Existing)
        	{
        		Title = dbProfile.Title,
        		FirstName = dbProfile.Firstname,
        		MiddleName = dbProfile.Middlename,
        		LastName = dbProfile.Lastname,
        		Sex = (PersonSex) dbProfile.Sex,
        		JobTitle = dbProfile.JobTitle,
        		Telephone = dbProfile.Telephone,
        		BillingAddress = dbProfile.BillingAddress,
        		BillingPostalCode = dbProfile.BillingPostalCode
        	};

        	if (dbProfile.BillingCountryID.HasValue)
                p.BillingCountry = Controller.Instance.PeripheralController.GetCountry(dbProfile.BillingCountryID.Value);
            
            p.Created = dbProfile.Created;
            p.LastUpdated = dbProfile.LastUpdated;

            return p;
        }
        #endregion
    }
}