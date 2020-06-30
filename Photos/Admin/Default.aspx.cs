using System;
using System.Web.Security;

public partial class AdminDefaultPage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        _currentVisitors.Text = Membership.GetNumberOfUsersOnline().ToString();
    }
}