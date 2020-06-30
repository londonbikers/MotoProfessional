using System;
using System.Configuration;
using System.Web.UI;
using App_Code;
using MPN.Framework.Communication;

public partial class AdminEmailTesterPage : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // details.
    	if (_templatePath != null)
    		_templatePath.Text = ConfigurationManager.AppSettings["MediaPanther.Framework.Email.TemplatePath"];
    	if (_templatePathIsWebPath != null)
    		_templatePathIsWebPath.Text = ConfigurationManager.AppSettings["MediaPanther.Framework.Email.TemplatePathIsWebPath"];
    	if (_fromAddress != null)
    		_fromAddress.Text = ConfigurationManager.AppSettings["MediaPanther.Framework.Email.FromAddress"];
    	if (_smtpServer != null)
    		_smtpServer.Text = ConfigurationManager.AppSettings["MediaPanther.Framework.Email.SmtpServer"];
    }

    #region event handlers
    protected void SendEmailHandler(object sender, EventArgs ea)
    {
        if (string.IsNullOrEmpty(_sendRecipient.Text) && string.IsNullOrEmpty(_sendFormCustomMessage.Text))
        {
            Helpers.AddUserResponse("<b>Error</b> - You must supply a recipient and a message.");
            Response.Redirect(Request.Url.AbsoluteUri);
        }

        var args = new[] { _sendFormCustomMessage.Text.Trim() };
        if (_sendRecipient.Text != null) EmailHelper.SendMail("EmailTest", false, _sendRecipient.Text.Trim(), args);
        Helpers.AddUserResponse("<b>Done!</b> - Mail sent.");
        Response.Redirect(Request.Url.AbsoluteUri);
    }
    #endregion
}
