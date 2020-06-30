using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Web;
using MotoProfessional.Controllers.Photos;
using MotoProfessional.Models;
using MotoProfessional.Models.Interfaces;
using MPN.Framework.Caching;
using MPN.Framework.Data;
using MPN.Framework.Files;

namespace MotoProfessional.Controllers
{
    public class PhotoController
    {
        #region members
        private ILatestCollections _latestCollections;
        private IHotPhotos _hotPhotos;
		private readonly Tags _tags;
        #endregion

        #region accessors
        public ILatestCollections LatestCollections 
        { 
            get
            {
                if (_latestCollections == null)
                    _latestCollections = new LatestCollections(100);

                return _latestCollections;
            }
        	internal set
			{
				_latestCollections = value;
			}
        }
        public IHotPhotos HotPhotos 
        { 
            get
            {
                if (_hotPhotos == null)
                    _hotPhotos = new HotPhotos(HotPhotosMode.RandomFromLatestCollections, 50);

                return _hotPhotos;
            }
        	internal set
			{
				_hotPhotos = value;
			}
        }
		public Tags Tags { get { return _tags; } }
        #endregion

        #region constructors
        internal PhotoController()
        {
			_tags = new Tags();
        }
        #endregion

        #region photo methods
        public IPhoto NewPhoto()
        {
            return new Photo(ClassMode.New);
        }

		/// <summary>
		/// Retrieves a specific Photo object.
		/// </summary>
		/// <remarks>
		/// Has code similar to GetCollectionPhotos(), so keep that in sync with changes here.
		/// </remarks>
        public IPhoto GetPhoto(int id)
        {
			if (id < 1)
				return null;

            var photo = CacheManager.RetrieveItem(Photo.DomainObjectType.ToString(), id, string.Empty) as IPhoto;
            if (photo == null)
            {
                var db = new MotoProfessionalDataContext();
                var dbPhoto = db.DbPhotos.SingleOrDefault(p => p.ID == id);
				if (dbPhoto == null)
					return null;

                photo = BuildPhotoObject(dbPhoto, null);
                CacheManager.AddItem(photo, photo.DomainObject.ToString(), photo.Id, string.Empty);
            }

            return photo;
        }

        public void UpdatePhoto(IPhoto photo)
        {
            if (photo == null || !photo.IsValid())
            {
                Controller.Instance.Logger.LogWarning("UpdatePhoto() - Null or invalid Photo passed in.");
                return;
            }

            var db = new MotoProfessionalDataContext();
        	photo.LastUpdated = DateTime.Now;
            var dbPhoto = photo.IsPersisted ? db.DbPhotos.Single(p => p.ID == photo.Id) : new DbPhoto();

            dbPhoto.Name = photo.Name.Trim();
            dbPhoto.Filename = photo.Filename.Trim();
            dbPhoto.Filesize = photo.Filesize;
            dbPhoto.Comment = (!string.IsNullOrEmpty(photo.Comment)) ? photo.Comment.Trim() : null;
            dbPhoto.Width = photo.Size.Width;
            dbPhoto.Height = photo.Size.Height;
            dbPhoto.Aspect = (byte)photo.Orientation;
            dbPhoto.PartnerID = photo.Partner.Id;
            dbPhoto.RateCardID = photo.RateCard.Id;
            dbPhoto.Status = (byte)photo.Status;
            dbPhoto.Created = photo.Created;
            dbPhoto.LastUpdated = DateTime.Now;

			if (photo.Captured != DateTime.MinValue)
				dbPhoto.DateCaptured = photo.Captured;
			else
				dbPhoto.DateCaptured = null;

			if (photo.Photographer != null)
				dbPhoto.PhotographerUID = (Guid)photo.Photographer.MembershipUser.ProviderUserKey;
			else
				dbPhoto.PhotographerUID = null;

			dbPhoto.Tags = photo.Tags.Count > 0 ? photo.Tags.ToCsv() : null;

            // new.
            if (!photo.IsPersisted)
                db.DbPhotos.InsertOnSubmit(dbPhoto);

            try
            {
                db.SubmitChanges();
            }
            catch (Exception ex)
            {
                Controller.Instance.Logger.LogError("UpdatePhoto() - Main update failed.", ex);
                return;
            }

            if (!photo.IsPersisted)
            {
                photo.Id = dbPhoto.ID;
                photo.IsPersisted = true;
                CacheManager.AddItem(photo, photo.DomainObject.ToString(), photo.Id, String.Empty);
                photo.Partner.Statistics.Reset();
            }

            // kill the photos collection property to lazy reload it.
            photo.Collections = null;

			// update the cached TaggedPhotoContainer this photo belongs to.
			foreach (var tag in photo.Tags)
				CacheManager.RemoveItem(typeof(TaggedPhotoContainer).ToString(), tag);

			// tags caches need updating if this photo was removed from any tags.
			foreach (var tag in photo.Tags.RemovedTags)
				CacheManager.RemoveItem(typeof(TaggedPhotoContainer).ToString(), tag);

			photo.Tags.PersistenceReset();
        }

