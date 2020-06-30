using System;
using System.Web.UI;
using _masterPages;
using App_Code;
using MotoProfessional;

namespace Account.Partner.Collections
{
    public partial class CollectionsPage : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Helpers.SecurePartnerPage();
            ((MasterPagesRegular) Page.Master).SelectedTab = "account";

            if (!Page.IsPostBack)
                PerformDefaultSearch();
        }

        #region event handlers
        protected void SearchHandler(object sender, EventArgs ea)
        {
            if (string.IsNullOrEmpty(_name.Text) && string.IsNullOrEmpty(_tag.Text))
            {
                PerformDefaultSearch();
                return;
            }

            var p = Helpers.GetCurrentUser().Company.Partner;
            if (!string.IsNullOrEmpty(_name.Text))
            {
                // name search.
                _collectionsGrid.DataSource = Controller.Instance.PhotoController.FindCollectionsByName(_name.Text.Trim(), 100, null, p);
            }
            else 
            { 
                // tag search.
                _collectionsGrid.DataSource = Controller.Instance.PhotoController.FindCollectionsByTag(_tag.Text.Trim(), 100, null, p);
            }

            _collectionsGrid.DataBind();
        }
        #endregion

        #region private methods
        private void PerformDefaultSearch()
        {
            _collectionsGrid.DataSource = Helpers.GetCurrentUser().Company.Partner.LatestCollections;
            _collectionsGrid.DataBind();
        }
        #endregion
    }
}