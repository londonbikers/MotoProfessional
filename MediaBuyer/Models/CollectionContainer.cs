using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MotoProfessional.Models.Interfaces;
using ICollection = MotoProfessional.Models.Interfaces.ICollection;

namespace MotoProfessional.Models
{
	/// <summary>
	/// Provides a lightweight means to hold a collection of photo collections. Internally only the ID's are stored
	/// and only upon retrieval is a whole Collection object retrieved (from cache or db).
	/// Can be derived from to provide more contextually relevant functionality, i.e. for latest collections.
	/// </summary>
	public class CollectionContainer : ICollectionContainer
	{
		#region members
		private readonly List<int> _ids;
		#endregion

		#region accessors
		/// <summary>
		/// The number of Collections within the container.
		/// </summary>
		public int Count { get { return _ids.Count; } }
		#endregion

		#region constructors
		internal CollectionContainer()
		{
			_ids = new List<int>();
		}
		#endregion

		#region indexers
		public ICollection this[int index]
		{
			get
			{
				if (index < 0 || index > _ids.Count - 1)
					throw new ArgumentOutOfRangeException();

				return Controller.Instance.PhotoController.GetCollection(_ids[index]);
			}
		}
		#endregion

		#region public methods
		public void AddCollection(ICollection collection)
		{
			if (collection == null)
				throw new ArgumentNullException("collection");

            lock (_ids)
            {
                if (!_ids.Contains(collection.Id))
                    _ids.Add(collection.Id);
            }
		}

		public void AddCollection(int collectionId)
		{
			if (collectionId < 1)
                throw new ArgumentOutOfRangeException("collectionId");

            lock (_ids)
            {
                if (!_ids.Contains(collectionId))
                    _ids.Add(collectionId);
            }
		}

		public void RemoveCollection(ICollection collection)
		{
			if (collection == null)
				throw new ArgumentNullException("collection");

            lock (_ids)
                _ids.Remove(collection.Id);
		}

		public void RemoveCollection(int collectionId)
		{
			if (collectionId < 1)
				throw new ArgumentOutOfRangeException("collectionId");

            lock (_ids)
			    _ids.Remove(collectionId);
		}

		public bool Contains(ICollection collection)
		{
            bool contains;
            lock (_ids)
			    contains = _ids.Contains(collection.Id);

            return contains;
		}

		public bool Contains(int collectionID)
		{
            bool contains;
            lock (_ids)
			    contains = _ids.Contains(collectionID);

            return contains;
		}

		public void Clear()
		{
            lock (_ids)
			    _ids.Clear();
		}
		#endregion

		#region IEnumerable methods
	    public IEnumerator<ICollection> GetEnumerator()
        {
        	return _ids.Select(id => Controller.Instance.PhotoController.GetCollection(id)).GetEnumerator();
        }

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _ids.Select(id => Controller.Instance.PhotoController.GetCollection(id)).GetEnumerator();
		}
		#endregion
	}
}