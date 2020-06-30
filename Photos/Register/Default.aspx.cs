using System;
using System.Web.Security;
using _masterPages;
using App_Code;
using MPN.Framework;
using MPN.Framework.Communication;

public partial class RegisterPage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ((MasterPagesRegular) Page.Master).SelectedTab = "register";
    }

    #region event handlers
    protected void RegisterMemberHandler(object sender, EventArgs ea)
    {
        #region validation
        if (!_acceptTerms.Checked)
        {
            _customErrorMessage.Text = "* Please accept the terms & conditions before registering.";
            _customErrorMessage.Visible = true;
            return;
        }
        if (!Common.IsEmail(_email.Text.Trim()))
        {
            _customErrorMessage.Text = "* That doesn't appear to be a valid email address.";
            _customErrorMessage.Visible = true;
            return;
        }

        #endregion

        MembershipCreateStatus result;
        var m = Membership.CreateUser(_username.Text.Trim(),
                                                 _password.Text.Trim(),
                                                 _email.Text.Trim(),
                                                 null,
                                                 null,
                                                 true,
                                                 out result);
        if (m == null)
        {
            _customErrorMessage.Visible = true;
            switch (result)
            {
                case MembershipCreateStatus.DuplicateEmail:
                    _customErrorMessage.Text = "* That e-mail address is already registered with us. If you've forgotten your details, <a href='../signin/reset/'>request them here</a>.";
                    break;
                case MembershipCreateStatus.DuplicateUserName:
                    _customErrorMessage.Text = "* That username is already registered with us. If you've forgotten your details, <a href='../signin/reset/'>request them here</a>.";
                    break;
                case MembershipCreateStatus.UserRejected:
                    _customErrorMessage.Text = "* Your registration was declined, there was no reason.";
                    break;
                default:
                    {
                        _customErrorMessage.Text = "* There was a problem creating your account. We have been notified. Sorry!";
                        var debug = string.Format("Username: '{0}'\nPassword: '{1}'\nConirmation Password: '{2}'\nEmail: '{3}'\nIP: {4}", _username.Text.Trim(), _password.Text.Trim(), _confirmPassword.Text.Trim(), _email.Text.Trim(), Request.UserHostAddress);
                        Logger.LogError("Registration - MembershipUser is null.\n" + debug);
                    }
                    break;
            }
        }
        else
        {
            // log the user in.
            FormsAuthentication.SetAuthCookie(m.UserName, true);

            var args = new string[] { m.UserName };
            EmailHelper.SendMail("RegistrationConfirmation", false, m.Email, args);
            Helpers.AddUserResponse(string.Format("<b>Welcome {0}!</b> - You're all signed up!", m.UserName));
            Response.Redirect("~/account/company/?r");
        }
    }
    #endregion
}