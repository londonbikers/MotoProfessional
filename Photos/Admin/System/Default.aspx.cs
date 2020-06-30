using System;
using System.Web.UI;
using App_Code;
using MotoProfessional;
using MotoProfessional.Models;

public partial class AdminSystemPage : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Page.IsPostBack) return;
        _cacheItemsGrid.DataSource = Controller.Instance.SystemController.GetTopCacheItems(200);
        _cacheItemsGrid.DataBind();

        _itemsCount.Text = Controller.Instance.SystemController.GetCacheSize().ToString();
        _cacheCapacity.Text = string.Format("{0}% (max {1} items)", Controller.Instance.SystemController.GetCacheCapacityUsed().ToString("N2"), Controller.Instance.SystemController.GetCacheCeiling().ToString("N0"));
    }

    #region event handlers
    /// <summary>
    /// Handles clearing out the application cache.
    /// </summary>
    protected void ClearCacheHandler(object sender, EventArgs ea)
    {
        Controller.Instance.SystemController.ClearCache();
        Helpers.AddUserResponse("<b>Done!</b> - The cache has been cleared.");
        Response.Redirect("./");
    }
    #endregion

    #region protected methods
    /// <summary>
    /// Attempts to interpret the type of the cache item and give its human-readable name.
    /// </summary>
    protected string GetItemTitle(object cacheItem)
    {
        if (cacheItem is Photo)
            return (cacheItem as Photo).Name;
        if (cacheItem is Collection)
            return (cacheItem as Collection).Name;
        if (cacheItem is RateCard)
            return (cacheItem as RateCard).Name;
        if (cacheItem is Company)
            return (cacheItem as Company).Name;
        if (cacheItem is Partner)
            return (cacheItem as Partner).Name;
        if (cacheItem is License)
            return (cacheItem as License).Name;
        if (!(cacheItem is Member))
            return "-";

        var m = cacheItem as Member;
        var name = m.Profile.ToFullName();
        if (string.IsNullOrEmpty(name))
            name = string.Format("{0} ({1})", m.MembershipUser.UserName, m.MembershipUser.Email);

        return name;
    }
    #endregion
}