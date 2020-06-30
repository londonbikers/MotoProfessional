using System;
using System.Linq;
using System.Web.UI.WebControls;
using System.Configuration;
using App_Code;
using MotoProfessional;
using System.Text;
using MotoProfessional.Models.Interfaces;

public partial class PartnersPage : System.Web.UI.Page
{
    #region members
    private int _cellCount;
    private const int ListSize = 25;
    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        RenderView();
    }

    #region event handlers
    protected void TopPartnerItemCreatedHandler(object sender, RepeaterItemEventArgs ea)
    {
        if (ea.Item.ItemType == ListItemType.Item || ea.Item.ItemType == ListItemType.AlternatingItem)
        {
        }
    }

    protected void PartnersItemCreatedHandler(object sender, RepeaterItemEventArgs ea)
    {
        if (ea.Item.ItemType != ListItemType.Item && ea.Item.ItemType != ListItemType.AlternatingItem) return;
        var p = ea.Item.DataItem as IPartner;
        if (p == null)
            return;

        var name = ea.Item.FindControl("_partnerName") as HyperLink;
        var cover = ea.Item.FindControl("_partnerCover") as Literal;
        var joined = ea.Item.FindControl("_partnerCreated") as Literal;
        var rowDiv = ea.Item.FindControl("_rowSeparator") as Literal;
        var cellStyle = ea.Item.FindControl("_cellStyle") as Literal;
        var collections = ea.Item.FindControl("_partnerCollections") as Literal;

        var linkUrl = Helpers.BuildLink(p);
        if (name != null)
        {
            name.Text = p.Name;
            name.NavigateUrl = linkUrl;
        }
        if (cover != null)
            cover.Text = string.Format("<a href=\"{0}\"><img src=\"/i.ashx?l=pr&i={1}&rw=1&d=100\" /></a>", linkUrl, p.Id);
        //description.Text = Common.ToWebFormString(Common.ToShortString(MediaPanther.Framework.Content.GetFirstParagraph(c.Description), 100));
        if (joined != null)
            joined.Text = p.Created.ToString(ConfigurationManager.AppSettings["ShortDateFormatString"]);

        // build a list of recent collections.
        var collectionsRaw = new StringBuilder();
        var collectionCount = (p.LatestCollections.Count < 4) ? p.LatestCollections.Count : 4;

        for (var i = 0; i < collectionCount; i++)
            collectionsRaw.AppendFormat("- <a href=\"{0}\">{1}</a><br />", Helpers.BuildLink(p.LatestCollections[i]), p.LatestCollections[i].Name);

        if (collections != null) collections.Text = collectionsRaw.ToString();

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

    protected void MorePartnersItemCreatedHandler(object sender, RepeaterItemEventArgs ea)
    {
        if (ea.Item.ItemType != ListItemType.Item && ea.Item.ItemType != ListItemType.AlternatingItem) return;
        var p = ea.Item.DataItem as IPartner;
        if (p == null)
            return;

        var link = ea.Item.FindControl("_link") as HyperLink;
        var meta = ea.Item.FindControl("_meta") as Literal;

        if (link != null)
        {
            link.NavigateUrl = Helpers.BuildLink(p);
            link.Text = p.Name;
        }
        if (meta != null) meta.Text = string.Format("{0} Collections", p.Statistics.Collections);
    }
    #endregion

    #region private methods
    private void RenderView()
    {
        var partners = Controller.Instance.PartnerController.LatestPartners.Take(100).ToList();
        var totalPartners = partners.Count;
        var ceiling = (partners.Count >= ListSize) ? ListSize : partners.Count;

        _partnersGrid.DataSource = partners.Take(ceiling);
        _partnersGrid.DataBind();

        if (totalPartners > ListSize)
        {
            _morePartners.DataSource = partners.GetRange(ListSize, partners.Count - ListSize);
            _morePartners.DataBind();
            _morePartnersArea.Visible = true;
        }
    }
    #endregion
}