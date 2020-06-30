using System;
using System.Text;
using System.Linq;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Configuration;
using _masterPages;
using App_Code;
using MotoProfessional;
using MotoProfessional.Models.Interfaces;
using MPN.Framework;
using MPN.Framework.Security;
using MPN.Framework.Web;

public partial class PhotoPage : Page
{
    #region members
    protected IPhoto Photo;
    private int _poIndex;
    private bool _signedIn;
    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(Request.QueryString["id"]))
        {
            Helpers.AddUserResponse("<b>Whoops!</b> - No photo specified.");
            Response.Redirect("~/photos/", true);
        }

        Photo = Controller.Instance.PhotoController.GetPhoto(Convert.ToInt32(Request.QueryString["id"]));
        if (Photo == null)
        {
            Logger.LogWarning("Photo.aspx - Retrieved photo is null. ID: " + Request.QueryString["id"]);
            Helpers.AddUserResponse("<b>Whoops!</b> - That photo couldn't be found, sorry.");
            Response.Redirect("~/photos/", true);
        }

        // only partners and staff can see unpublished photos.
        var m = Helpers.GetCurrentUser();
        if (Photo != null && Photo.Status != GeneralStatus.Active && (m == null || (!Roles.IsUserInRole("Administrators") && m.Company.Partner == null)))
        {
            Helpers.AddUserResponse("<b>Whoops!</b> - That photo isn't available right now, sorry.");
            Response.Redirect("~/photos/", true);
        }

        // postback events.
        if (!string.IsNullOrEmpty(Request.QueryString["lid"]))
            AddItemToBasket(Convert.ToInt32(Request.QueryString["lid"]));

        // basic page setup.
        var masterPage = Page.Master as MasterPagesRegular;
        if (masterPage != null)
        {
            masterPage.CustomBreadCrumbs = string.Format("<a href=\"{0}photos/\">Latest Photos</a> > Photo", Page.ResolveUrl("~/"));
            masterPage.SelectedTab = "photos";
        }

        // record view.
        RecordUserViewing();

        if (masterPage != null && Photo != null)
        {
            masterPage.MetaTitle = Photo.Name;
            masterPage.MetaImageUrl = Photo.Size.Width > Photo.Size.Height
                                          ? string.Format("{0}/i.ashx?i={1}&d=130", ConfigurationManager.AppSettings["FormalServiceUrl"], Photo.Id)
                                          : string.Format("{0}/i.ashx?i={1}&d=110", ConfigurationManager.AppSettings["FormalServiceUrl"], Photo.Id);
        }

        RenderView();
    }

    #region event handlers
    protected void PoItemCreatedHandler(object sender, RepeaterItemEventArgs ea)
    {
        if (ea.Item.ItemType != ListItemType.Item && ea.Item.ItemType != ListItemType.AlternatingItem) return;
        var l = ea.Item.DataItem as ILicense;
        if (l == null)
            return;

        var sizeIndicator = ea.Item.FindControl("_sizeIndicator") as Image;
        var title = ea.Item.FindControl("_poTitle") as Literal;
        var dimensions = ea.Item.FindControl("_poDimensions") as Literal;
        var shortDescription = ea.Item.FindControl("_poShortDescription") as Literal;
        var price = ea.Item.FindControl("_poPrice") as Literal;
        var licenseLink = ea.Item.FindControl("_licenseLink") as HyperLink;
        var addBtn = ea.Item.FindControl("_addBtn") as HyperLink;
        var signInLink = ea.Item.FindControl("_signInLink") as HyperLink;
        var noAddPlaceHolder = ea.Item.FindControl("_noAddPlaceHolder") as PlaceHolder;

        if (title != null) title.Text = l.Name;
        if (dimensions != null) dimensions.Text = l.PrimaryDimension.ToString("N0");
        if (licenseLink != null) licenseLink.NavigateUrl = Helpers.BuildLink(l);

        if (dimensions != null)
            dimensions.Text = l.PrimaryDimension < 9999 ? string.Format("{0} px {1}", l.PrimaryDimension, (Photo.Orientation == PhotoOrientation.Portrait) ? "tall" : "wide") : "Original";

        if (shortDescription != null) shortDescription.Text = l.ShortDescription;
        if (price != null) price.Text = l.GetRate(Photo).ToString("###,###.##");
        if (sizeIndicator != null)
            sizeIndicator.ImageUrl = string.Format("~/_images/objects/license-size-{0}.gif", _poIndex);

        if (_signedIn)
        {
            if (addBtn != null) addBtn.NavigateUrl = string.Format("{0}/add/{1}", Helpers.BuildLink(Photo), l.Id);
        }
        else
        {
            if (addBtn != null) addBtn.Visible = false;
            if (noAddPlaceHolder != null) noAddPlaceHolder.Visible = true;
            if (signInLink != null)
                signInLink.NavigateUrl = string.Format("{0}?ReturnURL={1}", Page.ResolveUrl("~/signin/"), Helpers.BuildLink(Photo));
        }

        _poIndex++;
    }

    protected void CollectionsItemCreated(object sender, RepeaterItemEventArgs ea)
    {
        if (ea.Item.ItemType != ListItemType.Item && ea.Item.ItemType != ListItemType.AlternatingItem) return;
        var c = ea.Item.DataItem as ICollection;
        if (c == null)
            return;

        var titleLink = ea.Item.FindControl("_collectionTitleLink") as HyperLink;
        var stats = ea.Item.FindControl("_collectionStats") as Literal;
        var prev = ea.Item.FindControl("_previousThumb") as HyperLink;
        var next = ea.Item.FindControl("_nextThumb") as HyperLink;
        var startGhost = ea.Item.FindControl("_startThumbGhost") as HtmlGenericControl;
        var endGhost = ea.Item.FindControl("_endThumbGhost") as HtmlGenericControl;
        var frame1 = ea.Item.FindControl("_frame1") as HtmlGenericControl;
        var frame2 = ea.Item.FindControl("_frame2") as HtmlGenericControl;

        if (titleLink != null)
        {
            titleLink.NavigateUrl = Helpers.BuildLink(c);
            titleLink.Text = c.Name;
        }

        // find location of photo within collections.
        var activeCPs = c.Photos.Where(qcp => qcp.Photo.Status == GeneralStatus.Active).ToList();
        var position = activeCPs.IndexOf((from cp in activeCPs where cp.Photo.Id == Photo.Id select cp).First());
        if (stats != null) stats.Text = string.Format("{0} photos.", activeCPs.Count);

        if (position == 0)
        {
            if (frame1 != null) frame1.Visible = false;
            if (startGhost != null) startGhost.Visible = true;
        }
        else
        {
            var prevPhoto = activeCPs[position - 1].Photo;
            if (prev != null)
            {
                prev.ImageUrl = string.Format("~/i.ashx?i={0}&d=100", prevPhoto.Id);
                prev.NavigateUrl = Helpers.BuildLink(prevPhoto);
            }
        }

        if (position + 1 == activeCPs.Count)
        {
            if (frame2 != null) frame2.Visible = false;
            if (endGhost != null) endGhost.Visible = true;
        }
        else
        {
            var nextPhoto = activeCPs[position + 1].Photo;
            if (next != null)
            {
                next.ImageUrl = string.Format("~/i.ashx?i={0}&d=100", nextPhoto.Id);
                next.NavigateUrl = Helpers.BuildLink(nextPhoto);
            }
        }
    }
    #endregion

    #region private methods
    private void RenderView()
    {
        _title.Text = MPN.Framework.Content.Text.CapitaliseEachWord(Photo.Name);
        Page.Title = string.Format("{0} - MP", _title.Text);

        _partner.NavigateUrl = Helpers.BuildLink(Photo.Partner);
        _partner.Text = Photo.Partner.Name;
        _orientation.Text = Photo.Orientation.ToString();
        _dimensions.Text = string.Format("{0} x {1}", Photo.Size.Width, Photo.Size.Height);
        _signedIn = (Helpers.GetCurrentUser() == null) ? false : true;

        if (Roles.IsUserInRole("Administrators"))
        {
            _views.Text = Photo.Views.ToString("N0");
            _viewCountRow.Visible = true;
        }

        if (Photo.Captured != DateTime.MinValue)
            _dateCaptured.Text = Photo.Captured.ToString("dddd, dd MMMM yyyy") + " - " + Photo.Captured.ToShortTimeString();

        if (Photo.Photographer != null)
            _photographer.Text = Photo.Photographer.GetFullName();

        if (!string.IsNullOrEmpty(Photo.Comment))
            _comment.Text = Common.ToWebFormString(Photo.Comment);

        // tags.
        if (Photo.Tags.Count > 0)
        {
            var t = new StringBuilder();
            for (var i = 0; i < Photo.Tags.Count; i++)
            {
                t.AppendFormat("<a href=\"{0}\">{1}</a>", Helpers.BuildLink(Photo, Photo.Tags[i]), Photo.Tags[i]);
                if (i < Photo.Tags.Count - 1)
                    t.Append(", ");
            }

            _tags.Text = t.ToString();
        }
    
        // purchase options - need to be signed-in to see them.
        _purchaseOptions.DataSource = Photo.GetSuitableActiveLicenses().OrderByDescending(ql => ql.GetRate(Photo));
        _purchaseOptions.DataBind();
	
        // collections.
        _collections.DataSource = Photo.ActiveCollections;
        _collections.DataBind();

        // preview images.
        string staticPreviewParams = Server.UrlEncode(SecurityHelpers.DesEncrypt(string.Format("i={0}&d=621", Photo.Id)));
        _staticPreview.ImageUrl = string.Format("{0}i.ashx?e={1}", Page.ResolveUrl("~/"), staticPreviewParams);
        _staticPreview.AlternateText = Web.ToSafeHtmlParameter(Photo.Name);

        // extended meta-data.
        _extendedMetaData.LoadData(Photo);
        if (_extendedMetaData.IsExtendedMetaDataEmpty())
            _extendedMetaDataView.Visible = false;
    }

    /// <summary>
    /// Increments the photo view count, based upon some logic to reduce mal-views.
    /// </summary>
    private void RecordUserViewing()
    {
        // is the user a bot?
        if (Web.IsUserABot())
            return;
	
        var m = Helpers.GetCurrentUser();
        if (m != null)
        {
            // is the user the owning parter?
            if (m.Company != null && m.Company.Partner != null && m.Company.Partner.Id == Photo.Partner.Id)
                return;

            // is the user a staff member?
            if (Roles.IsUserInRole("Administrators"))
                return;
        }

        // has the user viewed this photo within this session already?
        var sessionKey = string.Format("PhotoViewed:{0}", Photo.Id);
        if (Session[sessionKey] != null)
            return;

        // okay, record.
        Photo.IncrementViewCount();
        Session[sessionKey] = true;
    }

    private void AddItemToBasket(int licenseId)
    {
        // need a valid license-id.
        if (licenseId < 1)
            return;

        // we need a signed-in user. they should be already but it's better to be safe.
        var m = Helpers.GetCurrentUser();
        if (m == null)
            return;

        var license = Controller.Instance.LicenseController.GetLicense(licenseId);
        if (license == null)
            return;

        // no duplicates allowed!
        if (m.Basket.DoesBasketContainerPhotoProduct(license, Photo))
            return;

        m.Basket.AddPhotoProduct(license, Photo);
        Helpers.AddUserResponse(string.Format("<b>Added!</b> - {0} has been added to your basket.", Photo.Name));
        Response.Redirect(Helpers.BuildLink(Photo));
    }
    #endregion
}