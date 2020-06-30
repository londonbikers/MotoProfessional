using System;
using System.Linq;
using System.Web.UI.WebControls;
using _controls;
using _masterPages;
using App_Code;
using MotoProfessional;
using MotoProfessional.Models.Interfaces;
using MPN.Framework;

namespace Account.Company
{
    public partial class CompanyPage : System.Web.UI.Page
    {
        #region members
        private IMember _member;
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            var master = Page.Master as MasterPagesRegular;
            if (master != null)
            {
                master.SelectedTab = "account";
                master.DefaultFormButton = _companySearchBtn.UniqueID;
            }

            // company editor events.
            _companyEditor.CompanyUpdated += new CompanyDetailsEditor.UpdatedDelegate(CompanyCreatedHandler);
            _editCompanyEditor.CompanyUpdated += new CompanyDetailsEditor.UpdatedDelegate(CompanyEditedHandler);
                
            _member = Helpers.GetCurrentUser();
            if (_member.Company != null)
            {
                if (_member.Company.IsEmployeeConfirmed(_member))
                    RenderCompanyAssignedForm();
                else
                    RenderNotConfirmedForm();
            }
            else
            {
                RenderNoCompanyAssignedForm();
            }
        }

        #region event handlers
        protected void CompanySearchHandler(object sender, EventArgs ea)
        {
            _searchResults.Visible = true;
            _resultsGrid.DataSource = Controller.Instance.CompanyController.GetCompaniesByLooseName(_searchBox.Text.Trim(), 50);
            _resultsGrid.DataBind();

            if (_resultsGrid.Rows.Count == 0)
            {
                _searchResults.Visible = false;
                _newCompany.Visible = true;
                _companyEditor.Name = _searchBox.Text.Trim();
            }
            else
            {
                _newCompany.Visible = false;
            }
        }

        protected void JoinCompanyHandler(object sender, GridViewCommandEventArgs ea)
        {
            var selectedRow = ((GridView) ea.CommandSource).Rows[Convert.ToInt32(ea.CommandArgument)];
            var idField = selectedRow.Cells[0].FindControl("_companyID") as HiddenField;
            if (idField != null)
            {
                var company = Controller.Instance.CompanyController.GetCompany(Convert.ToInt32(idField.Value));
                Helpers.GetCurrentUser().AddToCompany(company, false);
            }

            Helpers.AddUserResponse("<b>Done!</b> - One of your collegues will need to confirm this before you can make a purchase.");
            Response.Redirect(Request.Url.AbsoluteUri);
        }

        protected void RemovePendingCompanyHandler(object sender, EventArgs ea)
        {
            Helpers.GetCurrentUser().RemoveFromCompany();
            Helpers.AddUserResponse("<b>Done!</b> - We've canceled your pending joining of the team.");
            Response.Redirect(Request.Url.AbsoluteUri);
        }

        /// <summary>
        /// Handles the company being created event from the company details editor control.
        /// </summary>
        protected void CompanyCreatedHandler(ICompany company)
        {
            Helpers.GetCurrentUser().AddToCompany(company, true);
            Helpers.AddUserResponse("<b>Registered!</b> - Your company or team is now registered.");
            Response.Redirect(Request.Url.AbsoluteUri);
        }

        /// <summary>
        /// Handles the company being updated event from the company details editor control.
        /// </summary>
        protected void CompanyEditedHandler(ICompany company)
        {
            Helpers.AddUserResponse("<b>Updated!</b> - Your company or team details have been updated.");
            Response.Redirect(Request.Url.AbsoluteUri);
        }

        protected void EditCompanyDetailsHandler(object sender, EventArgs ea)
        {
            _confirmedAssociation.Visible = false;
            _editConfirmedCompany.Visible = true;
            _editCompanyEditor.EditExistingCompany(_member.Company.Id, false);
        }

        protected void EmployeeItemCreatedHandler(object sender, RepeaterItemEventArgs ea)
        {
            if (ea.Item.ItemType != ListItemType.Item && ea.Item.ItemType != ListItemType.AlternatingItem) return;
            var ce = ea.Item.DataItem as ICompanyEmployee;
            if (ce == null)
                return;
            
            var name = ea.Item.FindControl("_employeeName") as Label;
            var authView = ea.Item.FindControl("_authControlView") as PlaceHolder;
            var confirmBtn = ea.Item.FindControl("_confirmAuthBtn") as LinkButton;
            var refuseBtn = ea.Item.FindControl("_refuseAuthBtn") as LinkButton;

            if (name != null) name.Text = ce.Member.GetFullName();
            if (ce.Status != CompanyEmployeeStatus.Pending) return;
            if (authView != null) authView.Visible = true;
            if (confirmBtn != null) confirmBtn.CommandArgument = ce.Member.Uid.ToString();
            if (refuseBtn != null) refuseBtn.CommandArgument = ce.Member.Uid.ToString();
        }

