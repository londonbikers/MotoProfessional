using System;
using System.Collections.Generic;
using System.Linq;
using MotoProfessional.Models.Interfaces;

namespace MotoProfessional.Models
{
    /// <summary>
    /// Represents biographical information for a person.
    /// </summary>
    public class Profile : CommonBase, IProfile
    {
		#region accessors
    	public string Title { get; set; }
    	public string FirstName { get; set; }
    	public string MiddleName { get; set; }
    	public string LastName { get; set; }
    	public PersonSex Sex { get; set; }
    	public string JobTitle { get; set; }
    	public string Telephone { get; set; }
    	public string BillingAddress { get; set; }
    	public string BillingPostalCode { get; set; }
    	public ICountry BillingCountry { get; set; }
    	#endregion

        #region static accessors
        public static DomainObject DomainObjectType { get { return DomainObject.Profile; } }
        #endregion

        #region constructors
        internal Profile(ClassMode mode)
        {
            if (mode == ClassMode.Existing)
                base.IsPersisted = true;

            DomainObject = DomainObjectType;
            Sex = PersonSex.Unknown;
        }
        #endregion

        #region public methods
        public string ToFullName()
        {
            var name = string.Empty;
            var nameParts = new List<string>();
            if (!string.IsNullOrEmpty(FirstName))
                nameParts.Add(FirstName);
            if (!string.IsNullOrEmpty(MiddleName))
                nameParts.Add(MiddleName);
            if (!string.IsNullOrEmpty(LastName))
                nameParts.Add(LastName);

        	name = nameParts.Aggregate(name, (current, part) => current + (part + " "));
        	name = name.Trim();
            return name;
        }
        #endregion

        #region internal methods
        public bool IsEmpty()
        {
        	return string.IsNullOrEmpty(Title) &&
        	       string.IsNullOrEmpty(FirstName) &&
        	       string.IsNullOrEmpty(MiddleName) &&
        	       string.IsNullOrEmpty(LastName) &&
        	       string.IsNullOrEmpty(JobTitle) &&
        	       string.IsNullOrEmpty(Telephone) &&
        	       string.IsNullOrEmpty(BillingAddress) &&
        	       string.IsNullOrEmpty(BillingPostalCode) &&
        	       BillingCountry == null;
        }
    	#endregion
    }
}