using System;
using System.Web.Security;
using System.Web.UI;
using App_Code;
using MotoProfessional;
using MotoProfessional.Models.Interfaces;

namespace _controls
{
	public partial class MemberDetailsEditor : UserControl
	{
		#region events
		public delegate void UpdatedDelegate(IMember member);
		public event UpdatedDelegate MemberUpdated;
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
		}

		#region event handlers
		protected void UpdateHandler(object sender, EventArgs ea)
		{
			IMember m = null;
			if (ViewState["MemberUID"] != null)
				m = ViewState["IsCurrentMember"] != null ? Helpers.GetCurrentUser() : Controller.Instance.MemberController.GetMember(Membership.GetUser(ViewState["MemberUID"]));

			if (m == null)
				return;

			m.Profile.Title = _title.Text.Trim();
			m.Profile.FirstName = _firstName.Text.Trim();
			m.Profile.MiddleName = _middleName.Text.Trim();
			m.Profile.LastName = _lastName.Text.Trim();
			m.MembershipUser.Email = _email.Text.Trim();
			m.Profile.Sex = (PersonSex)Enum.Parse(typeof(PersonSex), _sex.SelectedValue);
			m.Profile.JobTitle = _jobTitle.Text.Trim();
			m.Profile.Telephone = _telephone.Text.Trim();
			m.Profile.BillingAddress = _billingAddress.Text.Trim();
			m.Profile.BillingPostalCode = _billingPostalCode.Text.Trim();
			m.Profile.BillingCountry = Controller.Instance.PeripheralController.GetCountry(int.Parse(_billingCountryList.SelectedValue));

			Controller.Instance.MemberController.UpdateMember(m);
			Membership.UpdateUser(m.MembershipUser);

			// fire updated event.
			if (MemberUpdated != null)
				MemberUpdated(m);
		}
		#endregion

		#region public methods
		public void EditExistingMember(Guid memberUid, bool isCurrentUser)
		{
			IMember m;
			ViewState["MemberUID"] = memberUid;
			if (isCurrentUser)
			{
				ViewState["IsCurrentMember"] = true;
				m = Helpers.GetCurrentUser();
			}
			else
			{
				m = Controller.Instance.MemberController.GetMember(Membership.GetUser(memberUid));
			}

			if (m == null)
				return;

			RenderForm();

			_title.Text = m.Profile.Title;
			_firstName.Text = m.Profile.FirstName;
			_middleName.Text = m.Profile.MiddleName;
			_lastName.Text = m.Profile.LastName;
			_email.Text = m.MembershipUser.Email;
			_sex.SelectedValue = m.Profile.Sex.ToString();
			_jobTitle.Text = m.Profile.JobTitle;
			_telephone.Text = m.Profile.Telephone;
			_billingAddress.Text = m.Profile.BillingAddress;
			_billingPostalCode.Text = m.Profile.BillingPostalCode;

			if (m.Profile.BillingCountry != null)
				_billingCountryList.SelectedValue = m.Profile.BillingCountry.Id.ToString();
		}
		#endregion

		#region private methods
		private void RenderForm()
		{
			Helpers.PopulateCountryDropDown(_billingCountryList);
			_sex.DataSource = Enum.GetNames(typeof(PersonSex));
			_sex.DataBind();
			_sex.Items.FindByText(PersonSex.Unknown.ToString()).Text = "Not Supplied";
		}
		#endregion
	}
}