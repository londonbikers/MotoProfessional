using System;
using System.Web.UI;
using App_Code;
using MotoProfessional;
using MotoProfessional.Models.Interfaces;
using MPN.Framework;

namespace _controls
{
	public partial class CompanyDetailsEditor : UserControl
	{
		#region events
		public delegate void UpdatedDelegate(ICompany company);
		public event UpdatedDelegate CompanyUpdated;
		#endregion

		#region accessors
		/// <summary>
		/// If you need to pre-poplate the name field, then supply it here.
		/// </summary>
		public string Name { get { return _name.Text.Trim(); } set { _name.Text = value; } }
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Page.IsPostBack)
				RenderForm();
		}

		#region event handlers
		protected void UpdateCompanyHandler(object sender, EventArgs ea)
		{
			if (!Page.IsValid)
				return;

		    var c = ViewState["CompanyID"] != null ? Controller.Instance.CompanyController.GetCompany(Convert.ToInt32(ViewState["CompanyID"])) : Controller.Instance.CompanyController.NewCompany();
			c.Name = _name.Text.Trim();
			c.Telephone = _telephone.Text.Trim();
			c.Fax = _fax.Text.Trim();
			c.Description = _description.Text.Trim();
			c.Address = _postalAddress.Text.Trim();
			c.PostalCode = _postalCode.Text.Trim();
			c.Country = Controller.Instance.PeripheralController.GetCountry(Convert.ToInt32(_countryList.SelectedValue));
			c.Url = Common.TryUrlParse(_website.Text.Trim());

			if (!string.IsNullOrEmpty(_status.SelectedValue))
				c.Status = (GeneralStatus)Enum.Parse(typeof(GeneralStatus), _status.SelectedValue);

			Controller.Instance.CompanyController.UpdateCompany(c);

			// fire updated event.
			if (CompanyUpdated != null)
				CompanyUpdated(c);
		}
		#endregion

		#region public methods
		public void EditExistingCompany(int companyId, bool isAdminEdit)
		{
			ViewState["CompanyID"] = companyId;
			var c = Controller.Instance.CompanyController.GetCompany(companyId);
			if (c == null)
				return;

			_name.Text = c.Name;
			_telephone.Text = c.Telephone;
			_fax.Text = c.Fax;
			_description.Text = c.Description;
			_postalAddress.Text = c.Address;
			_postalCode.Text = c.PostalCode;
			_countryList.SelectedValue = c.Country.Id.ToString();
			_website.Text = c.Url != null ? c.Url.AbsoluteUri : string.Empty;

			if (isAdminEdit)
			{
				_statusRow.Visible = true;
				_status.DataSource = Enum.GetNames(typeof(GeneralStatus));
				_status.DataBind();
				_status.SelectedValue = c.Status.ToString();
			}
			else
			{
				_statusRow.Visible = false;
			}

			_updateBtn.Text = "Update";
		}
		#endregion

		#region private methods
		private void RenderForm()
		{
			Helpers.PopulateCountryDropDown(_countryList);
		}
		#endregion
	}
}