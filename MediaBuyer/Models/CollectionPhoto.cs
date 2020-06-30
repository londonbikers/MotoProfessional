using MotoProfessional.Models.Interfaces;

namespace MotoProfessional.Models
{
    /// <summary>
    /// A container-class for encapuslating a Photo and its relation to a Collection.
    /// </summary>
    public class CollectionPhoto : ICollectionPhoto
    {
        public IPhoto Photo { get; set; }
        public int Order { get; set; }
        public bool IsPersisted { get; set; }

        public CollectionPhoto()
        {
            // we're not ineriting off of CommonBase, but we still need to know if this is new or not.
            IsPersisted = false;
        }
    }
}