using System;
using App_Code;

public partial class Account_PasswordChange_Default : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
	}

	#region event handlers
	protected void ContinueHandler(object sender, EventArgs ea)
	{
		Helpers.AddUserResponse("<b>Changed!</b> - Your password has been changed.");
	}
	#endregion
}