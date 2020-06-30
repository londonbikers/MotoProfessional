using System;
using System.Web.UI;
using _masterPages;
using App_Code;

public partial class Account_Partner_Photos_Default : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
		Helpers.SecurePartnerPage();
		(Page.Master as MasterPagesRegular).SelectedTab = "account";
    }
}