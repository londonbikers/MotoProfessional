using System;
using _masterPages;

public partial class Signin_Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ((MasterPagesRegular) Page.Master).SelectedTab = "sign-in";
        (Page.Master as MasterPagesRegular).DefaultFormButton = "ctl00$ctl00$MainContentArea$PageContentArea$_login$LoginButton";
    }

    #region event handlers
    protected void OnLoginHandler(object sender, EventArgs ea)
    {
        Response.Redirect(!string.IsNullOrEmpty(Request.QueryString["ReturnURL"])
                              ? Request.QueryString["ReturnURL"]
                              : "~/account/");
    }
    #endregion
}