using System;
using System.Collections.Generic;

namespace MotoProfessional.Models.Interfaces
{
    public interface IPartner
    {
        string Name { get; set; }
        string Description { get; set; }
        string LogoFilename { get; set; }
        ICompany Company { get; set; }
        GeneralStatus Status { get; set; }
        List<IRateCard> RateCards { get; }

        /// <summary>
        /// Retrieves the partners rate-cards which are active and usable.
        /// </summary>
        List<IRateCard> LiveRateCards { get; }
        ILatestCollections LatestCollections { get; }
        Partner.Stats Statistics { get; } // <--- OH, CAN'T MAKE SUB-CLASS AN INTERFACE...
        IHotPhotos HotPhotos { get; }

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
        void AddRateCard(IRateCard rc);
        void RemoveRateCard(IRateCard rc);

        /// <summary>
        /// Returns the full file path for the logo file. I.E "C:\Filestores\MP\1\logo.jpg"
        /// </summary>
        string GetFullLogoFilePath();

        void DeleteLogoFile();
        void CreateFileStore();
    }
}