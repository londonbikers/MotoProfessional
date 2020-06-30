using System;
using System.Configuration;
using System.Linq;
using System.Web.Security;
using System.Web.UI.WebControls;
using _masterPages;
using App_Code;
using MotoProfessional;
using MotoProfessional.Models.Interfaces;
using MPN.Framework;
using MPN.Framework.Web;

public partial class PhotoCollectionPage : System.Web.UI.Page
{
    #region members
    private ICollection _collection;
    /// <summary>
    /// Default view type.
    /// </summary>
    private const PhotoViewType ViewType = PhotoViewType.Tile;
    private CollectionPhotoPaginator _paginator;
    private int _cellCount;
    private int _pageNumber;

    public PhotoCollectionPage()
    {
        _pageNumber = 1;
    }
    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        var masterPage = Page.Master as MasterPagesRegular;
        if (masterPage != null)
        {
            masterPage.CustomBreadCrumbs = string.Format("<a href=\"{0}photos/\">Latest Photos</a> > Collection", Page.ResolveUrl("~/"));
            masterPage.SelectedTab = "photos";
        }

        if (string.IsNullOrEmpty(Request.QueryString["id"]))
        {
            Helpers.AddUserResponse("<b>Whoops!</b> - No collection specified.");
            Response.Redirect("~/photos/", true);
        }

        _collection = Controller.Instance.PhotoController.GetCollection(Convert.ToInt32(Request.QueryString["id"]));
        if (_collection == null)
        {
            Logger.LogWarning("Collection.aspx - Collection retrieval returned null. ID: " + Request.QueryString["id"]);
            Helpers.AddUserResponse("<b>Whoops!</b> - Bad collection.");
            Response.Redirect("~/photos/", true);
        }

        // only partners and staff can see unpublished collections.
        var m = Helpers.GetCurrentUser();
        if (_collection != null)
        {
            if (_collection.Status != GeneralStatus.Active && (m == null || (!Roles.IsUserInRole("Administrators") && m.Company.Partner == null)))
            {
                Helpers.AddUserResponse("<b>Whoops!</b> - That collection isn't available right now.");
                Response.Redirect("~/photos/", true);
            }

            if (masterPage != null)
            {
                masterPage.MetaTitle = _collection.Name;
                masterPage.MetaImageUrl = string.Format(_collection.Photos[0].Photo.Size.Width > _collection.Photos[0].Photo.Size.Height ? "{0}/i.ashx?i={1}&d=130" : "{0}/i.ashx?i={1}&d=110", ConfigurationManager.AppSettings["FormalServiceUrl"], _collection.Photos[0].Photo.Id);
            }
        }

        RenderView();
    }

    #region event handlers
    protected void PhotoGridItemCreatedHandler(object sender, RepeaterItemEventArgs ea)
    {
        if (ea.Item.ItemType != ListItemType.Item && ea.Item.ItemType != ListItemType.AlternatingItem) return;
        var cp = ea.Item.DataItem as ICollectionPhoto;
        if (cp == null)
            return;

        var title = ea.Item.FindControl("_photoTitle") as HyperLink;
        var cover = ea.Item.FindControl("_photoCover") as Literal;
        var tags = ea.Item.FindControl("_photoTags") as Literal;
        var rowDiv = ea.Item.FindControl("_rowSeparator") as Literal;
        var cellStyle = ea.Item.FindControl("_cellStyle") as Literal;
        var captured = ea.Item.FindControl("_photoCaptured") as Literal;

        var linkUrl = Helpers.BuildLink(cp.Photo);
        if (title != null)
        {
            title.Text = cp.Photo.Name;
            title.NavigateUrl = linkUrl;
        }
        if (cover != null)
            cover.Text = string.Format("<a href=\"{0}\" class=\"SimpleFrame\"><img src=\"{1}i.ashx?i={2}&d=150\" border=\"0\" /></a>", linkUrl, Page.ResolveUrl("~/"), cp.Photo.Id);

        for (var i = 0; i < cp.Photo.Tags.Count; i++)
        {
            if (tags == null) continue;
            tags.Text += string.Format("<a href=\"{0}\">{1}</a>", Helpers.BuildLink(_collection.Photos[0].Photo, cp.Photo.Tags[i]), cp.Photo.Tags[i]);
            if (i < (cp.Photo.Tags.Count - 1))
                tags.Text += ", ";
        }

        if (cp.Photo.Captured != DateTime.MinValue && captured != null)
            captured.Text = cp.Photo.Captured.ToString(ConfigurationManager.AppSettings["ShortDateTimeFormatString"]);

        if (_cellCount < 2 && cellStyle != null)
            cellStyle.Text = "padding-right: 20px;";

        if (_cellCount == 2)
        {
            if (rowDiv != null) rowDiv.Visible = true;
            _cellCount = 0;
        }
        else
        {
            _cellCount++;
        }
    }
    #endregion

    #region private methods
    private void RenderView()
    {
        _title.Text = _collection.Name;
        _created.Text = _collection.Created.ToString("dd MMMM yyyy");
        _photoCount.Text = _collection.ActivePhotoCount.ToString("N0");
        _uniqueTags.Text = _collection.TagStats.Count.ToString("N0");
        _description.Text = Common.ToWebFormString(_collection.Description);
        Page.Title = _collection.Name;

        if (string.IsNullOrEmpty(_description.Text))
            _description.Text = "No description yet...";

        if (_collection.TagStats.Count > 0)
        {
            // bind the tag-cloud.
            var tagCharLength = 0;
            _tagCloud.Items.Clear();
            foreach (var ts in _collection.TagStats)
            {
                // link requires a url-rewrite rule to catch the tag.
                tagCharLength += ts.Tag.Length;
                var tagLink = Helpers.BuildLink(_collection) + "/tags/" + Web.EncodeString(Web.UrlEncodingType.MediaPanther, Web.EncodedStringMode.Compliant, ts.Tag);
                _tagCloud.Items.Add(new VRK.Controls.CloudItem(ts.Tag, ts.Count, tagLink));
            }

            _tagCloud.RenderCloud();
        
            if (tagCharLength < 1150)
            {
                _tagsContainer.Attributes.Remove("style");
                _expandTagsBox.Visible = false;
            }
        }
        else
        {
            _noTags.Visible = true;
            _tagCloud.Visible = false;
            _tagsContainer.Attributes.Remove("style");
            _expandTagsBox.Visible = false;
        }

        // photos.
        _paginator = new CollectionPhotoPaginator {PageSize = 100};

        // filter photos?
        if (!string.IsNullOrEmpty(Request.QueryString["tag"]))
        {
            var tag = Web.DecodeString(Web.UrlEncodingType.MediaPanther, Request.QueryString["tag"]);
            _filterHeading.InnerText = "Filtered by: " + MPN.Framework.Content.Text.CapitaliseEachWord(tag);
            _paginator.DataSource = _collection.Photos.Where(qcp => qcp.Photo.Tags.Contains(tag) && qcp.Photo.Status == GeneralStatus.Active).ToList();
            _filterBox.Visible = true;
            _filterNoneLink.NavigateUrl = Helpers.BuildLink(_collection);
            _noFilterSeparator.Visible = false;
        }
        else
        {
            _paginator.DataSource = _collection.Photos.Where(qcp => qcp.Photo.Status == GeneralStatus.Active).ToList();
        }

        // paginator.
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
        _paginationControls.Text = _paginator.PaginationControls;
        _bottomPaginationControls.Text = _paginationControls.Text;
    }
    #endregion
}