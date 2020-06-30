using System;

namespace MotoProfessional.Models.Interfaces
{
    public interface IProfile
    {
        string Title { get; set; }
        string FirstName { get; set; }
        string MiddleName { get; set; }
        string LastName { get; set; }
        PersonSex Sex { get; set; }
        string JobTitle { get; set; }
        string Telephone { get; set; }
        string BillingAddress { get; set; }
        string BillingPostalCode { get; set; }
        ICountry BillingCountry { get; set; }

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

        string ToFullName();
        bool IsEmpty();
    }
}