		/// <summary>
		/// Attempts to permenantly delete a photo from the app and file-store.
		/// </summary>
        /// <remarks>When making changes to this method, make sure you also make changes to DeleteCollection() which has similar code.</remarks>
		public bool DeletePhoto(IPhoto photo)
		{
			if (!Controller.Instance.BusinessRuleController.CanPhotoBeDeleted(photo))
				return false;

			var db = new MotoProfessionalDataContext();

			// remove from memory.
			if (photo.Collections.Count > 0)
			{
				foreach (var c in photo.Collections)
				{
					c.RemovePhoto(photo);
					UpdateCollection(c, true);
				}
			}

			photo.Collections.Clear();

            // remove the photo from any baskets.
			var photo1 = photo;
			db.DbBasketItems.DeleteAllOnSubmit(db.DbBasketItems.Where(qbi => qbi.PhotoID == photo1.Id));

			// remove from the database.
			var photo2 = photo;
			db.DbPhotos.DeleteOnSubmit(db.DbPhotos.Single(p => p.ID == photo2.Id));

			try
			{
				db.SubmitChanges();
                CacheManager.RemoveItem(Photo.DomainObjectType.ToString(), photo.Id);
			}
			catch (Exception ex)
			{
				Controller.Instance.Logger.LogError("DeletePhoto() - DB delete failed.", ex);
				return false;
			}

			// remove from the file-store.
			photo.DeletePreviews();
			Files.DeleteFile(1000, photo.FullStorePath);
            photo.Partner.Statistics.Reset();
			return true;
		}

        /// <summary>
        /// Retrieves an object representing a photo tag. Provides the latest photos for the tag as part.
        /// </summary>
        public ITaggedPhotoContainer GetPhotoTag(string tag)
        {
            tag = tag.ToLower();
            var tpc = CacheManager.RetrieveItem(typeof(ITaggedPhotoContainer).ToString(), 0, tag) as ITaggedPhotoContainer;
            if (tpc == null)
            {
            	tpc = new TaggedPhotoContainer(tag);
            	CacheManager.AddItem(tpc, tpc.GetType().ToString(), 0, tpc.Tag);
            }

        	return tpc;
        }

		/// <summary>
		/// Gets all the photos belonging to a Collection in one bulk operation. Much faster than getting them individually when initiating a Collection.
		/// </summary>
		/// <remarks>
		/// Has code similar to GetPhoto(), so keep in sync with that.
		/// </remarks>
		internal void GetCollectionPhotos(ICollection collection)
		{
			var photos = new List<ICollectionPhoto>();
			var db = new MotoProfessionalDataContext();
			var results = db.GetCollectionPhotos(collection.Id);

			foreach (var result in results)
			{
				var cp = new CollectionPhoto {Order = result.Order};
				var photo = CacheManager.RetrieveItem(Photo.DomainObjectType.ToString(), result.ID, string.Empty) as IPhoto;
				if (photo == null)
				{
					photo = BuildPhotoObject(null, result);
					CacheManager.AddItem(photo, photo.DomainObject.ToString(), photo.Id, string.Empty);
				}

				cp.Photo = photo;
				cp.IsPersisted = true;
				photos.Add(cp);
			}

			collection.Photos = photos;
			collection.PersistenceReset();
		}
        #endregion

        #region collection methods
        public ICollection NewCollection()
        {
            return new Collection(ClassMode.New);
        }

