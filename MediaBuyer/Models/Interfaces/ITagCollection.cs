using System.Collections.Generic;

namespace MotoProfessional.Models.Interfaces
{
    public interface ITagCollection : IEnumerable<string>
    {
        /// <summary>
        /// Returns the number of tags in the collection.
        /// </summary>
        int Count { get; }

        List<string> RemovedTags { get; set; }

        /// <summary>
        /// Allows the tag collection to be updated from a single collection of tags, i.e.
        /// works if any tags were removed and adds any new ones.
        /// </summary>
        void UpdateFrom(List<string> tags);

        /// <summary>
        /// Allows the tag collection to be updated from a single collection of tags, i.e.
        /// works if any tags were removed and adds any new ones.
        /// </summary>
        /// <param name="tags">A comma-separated list of tags, i.e. "honda, fireblade, red".</param>
        void UpdateFrom(string tags);

        /// <summary>
        /// Adds a tag to the collection.
        /// </summary>
        bool Add(string tag);

        /// <summary>
        /// Removes a tag from the collection.
        /// </summary>
        bool Remove(string tag);

        /// <summary>
        /// Determines whether or not the collection contains a specific tag.
        /// </summary>
        bool Contains(string tag);

        /// <summary>
        /// Removes all tags from the collection.
        /// </summary>
        void Clear();

        /// <summary>
        /// Public default indexer.
        /// </summary>
        string this[int index] { get; set; }

        /// <summary>
        /// Converts the tag collection to a comma-seperated string.
        /// </summary>
        string ToCsv();

        void PersistenceReset();
    }
}