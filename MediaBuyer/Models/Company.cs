using System;
using System.Collections.Generic;
using System.Linq;
using MotoProfessional.Models.Interfaces;

namespace MotoProfessional.Models
{
	public class Company : CommonBase, ICompany
	{
		#region members
		private IPartner _partner;
		private List<ICompanyEmployee> _employees;
		#endregion

		#region accessors
		public string Name { get; set; }
		public string Description { get; set; }
		public string Telephone { get; set; }
		public string Fax { get; set; }
		public string Address { get; set; }
		public string PostalCode { get; set; }
		public Uri Url { get; set; }

		/// <summary>
        /// If this company is also a partner then the Partner will be available here.
        /// </summary>
        public IPartner Partner 
		{ 
			get 
			{
				if (_partner == null)
					RetrievePartner();

				return _partner; 
			} 
			set { _partner = value; } 
		}

		public ChargeMethod ChargeMethod { get; set; }
		public ICountry Country { get; set; }
		public GeneralStatus Status { get; set; }

		public List<ICompanyEmployee> Employees
        { 
            get 
            {
                if (_employees == null)
                    RetrieveEmployees();

                return _employees; 
            }
            set
            {
            	_employees = value;
            } 
        }
		#endregion

		#region static accessors
		public static DomainObject DomainObjectType { get { return DomainObject.Company; } }
		#endregion

		#region constructors
		internal Company(ClassMode mode)
        {
            if (mode == ClassMode.Existing)
                IsPersisted = true;

            DomainObject = DomainObjectType;
        }
		#endregion

        #region public methods
        public bool IsValid()
        {
        	return !string.IsNullOrEmpty(Name);
        }

		public void AddEmployee(IMember member, CompanyEmployeeStatus status)
        {
            // no duplicates.
            if (Employees.Exists(ce => ce.Member.Uid == member.Uid))
                return;

            var companyEmployee = new CompanyEmployee {Company = this, Member = member, Status = status};
			Employees.Add(companyEmployee);
        }

        public void RemoveEmployee(IMember member)
        {
            Employees.RemoveAll(ce => ce.Member.Uid == member.Uid);
        }

        public bool IsEmployeeConfirmed(IMember member)
        {
            return Employees.Exists(ce => ce.Member.Uid == member.Uid && ce.Status == CompanyEmployeeStatus.Confirmed);
        }
        #endregion

        #region private methods
        private void RetrieveEmployees()
        {
            _employees = new List<ICompanyEmployee>();
			lock (_employees)
			{
				var db = new MotoProfessionalDataContext();
				var dbCompanyStaffers = from cs in db.DbCompanyStaffs
										where cs.CompanyID == Id
										select cs;

				foreach (var dbStaffer in dbCompanyStaffers)
					_employees.Add(Controller.Instance.CompanyController.BuildCompanyEmployeeObject(dbStaffer));
			}
        }

		private void RetrievePartner()
		{
			var db = new MotoProfessionalDataContext();
			var partnerId = (from p in db.DbPartners
							 where p.CompanyID == Id
							 select p.ID).FirstOrDefault();

			if (partnerId > 0)
				_partner = Controller.Instance.PartnerController.GetPartner(partnerId);
		}
        #endregion
    }
}