        public ICollection GetCollection(int id)
        {
            var collection = CacheManager.RetrieveItem(Collection.DomainObjectType.ToString(), id, string.Empty) as ICollection;
            if (collection == null)
            {
                var db = new MotoProfessionalDataContext();
                var dbCollection = db.DbCollections.SingleOrDefault(c => c.ID == id);
				if (dbCollection == null)
					return null;

                collection = BuildCollectionObject(dbCollection);
                CacheManager.AddItem(collection, collection.DomainObject.ToString(), collection.Id, string.Empty);
            }

            return collection;
        }

		/// <summary>
		/// Persists any changes to a new or existing Collection. Performs a surface update. Use the other
		/// overload for a complete update.
		/// </summary>
		public void UpdateCollection(ICollection collection)
		{
			UpdateCollection(collection, false);
		}

    	/// <summary>
    	/// Persists any changes to a new or existing Collection.
    	/// </summary>
    	/// <param name="collection">The photo-collection to update.</param>
    	/// <param name="completeUpdate">
    	/// Determines whether or not a complete or surface update is performed. A complete one will persist all photos
    	/// in the collection as well as the surface update. The surface update will persist only the Collection data.
    	/// </param>
    	public void UpdateCollection(ICollection collection, bool completeUpdate)
        {
            if (collection == null || !collection.IsValid())
            {
                Controller.Instance.Logger.LogWarning("UpdateCollection() - Null or invalid Collection passed in.");
                return;
            }

            var db = new MotoProfessionalDataContext();
            DbCollection dbCollection;
            var previousStatus = GeneralStatus.New;

            if (collection.IsPersisted)
            {
                // update.
                dbCollection = db.DbCollections.Single(c => c.ID == collection.Id);
                previousStatus = (GeneralStatus)dbCollection.Status;
            }
            else
            {
                // insert.
                dbCollection = new DbCollection();
            }

            dbCollection.Name = collection.Name.Trim();
            dbCollection.Description = (!string.IsNullOrEmpty(collection.Description)) ? collection.Description.Trim() : null;
            dbCollection.Status = (byte)collection.Status;
            dbCollection.PartnerID = collection.Partner.Id;
            dbCollection.Created = collection.Created;
            dbCollection.LastUpdated = DateTime.Now;

            // new.
            if (!collection.IsPersisted)
                db.DbCollections.InsertOnSubmit(dbCollection);

            try
            {
                db.SubmitChanges();
            }
            catch (Exception ex)
            {
                Controller.Instance.Logger.LogError("UpdateCollection() - Main update failed.", ex);
                return;
            }

            if (!collection.IsPersisted)
            {
                collection.Id = dbCollection.ID;
                collection.IsPersisted = true;
                CacheManager.AddItem(collection, collection.DomainObject.ToString(), collection.Id, string.Empty);
            }

			if (completeUpdate)
			{
				Tags.RebuildTagStats();
				collection.RebuildTagStats();
				collection.ReOrderPhotos();

                #region photo updates
				//-- work out new cp's.
				//-- update any cp's with new details.
				//-- delete removed cp's.
				foreach (var cp in collection.Photos)
				{
                    if (!cp.Photo.IsPersisted)
                        UpdatePhoto(cp.Photo);

                    DbCollectionPhoto dbCollectionPhoto;
                    if (cp.IsPersisted)
                    {
                        var cp1 = cp;
                        dbCollectionPhoto = db.DbCollectionPhotos.Single(qcp => qcp.CollectionID == collection.Id && qcp.PhotoID == cp1.Photo.Id);
                    }
                    else
                    {
                        dbCollectionPhoto = new DbCollectionPhoto { CollectionID = collection.Id, PhotoID = cp.Photo.Id, Created = DateTime.Now };
                        db.DbCollectionPhotos.InsertOnSubmit(dbCollectionPhoto);
                    }

                    dbCollectionPhoto.Order = cp.Order;
                    cp.IsPersisted = true;
				}

                if (collection.RemovedPhotos != null && collection.RemovedPhotos.Count > 0)
                {
                    var dbCPs = collection.RemovedPhotos.Select(cp => db.DbCollectionPhotos.Single(qcp => qcp.CollectionID == collection.Id && qcp.PhotoID == cp.Photo.Id)).ToList();
                	db.DbCollectionPhotos.DeleteAllOnSubmit(dbCPs);
                    collection.PersistenceReset();
                }
                #endregion
			}

            try
            {
                db.SubmitChanges();
            }
            catch (Exception ex)
            {
                Controller.Instance.Logger.LogError("UpdateCollection() - Persisting photos failed.", ex);

                #if DEBUG
                throw;
                #endif
            }

            if (previousStatus != GeneralStatus.Active && collection.Status == GeneralStatus.Active || previousStatus == GeneralStatus.Active && collection.Status != GeneralStatus.Active)
                collection.Partner.Statistics.Reset();

            ReloadCaches();
			collection.Partner.LatestCollections.Reload();
            collection.Partner.HotPhotos.Reload();
        }

