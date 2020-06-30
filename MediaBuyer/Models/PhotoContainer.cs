using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MotoProfessional.Models.Interfaces;

namespace MotoProfessional.Models
{
	/// <summary>
	/// Provides a lightweight means to hold a collection of photos. Internally only the ID's are stored
	/// and only upon retrieval is a whole Photo object retrieved (from cache or db).
	/// Can be derived from to provide more contextually relevant functionality, i.e. for tag photos or latest photos.
	/// </summary>
	public class PhotoContainer : IPhotoContainer
	{
		#region members
		private readonly List<int> _ids;
		#endregion

		#region accessors
		/// <summary>
		/// The number of Photos within the container.
		/// </summary>
		public int Count { get { return _ids.Count; } }
		#endregion

		#region constructors
		internal PhotoContainer()
		{
			_ids = new List<int>();
		}
		#endregion

		#region indexers
		public IPhoto this[int index]
		{
			get
			{
				if (index < 0 || index > _ids.Count - 1)
					throw new ArgumentOutOfRangeException();

				return Controller.Instance.PhotoController.GetPhoto(_ids[index]);
			}
		}
		#endregion

		#region public methods
		public void AddPhoto(IPhoto photo)
		{
			if (photo == null)
				throw new ArgumentNullException("photo");

			if (!_ids.Contains(photo.Id))
				_ids.Add(photo.Id);
		}

		public void AddPhoto(int photoId)
		{
			if (photoId < 1)
				throw new ArgumentOutOfRangeException("photoId");

			if (!_ids.Contains(photoId))
				_ids.Add(photoId);
		}

		public void RemovePhoto(IPhoto photo)
		{
			if (photo == null)
				throw new ArgumentNullException("photo");

			_ids.Remove(photo.Id);
		}

		public void RemovePhoto(int photoId)
		{
			if (photoId < 1)
				throw new ArgumentOutOfRangeException("photoId");

			_ids.Remove(photoId);
		}

		public bool Contains(IPhoto photo)
		{
			return _ids.Contains(photo.Id);
		}

		public bool Contains(int photoId)
		{
			return _ids.Contains(photoId);
		}

		public void Clear()
		{
			_ids.Clear();
		}
		#endregion

		#region IEnumerable methods
        public IEnumerator<IPhoto> GetEnumerator()
        {
        	return _ids.Select(id => Controller.Instance.PhotoController.GetPhoto(id)).GetEnumerator();
        }

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _ids.Select(id => Controller.Instance.PhotoController.GetPhoto(id)).GetEnumerator();
		}
		#endregion
	}
}