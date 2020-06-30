using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Drawing;
using System.IO;
using System.Xml;
using MotoProfessional.Models.Interfaces;
using MotoProfessional.Models.MetaDataParsers;
using MPN.Framework.Files;
using MPN.Framework.Imaging;

namespace MotoProfessional.Models
{
	public class Photo : CommonBase, IPhoto
	{
		#region members
		private List<ICollection> _collections;
		private long _views;
		#endregion

		#region accessors
		public string Filename { get; set; }
		public string Name { get; set; }
		public string Comment { get; set; }
		public ITagCollection Tags { get; set; }
		/// <summary>
		/// The size of the photo file in bytes.
		/// </summary>
		public long Filesize { get; set; }
		public Size Size { get; set; }
		public PhotoOrientation Orientation { get; set; }
		public IPartner Partner { get; set; }
		public GeneralStatus Status { get; set; }
		public IRateCard RateCard { get; set; }
		public DateTime Captured { get; set; }
		public IMember Photographer { get; set; }

		/// <summary>
		/// Returns the length of the primary-dimension for the photo, i.e. the longest side.
		/// </summary>
		public int PrimaryDimension { get { return (Size.Width > Size.Height) ? Size.Width : Size.Height; } }

		/// <summary>
		/// If present, this represents a cached sub-set of the meta-data taken from the photo file itself. Can be used
		/// to help automatically assign titles, description and tags. Use the RetrieveMetaData() method to update it or retrieve it.
		/// </summary>
		public IMetaData MetaData { get; set; }

