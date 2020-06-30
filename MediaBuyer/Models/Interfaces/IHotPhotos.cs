namespace MotoProfessional.Models.Interfaces
{
    public interface IHotPhotos : IPhotoContainer
    {
        /// <summary>
        /// Sets the type of photos being selected. Can change automatically if there's no prefered photos available.
        /// </summary>
        HotPhotosMode Mode { get; }

        void Reload();
    }
}