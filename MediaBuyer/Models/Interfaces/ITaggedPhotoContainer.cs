namespace MotoProfessional.Models.Interfaces
{
    public interface ITaggedPhotoContainer
    {
        string Tag { get; }

        /// <summary>
        /// Provides access to the latest photos with this tag.
        /// </summary>
        ILatestPhotos LatestPhotos { get; }
    }
}