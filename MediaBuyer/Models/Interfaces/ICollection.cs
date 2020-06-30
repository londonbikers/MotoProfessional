using System;
using System.Collections.Generic;

namespace MotoProfessional.Models.Interfaces
{
    public interface ICollection
    {
        string Name { get; set; }
        string Description { get; set; }
        GeneralStatus Status { get; set; }
        IPartner Partner { get; set; }
        List<ICollectionPhoto> Photos { get; set; }
        List<ITagStat> TagStats { get; }
        int ActivePhotoCount { get; }

        /// <summary>
        /// Indicates whether or not a photo-import is currently in progress.
        /// </summary>
        bool ImportInProgress { get; set; }

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

        List<ICollectionPhoto> RemovedPhotos { get; set; }

        bool IsValid();

        /// <summary>
        /// Imports photos from a given path into the file-store and associates them with the collection.
        /// </summary>
        void ImportPhotos(string path);

        void AddPhoto(IPhoto photo);
        void RemovePhoto(IPhoto photo);

        /// <summary>
        /// Changes the order of a photo in the collection.
        /// </summary>
        void ChangePhotoOrder(IPhoto photo, int newOrder);

        /// <summary>
        /// Builds statistics for all the tags in each photo.
        /// </summary>
        void RebuildTagStats();

        /// <summary>
        /// Re-orders the photos in the collection using a specific sequence of photo id's. The first
        /// element should be the first-positioned photo.
        /// </summary>
        /// <returns>
        /// A bool indicating whether or not there were any order changes.
        /// </returns>
        bool OrderPhotos(List<int> photoIDs);

        void PersistenceReset();
        void ReOrderPhotos();
    }
}