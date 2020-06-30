using System;
using System.Web.UI;
using _controls;
using _masterPages;
using App_Code;
using MotoProfessional.Models.Interfaces;
using MPN.Framework;

namespace Account
{
    public partial class AccountPage : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ((MasterPagesRegular) Page.Master).SelectedTab = "account";
            _memberEditor.MemberUpdated += new MemberDetailsEditor.UpdatedDelegate(MemberEditedHandler);

            if (Helpers.GetCurrentUser().Company == null)
                _noCompanyBox.Visible = true;

            RenderDetailsSummary();
        }

        #region event handlers
        protected void EditAccountHandler(object sender, EventArgs ea)
        {
            _summaryView.Visible = false;
            _editView.Visible = true;
            _memberEditor.EditExistingMember(Guid.Empty, true);
        }

        protected void MemberEditedHandler(IMember member)
        {
            Helpers.AddUserResponse("<b>Updated!</b> - Your details have been updated.");
            Response.Redirect(Request.Url.AbsoluteUri);
        }

        protected void CancelEditHandler(object sender, EventArgs ea)
        {
            Response.Redirect(Request.Url.AbsoluteUri);
        }
        #endregion

        #region private methods
        private void RenderDetailsSummary()
        {
            var member = Helpers.GetCurrentUser();

            _username.Text = member.MembershipUser.UserName;
            _email.Text = member.MembershipUser.Email;
            _telephone.Text = (!string.IsNullOrEmpty(member.Profile.Telephone)) ? member.Profile.Telephone : "-";
        
            _name.Text = member.Profile.ToFullName();
            if (string.IsNullOrEmpty(_name.Text))
                _name.Text = "-";

            if (member.Profile.Sex != MotoProfessional.PersonSex.Unknown)
                _sex.Text = member.Profile.Sex.ToString();

            _jobTitle.Text = (!string.IsNullOrEmpty(member.Profile.JobTitle)) ? member.Profile.JobTitle : "-";
            _billingAddress.Text = Common.ToWebFormString(member.Profile.BillingAddress);
            _billingPostalCode.Text = member.Profile.BillingPostalCode;

            if (member.Profile.BillingCountry == null) return;
            _billingCountry.Text = member.Profile.BillingCountry.Name;
            _countryFlag.Visible = true;
            _countryFlag.ImageUrl = Helpers.GetCountryIconUrl(member.Profile.BillingCountry);
        }
        #endregion
    }
}