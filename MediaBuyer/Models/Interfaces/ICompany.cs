using System;
using System.Collections.Generic;

namespace MotoProfessional.Models.Interfaces
{
    public interface ICompany
    {
        string Name { get; set; }
        string Description { get; set; }
        string Telephone { get; set; }
        string Fax { get; set; }
        string Address { get; set; }
        string PostalCode { get; set; }
        Uri Url { get; set; }

        /// <summary>
        /// If this company is also a partner then the Partner will be available here.
        /// </summary>
        IPartner Partner { get; set; }

        ChargeMethod ChargeMethod { get; set; }
        ICountry Country { get; set; }
        GeneralStatus Status { get; set; }
        List<ICompanyEmployee> Employees { get; set; }

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

        bool IsValid();
        void AddEmployee(IMember member, CompanyEmployeeStatus status);
        void RemoveEmployee(IMember member);
        bool IsEmployeeConfirmed(IMember member);
    }
}