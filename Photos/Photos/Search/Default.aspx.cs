using System;
using System.Configuration;
using System.Linq;
using System.Web.UI.WebControls;
using _masterPages;
using App_Code;
using MotoProfessional;
using MotoProfessional.Models;
using MotoProfessional.Models.Interfaces;
using MPN.Framework;

public partial class PhotoSearchPage : System.Web.UI.Page
{
    #region members
    /// <summary>
    /// Default view type.
    /// </summary>
    private const PhotoViewType ViewType = PhotoViewType.Tile;
    private int _cellCount = 0;
    private int _pageNumber = 1;
    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        var masterPage = Page.Master as MasterPagesRegular;
        if (masterPage != null)
        {
            masterPage.DefaultFormButton = _searchBtn.UniqueID;
            masterPage.SelectedTab = "search";
        }
        PhotoSearchContainer container = null;

        // new querystring search?
        if ((!Page.IsPostBack && string.IsNullOrEmpty(Request.QueryString["c"]) && !string.IsNullOrEmpty(Request.QueryString["t"])) ||
            (!Page.IsPostBack && !string.IsNullOrEmpty(Request.QueryString["c"]) && !DoesContainerExist(Request.QueryString["c"])))
        {
            BuildAndRunSearch();
            return;
        }

        // pagination request?
        if (!Page.IsPostBack && !string.IsNullOrEmpty(Request.QueryString["c"]))
            container = Session[typeof(PhotoSearchContainer).ToString() + ":" + Request.QueryString["c"]] as PhotoSearchContainer;

        RenderView(container);
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

        if (_cellCount == 2 && rowDiv != null)
        {
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
        var link = ea.Item.FindControl("_photoCollectionLink") as Literal;
        if (link != null) link.Text = string.Format("<a href=\"{0}\">{1}</a>", Helpers.BuildLink(c), c.Name);
    }

    protected void SearchHandler(object sender, EventArgs ea)
    {
        BuildAndRunSearch();
    }
    #endregion

    #region private methods
    /// <summary>
    /// A new search requires a PhotoSearchContainer.
    /// </summary>
    private void BuildAndRunSearch()
    {
        // resolve parameters via querystring or postback.
        var psc = new PhotoSearchContainer();
        if (Page.IsPostBack)
        {
            psc.Term = _term.Text.Trim();
            if (_orientationAll.Checked)
                psc.Orientation = PhotoOrientation.NotDefined;
            else if (_orientationLandscape.Checked)
                psc.Orientation = PhotoOrientation.Landscape;
            else if (_orientationPortrait.Checked)
                psc.Orientation = PhotoOrientation.Portrait;
            else if (_orientationSquare.Checked)
                psc.Orientation = PhotoOrientation.Square;

            if (_nameCheckBox.Checked)
                psc.Property = PhotoSearchProperty.Name;
            else if (_commentCheckBox.Checked)
                psc.Property = PhotoSearchProperty.Comment;
            else if (_tagsChecked.Checked)
                psc.Property = PhotoSearchProperty.Tags;

            if (Common.IsDate(_capturedFrom.Text.Trim()))
                psc.CapturedFrom = DateTime.Parse(_capturedFrom.Text.Trim());

            if (Common.IsDate(_capturedUntil.Text.Trim()))
                psc.CapturedUntil = DateTime.Parse(_capturedUntil.Text.Trim());
        }
        else
        {
            // querystring method.
            psc.Term = Server.UrlDecode(Request.QueryString["t"]).Trim();

            if (!string.IsNullOrEmpty(Request.QueryString["f"]))
                psc.Property = (PhotoSearchProperty)Enum.Parse(typeof(PhotoSearchProperty), Request.QueryString["f"].Trim(), true);

            try
            {
                if (!string.IsNullOrEmpty(Request.QueryString["o"]))
                    psc.Orientation = (PhotoOrientation)Enum.Parse(typeof(PhotoOrientation), Request.QueryString["o"].Trim(), true);
            }
            catch
            {
            }

            if (!string.IsNullOrEmpty(Request.QueryString["cf"]) && Common.IsDate(Request.QueryString["cf"]))
                psc.CapturedFrom = DateTime.Parse(Request.QueryString["cf"]);

            if (!string.IsNullOrEmpty(Request.QueryString["cu"]) && Common.IsDate(Request.QueryString["cu"]))
                psc.CapturedUntil = DateTime.Parse(Request.QueryString["cu"]);
        }

        if (!psc.IsValid())
        {
            Helpers.AddUserResponse("<b>Whoops!</b> - Not a valid search, please try again.");
            Response.Redirect("s.aspx", true);
        }

        RenderView(psc);
    }

