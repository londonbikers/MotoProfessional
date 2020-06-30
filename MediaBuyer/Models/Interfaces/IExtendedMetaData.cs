namespace MotoProfessional.Models.Interfaces
{
    public interface IExtendedMetaData
    {
        string Manufacturer { get; set; }
        string Model { get; set; }
        string XResolution { get; set; }
        string YResolution { get; set; }
        string ExposureTime { get; set; }
        string FNumber { get; set; }
        string FocalLength { get; set; }
        string Iso { get; set; }
        string FlashFired { get; set; }
        string FlashReturn { get; set; }
        string FlashMode { get; set; }
        string FlashFunction { get; set; }
        string FlashRedEyeMode { get; set; }
        string CreatorTool { get; set; }
        string Lens { get; set; }
    }
}