using System;

namespace MotoProfessional.Models.Interfaces
{
    public interface ILicense
    {
        string Name { get; set; }

        /// <summary>
        /// A short, one sentence description of the license.
        /// </summary>
        string ShortDescription { get; set; }

        /// <summary>
        /// The full, page-long description for the license.
        /// </summary>
        string Description { get; set; }

        GeneralStatus Status { get; set; }

        /// <summary>
        /// To satisfy the license, the photo must be resized to match this primary-dimensions. 
        /// A value of 9999 represents unbounded, i.e. the original image.
        /// </summary>
        int PrimaryDimension { get; set; }

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

        /// <summary>
        /// Determines the rate of the license for a particular Photo based upon the Photo's rate-card.
        /// </summary>
        decimal GetRate(IPhoto photo);

        bool IsValid();
    }
}