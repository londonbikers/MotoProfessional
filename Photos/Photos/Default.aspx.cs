using System;
using System.Linq;
using System.Web.UI.WebControls;
using System.Configuration;
using _masterPages;
using App_Code;
using MotoProfessional;
using MotoProfessional.Models.Interfaces;
using MPN.Framework;

public partial class PhotosDefaultPage : System.Web.UI.Page
{
    #region members
    private int _cellCount = 0;
    private const int ThumbnailListSize = 25;
    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        var masterPage = Page.Master as MasterPagesRegular;
        if (masterPage != null) masterPage.SelectedTab = "photos";

        RenderView();
    }

    #region event handlers
    protected void TopPartnersItemCreatedHandler(object sender, RepeaterItemEventArgs ea)
    {
        if (ea.Item.ItemType != ListItemType.Item && ea.Item.ItemType != ListItemType.AlternatingItem) return;
        var p = ea.Item.DataItem as IPartner;
        if (p == null)
            return;

        var link = ea.Item.FindControl("_partnerLink") as HyperLink;
        if (link == null) return;
        link.NavigateUrl = Helpers.BuildLink(p);
        link.Text = p.Name;
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
            cover.Text = string.Format("<a href=\"{0}\"><img src=\"../i.ashx?i={1}&d=100\" border=\"0\" /></a>", linkUrl, c.Photos[0].Photo.Id);
        if (description != null)
            description.Text = Common.ToWebFormString(Common.ToShortString(MPN.Framework.Content.Text.GetFirstParagraph(c.Description), 100));
        if (created != null)
            created.Text = c.Created.ToString(ConfigurationManager.AppSettings["ShortDateFormatString"]);
        if (photoCount != null) photoCount.Text = c.ActivePhotoCount.ToString();

        if (_cellCount == 0)
            if (cellStyle != null) cellStyle.Text = "padding-right: 20px;";

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
        var collections = Controller.Instance.PhotoController.LatestCollections.Take(100).ToList();
        var totalCollections = collections.Count;
        var ceiling = (collections.Count >= ThumbnailListSize) ? ThumbnailListSize : collections.Count;

        _collectionsGrid.DataSource = collections.Take(ceiling);
        _collectionsGrid.DataBind();

        if (totalCollections > ThumbnailListSize)
        {
            _moreCollections.DataSource = collections.GetRange(ThumbnailListSize, collections.Count - ThumbnailListSize);
            _moreCollections.DataBind();
        }

        _topPartners.DataSource = Controller.Instance.PartnerController.TopPartners.Take(10);
        _topPartners.DataBind();
    }
    #endregion
}