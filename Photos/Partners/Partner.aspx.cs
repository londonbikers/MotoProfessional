using System;
using System.Linq;
using System.Web.UI.WebControls;
using System.Configuration;
using _masterPages;
using App_Code;
using MotoProfessional;
using MotoProfessional.Models;
using MotoProfessional.Models.Interfaces;
using MPN.Framework;

public partial class PartnerPage : System.Web.UI.Page
{
    #region members
    private int _cellCount = 0;
    private const int ThumbnailListSize = 25;
    private IPartner _partner;
    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(Request.QueryString["id"]))
        {
            Helpers.AddUserResponse("<b>Whoops!</b> - No photo specified.");
            Response.Redirect("~/photos/", true);
        }

        _partner = Controller.Instance.PartnerController.GetPartner(Convert.ToInt32(Request.QueryString["id"]));
        if (_partner == null)
        {
            Logger.LogWarning("Partner.aspx - Retrieved partner is null. ID: " + Request.QueryString["id"]);
            Helpers.AddUserResponse("<b>Whoops!</b> - That partner couldn't be found, sorry.");
            Response.Redirect("~/photos/", true);
        }

        var masterPage = Page.Master as MasterPagesRegular;
        if (masterPage != null) masterPage.SelectedTab = "photos";
        RenderView();
    }

    #region event handlers
    protected void PhotoGridItemCreatedHandler(object sender, RepeaterItemEventArgs ea)
    {
        if (ea.Item.ItemType != ListItemType.Item && ea.Item.ItemType != ListItemType.AlternatingItem) return;
        var p = ea.Item.DataItem as IPhoto;
        if (p == null)
            return;

        var thumb = ea.Item.FindControl("_photoThumb") as Literal;
        var linkUrl = Helpers.BuildLink(p);
        if (thumb != null)
            thumb.Text = string.Format("<a href=\"{0}\" title=\"{1}\"><img src=\"/i.ashx?i={2}&d=100\" border=\"0\" /></a>", linkUrl, MPN.Framework.Web.Web.ToSafeHtmlParameter(p.Name), p.Id);
    }

    protected void CollectionGridItemCreatedHandler(object sender, RepeaterItemEventArgs ea)
    {
        if (ea.Item.ItemType != ListItemType.Item && ea.Item.ItemType != ListItemType.AlternatingItem) return;
        var c = ea.Item.DataItem as ICollection;
        if (c == null)
            return;

        var title = ea.Item.FindControl("_collectionTitle") as HyperLink;
        var cover = ea.Item.FindControl("_collectionCover") as Literal;
        var description = ea.Item.FindControl("_collectionDescription") as Literal;
        var rowDiv = ea.Item.FindControl("_rowSeparator") as Literal;
        var cellStyle = ea.Item.FindControl("_cellStyle") as Literal;
        var created = ea.Item.FindControl("_created") as Literal;
        var photoCount = ea.Item.FindControl("_photoCount") as Literal;

        var linkUrl = Helpers.BuildLink(c);
        if (title != null)
        {
            title.Text = c.Name;
            title.NavigateUrl = linkUrl;
        }
        if (cover != null)
            cover.Text = string.Format("<a href=\"{0}\"><img src=\"/i.ashx?i={1}&d=100\" border=\"0\" /></a>", linkUrl, c.Photos[0].Photo.Id);
        if (description != null)
            description.Text = Common.ToWebFormString(Common.ToShortString(MPN.Framework.Content.Text.GetFirstParagraph(c.Description), 100));
        if (created != null)
            created.Text = c.Created.ToString(ConfigurationManager.AppSettings["ShortDateFormatString"]);
        if (photoCount != null) photoCount.Text = c.ActivePhotoCount.ToString();

        if (_cellCount == 0 && cellStyle != null)
            cellStyle.Text = "padding-right: 20px;";

        if (_cellCount == 1)
        {
            if (rowDiv != null) rowDiv.Visible = true;
            _cellCount = 0;
        }
        else
        {
            _cellCount++;
        }
    }

    protected void MoreCollectionsItemCreatedHandler(object sender, RepeaterItemEventArgs ea)
    {
        if (ea.Item.ItemType != ListItemType.Item && ea.Item.ItemType != ListItemType.AlternatingItem) return;
        var c = ea.Item.DataItem as ICollection;
        if (c == null)
            return;

        var link = ea.Item.FindControl("_link") as HyperLink;
        var meta = ea.Item.FindControl("_meta") as Literal;

        if (link != null)
        {
            link.NavigateUrl = Helpers.BuildLink(c);
            link.Text = c.Name;
        }
        if (meta != null) meta.Text = c.Created.ToShortDateString();
    }
    #endregion

    #region private methods
    private void RenderView()
    {
        _name.Text = _partner.Name;
        _smallName.Text = _partner.Name;
        _description.Text = Common.ToWebFormString(_partner.Description);
        Page.Title = string.Format("{0} - MP", _partner.Name);

        _collectionCount.Text = _partner.Statistics.Collections.ToString("#,###");
        _photoCount.Text = _partner.Statistics.Photos.ToString("###,###");

        if (_partner.Company.Url != null)
        {
            _websitePlaceHolder.Visible = true;
            _websiteLink.NavigateUrl = _partner.Company.Url.ToString();
            _websiteLink.Text = _partner.Company.Url.Host;
        }
    
        _joined.Text = _partner.Created.ToLongDateString();

        if (!string.IsNullOrEmpty(_partner.LogoFilename))
        {
            _logo.ImageUrl = string.Format("~/i.ashx?l=pr&i={0}&rw=1&d=200", _partner.Id);
            _logo.Visible = true;
        }

        if (_partner.Statistics.Photos == 0)
        {
            _noContent.Visible = true;
            _mainContent.Visible = false;
            return;
        }

        _hotPhotosGrid.DataSource = _partner.HotPhotos.Take(5).ToList();
        _hotPhotosGrid.DataBind();
        _hotPhotosMode.Text = MPN.Framework.Content.Text.SplitCamelCaseWords(_partner.HotPhotos.Mode.ToString());

        var collections = _partner.LatestCollections.Where(q=>q.Status == GeneralStatus.Active).Take(100).ToList();
        var totalCollections = collections.Count;
        var ceiling = (collections.Count >= ThumbnailListSize) ? ThumbnailListSize : collections.Count;

        _collectionsGrid.DataSource = collections.Take(ceiling);
        _collectionsGrid.DataBind();

        if (totalCollections > ThumbnailListSize)
        {
            _moreCollections.DataSource = collections.GetRange(ThumbnailListSize, collections.Count - ThumbnailListSize);
            _moreCollections.DataBind();
        }
    }
    #endregion
}