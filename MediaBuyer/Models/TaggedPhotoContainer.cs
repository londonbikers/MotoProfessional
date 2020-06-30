using MotoProfessional.Models.Interfaces;

namespace MotoProfessional.Models
{
    public class TaggedPhotoContainer : ITaggedPhotoContainer
    {
        #region members
        private readonly string _tag;
        private readonly ILatestPhotos _latestPhotos;
        #endregion

        #region accessors
        public string Tag { get { return _tag; } }
        /// <summary>
        /// Provides access to the latest photos with this tag.
        /// </summary>
        public ILatestPhotos LatestPhotos { get { return _latestPhotos; } }
        #endregion

        #region constructors
        public TaggedPhotoContainer(string tag)
        {
            _tag = tag;
            _latestPhotos = new LatestPhotos(300, _tag);
        }
        #endregion
    }
}