		/// <summary>
		/// Attempts to permentantly delete a collection and all its photos from the app and file-store.
		/// </summary>
        /// <remarks>Performance might not be acceptable for large collections due to individual photo deletions. In this case, refactor to delete photos in one go from here where possible.</remarks>
		public bool DeleteCollection(ICollection collection)
		{
			if (!Controller.Instance.BusinessRuleController.CanCollectionBeDeleted(collection))
				return false;
			
			try
			{
                // delete photos.
                // -- photos can be fully deleted if they belong to no other collections.
                // -- if they belong to other collections, then only the relationship between the collection and photo can be deleted.

                // build a temporary collection of photos so we don't enumerate the collection's photos (which will change on each delete)
                var photos = collection.Photos.Select(cp => cp.Photo).ToList();
				foreach (var p in photos)
                    DeletePhoto(p);

                // remove collection from the database.
                var db = new MotoProfessionalDataContext();
				var collection1 = collection;
				db.DbCollections.DeleteOnSubmit(db.DbCollections.Single(qc => qc.ID == collection1.Id));
                db.SubmitChanges();
                
			}
			catch (Exception ex)
			{
				Controller.Instance.Logger.LogError("DeleteCollection() - DB deletes failed.", ex);
				return false;
			}

            CacheManager.RemoveItem(Collection.DomainObjectType.ToString(), collection.Id);
			ReloadCaches();
			collection.Partner.LatestCollections.Reload();
            collection.Partner.Statistics.Reset();
			return true;
		}
        #endregion

        #region search methods
    	/// <summary>
    	/// Searches for specific photos within the system that match the supplied criteria.
    	/// </summary>
    	/// <param name="property">The photo property to search against.</param>
    	/// <param name="orientation">If spercified, determines which orientation photos to return.</param>
    	/// <param name="dateCapturedFrom">The date from which the photo was captured. If supplied without an until date then just the 24hr period this date falls on will be used.</param>
    	/// <param name="dateCapturedUntil">The date up until which the photo was captured. Must be supplied with a from date.</param>
    	/// <param name="status">Allows for a specific status criteria to be met.</param>
    	/// <param name="member">If the user conducting the search is a member then supply it here for tracking.</param>
    	/// <param name="term">The term (or word) to search for.</param>
    	public IPhotoContainer FindPhotos(string term, 
										    PhotoSearchProperty property, 
										    PhotoOrientation? orientation, 
										    DateTime? dateCapturedFrom, 
										    DateTime? dateCapturedUntil, 
										    GeneralStatus? status,
										    IMember member)
		{
			var pc = new PhotoContainer();
			var db = new MotoProfessionalDataContext();
            var q = new StringBuilder();
            term = ConstructFtiStatement(Data.FilterOutSqlInjection(term));

            if (term.ToLower() == "name")
                term = string.Format("[{0}]", term);

            q.Append("SELECT TOP 300 p.ID FROM Photos p\n");
            q.Append("INNER JOIN CollectionPhotos cp on cp.PhotoID = p.ID\n");
            q.Append("INNER JOIN Collections c on c.ID = cp.CollectionID\n");
			q.AppendFormat("WHERE CONTAINS(p.{0}, '{1}')", property, term);

            if (dateCapturedFrom.HasValue && dateCapturedFrom.Value != DateTime.MinValue)
            {
                var from = dateCapturedFrom.Value.Date;
				var until = DateTime.Parse(dateCapturedFrom.Value.ToLongDateString() + " 23:59:59");
				if (dateCapturedUntil.HasValue)
					until = DateTime.Parse(dateCapturedUntil.Value.ToLongDateString() + " 23:59:59");

                q.AppendFormat(" AND p.DateCaptured BETWEEN '{0}' AND '{1}'", from, until);
            }

            if (orientation.HasValue && orientation.Value != PhotoOrientation.NotDefined)
                q.AppendFormat(" AND p.Aspect = {0}", (byte)orientation);

            if (status.HasValue)
                q.AppendFormat(" AND (c.Status = {0} AND p.Status = {0})", (byte)status);

			//q.Append("\nORDER BY p.CustomerViews DESC, p.Created DESC");
            q.Append("\nORDER BY p.Created DESC");
			var query = q.ToString();

			var ids = db.ExecuteQuery<int>(query);
            foreach (var id in ids)
                pc.AddPhoto(id);

			#region logging
			try
			{
				DateTime? logFrom = null;
				if (dateCapturedFrom.HasValue && dateCapturedFrom.Value != DateTime.MinValue)
					logFrom = dateCapturedFrom.Value;
				DateTime? logUntil = null;
				if (dateCapturedUntil.HasValue && dateCapturedUntil.Value != DateTime.MinValue)
					logUntil = dateCapturedUntil.Value;
				string ipAddress = null;
				if (HttpContext.Current != null)
					ipAddress = HttpContext.Current.Request.UserHostAddress;

				if (orientation != null)
					db.LogSearch(term, logFrom, logUntil, (byte)property, (byte)orientation, (member != null) ? member.Uid : (Guid?)null, ipAddress, pc.Count);
			}
			catch (Exception ex)
			{
				Controller.Instance.Logger.LogError("FindPhotos() - Cannot log search.", ex);
			}
			#endregion

			return pc;
		}

