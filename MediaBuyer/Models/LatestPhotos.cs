using System.Collections.Generic;
using System.Linq;
using MotoProfessional.Models.Interfaces;

namespace MotoProfessional.Models
{
	/// <summary>
	/// Provides a way to access the latest Photos within the system.
	/// </summary>
	/// <remarks>
	/// Implements the PhotoContainer class so it can hold the Photos in a lightweight 
	/// manner, merely supplying it with population functionality.
	/// </remarks>
	public class LatestPhotos : PhotoContainer, ILatestPhotos
	{
		#region members
		private readonly int _maxPhotos;
        private readonly string _tag;
		#endregion

		#region constructors
        /// <summary>
        /// Builds a collection of the latest published photos.
        /// </summary>
		internal LatestPhotos(int maxPhotos)
		{
			_maxPhotos = maxPhotos;
            Reload();
		}

        /// <summary>
        /// Builds a collection of the latest published photos for a specific tag.
        /// </summary>
        internal LatestPhotos(int maxPhotos, string tag)
        {
            _maxPhotos = maxPhotos;
            _tag = tag;
            Reload();
        }
		#endregion

		#region public methods
		/// <summary>
		/// Clears the collection and retrieves the photos fresh from the data
		/// </summary>
		public void Reload()
		{
			Clear();
			var db = new MotoProfessionalDataContext();
            IEnumerable<int> ids;

            if (string.IsNullOrEmpty(_tag))
            {
				ids = (from p in db.DbPhotos
					   join cp in db.DbCollectionPhotos on p.ID equals cp.PhotoID
					   join c in db.DbCollections on cp.CollectionID equals c.ID
					   where p.Status == (byte)GeneralStatus.Active && c.Status == (byte)GeneralStatus.Active
					   orderby p.Created descending
					   select p.ID).Distinct().Take(_maxPhotos);
            }
            else
            {
				ids = new List<int>();
				var tempIDs = db.FindPhotosByTag(_tag, _maxPhotos, (byte)GeneralStatus.Active);
				foreach (var result in tempIDs)
					(ids as List<int>).Add(result.ID);
            }

			foreach (var id in ids)
				AddPhoto(id);
		}
		#endregion
	}
}