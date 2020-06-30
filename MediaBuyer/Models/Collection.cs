using System;
using System.IO;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Threading;
using MotoProfessional.Exceptions;
using MotoProfessional.Models.Interfaces;
using MPN.Framework.Files;

namespace MotoProfessional.Models
{
	/// <summary>
	/// Represents a collection of photos.
	/// </summary>
    public class Collection : CommonBase, ICollection
	{
        #region members
		private List<ICollectionPhoto> _photos;
		private List<ICollectionPhoto> _removedPhotos;
		private List<ITagStat> _tagStats;
		#endregion

        #region accessors
		public string Name { get; set; }
		public string Description { get; set; }
		public GeneralStatus Status { get; set; }
		public IPartner Partner { get; set; }
		public List<ICollectionPhoto> Photos
        {
            get
            {
				if (_photos == null)
					Controller.Instance.PhotoController.GetCollectionPhotos(this);

                return _photos;
            }
            set { _photos = value; }
        }
		public List<ICollectionPhoto> RemovedPhotos
		{
		    get { return _removedPhotos ?? (_removedPhotos = new List<ICollectionPhoto>()); }
		    set { _removedPhotos = value; }
		}
	    public List<ITagStat> TagStats
		{
			get
			{
				if (_tagStats == null)
					RebuildTagStats();

				return _tagStats;
			}
		}
        public int ActivePhotoCount
        {
            get
            {
				return Photos.Count(qcp => qcp.Photo.Status == GeneralStatus.Active);
            }
        }
		/// <summary>
		/// Indicates whether or not a photo-import is currently in progress.
		/// </summary>
		public bool ImportInProgress { get; set; }
        #endregion

        #region static accessors
        public static DomainObject DomainObjectType { get { return DomainObject.Collection; } }
        #endregion

        #region constructors
        internal Collection(ClassMode mode)
        {
            if (mode == ClassMode.Existing)
                IsPersisted = true;

            DomainObject = DomainObjectType;
			ImportInProgress = false;
        }
        #endregion

