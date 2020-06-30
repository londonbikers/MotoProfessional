using System;
using System.Configuration;
using System.Web.UI.WebControls;
using _masterPages;
using App_Code;
using MotoProfessional;
using MotoProfessional.Models.Interfaces;
using MPN.Framework.Web;

public partial class PhotosTagPage : System.Web.UI.Page
{
    #region members
    /// <summary>
    /// Default view type.
    /// </summary>
    private const PhotoViewType ViewType = PhotoViewType.Tile;
    private PhotoPaginator _paginator;
    private int _cellCount;
    private int _pageNumber;
    private ITaggedPhotoContainer _tag;

    public PhotosTagPage()
    {
        _pageNumber = 1;
    }
    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(Request.QueryString["tag"]))
        {
            Helpers.AddUserResponse("<b>Whoops!</b> - No tag specified.");
            Response.Redirect("~/photos/", true);
        }

        var tagName = Web.DecodeString(Web.UrlEncodingType.MediaPanther, Request.QueryString["tag"]);
        _tag = Controller.Instance.PhotoController.GetPhotoTag(tagName);
        if (_tag == null)
        {
            Helpers.AddUserResponse("<b>Whoops!</b> - Tag couldn't be shown.");
            Response.Redirect("~/photos/", true);
        }

        var masterPage = Page.Master as MasterPagesRegular;
        if (masterPage != null)
        {
            masterPage.CustomBreadCrumbs = string.Format("<a href=\"{0}photos/\">Latest Photos</a> > Tag", Page.ResolveUrl("~/"));
            masterPage.SelectedTab = "photos";
        }

        RenderView();
    }

    #region event handlers
    protected void PhotoGridItemCreatedHandler(object sender, RepeaterItemEventArgs ea)
    {
        if (ea.Item.ItemType != ListItemType.Item && ea.Item.ItemType != ListItemType.AlternatingItem) return;
        var p = ea.Item.DataItem as IPhoto;
        if (p == null)
            return;

        var title = ea.Item.FindControl("_photoTitle") as HyperLink;
        var cover = ea.Item.FindControl("_photoCover") as Literal;
        var rowDiv = ea.Item.FindControl("_rowSeparator") as Literal;
        var cellStyle = ea.Item.FindControl("_cellStyle") as Literal;
        var captured = ea.Item.FindControl("_photoCaptured") as Literal;
        var collections = ea.Item.FindControl("_photoCollections") as Repeater;

        var linkUrl = Helpers.BuildLink(p);
        if (title != null)
        {
            title.Text = p.Name;
            title.NavigateUrl = linkUrl;
        }
        if (cover != null)
            cover.Text = string.Format("<a href=\"{0}\" class=\"SimpleFrame\"><img src=\"{1}i.ashx?i={2}&d=150\" border=\"0\" /></a>", linkUrl, Page.ResolveUrl("~/"), p.Id);

        if (collections != null)
        {
            collections.DataSource = p.Collections;
            collections.DataBind();
        }

        if (p.Captured != DateTime.MinValue && captured != null)
            captured.Text = p.Captured.ToString(ConfigurationManager.AppSettings["ShortDateTimeFormatString"]);

        if (_cellCount < 2 && cellStyle != null)
            cellStyle.Text = "padding-right: 20px;";

        if (_cellCount == 2)
        {
            if (rowDiv != null) 
                rowDiv.Visible = true;
            _cellCount = 0;
        }
        else
        {
            _cellCount++;
        }
    }

    protected void PhotoCollectionItemCreatedHandler(object sender, RepeaterItemEventArgs ea)
    {
        if (ea.Item.ItemType != ListItemType.Item && ea.Item.ItemType != ListItemType.AlternatingItem) return;
        var c = ea.Item.DataItem as ICollection;
        if (c == null)
            return;

        var link = ea.Item.FindControl("_photoCollectionLink") as Literal;
        if (link != null) link.Text = string.Format("<a href=\"{0}\">{1}</a>", Helpers.BuildLink(c), c.Name);
    }
    #endregion

    #region private methods
    private void RenderView()
    {
        _title.Text = MPN.Framework.Content.Text.CapitaliseEachWord(_tag.Tag);
        Page.Title = string.Format("{0} - MP", MPN.Framework.Content.Text.CapitaliseEachWord(_tag.Tag));
        _introTagName.Text = _title.Text;

        // rss feed.
        _rssLink.Text = string.Format("subscribe to {0}", _tag.Tag.ToLower());
        _rssLink.NavigateUrl = string.Format("~/api/rss.ashx?tag={0}", Web.EncodeString(Web.UrlEncodingType.MediaPanther, _tag.Tag));

        // photos.
        _paginator = new PhotoPaginator {PageSize = 100, DataSource = _tag.LatestPhotos};

        // pagination.
        if (!string.IsNullOrEmpty(Request.QueryString["page"]))
        {
            _pageNumber = Convert.ToInt32(Request.QueryString["page"]);
            if (_pageNumber < 1)
                _pageNumber = 1;
        }

        if (ViewType == PhotoViewType.Tile)
        {
            _photosGrid.DataSource = _paginator.GetPage(_pageNumber);
            _photosGrid.DataBind();
        }

        _paginationStats.Text = string.Format("{0} items, showing {1} max - Page {2} of {3}.", _paginator.DataSource.Count, _paginator.PageSize, _paginator.CurrentPage, _paginator.TotalPages);
        _bottomPaginationStats.Text = _paginationStats.Text;
        _paginationControls.Text = _paginator.BuildPaginatorControls(string.Empty);
        _bottomPaginationControls.Text = _paginationControls.Text;
    }
    #endregion
}