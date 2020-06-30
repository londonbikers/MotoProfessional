using System.Collections.Generic;

namespace MotoProfessional.Models.Interfaces
{
    public interface IPhotoContainer : IEnumerable<IPhoto>
    {
        /// <summary>
        /// The number of Photos within the container.
        /// </summary>
        int Count { get; }

        IPhoto this[int index] { get; }
        void AddPhoto(IPhoto photo);
        void AddPhoto(int photoId);
        void RemovePhoto(IPhoto photo);
        void RemovePhoto(int photoId);
        bool Contains(IPhoto photo);
        bool Contains(int photoId);
        void Clear();
    }
}