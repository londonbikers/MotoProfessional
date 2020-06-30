using System;
using _masterPages;
using App_Code;
using MotoProfessional;

public partial class PhotosLicensePage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(Request.QueryString["id"]))
        {
            Helpers.AddUserResponse("<b>Whoops!</b> - No license specified.");
            Response.Redirect("~/help/", true);
        }

        var license = Controller.Instance.LicenseController.GetLicense(Convert.ToInt32(Request.QueryString["id"]));
        if (license == null)
        {
            Logger.LogWarning("License.aspx - Retrieved license is null. ID: " + Request.QueryString["id"]);
            Helpers.AddUserResponse("<b>Whoops!</b> - That license couldn't be retrieved.");
            Response.Redirect("~/help/", true);
        }

        // basic page setup.
        var masterPage = Page.Master as MasterPagesRegular;
        if (masterPage != null)
        {
            masterPage.CustomBreadCrumbs = string.Format("<a href=\"{0}help/\">Help</a> > License", Page.ResolveUrl("~/"));
            masterPage.SelectedTab = "help";
        }

        if (license == null) return;
        _licenseTitle.Text = license.Name;
        _licenseBody.Text = license.Description;
        Page.Title = string.Format("{0} - MP", license.Name);
    }
}