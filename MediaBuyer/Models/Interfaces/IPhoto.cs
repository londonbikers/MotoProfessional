using System;
using System.Collections.Generic;
using System.Drawing;

namespace MotoProfessional.Models.Interfaces
{
    public interface IPhoto
    {
        string Filename { get; set; }
        string Name { get; set; }
        string Comment { get; set; }
        ITagCollection Tags { get; set; }

        /// <summary>
        /// The size of the photo file in bytes.
        /// </summary>
        long Filesize { get; set; }

        Size Size { get; set; }
        PhotoOrientation Orientation { get; set; }
        IPartner Partner { get; set; }
        GeneralStatus Status { get; set; }
        IRateCard RateCard { get; set; }
        DateTime Captured { get; set; }
        IMember Photographer { get; set; }

        /// <summary>
        /// Returns the length of the primary-dimension for the photo, i.e. the longest side.
        /// </summary>
        int PrimaryDimension { get; }

        /// <summary>
        /// If present, this represents a cached sub-set of the meta-data taken from the photo file itself. Can be used
        /// to help automatically assign titles, description and tags. Use the RetrieveMetaData() method to update it or retrieve it.
        /// </summary>
        IMetaData MetaData { get; set; }

        /// <summary>
        /// The full file or unc path to the photo file. Access type determined by application configuration.
        /// </summary>
        string FullStorePath { get; }

        /// <summary>
        /// Collections that this Photo is a member of.
        /// </summary>
        List<ICollection> Collections { get; set; }

        List<ICollection> ActiveCollections { get; }

        /// <summary>
        /// Indicates how many times this photo has been viewed by customers.
        /// </summary>
        long Views { get; }

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
        /// Determines if the Photo is valid for use and persistence.
        /// </summary>
        bool IsValid();

        /// <summary>
        /// Attempts to populate the child MetaData object with data embedded in the photo file.
        /// </summary>
        void RetrieveMetaData(bool deepRetrieval);

        /// <summary>
        /// If meta-data exists (call RetrieveMetaData() first), useful data will be assigned to the Photo. 
        /// The photo will need to be persisted afterwards.
        /// </summary>
        void PopulateFromMetaData();

        /// <summary>
        /// Attempts to populate dimension, orientation and filesize properties by inspecting the photo file itself.
        /// </summary>
        void RetrievePhotoInfo();

        /// <summary>
        /// If one doesn't already exist, this will create a preview image of a set size within the root photo path.
        /// </summary>
        void MakePreviewImage(int primaryDimensionSize, bool constrainToSquare, bool resizeByWidth);

        /// <summary>
        /// If one doesn't already exist, this will create a preview image of a set size within the root photo path.
        /// </summary>
        void MakePreviewImage(Size size);

        /// <summary>
        /// Returns the file-system path to a preview-image for the original image. Does not guarantee the path exists.
        /// </summary>
        string GetPreviewImagePath(int primaryDimensionSize);

        /// <summary>
        /// Returns a list of Licenses that this photo can be sold under, i.e. the photo meets the license specifications.
        /// </summary>
        List<ILicense> GetSuitableActiveLicenses();

        /// <summary>
        /// Increments a count of how many people have viewed the photo. Should not include staff, partners or web-bots.
        /// </summary>
        void IncrementViewCount();

        void DeletePreviews();
    }
}