using System;
using App_Code;

public partial class SubNavigation : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        RenderView();
    }

    #region private methods
    private void RenderView()
    {
        var showNav = false;
        var p = Request.Url.PathAndQuery;
        var m = Helpers.GetCurrentUser();

        #region account
        if (p.Contains("account/"))
        {
            _accountOptionsView.Visible = true;
            showNav = true;

            if (m.Company != null && m.Company.Partner != null)
                _partnerCPView.Visible = true;
        }
        #endregion

        if (!showNav) return;
        _headerView.Visible = true;
        _footerView.Visible = true;
    }
    #endregion
}