using System;
using System.Text;
using System.Web.Security;
using System.Web.UI;

public partial class _controls_TabbedNavigation : UserControl
{
    #region members
    private string _selectedTab;
    #endregion

    #region accessors
    public string SelectedTab 
    { 
        get { return _selectedTab; } 
        set { _selectedTab = value.ToLower(); } 
    }
    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        // quick and dirty.
        const string activeTab = " class=\"active\"";
        var root = Page.ResolveUrl("~");
        var items = new StringBuilder();

        items.AppendFormat("<li{0}><a href=\"{1}\"><span>Home</span></a></li>", SelectedTab == "home" ? activeTab : String.Empty, root);
        items.AppendFormat("<li{0}><a href=\"{1}photos/\"><span>Photos</span></a></li>", SelectedTab == "photos" ? activeTab : String.Empty, root);
        items.AppendFormat("<li{0}><a href=\"{1}photos/search/\"><span>Search</span></a></li>", SelectedTab == "search" ? activeTab : String.Empty, root);

        if (Page.User.Identity.IsAuthenticated)
            items.AppendFormat("<li{0}><a href=\"{1}account/\"><span>Account</span></a></li>", SelectedTab == "account" ? activeTab : String.Empty, root);

        items.AppendFormat("<li{0}><a href=\"{1}contact/\"><span>Contact Us</span></a></li>", SelectedTab == "contact-us" ? activeTab : String.Empty, root);

        if (!Page.User.Identity.IsAuthenticated)
        {
            items.AppendFormat("<li{0}><a href=\"{1}signin/\"><span>Sign-In</span></a></li>", SelectedTab == "sign-in" ? activeTab : String.Empty, root);
            items.AppendFormat("<li{0}><a href=\"{1}register/\"><span>Register</span></a></li>", SelectedTab == "register" ? activeTab : String.Empty, root);
        }

        if (Roles.IsUserInRole("Administrators"))
            items.AppendFormat("<li{0}><a href=\"{1}admin/\"><span>Admin</span></a></li>", SelectedTab == "admin" ? activeTab : String.Empty, root);

        _listItems.Text = items.ToString();
    }
}