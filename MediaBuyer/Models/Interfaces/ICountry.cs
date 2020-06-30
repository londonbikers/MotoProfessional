using System;

namespace MotoProfessional.Models.Interfaces
{
    public interface ICountry
    {
        /// <summary>
        /// The formal English name for the country.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// The three-numeral country-code. Includes leading zero's.
        /// </summary>
        string Numeric { get; set; }

        /// <summary>
        /// The two-letter country-code.
        /// </summary>
        string Alpha2 { get; set; }

        /// <summary>
        /// The three-letter country-code.
        /// </summary>
        string Alpha3 { get; set; }

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
    }
}