		/// <summary>
		/// The full file or unc path to the photo file. Access type determined by application configuration.
		/// </summary>
		public string FullStorePath { get { return RootPhotoStorePath + Filename; } }
        /// <summary>
        /// Collections that this Photo is a member of.
        /// </summary>
        public List<ICollection> Collections
        {
            get
            {
                if (_collections == null)
                    ReloadCollections();

                return _collections;
            }
            set
            {
                _collections = value;
            }
        }
        public List<ICollection> ActiveCollections
        {
            get
            {
                return (from c in Collections where c.Status == GeneralStatus.Active select c).ToList();
            }
        }
        /// <summary>
        /// The root folder where the original image and any derivatives are stored.
        /// </summary>
        private string RootPhotoStorePath
        {
            get
            {
                var d = Created;
                return string.Format(@"{0}{1}\{2}\{3}\{4}\p\", ConfigurationManager.AppSettings["MediaPath"], Partner.Id, d.Year, d.Month, d.Day);
            }
        }
		/// <summary>
		/// Indicates how many times this photo has been viewed by customers.
		/// </summary>
		public long Views { get { return _views; } internal set { _views = value; } }
		#endregion

		#region static accessors
		public static DomainObject DomainObjectType { get { return DomainObject.Photo; } }
		#endregion

		#region constructors
		internal Photo(ClassMode mode)
        {
            if (mode == ClassMode.Existing)
                IsPersisted = true;

            DomainObject = DomainObjectType;
			Captured = DateTime.MinValue;
			Tags = new TagCollection();
        }
		#endregion

        #region public methods
		/// <summary>
		/// Determines if the Photo is valid for use and persistence.
		/// </summary>
        public bool IsValid()
        {
            if (string.IsNullOrEmpty(Filename))
                return false;

            if (string.IsNullOrEmpty(Name))
                return false;

            if (Partner == null)
                return false;

            return RateCard != null;
        }

        /// <summary>
        /// Attempts to populate the child MetaData object with data embedded in the photo file.
        /// </summary>
        public void RetrieveMetaData(bool deepRetrieval)
        {
            var xml = GetXmpXmlDocFromImage(FullStorePath);
            if (!string.IsNullOrEmpty(xml))
				ParseMetaData(xml, deepRetrieval);
        }

        /// <summary>
        /// If meta-data exists (call RetrieveMetaData() first), useful data will be assigned to the Photo. 
        /// The photo will need to be persisted afterwards.
        /// </summary>
        public void PopulateFromMetaData()
        {
            if (MetaData == null)
                return;

            if (!string.IsNullOrEmpty(MetaData.Name))
                Name = MetaData.Name;

            if (!string.IsNullOrEmpty(MetaData.Comment))
                Comment = MetaData.Comment;

            if (MetaData.Captured != DateTime.MinValue)
                Captured = MetaData.Captured;

            // try to match the photographer.
            if (!string.IsNullOrEmpty(MetaData.Creator))
            {
                foreach (var ce in from ce in Partner.Company.Employees
                                   let staffName = ce.Member.GetFullName().ToLower()
                                   where staffName == MetaData.Creator
                                   select ce)
                {
                	Photographer = ce.Member;
                	break;
                }
            }

            // add tags, don't replace.
        	if (MetaData.Tags == null) return;
        	foreach (var metaTag in MetaData.Tags.Where(metaTag => !Tags.Contains(metaTag)))
        		Tags.Add(metaTag);
        }

		/// <summary>
		/// Attempts to populate dimension, orientation and filesize properties by inspecting the photo file itself.
		/// </summary>
		public void RetrievePhotoInfo()
		{
			using (var i = Image.FromFile(FullStorePath))
			{
				if (i == null)
					return;

				Size = i.Size;
			}

			if (Size.Width > Size.Height)
				Orientation = PhotoOrientation.Landscape;
			else if (Size.Width < Size.Height)
				Orientation = PhotoOrientation.Portrait;
			else
				Orientation = PhotoOrientation.Square;

			Filesize = new FileInfo(FullStorePath).Length;
		}

	    /// <summary>
        /// If one doesn't already exist, this will create a preview image of a set size within the root photo path.
        /// </summary>
        public void MakePreviewImage(int primaryDimensionSize, bool constrainToSquare, bool resizeByWidth)
        {
            var newPath = string.Format(@"{0}{1}\{2}", RootPhotoStorePath, primaryDimensionSize, Filename);
            if (File.Exists(newPath))
                return;

            var ir = new ImageResizer();
	    	ir.SaveResizeImage(FullStorePath, newPath, primaryDimensionSize, constrainToSquare, resizeByWidth ? ImageResizer.Axis.Horizontal : ImageResizer.Axis.Undefined, null);
        }

		/// <summary>
		/// If one doesn't already exist, this will create a preview image of a set size within the root photo path.
		/// </summary>
		public void MakePreviewImage(Size size)
		{
			var primaryDimension = (size.Width > size.Height) ? size.Width : size.Height;
			var newPath = string.Format("{0}{1}\\{2}", RootPhotoStorePath, primaryDimension, Filename);
			if (File.Exists(newPath))
				return;

			var ir = new ImageResizer();
			ir.SaveResizeImage(FullStorePath, newPath, primaryDimension, false, ImageResizer.Axis.Undefined, size);
		}

        /// <summary>
        /// Returns the file-system path to a preview-image for the original image. Does not guarantee the path exists.
        /// </summary>
        public string GetPreviewImagePath(int primaryDimensionSize)
        {
            return string.Format("{0}{1}\\{2}", RootPhotoStorePath, primaryDimensionSize, Filename);
        }

		/// <summary>
		/// Returns a list of Licenses that this photo can be sold under, i.e. the photo meets the license specifications.
		/// </summary>
		public List<ILicense> GetSuitableActiveLicenses()
		{
			var pd = PrimaryDimension;
			return Controller.Instance.LicenseController.GetActiveLicenses().Where(l => l.PrimaryDimension == 9999 || pd >= l.PrimaryDimension).ToList();
		}

		/// <summary>
		/// Increments a count of how many people have viewed the photo. Should not include staff, partners or web-bots.
		/// </summary>
		public void IncrementViewCount()
		{
			if (!IsPersisted)
				return;

			// using a stored-procedure for max performance. could even be an async op?
			_views++;
			var db = new MotoProfessionalDataContext();
			db.IncrementPhotoViewCount(Id);
		}
        #endregion

        #region internal methods
        internal void ReloadCollections()
        {
            _collections = new List<ICollection>();
            var db = new MotoProfessionalDataContext();
            var ids = from cp in db.DbCollectionPhotos
                      where cp.PhotoID == Id
                      orderby cp.DbCollection.Created descending
                      select cp.CollectionID;

            foreach (var id in ids)
                _collections.Add(Controller.Instance.PhotoController.GetCollection(id));
        }

		/// <summary>
		/// Deletes any preview images off the file-store.
		/// </summary>
		public void DeletePreviews()
		{
			var di = new DirectoryInfo(RootPhotoStorePath);
			var subs = di.GetDirectories();

			foreach (var sub in subs)
			{
				var files = sub.GetFiles();
				foreach (var file in files)
				{
					if (file.Name == Filename)
						Files.DeleteFile(1000, file.FullName);
				}
			}
		}
        #endregion

        #region private methods
        /// <summary>
        /// Retrieves the RDF formatted XMP xml data that may be embedded in an image file.
        /// </summary>
        private string GetXmpXmlDocFromImage(string filename)
        {
        	const string beginCapture = "<rdf:RDF";
            const string endCapture = "</rdf:RDF>";
            var collection = string.Empty;
            var collecting = false;
            var matching = false;
            var collectionCount = 0;

			try
			{
				using (var sr = new StreamReader(filename))
				{
					while (!sr.EndOfStream)
					{
						var contents = (char)sr.Read();
						if (!matching && !collecting && contents == '<')
							matching = true;

						if (matching)
						{
							collection += contents;

							if (collection.Contains(beginCapture))
							{
								// found the begin element we can stop matching and start collecting.
								matching = false;
								collecting = true;
							}
							else if (contents == beginCapture[collectionCount++])
							{
								// we are still looking, but on track to start collecting.
								continue;
							}
							else
							{
								// false start reset everything.
								collection = string.Empty;
								matching = false;
								collecting = false;
								collectionCount = 0;
							}
						}
						else if (collecting)
						{
							collection += contents;
							if (collection.Contains(endCapture))
							{
								// we are finished found the end of the XMP data.
								break;
							}
						}
					}
				}
			}
			catch
			{
			}

            return collection;
        }

        /// <summary>
        /// Parses out useful XMP data from the xml string.
        /// </summary>
        private void ParseMetaData(string xmpXmlDoc, bool includeExtendedData)
        {
            var doc = new XmlDocument();
            try
            {
                doc.LoadXml(xmpXmlDoc);
            }
            catch (Exception ex)
            {
                Controller.Instance.Logger.LogWarning("Photo.ParseMetaData() - Could not parse XMP photo data. Data:\n" + xmpXmlDoc,ex);
                return;
            }

            // determine meta-data format.
            // -- only one supported format for now...
            MetaData = new Lightroom().ParseMetaData(doc, includeExtendedData);

            // add support for:
            // -- Photo Mechanic
        } 
        #endregion
    }
}