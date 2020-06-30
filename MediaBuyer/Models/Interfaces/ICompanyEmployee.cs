namespace MotoProfessional.Models.Interfaces
{
    public interface ICompanyEmployee
    {
        IMember Member { get; set; }
        ICompany Company { get; set; }
        CompanyEmployeeStatus Status { get; set; }
    }
}