        public ICollectionContainer FindCollectionsByName(string name, int maxRecords, GeneralStatus? status, IPartner partner)
        {
            var cc = new CollectionContainer();
            var db = new MotoProfessionalDataContext();
            var statusVal = (status.HasValue) ? (byte)status.Value : (byte)255;

			if (partner != null)
			{
				var ids = db.FindPartnerCollectionsByName(partner.Id, name, maxRecords, statusVal);
				foreach (var result in ids)
					cc.AddCollection(result.ID);
			}
			else
			{
				var ids = db.FindCollectionsByName(name, maxRecords, statusVal);
				foreach (var result in ids)
					cc.AddCollection(result.ID);
			}

            return cc;
        }

        public ICollectionContainer FindCollectionsByTag(string tag, int maxRecords, GeneralStatus? status, IPartner partner)
        {
            var cc = new CollectionContainer();
            var db = new MotoProfessionalDataContext();
            var statusVal = (status.HasValue) ? (byte)status.Value : (byte)255;

			if (partner != null)
			{
				var ids = db.FindPartnerCollectionsByTag(partner.Id, tag, maxRecords, statusVal);
				foreach (var result in ids)
					cc.AddCollection(result.ID);
			}
			else
			{
				var ids = db.FindCollectionsByPhotoTag(tag, maxRecords, statusVal);
				foreach (var result in ids)
					cc.AddCollection(result.ID);
			}

            return cc;
        }           
        #endregion

        #region private methods
        private void ReloadCaches()
        {
			// the order is important.
			LatestCollections.Reload();
            HotPhotos.Reload();
        }

