using System;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using App_Code;
using MotoProfessional.Models.Interfaces;

namespace _controls
{
    public partial class SearchResultsBarContent : System.Web.UI.Page
    {
        #region members
        private PhotoSearchContainer _container;
        private int _currentPhotoId;
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            _container = Helpers.GetLatestPhotoSearchContainer();
            if (_container == null)
                return;

            if (!Page.IsPostBack)
                RenderView();
        }

        #region event handlers
        protected void ResultItemCreatedHandler(object sender, RepeaterItemEventArgs ea)
        {
            if (ea.Item.ItemType != ListItemType.Item && ea.Item.ItemType != ListItemType.AlternatingItem) return;
            var p = ea.Item.DataItem as IPhoto;
            if (p == null)
                return;

            var photo = ea.Item.FindControl("_tilePhoto") as Literal;
            var frame = ea.Item.FindControl("_frame") as HtmlGenericControl;

            if (frame != null)
                frame.Attributes["class"] = (p.Id == _currentPhotoId) ? "PhotoTileImageFrameHighlight" : "PhotoTileImageFrame";
            if (photo != null)
                photo.Text = string.Format("<a href=\"{0}\" target=\"_top\"><img src=\"{1}i.ashx?i={2}&d=74\" border=\"0\" alt=\"view photo\" /></a>", Helpers.BuildLink(p), Page.ResolveUrl("~/"), p.Id);
        }
        #endregion

        #region private methods
        private void RenderView()
        {
            if (!string.IsNullOrEmpty(Request.QueryString["i"]))
                _currentPhotoId = Convert.ToInt32(Request.QueryString["i"]);

            // REFACTOR TO SET CURRENT PAGE TO SHOW CURRENT PHOTO!

            // pagination.
            _container.Paginator.PageSize = 10;
            if (!string.IsNullOrEmpty(Request.QueryString["p"]))
            {
                _container.Page = Convert.ToInt32(Request.QueryString["p"]);
                if (_container.Page < 1)
                    _container.Page = 1;
            }

            // now, the clever thing is to pull out a ticker view of the results, i.e. center the current photo and have
            // some before and after it that the user can click on.
        
            _results.DataSource = _container.Paginator.GetPage(_container.Page);
            _results.DataBind();

            if (_container.Paginator.TotalPages == 1)
            {
                _paginationView.Visible = false;
                _noPaginationView.Visible = true;
                _noPaginationItemCount.Text = _container.Paginator.DataSource.Count.ToString();
            }
            else
            {
                _paginationControls.Text = _container.Paginator.BuildPaginatorControls(Request.Url.AbsoluteUri);
            }
        }
        #endregion
    }
}
