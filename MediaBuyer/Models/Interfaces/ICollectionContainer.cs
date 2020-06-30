using System.Collections.Generic;

namespace MotoProfessional.Models.Interfaces
{
    public interface ICollectionContainer : IEnumerable<ICollection>
    {
        /// <summary>
        /// The number of Collections within the container.
        /// </summary>
        int Count { get; }

        ICollection this[int index] { get; }
        void AddCollection(ICollection collection);
        void AddCollection(int collectionId);
        void RemoveCollection(ICollection collection);
        void RemoveCollection(int collectionId);
        bool Contains(ICollection collection);
        bool Contains(int collectionID);
        void Clear();
    }
}