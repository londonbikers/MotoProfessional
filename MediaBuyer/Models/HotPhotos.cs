using System;
using System.Linq;
using MotoProfessional.Models.Interfaces;

namespace MotoProfessional.Models
{
    /// <summary>
    /// Provides easy access to a collection of hot and latest photos in the system.
    /// </summary>
    public class HotPhotos : PhotoContainer, IHotPhotos
    {
        #region members
    	private readonly IPartner _partner;
        private readonly int _maxPhotos;
        private const int CollectionsToSampleFrom = 5;
        #endregion

        #region accessors
    	/// <summary>
    	/// Sets the type of photos being selected. Can change automatically if there's no prefered photos available.
    	/// </summary>
    	public HotPhotosMode Mode { get; private set; }
    	#endregion

        #region constructors
        internal HotPhotos(HotPhotosMode mode, int maxPhotos)
        {
            Mode = mode;
            _maxPhotos = maxPhotos;
            Reload();
        }

        internal HotPhotos(HotPhotosMode mode, int maxPhotos, IPartner partner)
        {
            Mode = mode;
            _partner = partner;
            _maxPhotos = maxPhotos;
            Reload();
        }
        #endregion

        #region public methods
        public void Reload()
        {
        	if (Mode == HotPhotosMode.RandomFromLatestCollections)
            {
                ReloadFromRandomLatest();
                return;
            }

        	switch (Mode)
        	{
        		case HotPhotosMode.TopSellingPhotos:
        			if (_partner != null)
        			{
        				GetPartnerTopSellingPhotos();
        				if (Count == 0)
        					GetPartnerTopViewedPhotos();
        			}
        			else
        			{
        				GetTopSellingPhotos();
        				if (Count == 0)
        					GetTopViewedPhotos();
        			}
        			break;
        		case HotPhotosMode.TopViewedPhotos:
        			if (_partner != null)
        				GetPartnerTopViewedPhotos();
        			else
        				GetTopViewedPhotos();
        			break;
        	}
        }
    	#endregion

        #region private methods
        private void ReloadFromRandomLatest()
        {
            // take the photos from the last five galleries.
            Clear();
            var r = new Random();
            var photosPerCollection = _maxPhotos / CollectionsToSampleFrom;
            var counter = 0;

            var collections = (_partner == null) ? Controller.Instance.PhotoController.LatestCollections : _partner.LatestCollections;
            foreach (var photos in from Collection collection in collections.Take(CollectionsToSampleFrom)
                                   select (from cp in collection.Photos
                                           where cp.Photo.Status == GeneralStatus.Active
                                           select cp.Photo).ToList())
            {
                if (photos.Count > 0)
                {
                    while (counter < photosPerCollection - 1)
                    {
                        AddPhoto(photos[r.Next(0, photos.Count - 1)].Id);
                        counter++;
                    }
                }

                counter = 0;
            }
        }

        private void GetPartnerTopSellingPhotos()
        {
            Clear();
            Mode = HotPhotosMode.TopSellingPhotos;
            using (var db = new MotoProfessionalDataContext())
            {
                foreach (var r in db.GetPartnerTopSellingPhotos(_partner.Id, 10))
                    AddPhoto(r.ID);
            }
        }

        private void GetTopSellingPhotos()
        {
            Clear();
            Mode = HotPhotosMode.TopSellingPhotos;
            using (var db = new MotoProfessionalDataContext())
            {
                foreach (var r in db.GetTopSellingPhotos(10))
                    AddPhoto(r.ID);
            }
        }

        private void GetPartnerTopViewedPhotos()
        {
            Clear();
            Mode = HotPhotosMode.TopViewedPhotos;
            using (var db = new MotoProfessionalDataContext())
            {
                foreach (var r in db.GetPartnerTopViewedPhotos(_partner.Id, 10))
                    AddPhoto(r.ID);
            }
        }

        private void GetTopViewedPhotos()
        {
            Clear();
            Mode = HotPhotosMode.TopViewedPhotos;
            using (var db = new MotoProfessionalDataContext())
            {
                foreach (var r in db.GetTopViewedPhotos(10))
                    AddPhoto(r.ID);
            }
        }
        #endregion
    }
}