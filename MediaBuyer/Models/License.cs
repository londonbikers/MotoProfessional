using System;
using MotoProfessional.Models.Interfaces;

namespace MotoProfessional.Models
{
    public class License : CommonBase, ILicense
    {
        #region accessors
    	public string Name { get; set; }

    	/// <summary>
    	/// A short, one sentence description of the license.
    	/// </summary>
    	public string ShortDescription { get; set; }

    	/// <summary>
    	/// The full, page-long description for the license.
    	/// </summary>
    	public string Description { get; set; }

    	public GeneralStatus Status { get; set; }

    	/// <summary>
    	/// To satisfy the license, the photo must be resized to match this primary-dimensions. 
    	/// A value of 9999 represents unbounded, i.e. the original image.
    	/// </summary>
    	public int PrimaryDimension { get; set; }
    	#endregion

        #region static accessors
        public static DomainObject DomainObjectType { get { return DomainObject.License; } }
        #endregion

        #region constructors
        internal License(ClassMode mode)
        {
            if (mode == ClassMode.Existing)
                IsPersisted = true;

            DomainObject = DomainObjectType;
        }
        #endregion

        #region public methods
        /// <summary>
        /// Determines the rate of the license for a particular Photo based upon the Photo's rate-card.
        /// </summary>
        public decimal GetRate(IPhoto photo)
        {
            return Controller.Instance.BusinessRuleController.GetPhotoRate(photo, this);
        }

        public bool IsValid()
        {
        	return !string.IsNullOrEmpty(Name) || !string.IsNullOrEmpty(Description);
        }
    	#endregion
    }
}