using MotoProfessional.Models.Interfaces;

namespace MotoProfessional.Models
{
    /// <summary>
    /// Represents more exhaustive meta-data that can be found embedded within an image file
    /// that comes in the form of various XML schemas.
    /// </summary>
    public class ExtendedMetaData : IExtendedMetaData
    {
        #region tiff accessors
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public string XResolution { get; set; }
        public string YResolution { get; set; }
        #endregion

        #region exif accessors
        public string ExposureTime { get; set; }
        public string FNumber { get; set; }
        public string FocalLength { get; set; }
        public string Iso { get; set; }
        public string FlashFired { get; set; }
        public string FlashReturn { get; set; }
        public string FlashMode { get; set; }
        public string FlashFunction { get; set; }
        public string FlashRedEyeMode { get; set; }
        #endregion

        #region xap accessors
        public string CreatorTool { get; set; }
        #endregion

        #region aux accessors
        public string Lens { get; set; }
        #endregion

        #region constructors
        internal ExtendedMetaData()
        {
        }
        #endregion
    }
}