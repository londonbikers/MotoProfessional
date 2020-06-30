using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MotoProfessional.Models.Interfaces;

namespace MotoProfessional.Models
{
	/// <summary>
	/// Represents a collection of string-based tags.
	/// </summary>
	public class TagCollection : ITagCollection
	{
		#region members
		private readonly List<string> _list;
		private List<string> _removedTags;
		#endregion

		#region accessors
		/// <summary>
		/// Returns the number of tags in the collection.
		/// </summary>
		public int Count { get { return _list.Count; } }
		/// <summary>
		/// Provides access to the internal removed-tags list.
		/// </summary>
		public List<string> RemovedTags
		{
		    get { return _removedTags; }
            set { _removedTags = value; }
		}

	    #endregion

		#region constructors
		/// <summary>
		/// Creates a new TagCollection object.
		/// </summary>
		internal TagCollection() 
		{
			_list = new List<string>();
			_removedTags = new List<string>();
		}
		#endregion

		#region public methods
		/// <summary>
		/// Allows the tag collection to be updated from a single collection of tags, i.e.
		/// works if any tags were removed and adds any new ones.
		/// </summary>
		public void UpdateFrom(List<string> tags)
		{
			// look for removed tags.
			var removedTags = (from tag in _list
			                   let tag1 = tag
			                   let foundTag = tags.Where(suppliedTag => suppliedTag.Trim() != string.Empty).Any(suppliedTag => suppliedTag.Trim().ToLower() == tag1.ToLower())
			                   where !foundTag
			                   select tag).ToList();

		    foreach (var removedTag in removedTags)
				Remove(removedTag);

			// add new tags. -- Add() will not add duplicates so we can just throw the lot in.
			foreach (var tag in tags)
				Add(tag.Trim().ToLower());
		}

        /// <summary>
        /// Allows the tag collection to be updated from a single collection of tags, i.e.
        /// works if any tags were removed and adds any new ones.
        /// </summary>
        /// <param name="tags">A comma-separated list of tags, i.e. "honda, fireblade, red".</param>
        public void UpdateFrom(string tags)
        {
            var delimiter = new[] { char.Parse(",") };
            var delimitedTags = tags.Split(delimiter).ToList();
            UpdateFrom(delimitedTags);
        }

		/// <summary>
		/// Adds a tag to the collection.
		/// </summary>
		public bool Add(string tag)
		{
			if (string.IsNullOrEmpty(tag))
				return false;

			if (Contains(tag))
				return false;

			// if this tag was already removed but is being re-added then we need to remove this reference.
			if (RemovedListContains(tag))
				RemoveRemovedTag(tag);

			_list.Add(tag);
			return true;
		}

		/// <summary>
		/// Removes a tag from the collection.
		/// </summary>
		public bool Remove(string tag)
		{
			if (string.IsNullOrEmpty(tag))
				return false;

			if (!_list.Contains(tag))
				return false;

			_removedTags.Add(tag);
			_list.Remove(tag);
			return false;
		}

		/// <summary>
		/// Determines whether or not the collection contains a specific tag.
		/// </summary>
		public bool Contains(string tag)
		{
			return !string.IsNullOrEmpty(tag) && _list.Contains(tag);
		}

		/// <summary>
		/// Removes all tags from the collection.
		/// </summary>
		public void Clear() 
		{
			_removedTags.AddRange(_list);
			_list.Clear();
		}

		/// <summary>
		/// Public default indexer.
		/// </summary>
		public string this[int index] 
		{
			get 
			{
				if (index > _list.Count - 1)				
					throw new System.IndexOutOfRangeException();

				return _list[index];
			}
			set 
			{
				_list[index] = value;
			}
		}

        /// <summary>
        /// Converts the tag collection to a comma-seperated string.
        /// </summary>
        public string ToCsv()
        {
            var tags = new StringBuilder();
            for (var i = 0; i < _list.Count; i++)
            {
				tags.Append(_list[i]);
                if (i < _list.Count - 1)
                    tags.Append(", ");
            }

            return tags.ToString();
        }
        #endregion

        #region internal methods
        /// <summary>
        /// Resets the collection after it's been persisted.
        /// </summary>
        public void PersistenceReset()
		{
			// clear all removed-tag references.
			_removedTags.Clear();
		}
		#endregion

		#region private methods
		/// <summary>
		/// Determines whether or not the removed-tags collection contains a specific instance.
		/// </summary>
		private bool RemovedListContains(string tag) 
		{
			return _removedTags.Contains(tag);
		}

		/// <summary>
		/// Removes a specific Tag from the removed-tags collection.
		/// </summary>
		private void RemoveRemovedTag(string tag) 
		{
			_removedTags.Remove(tag);
		}
		#endregion

		#region IEnumerable methods
		public IEnumerator<string> GetEnumerator()
		{
			return _list.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _list.GetEnumerator();
		}
		#endregion

		#region static methods

		/// <summary>
		/// Builds a collection of Tags from a comma-delimited string.
		/// </summary>
		/// <param name="delimitedTags">Tags in a comma-seperated format, i.e. 'honda, suzuki, yamaha'.</param>
		internal static ITagCollection DelimitedTagsToCollection(string delimitedTags)
		{
			var tags = new TagCollection();
			if (string.IsNullOrEmpty(delimitedTags))
				return tags;

			var rawTags = delimitedTags.ToLower().Split(char.Parse(","));
			foreach (var tag in rawTags)
				tags.Add(tag.Trim().ToLower());

			return tags;
		}
		#endregion
	}
}