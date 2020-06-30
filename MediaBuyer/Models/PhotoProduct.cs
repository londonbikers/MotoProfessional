using MotoProfessional.Models.Interfaces;

namespace MotoProfessional.Models
{
    /// <summary>
    /// A container class that encapsulates the concept of what a customer buys, i.e. a photo and license at a rate (determined from photo).
    /// </summary>
    public class PhotoProduct : IPhotoProduct
    {
		#region accessors
    	public IPhoto Photo { get; set; }
    	public ILicense License { get; set; }

    	/// <summary>
        /// Returns a CSV of the photo and license id's to act as a user-scope unique identifier for this PhotoProduct.
        /// </summary>
        public string Id { get { return string.Format("{0},{1}", Photo.Id, License.Id); } }
        public decimal Rate { get { return Controller.Instance.BusinessRuleController.GetPhotoRate(Photo, License); } }
		#endregion

		#region constructors
        internal PhotoProduct()
		{
		}

        internal PhotoProduct(IPhoto photo, ILicense license)
        {
            Photo = photo;
            License = license;
        }
		#endregion

		#region public methods
        /// <summary>
        /// Determines if this object is valid for use.
        /// </summary>
		public bool IsValid()
        {
        	return Photo != null && License != null;
        }
    	#endregion
    }
}
