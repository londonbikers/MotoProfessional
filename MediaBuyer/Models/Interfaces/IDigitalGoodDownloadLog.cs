using System;

namespace MotoProfessional.Models.Interfaces
{
    public interface IDigitalGoodDownloadLog
    {
        IDigitalGood DigitalGood { get; set; }
        Guid CustomerUid { get; set; }
        string IpAddress { get; set; }
        string HttpReferrer { get; set; }
        string ClientName { get; set; }

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
    }
}