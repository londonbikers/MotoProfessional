using System.Collections.Generic;

namespace MotoProfessional.Models.Interfaces
{
    public interface ILatestPhotos : IPhotoContainer
    {
        /// <summary>
        /// Clears the collection and retrieves the photos fresh from the data
        /// </summary>
        void Reload();
    }
}