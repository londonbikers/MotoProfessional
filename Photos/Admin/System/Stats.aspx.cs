using System;
using System.Web.UI;
using App_Code;
using MotoProfessional;

public partial class AdminStatsPage : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        _partners.Text = Controller.Instance.SystemController.BasicStatistics.Partners.ToString("N0");
        _collections.Text = Controller.Instance.SystemController.BasicStatistics.Collections.ToString("N0");
        _photos.Text = Controller.Instance.SystemController.BasicStatistics.Photos.ToString("N0");
        _photoArchiveSize.Text = Controller.Instance.SystemController.BasicStatistics.PhotosSizeGigabytes.ToString("N2");
        _members.Text = Controller.Instance.SystemController.BasicStatistics.Members.ToString("N0");
        _countriesRepresented.Text = Controller.Instance.SystemController.BasicStatistics.CountriesRepresented.ToString("N0");
        _baskets.Text = Controller.Instance.SystemController.BasicStatistics.Baskets.ToString("N0");
        _completeOrders.Text = Controller.Instance.SystemController.BasicStatistics.CompleteOrders.ToString("N0");
        _incompleteOrders.Text = Controller.Instance.SystemController.BasicStatistics.IncompleteOrders.ToString("N0");
        _downloads.Text = Controller.Instance.SystemController.BasicStatistics.Downloads.ToString("N0");
        _tags.Text = Controller.Instance.SystemController.BasicStatistics.Tags.ToString("N0");
    }

    #region event handlers
    protected void RefreshStatsHandler(object sender, EventArgs ea)
    {
        Controller.Instance.SystemController.BasicStatistics.RebuildStatistics();
        Helpers.AddUserResponse("<b>Done!</b> - Statistics refreshed.");
        Response.Redirect(Request.Url.AbsoluteUri);
    }
    #endregion
}