		/// <summary>
		/// Builds a Photo object from a database row record. Pass in either the DbPhoto or GetCollectionPhotosResult representation.
		/// </summary>
		private IPhoto BuildPhotoObject(DbPhoto dbPhoto, GetCollectionPhotosResult dbPhotoResult)
        {
            var photo = new Photo(ClassMode.Existing)
        	{
        		Id = (dbPhoto != null) ? dbPhoto.ID : dbPhotoResult.ID,
        		Name = (dbPhoto != null) ? dbPhoto.Name : dbPhotoResult.Name,
        		Comment = (dbPhoto != null) ? dbPhoto.Comment : dbPhotoResult.Comment,
        		Filename = (dbPhoto != null) ? dbPhoto.Filename : dbPhotoResult.Filename,
        		Tags = TagCollection.DelimitedTagsToCollection((dbPhoto != null) ? dbPhoto.Tags : dbPhotoResult.Tags),
        		Filesize = (dbPhoto != null) ? dbPhoto.Filesize : dbPhotoResult.Filesize,
        		Size = new Size((dbPhoto != null) ? dbPhoto.Width : dbPhotoResult.Width, (dbPhoto != null) ? dbPhoto.Height : dbPhotoResult.Height),
        		Partner = Controller.Instance.PartnerController.GetPartner((dbPhoto != null) ? dbPhoto.PartnerID : dbPhotoResult.PartnerID),
        		Status = (dbPhoto != null) ? (GeneralStatus) dbPhoto.Status : (GeneralStatus) dbPhotoResult.Status,
        		RateCard = Controller.Instance.LicenseController.GetRateCard((dbPhoto != null) ? dbPhoto.RateCardID : dbPhotoResult.RateCardID),
        		Views = (dbPhoto != null) ? dbPhoto.CustomerViews : dbPhotoResult.CustomerViews
        	};

			if (dbPhoto != null)
			{
				if (dbPhoto.DateCaptured.HasValue)
					photo.Captured = dbPhoto.DateCaptured.Value;
			}
			else
			{
				if (dbPhotoResult.DateCaptured.HasValue)
					photo.Captured = dbPhotoResult.DateCaptured.Value;
			}

			if (dbPhoto != null)
			{
				if (dbPhoto.PhotographerUID.HasValue)
					photo.Photographer = Controller.Instance.MemberController.GetMember(dbPhoto.PhotographerUID.Value);
			}
			else
			{
				if (dbPhotoResult.PhotographerUID.HasValue)
					photo.Photographer = Controller.Instance.MemberController.GetMember(dbPhotoResult.PhotographerUID.Value);
			}

            if (photo.Size.Width == photo.Size.Height)
                photo.Orientation = PhotoOrientation.Square;
            else if (photo.Size.Width > photo.Size.Height)
                photo.Orientation = PhotoOrientation.Landscape;
            else
                photo.Orientation = PhotoOrientation.Portrait;
            
            // base properties.
            photo.Created = (dbPhoto != null) ? dbPhoto.Created : dbPhotoResult.Created;
            photo.LastUpdated = (dbPhoto != null) ? dbPhoto.LastUpdated : dbPhotoResult.LastUpdated;

            return photo;
        }

        private ICollection BuildCollectionObject(DbCollection dbCollection)
        {
            var collection = new Collection(ClassMode.Existing)
     	    {
     		    Id = dbCollection.ID,
     		    Name = dbCollection.Name,
     		    Description = dbCollection.Description,
     		    Status = (GeneralStatus) dbCollection.Status,
     		    Partner = Controller.Instance.PartnerController.GetPartner(dbCollection.PartnerID),
     		    Created = dbCollection.Created,
     		    LastUpdated = dbCollection.LastUpdated
     	    };
        	return collection;
        }

		/// <summary>
		/// Search terms need to be processed before being handed to SQL Server. Terms need encapsulating in double-quote marks, but we
		/// need to also handle conditional statements, i.e. AND, OR and NOT.
		/// </summary>
		/// <param name="searchTerm">The search term from the user, i.e. "leon camier and crashes"</param>
		private string ConstructFtiStatement(string searchTerm)
		{
			// basic pre-formatting
			searchTerm = searchTerm.ToLower();
			searchTerm = searchTerm.Replace(" + ", " and ");
			if (searchTerm.Contains(" not ") && !searchTerm.Contains(" and not "))
				searchTerm = searchTerm.Replace(" not ", " and not ");

			// if there's no conditionals, just return encapsulated.
			if (!searchTerm.Contains(" and ") && !searchTerm.Contains(" or ") && !searchTerm.Contains(" not "))
			{
				searchTerm = string.Format("\"{0}\"", searchTerm);
				return searchTerm;
			}

			var requireQuoteMark = true;
			var statement = new StringBuilder();
			var conditionals = new[] { "and", "or", "not" };
			var parts = searchTerm.Split(char.Parse(" "));

			for (var i = 0; i < parts.Length; i++)
			{
				if (conditionals.Contains(parts[i]))
				{
					var quote = (i > 0 && !conditionals.Contains(parts[i - 1])) ? "\"" : string.Empty;
					statement.AppendFormat("{0} {1} ", quote, parts[i]);
					requireQuoteMark = true;
				}
				else
				{
					statement.AppendFormat("{0}{1}", (requireQuoteMark) ? "\"" : string.Empty, parts[i]);
					requireQuoteMark = false;

					if (i < parts.Length - 1 && !conditionals.Contains(parts[i + 1]))
						statement.Append(" ");
				}
			}
			
			statement.Append("\"");
			return statement.ToString().Replace("  ", " ");
		}
        #endregion
    }
}