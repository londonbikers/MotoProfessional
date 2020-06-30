namespace MotoProfessional.Models.Interfaces
{
    public interface ICollectionPhoto
    {
        IPhoto Photo { get; set; }
        int Order { get; set; }
        bool IsPersisted { get; set; }
    }
}