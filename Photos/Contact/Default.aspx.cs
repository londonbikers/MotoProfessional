using System;
using _masterPages;

public partial class Contact_Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ((MasterPagesRegular) Page.Master).SelectedTab = "contact-us";
    }
}