using System;
using System.Web.Security;
using _masterPages;

public partial class Whoops : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
        var masterPage = Page.Master as MasterPagesRegular;
        masterPage.CustomBreadCrumbs = "Whoops";
        var showError = (Roles.IsUserInRole("Administrators")) ? true : false;

        #if DEBUG
        showError = true;
        #endif

	    if (!showError) return;
	    _detailBox.Visible = true;
	    _exception.Text = Server.GetLastError().ToString();
	    Server.ClearError();
	}
}