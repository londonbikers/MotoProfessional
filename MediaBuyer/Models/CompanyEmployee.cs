using MotoProfessional.Models.Interfaces;

namespace MotoProfessional.Models
{
    public class CompanyEmployee : ICompanyEmployee
    {
        #region accessors
    	public IMember Member { get; set; }
    	public ICompany Company { get; set; }
    	public CompanyEmployeeStatus Status { get; set; }
    	#endregion

        #region constructors
        internal CompanyEmployee()
        {
        }
        #endregion
	}
}