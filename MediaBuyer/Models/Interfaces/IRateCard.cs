using System;
using System.Collections.Generic;

namespace MotoProfessional.Models.Interfaces
{
    public interface IRateCard
    {
        IPartner Partner { get; set; }
        string Name { get; set; }

        /// <summary>
        /// A default and active rate-card is needed before any photos can be imported as a photo requires a rate-card.
        /// </summary>
        bool IsDefault { get; set; }

        GeneralStatus Status { get; set; }
        List<IRateCardItem> Items { get; set; }

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
        /// If the RateCard is new, this will create place-holder items inside it for it to be filled out entirely.
        /// The items consists of all the active licenses in the system. The method can be used to fill in missing
        /// licenses as well.
        /// </summary>
        void PopulateRateCardItems();

        void AddRateCardItem(IRateCardItem rci);
        void RemoveRateCardItem(IRateCardItem rci);

        /// <summary>
        /// Determines if the rate-card is valid for use and persistence.
        /// </summary>
        bool IsValid();
    }
}