        #region public methods
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Name);
        }

	    /// <summary>
        /// Imports photos from a given path into the file-store and associates them with the collection.
        /// </summary>
        public void ImportPhotos(string path)
        {
			var queued = ThreadPool.QueueUserWorkItem(PerformImport, path);
			if (!queued)
				Controller.Instance.Logger.LogError("Photo.ImportPhotos() - Work could not be queued.");
        }

		public void AddPhoto(IPhoto photo)
		{
			if (photo.IsPersisted && Photos.Exists(qCp => qCp.Photo.Id == photo.Id))
				return;

			if (photo.RateCard == null)
			{
				// assign default rate-card.
				photo.RateCard = Partner.RateCards.Single(rc => rc.Status == GeneralStatus.Active && rc.IsDefault);
			}

			var cp = new CollectionPhoto {Photo = photo, Order = (Photos.Count > 0) ? Photos.Max(qCp => qCp.Order) + 1 : 1};
			Photos.Add(cp);
		}

		public void RemovePhoto(IPhoto photo)
		{
			var cp = Photos.SingleOrDefault(qCp => qCp.Photo.Id == photo.Id);
			if (cp == null) return;
			Photos.Remove(cp);
			RemovedPhotos.Add(cp); // null!!
		}

        /// <summary>
        /// Changes the order of a photo in the collection.
        /// </summary>
        public void ChangePhotoOrder(IPhoto photo, int newOrder)
        {
            if (!Photos.Exists(qcp => qcp.Photo.Id == photo.Id))
                throw new ArgumentException("Photo is not in the Collection!");

            var cp = Photos.Single(qcp => qcp.Photo.Id == photo.Id);

            if (Photos.Count == 1)
            {
                cp.Order = 1;
                ReOrderPhotos();
                return;
            }

            if (newOrder > Photos.Count - 1)
            {
                // out of bounds. just set as the new end photo.
                cp.Order = newOrder;
                ReOrderPhotos();
                return;
            }

            // find current holder of the new-order and shuffle photos up.
            for (var i = (newOrder - 1); i < Photos.Count; i++)
                Photos[i].Order = i + 2;

            cp.Order = newOrder;
            ReOrderPhotos();
        }

		/// <summary>
		/// Builds statistics for all the tags in each photo.
		/// </summary>
		public void RebuildTagStats()
		{
			_tagStats = new List<ITagStat>();
			foreach (var cp in Photos.Where(qcp => qcp.Photo.Status == GeneralStatus.Active))
			{
				foreach (var tag in cp.Photo.Tags)
				{
					var tag1 = tag;
					var ts = _tagStats.SingleOrDefault(qts => qts.Tag == tag1);
					if (ts != null)
						ts.Count++;
					else
						_tagStats.Add(new TagStat(tag));
				}
			}

			// sort the tag-stats.
			_tagStats.Sort((t1, t2) => t2.Count.CompareTo(t1.Count));

			// sort by name.
			_tagStats.Sort((t1, t2) => t1.Tag.CompareTo(t2.Tag));

			Controller.Instance.PhotoController.Tags.RebuildTagStats();
		}

		/// <summary>
		/// Re-orders the photos in the collection using a specific sequence of photo id's. The first
		/// element should be the first-positioned photo.
		/// </summary>
        /// <returns>
        /// A bool indicating whether or not there were any order changes.
        /// </returns>
		public bool OrderPhotos(List<int> photoIDs)
		{
            if (photoIDs == null)
                return false;

            var hasChanged = false;
			CollectionPhoto cp;
			for (var i = 0; i < photoIDs.Count; i++)
			{
				cp = (CollectionPhoto) Photos.SingleOrDefault(qcp => qcp.Photo.Id == photoIDs[i]);
				if (cp == null)
					continue;

                var oldOrder = cp.Order;
				cp.Order = i + 1;

                if (cp.Order != oldOrder)
                    hasChanged = true;
			}

			// do a normal re-order in case there were some oddities.
			ReOrderPhotos();
            return hasChanged;
		}
        #endregion

        #region internal methods
        /// <summary>
        /// The CollectionPhoto orders can lose their sequence if a photo is removed, so before being 
        /// persisted this method will re-order them.
        /// </summary>
        public void ReOrderPhotos()
		{
            var count = 0;
            foreach (var cp in Photos.OrderBy(qcp => qcp.Order))
            {
                cp.Order = count + 1;
                count++;
            }

            Photos = Photos.OrderBy(qcp => qcp.Order).ToList();
		}

		/// <summary>
		/// Resets the Collection after it's been persisted.
		/// </summary>
		public void PersistenceReset()
		{
			// clear all removed collection-photo references.
			if (_removedPhotos != null)
				_removedPhotos.Clear();
		}
		#endregion

		#region private methods
		/// <summary>
		/// If a problem is encountered during the import operation then this will move the files back to their original location.
		/// </summary>
		private void RollbackImportPhotos(IEnumerable<string> files, string storePath)
		{
			try
			{
				foreach (var file in files)
				{
					try
					{
						var filename = Path.GetFileName(file);
						var fullStorePath = storePath + filename;

						// file might not have been moved yet.
						if (!File.Exists(fullStorePath))
							continue;

						File.Move(fullStorePath, file);

						// remove any Photo object from collection.
						if (Photos.Exists(cp => cp.Photo.Filename == filename))
							Photos.Remove(Photos.Single(cp => cp.Photo.Filename == filename));
					}
					catch (Exception ex)
					{
						Controller.Instance.Logger.LogError("Collection.RollbackImportPhotos() - Rollback of a file failed:\n" + file, ex);
					}
				}
			}
			catch (Exception ex)
			{
				Controller.Instance.Logger.LogError("Collection.RollbackImportPhotos() - Rollback failed.", ex);
			}
		}

		/// <summary>
		/// Performs the actual photo import process. Designed to be done asyncronously so fire off on a new thread.
		/// </summary>
		/// <param name="untypedPath">The source path for the photos.</param>
		private void PerformImport(object untypedPath)
		{
			ImportInProgress = true;
			var path = untypedPath as string;

			// check collection is persisted.
			if (!IsPersisted)
				throw new Exception("Collection not persisted!");

			// check for path.
			if (!Directory.Exists(path))
				throw new ArgumentException("Path does not exist!");

			// time the operation.
			var timer = new Timer("Collection.ImportPhotos() path: " + path);

			// check for any matching photos. -- should be configurable
			if (path != null)
			{
				var filenames = from f in Directory.GetFiles(path)
				                where f.ToLower().EndsWith(".jpg") ||
				                      f.ToLower().EndsWith(".jpeg") ||
				                      f.ToLower().EndsWith(".png") ||
				                      f.ToLower().EndsWith(".gif") ||
				                      f.ToLower().EndsWith(".bmp") ||
				                      f.ToLower().EndsWith(".hdp")
				                select f;

				if (filenames.Count() == 0)
					throw new ArgumentException("No suitable photos at supplied path!\nPath: " + path);

				// setup root store path.
				// as: {media-store}\{partner id}\{year}\{month}\{day}\{media type prefix}\
				var d = DateTime.Now;
				var storePath = string.Format("{0}{1}\\{2}\\{3}\\{4}\\p\\", ConfigurationManager.AppSettings["MediaPath"], Partner.Id, d.Year, d.Month, d.Day);
				if (!Directory.Exists(storePath))
					Directory.CreateDirectory(storePath);

				var importSuccessful = true;
				var storeFullPath = string.Empty;
				foreach (var filename in filenames)
				{
					// ensure unique filename.
					var storeFileName = Files.GetUniqueFilename(storePath, Path.GetFileName(filename));

					try
					{
						// move file to store.
						storeFullPath = storePath + storeFileName;
						File.Move(filename, storeFullPath);

						// build photo.
						var p = Controller.Instance.PhotoController.NewPhoto();
						p.Filename = storeFileName;
						p.Name = Files.GetFriendlyNameFromFilename(Path.GetFileNameWithoutExtension(p.Filename));
						p.Partner = Partner;
						p.Status = GeneralStatus.Active;
						p.RetrievePhotoInfo();
						p.RetrieveMetaData(false);
						p.PopulateFromMetaData();

						// preview image creation.
						p.MakePreviewImage(150, false, false); // regular thumbnails
						p.MakePreviewImage(100, true, false); // smaller thumbnails
						p.MakePreviewImage(74, true, false); // mini thumbnails
						p.MakePreviewImage(621, false, false); // main preview
						p.MakePreviewImage(new Size(620, 230)); // homepage preview

						// associate with collection.
						AddPhoto(p);
					}
					catch (Exception ex)
					{
						Controller.Instance.Logger.LogError(string.Format("Collection.ImportPhotos() - Failed on move process.\nfilename: {0}\nstoreFullPath: {1}", filename, storeFullPath), ex);
						importSuccessful = false;
						RollbackImportPhotos(filenames, storePath);
						var pie = new PhotoImportException("Photo import failed. We'll look into it. Sorry!");
						throw pie;
					}
				}

				// update collection.
				Controller.Instance.PhotoController.UpdateCollection(this, true);

				try
				{
					// delete the import folder if it's not the root incoming one.
					if (importSuccessful && !path.ToLower().EndsWith("\\incoming\\"))
						Directory.Delete(path, true);
				}
				catch (Exception ex)
				{
					Controller.Instance.Logger.LogError("Collection.ImportPhotos() - Cannot delete source folder: " + path, ex);
				}
			}

			ImportInProgress = false;
			timer.Stop();
		}
        #endregion
    }
}