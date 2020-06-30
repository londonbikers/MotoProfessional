using System;
using System.Text;
using System.Web.Security;
using System.Configuration;
using System.Web.UI.WebControls;
using _controls;
using App_Code;
using MotoProfessional;
using MotoProfessional.Models.Interfaces;
using MPN.Framework;

namespace Admin.Sales.Customers
{
    public partial class CustomerPage : System.Web.UI.Page
    {
        #region members
        private IMember _member;
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Request.QueryString["uid"]))
            {
                Helpers.AddUserResponse("<b>Error</b> - No customer ID supplied.");
                Response.Redirect(".");
            }

            _member = Controller.Instance.MemberController.GetMember(new Guid(Request.QueryString["uid"]));
            if (_member == null)
            {
                Helpers.AddUserResponse("<b>Error</b> - No such customer found!");
                Response.Redirect(".");
            }

            _memberEditor.MemberUpdated += new MemberDetailsEditor.UpdatedDelegate(MemberEditedHandler);
        
            if (!Page.IsPostBack)
                RenderForm();
        }

        #region event handlers
        protected void EditProfileHandler(object sender, EventArgs ea)
        {
            RenderProfileEditForm();
        }

        protected void EditMembershipUserHandler(object sender, EventArgs ea)
        {
            RenderBasicDetailsEditForm();
        }

        protected void CancelEditHandler(object sender, EventArgs ea)
        {
            Response.Redirect(Request.Url.AbsoluteUri);
        }

        protected void UpdateBasicDetailsHandler(object sender, EventArgs ea)
        {
            if (!Page.IsValid)
                return;
        
            _member.MembershipUser.Email = _editBasicEmail.Text.Trim();
            _member.MembershipUser.Comment = _editBasicComment.Text.Trim();
            var isApproved = (_editIsApproved.SelectedValue == "Yes") ? true : false;

            if (_editBasicIsStaff.Checked)
            {
                if (!Roles.IsUserInRole(_member.MembershipUser.UserName, "Administrators"))
                    Roles.AddUserToRole(_member.MembershipUser.UserName, "Administrators");
            }
            else
            {
                if (Roles.IsUserInRole(_member.MembershipUser.UserName, "Administrators"))
                    Roles.RemoveUserFromRole(_member.MembershipUser.UserName, "Administrators");
            }

            if (isApproved != _member.MembershipUser.IsApproved)
            {
                _member.MembershipUser.IsApproved = isApproved;
                Membership.UpdateUser(_member.MembershipUser);
            }

            Controller.Instance.MemberController.UpdateMember(_member);
            Helpers.AddUserResponse("<b>Updated!</b> - The details have been updated.");
            Response.Redirect(Request.Url.AbsoluteUri);
        }

        protected void MemberEditedHandler(IMember member)
        {
            Helpers.AddUserResponse("<b>Updated!</b> - The details have been updated.");
            Response.Redirect(Request.Url.AbsoluteUri);
        }

        /// <summary>
        /// Reverses the ASPNET Membership lockout on the account.
        /// </summary>
        protected void UnlockAccountHandler(object sender, EventArgs ea)
        {
            _member.MembershipUser.UnlockUser();
            Helpers.AddUserResponse("<b>Unlocked!</b> - The account has now been unlocked. The person will be able to sign-in again now.");
            Response.Redirect(Request.Url.AbsoluteUri);
        }

        protected void OrderRowCreatedHandler(object sender, GridViewRowEventArgs ea)
        {
            if (ea.Row.RowType != DataControlRowType.DataRow) return;
            var o = ea.Row.DataItem as IOrder;
            if (o == null)
                return;

            var link = ea.Row.FindControl("_link") as HyperLink;
            var ordered = ea.Row.FindControl("_ordered") as Literal;
            var items = ea.Row.FindControl("_items") as Literal;
            var total = ea.Row.FindControl("_total") as Literal;
            var method = ea.Row.FindControl("_method") as Literal;
            var state = ea.Row.FindControl("_state") as Literal;

            if (link != null) link.NavigateUrl = Helpers.BuildAdminLink(o);
            if (ordered != null)
                ordered.Text = o.Created.ToString(ConfigurationManager.AppSettings["ShortDateFormatString"]);
            if (items != null) items.Text = o.Items.Count.ToString();
            if (method != null) method.Text = o.ChargeMethod.ToString();
            if (total != null) total.Text = o.ChargeAmount.ToString("N2");

            switch (o.ChargeStatus)
            {
                case ChargeStatus.PartialRefund:
                    if (state != null) state.Text = "Partial Refund";
                    break;
                case ChargeStatus.Outstanding:
                    if (state != null) state.Text = "In Progress";
                    break;
                default:
                    if (state != null) state.Text = o.ChargeStatus.ToString();
                    break;
            }
        }
        #endregion

        #region private methods
        private void RenderForm()
        {
            _titlePrefix.Text = Roles.IsUserInRole(_member.MembershipUser.UserName, "Administrators") ? "Staff:" : "Customer:";
            _customerHeadingName.Text = (_member.Profile.ToFullName() != string.Empty) ? _member.Profile.ToFullName() : _member.MembershipUser.UserName;

            #region summary
            _signedIn.Text = (_member.MembershipUser.IsOnline) ? "<b>Yes</b>" : "No";
            _lastSignIn.Text = _member.MembershipUser.LastLoginDate.ToString("r");
            _registeredDate.Text = _member.MembershipUser.CreationDate.ToString("r");

            if (_member.Basket.Items.Count > 0)
            {
                var list = new StringBuilder();
                _basketContents.Text = string.Empty;

                list.Append("<ul>");
                foreach (var bi in _member.Basket.Items)
                    list.AppendFormat("<li><a href=\"{0}\" target=\"_blank\">{1}</a><br /><span class=\"Faint\">{2} (£{3})</span></li>", Helpers.BuildLink(bi.PhotoProduct.Photo), bi.PhotoProduct.Photo.Name, bi.PhotoProduct.License.Name, bi.PhotoProduct.Rate.ToString("N2"));

                list.Append("</ul>");
                list.AppendFormat("<hr />Total: £ {0}", _member.Basket.TotalValue.ToString("N2"));
                _basketContents.Text = list.ToString();
            }
            #endregion

            #region basic details
            Page.Title = "MPa: " + _member.GetFullName();
            _basicUsername.Text = _member.MembershipUser.UserName;
            _basicEmail.Text = _member.MembershipUser.Email;
            _basicEmail.NavigateUrl = "mailto:" + _member.MembershipUser.Email;
            _isApproved.Text = (_member.MembershipUser.IsApproved) ? "Yes" : "No";
            _basicIsStaff.Text = (Roles.IsUserInRole(_member.MembershipUser.UserName, "Administrators")) ? "Yes" : "No";

            if (_member.Company != null)
            {
                _companyLink.Enabled = true;
                _companyLink.Text = _member.Company.Name;
                _companyLink.NavigateUrl = Helpers.BuildAdminLink(_member.Company);
            }

            if (_member.MembershipUser.IsLockedOut)
            {
                _basicIsLockedOut.Text = "<img src=\"../../_images/silk/lock.png\" alt=\"locked\" /> <b>Yes</b> - ";
                _basicLockOutLinkBtn.Visible = true;
            }

            if (!string.IsNullOrEmpty(_member.MembershipUser.Comment))
                _basicComment.Text = Common.ToWebFormString(_member.MembershipUser.Comment);
            #endregion

            #region profile
            if (!string.IsNullOrEmpty(_member.Profile.ToFullName()))
                _profileName.Text = _member.Profile.ToFullName();

            _profileSex.Text = _member.Profile.Sex.ToString();

            if (!string.IsNullOrEmpty(_member.Profile.JobTitle))
                _profileJobTitle.Text = _member.Profile.JobTitle;

            if (!string.IsNullOrEmpty(_member.Profile.Telephone))
                _profileTelephone.Text = _member.Profile.Telephone;

            if (!string.IsNullOrEmpty(_member.Profile.BillingAddress))
                _profileBillingAddress.Text = Common.ToWebFormString(_member.Profile.BillingAddress);

            if (!string.IsNullOrEmpty(_member.Profile.BillingPostalCode))
                _profileBillingPostalCode.Text = _member.Profile.BillingPostalCode;

            if (_member.Profile.BillingCountry != null)
            {
                _profileBillingCountry.Text = _member.Profile.BillingCountry.Name;
                _profileCountryFlag.Visible = true;
                _profileCountryFlag.ImageUrl = Helpers.GetCountryIconUrl(_member.Profile.BillingCountry);
                _profileCountryFlag.AlternateText = _member.Profile.BillingCountry.Name;
            }
            #endregion

            #region orders
            if (_member.Orders.Count > 0)
            {
                _orders.DataSource = _member.Orders;
                _orders.DataBind();
                _noOrders.Visible = false;
            }
            else
            {
                _noOrders.Visible = true;
            }
            #endregion
        }

        private void RenderBasicDetailsEditForm()
        {
            _basicDetailsView.Visible = false;
            _basicDetailsEditView.Visible = true;
            _editBasicEmail.Text = _member.MembershipUser.Email;
            _editBasicComment.Text = _member.MembershipUser.Comment;
            _editIsApproved.SelectedValue = (_member.MembershipUser.IsApproved) ? "Yes" : "No";
            _editBasicIsStaff.Checked = Roles.IsUserInRole(_member.MembershipUser.UserName, "Administrators");
        }

        private void RenderProfileEditForm()
        {
            _profileDetailsView.Visible = false;
            _profileEditView.Visible = true;
            _memberEditor.EditExistingMember((Guid)_member.MembershipUser.ProviderUserKey, false);
        }
        #endregion
    }
}