using System;
using System.Configuration;
using App_Code;

public partial class ResetPasswordPage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // this page is not for signed-in members.
        if (Helpers.GetCurrentUser() != null)
            Response.Redirect("~/account/");

        _passwordRecovery.MailDefinition.From = ConfigurationManager.AppSettings["MediaPanther.Framework.Email.FromAddress"];
    }

    #region event handlers
    ///// <summary>
    ///// not in use
    ///// </summary>
    //protected void ContinueHandler(object sender, EventArgs ea)
    //{
    //    Helpers.AddUserResponse("<b>Reset!</b> - Your password has been reset and an email sent out to you. When you've got it, you can sign-in below.");
    //}
    #endregion
}