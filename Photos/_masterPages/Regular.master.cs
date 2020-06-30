using System;
using System.Web.UI;
using _controls;
using App_Code;

namespace _masterPages
{
    public partial class MasterPagesRegular : MasterPage
    {
        #region members
        private bool _fluidWidth;
        private string _customBreadCrumbs;
        protected string PageWidth;
        #endregion

        #region accessors
        public string SelectedTab { get { return _tabbedNavigation.SelectedTab; } set { _tabbedNavigation.SelectedTab = value; } }
        /// <summary>
        /// Pass-through for the MPMasterPage. Sets the default form button. Supply the ID of the button to be the form default.
        /// </summary>
        public string DefaultFormButton { get { return ((MpMasterPage) Master).DefaultFormButton; } set { ((MpMasterPage) Master).DefaultFormButton = value; } }
        /// <summary>
        /// Allows for the over-riding of the ASPNET SiteMapPath control with a custom one. Already starts with the home element.
        /// </summary>
        public string CustomBreadCrumbs { get { return _customBreadCrumbs; } set { _customBreadCrumbs = value; } }
        public bool ShowJQueryAutoGrow { get { return ((MpMasterPage) Master).ShowJQueryAutoGrow; } set { ((MpMasterPage) Master).ShowJQueryAutoGrow = value; } }
        public bool ShowJQueryUi { get { return ((MpMasterPage) Master).ShowJQueryUi; } set { ((MpMasterPage) Master).ShowJQueryUi = value; } }
        public bool ShowJQueryBlockUi { get { return ((MpMasterPage) Master).ShowJQueryBlockUi; } set { ((MpMasterPage) Master).ShowJQueryBlockUi = value; } }
        public bool ShowJQueryAlphanumeric { get { return ((MpMasterPage) Master).ShowJQueryAlphanumeric; } set { ((MpMasterPage) Master).ShowJQueryAlphanumeric = value; } }
        public bool FluidLayout { get { return _fluidWidth; } set { _fluidWidth = value; } }
        public string MetaTitle { set { ((MpMasterPage) Master).MetaTitle = value; } }
        public string MetaImageUrl { set { ((MpMasterPage) Master).MetaImageUrl = value; } }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            RenderView();
        }

        #region event handlers
        protected void OnLoggedOutHandler(object sender, EventArgs ea)
        {
            Session.Abandon();
        }
        #endregion

        #region private methods
        private void RenderView()
        {
            var m = Helpers.GetCurrentUser();
            _responseBox.InnerHtml = Helpers.RenderUserResponse();
            if (!string.IsNullOrEmpty(_responseBox.InnerHtml))
                _responseHolder.Visible = true;

            if (_fluidWidth)
                PageWidth = " width: 98%;";

            if (!string.IsNullOrEmpty(_customBreadCrumbs))
            {
                _siteMapPath.Visible = false;
                _customSiteMapPath.Text = string.Format("<a href=\"{0}\">Home</a> > ", Page.ResolveUrl("~/")) + CustomBreadCrumbs;
                _customSiteMapPath.Visible = true;
            }

            // is there a search result in session for the SearchResultsBar?
            // don't show on the search page.
            if (!Request.RawUrl.ToLower().Contains("photos/search/default.aspx"))
            {
                var photoSearchContainer = Helpers.GetLatestPhotoSearchContainer();
                if (photoSearchContainer != null)
                {
                    var bar = LoadControl("~/_controls/SearchResultsBarCtrl.ascx") as SearchResultsBarCtrl;
                    if (Request.RawUrl.ToLower().Contains("/photo.aspx") && bar != null)
                        bar.CurrentPhotoId = Convert.ToInt32(Request.QueryString["id"]);

                    _searchResultsBarContainer.Controls.Add(bar);
                }
            }

            // show the basket control?
            if (Request.RawUrl.ToLower().Contains("basket/default.aspx") || Request.RawUrl.ToLower().Contains("checkout/default.aspx") || m == null || m.Basket.Items.Count <= 0)
                return;

            var basketView = LoadControl("~/_controls/BasketView.ascx") as BasketView;
            if (Request.RawUrl.ToLower().Contains("/photo.aspx") && basketView != null)
                basketView.CurrentPhotoID = Convert.ToInt32(Request.QueryString["id"]);

            _basketViewContainer.Controls.Add(basketView);
        }
        #endregion
    }
}