        protected void CancelEditHandler(object sender, EventArgs ea)
        {
            Response.Redirect(Request.Url.AbsoluteUri);
        }

        protected void ProcessPendingUserHandler(object sender, CommandEventArgs ea)
        {
            if (ea.CommandArgument == null)
                return;

            var memberUid = new Guid(ea.CommandArgument.ToString());
            if (memberUid == Guid.Empty)
                return;

            var ce = Helpers.GetCurrentUser().Company.Employees.Find(qce => qce.Member.Uid == memberUid);
            if (ce == null)
                return;

            switch (ea.CommandName)
            {
                case "Confirm":
                    ce.Status = CompanyEmployeeStatus.Confirmed;
                    Helpers.AddUserResponse(string.Format("<b>Done!</b> - {0} has been confirmed as part of your company/team.", ce.Member.GetFullName()));
                    break;
                case "Refuse":
                    ce.Company.RemoveEmployee(ce.Member);
                    Helpers.AddUserResponse(string.Format("<b>Done!</b> - {0} has been removed from your company/team.", ce.Member.GetFullName()));
                    break;
            }

            Controller.Instance.CompanyController.UpdateCompany(ce.Company);
            Response.Redirect("./");
        }
        #endregion

        #region private methods
        private void RenderNoCompanyAssignedForm()
        {
            _noAssociation.Visible = true;
        }

        private void RenderNotConfirmedForm()
        {
            // give the user the ability to leave the pending company incase they've made a mistake.
            _pendingAssociation.Visible = true;
            _pendingCompanyName.Text = _member.Company.Name;
        }

        private void RenderCompanyAssignedForm()
        {
            _confirmedAssociation.Visible = true;

            // required properties.
            _detailsName.Text = _member.Company.Name;
            _detailsTelephoneNumber.Text = _member.Company.Telephone;

            // optional properties.
            if (!string.IsNullOrEmpty(_member.Company.Description))
                _detailsDescription.Text = _member.Company.Description;

            if (!string.IsNullOrEmpty(_member.Company.Fax))
                _detailsFaxNumber.Text = _member.Company.Fax;

            if (!string.IsNullOrEmpty(_member.Company.Address))
                _detailsAddress.Text = Common.ToWebFormString(_member.Company.Address);

            if (!string.IsNullOrEmpty(_member.Company.PostalCode))
                _detailsPostalCode.Text = _member.Company.PostalCode;

            if (_member.Company.Country != null)
            {
                _detailsCountry.Text = _member.Company.Country.Name;
                _countryFlag.Visible = true;
                _countryFlag.ImageUrl = Helpers.GetCountryIconUrl(_member.Company.Country);
            }

            if (_member.Company.Url != null)
            {
                _detailsNoWebsite.Visible = false;
                _detailsWebsiteLink.Visible = true;
                _detailsWebsiteLink.NavigateUrl = _member.Company.Url.AbsoluteUri;
                _detailsWebsiteLink.Text = _member.Company.Url.Host;
            }

            switch (_member.Company.ChargeMethod)
            {
                case ChargeMethod.PointOfSale:
                    _paymentTerms.Text = "Point of sale";
                    _paymentTermsDescription.Text = "Payment is required at the checkout by credit/debit-card or Google Checkout account.";
                    break;

                case ChargeMethod.Invoiced:
                    _paymentTerms.Text = "Invoice";
                    _paymentTermsDescription.Text = "Your orders will be payable at the end of the calendar month.";
                    break;

                case ChargeMethod.NoCharge:
                    _paymentTerms.Text = "No Charge";
                    _paymentTermsDescription.Text = "As a special client, there are no charge for your orders and you can download straight-away.";
                    break;
            }

            // employees.
            _employees.DataSource = from ce in _member.Company.Employees orderby ce.Status select ce;
            _employees.DataBind();
        }
        #endregion
    }
}