    private void RenderView(PhotoSearchContainer container)
    {
        if (container == null)
            return;

        Page.Title = string.Format("{0} - MP", MPN.Framework.Content.Text.CapitaliseEachWord(container.Term));

        // if the session container doesn't have a paginator already then we need one.
        if (container.Paginator == null)
        {
            // perform search.
            container.Paginator = new PhotoPaginator
            {
                PageSize = 100,
                UrlFormat = PhotoPaginator.PaginationControlsUrlFormat.QueryString,
                DataSource = Controller.Instance.PhotoController.FindPhotos(
                    container.Term,
                    container.Property,
                    container.Orientation,
                    container.CapturedFrom,
                    container.CapturedUntil,
                    GeneralStatus.Active,
                    Helpers.GetCurrentUser())
            };
        }

        // pagination.
        if (!string.IsNullOrEmpty(Request.QueryString["p"]))
        {
            _pageNumber = Convert.ToInt32(Request.QueryString["p"]);
            if (_pageNumber < 1)
                _pageNumber = 1;
        }

        container.Page = _pageNumber;

        if (ViewType == PhotoViewType.Tile)
        {
            _photosGrid.DataSource = container.Paginator.GetPage(_pageNumber);
            _photosGrid.DataBind();
        }

        if (container.Paginator.DataSource.Count() > 0)
        {
            if (Page.IsPostBack)
                _pageNumber = 1;

            // create baseUrl for pagination controls.
            var baseUrl = BuildBaseUrl(container);
            _paginationStats.Text = string.Format("{0} items, showing {1} max - Page {2} of {3}.", container.Paginator.DataSource.Count, container.Paginator.PageSize, container.Paginator.CurrentPage, container.Paginator.TotalPages);
            _bottomPaginationStats.Text = _paginationStats.Text;
            _paginationControls.Text = container.Paginator.BuildPaginatorControls(baseUrl);
            _bottomPaginationControls.Text = _paginationControls.Text;
        
            _photosView.Visible = true;
            _noResultsView.Visible = false;
            _searchResultsTitleTerm.Text = MPN.Framework.Content.Text.CapitaliseEachWord(container.Term);
        }
        else
        {
            // no results.
            _noResultsView.Visible = true;
            _photosView.Visible = false;
        }

        // no point putting no-result searches into session.
        if (container.Paginator.DataSource.Count > 0)
            Session[container.SessionId] = container;

        if (!Page.IsPostBack)
            PopulateSearchForm(container);
    }

    private void PopulateSearchForm(PhotoSearchContainer container)
    {
        _term.Text = container.Term;
        if (container.CapturedFrom != DateTime.MinValue)
            _capturedFrom.Text = container.CapturedFrom.ToShortDateString();

        if (container.CapturedUntil != DateTime.MinValue)
            _capturedUntil.Text = container.CapturedUntil.ToShortDateString();

        _orientationAll.Checked = (container.Orientation == PhotoOrientation.NotDefined) ? true : false;
        _orientationLandscape.Checked = (container.Orientation == PhotoOrientation.Landscape) ? true : false;
        _orientationPortrait.Checked = (container.Orientation == PhotoOrientation.Portrait) ? true : false;
        _orientationSquare.Checked = (container.Orientation == PhotoOrientation.Square) ? true : false;

        _commentCheckBox.Checked = (container.Property == PhotoSearchProperty.Comment) ? true : false;
        _nameCheckBox.Checked = (container.Property == PhotoSearchProperty.Name) ? true : false;
        _tagsChecked.Checked = (container.Property == PhotoSearchProperty.Tags) ? true : false;
    }

    /// <summary>
    /// Checks to see whether or not a PhotoSearchContainer exists in the users Session object.
    /// </summary>
    private bool DoesContainerExist(int id)
    {
        var key = typeof(PhotoSearchContainer) + ":" + id.ToString();
        return (Session[key] != null) ? true : false;
    }

    /// <summary>
    /// Checks to see whether or not a PhotoSearchContainer exists in the users Session object.
    /// </summary>
    private bool DoesContainerExist(string id)
    {
        var key = typeof(PhotoSearchContainer) + ":" + id;
        return (Session[key] != null) ? true : false;
    }

    /// <summary>
    /// Builds a base search url with the search parameters within it.
    /// </summary>
    private string BuildBaseUrl(PhotoSearchContainer container)
    {
        var url = string.Format("default.aspx?c={0}&t={1}&f={2}&o={3}", container.Id, Server.UrlEncode(container.Term), container.Property.ToString().ToLower(), container.Orientation.ToString().ToLower());

        if (container.CapturedFrom != DateTime.MinValue)
            url += string.Format("&cf={0}", container.CapturedFrom.ToShortDateString());

        if (container.CapturedUntil != DateTime.MinValue)
            url += string.Format("&cu={0}", container.CapturedUntil.ToShortDateString());

        return url;
    }
    #endregion
}