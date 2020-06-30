namespace MotoProfessional.Models.Interfaces
{
    public interface IPhotoProduct
    {
        IPhoto Photo { get; set; }
        ILicense License { get; set; }

        /// <summary>
        /// Returns a CSV of the photo and license id's to act as a user-scope unique identifier for this PhotoProduct.
        /// </summary>
        string Id { get; }

        decimal Rate { get; }

        /// <summary>
        /// Determines if this object is valid for use.
        /// </summary>
        bool IsValid();
    }
}