using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using MotoProfessional.Models.Interfaces;

namespace MotoProfessional.Models
{
	public class Partner : CommonBase, IPartner
	{
		#region members
		private List<IRateCard> _rateCards;
		private ILatestCollections _latestCollections;
        private Stats _stats;
        private IHotPhotos _hotPhotos;
		#endregion

		#region accessors
		public string Name { get; set; }
		public string Description { get; set; }
		public string LogoFilename { get; set; }
		public ICompany Company { get; set; }
		public GeneralStatus Status { get; set; }

		public List<IRateCard> RateCards
		{
			get
			{
				if (_rateCards == null)
					RetrieveRateCards();

				return _rateCards;
			}
		}
        /// <summary>
        /// Retrieves the partners rate-cards which are active and usable.
        /// </summary>
        public List<IRateCard> LiveRateCards
        {
            get
            {
                return RateCards.Where(rc => rc.Status == GeneralStatus.Active).ToList();
            }
        }
		public ILatestCollections LatestCollections
		{
			get { return _latestCollections ?? (_latestCollections = new LatestCollections(100, Id)); }
		}
        public Stats Statistics 
        { 
            get { return _stats ?? (_stats = new Stats(Id)); }
        }
        public IHotPhotos HotPhotos
        {
            get { return _hotPhotos ?? (_hotPhotos = new HotPhotos(HotPhotosMode.TopSellingPhotos, 10, this)); }
        }
		private string RootFileStorePath { get { return string.Format("{0}{1}", ConfigurationManager.AppSettings["MediaPath"], Id); } }
		#endregion

		#region static accessors
		public static DomainObject DomainObjectType { get { return DomainObject.Partner; } }
		#endregion

		#region constructors
		internal Partner(ClassMode mode)
		{
			if (mode == ClassMode.Existing)
				IsPersisted = true;

			DomainObject = DomainObjectType;
		}
		#endregion

        #region public methods
        public bool IsValid()
        {
            if (string.IsNullOrEmpty(Name))
                return false;

            return true;
        }

		public void AddRateCard(IRateCard rc)
		{
			if (!RateCards.Exists(qRc => qRc.Id == rc.Id))
				RateCards.Add(rc);
		}

		public void RemoveRateCard(IRateCard rc)
		{
			RateCards.RemoveAll(qRc => qRc.Id == rc.Id);
		}

		/// <summary>
		/// Returns the full file path for the logo file. I.E "C:\Filestores\MP\1\logo.jpg"
		/// </summary>
		public string GetFullLogoFilePath()
		{
			if (string.IsNullOrEmpty(LogoFilename))
				return string.Empty;

			// requires a trailing slash after the media-path value.
			var path = string.Format("{0}\\{1}", RootFileStorePath, LogoFilename);
			return path;
		}

		public void DeleteLogoFile()
		{
			if (string.IsNullOrEmpty(LogoFilename))
				return;

			File.Delete(GetFullLogoFilePath());
			LogoFilename = null;
			Controller.Instance.PartnerController.UpdatePartner(this);
		}

		public static string GetFullLogoPath(IPartner partner, string filename)
		{
			if (string.IsNullOrEmpty(filename))
				return string.Empty;

			// requires a trailing slash after the media-path value.
			var path = string.Format("{0}{1}\\{2}", ConfigurationManager.AppSettings["MediaPath"], partner.Id, filename);
			return path;
		}
        #endregion

		#region internal methods
		/// <summary>
		/// Folders are required within the File-Store to store Partner files, i.e. logos and library photos. This method will ensure that if they don't exist, they're created.
		/// </summary>
		public void CreateFileStore()
		{
			if (Directory.Exists(RootFileStorePath)) return;
			Directory.CreateDirectory(RootFileStorePath);
			Directory.CreateDirectory(string.Format("{0}\\Incoming", RootFileStorePath));
		}
		#endregion

		#region private methods
		private void RetrieveRateCards()
		{
			_rateCards = new List<IRateCard>();
			var db = new MotoProfessionalDataContext();
			var ids = from rc in db.DbRateCards
					  where rc.PartnerID == Id
					  select rc.ID;

			foreach (var id in ids)
				_rateCards.Add(Controller.Instance.LicenseController.GetRateCard(id));
		}
		#endregion

        /// <summary>
        /// Provides basic statistical information for the Partner.
        /// </summary>
        public class Stats
        {
            #region members
            private readonly int _partnerId;
            private int _collections;
            private int _photos;
            private int _photosSold;
            private decimal _photosSoldValue;
            private DateTime _timeOfLastRebuild;
            private bool _requiresRebuild;
            private bool _overrideDelay;
            #endregion

            #region accessors
            public int Collections
            {
                get
                {
                    if (_requiresRebuild)
                        RebuildStats();

                    return _collections;
                }
            }
            public int Photos
            {
                get
                {
                    if (_requiresRebuild)
                        RebuildStats();

                    return _photos;
                }
            }
            public int PhotosSold
            {
                get
                {
                    if (_requiresRebuild)
                        RebuildStats();

                    return _photosSold;
                }
            }
            public decimal PhotosSoldValue
            {
                get
                {
                    if (_requiresRebuild)
                        RebuildStats();

                    return _photosSoldValue;
                }
            }
            #endregion

            #region constructors
            internal Stats(int partnerId)
            {
                _partnerId = partnerId;
                _requiresRebuild = true;
            }
            #endregion

            #region internal methods
            /// <summary>
            /// When a photo/collection is added or removed, or when a sale is made, these stats should be reset
            /// so that they can be updated the next time they're requested.
            /// </summary>
            internal void Reset()
            {
                Reset(false);
            }

            /// <summary>
            /// When a photo/collection is added or removed, or when a sale is made, these stats should be reset
            /// so that they can be updated the next time they're requested.
            /// </summary>
            /// <param name="overrideDelay">
            /// By default there's a built-in delay before stats are rebuilt, to keep app performance from degrading when there's
            /// a high number of library changes or sales.
            /// </param>
            internal void Reset(bool overrideDelay)
            {
                _requiresRebuild = true;
                _overrideDelay = overrideDelay;
            }
            #endregion

            #region private methods
            private void RebuildStats()
            {
            	if (!_overrideDelay && DateTime.Now - _timeOfLastRebuild < TimeSpan.FromMinutes(15D)) return;
            	_timeOfLastRebuild = DateTime.Now;
            	using (var db = new MotoProfessionalDataContext())
            	{
            		_collections = db.DbCollections.Count(q => q.PartnerID == _partnerId && q.Status == (byte)GeneralStatus.Active);
            		_photos = db.DbPhotos.Count(q => q.PartnerID == _partnerId && q.Status == (byte)GeneralStatus.Active);
            		_photosSold = db.DbOrderItems.Count(q => q.DbPhoto.PartnerID == _partnerId && q.DbOrder.ChargeStatus == (byte)ChargeStatus.Complete);

            		try
            		{
            			_photosSoldValue = db.DbOrderItems.Where(q => q.DbPhoto.PartnerID == _partnerId && q.DbOrder.ChargeStatus == (byte)ChargeStatus.Complete).Sum(q => q.SaleRate);
            		}
            		catch { }

            		_requiresRebuild = false;
            		_overrideDelay = false;
            	}
            }
            #endregion
        }
	}
}