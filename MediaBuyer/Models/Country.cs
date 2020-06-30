using MotoProfessional.Models.Interfaces;

namespace MotoProfessional.Models
{
    /// <summary>
    /// Represents a country with the official ISO-3166 data.
    /// </summary>
    public class Country : CommonBase, ICountry
    {
        #region accessors
    	/// <summary>
    	/// The formal English name for the country.
    	/// </summary>
    	public string Name { get; set; }

    	/// <summary>
    	/// The three-numeral country-code. Includes leading zero's.
    	/// </summary>
    	public string Numeric { get; set; }

    	/// <summary>
    	/// The two-letter country-code.
    	/// </summary>
    	public string Alpha2 { get; set; }

    	/// <summary>
    	/// The three-letter country-code.
    	/// </summary>
    	public string Alpha3 { get; set; }
    	#endregion

        #region static accessors
        public static DomainObject DomainObjectType { get { return DomainObject.Country; } }
        #endregion

        #region constructors
        internal Country(ClassMode mode)
        {
            if (mode == ClassMode.Existing)
                IsPersisted = true;

            DomainObject = DomainObjectType;
        }
        #endregion
    }
}