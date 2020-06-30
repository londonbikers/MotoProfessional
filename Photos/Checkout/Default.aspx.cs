using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using _masterPages;
using App_Code;
using MotoProfessional;
using MotoProfessional.Exceptions;
using MotoProfessional.Models.Interfaces;

public partial class CheckoutPage : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ((MasterPagesRegular) Page.Master).ShowJQueryBlockUi = true;
        if (!Page.IsPostBack)
            RenderView();
    }

    #region event handlers
    protected void PostCartToGoogleHandler(object sender, ImageClickEventArgs ea)
    {
        var m = Helpers.GetCurrentUser();
        if (m == null)
            return;

        var gRequest = _checkoutBtn.CreateRequest();
        gRequest.AnalyticsData = Request.Form["analyticsdata"];

        var gResponse = Controller.Instance.CommerceController.BeginPosTransaction(m, gRequest);
        if (gResponse.IsGood)
        {
            Response.Redirect(gResponse.RedirectUrl, true);
        }
        else
        {
            Helpers.AddUserResponse("<b>Whoops!</b> There was a problem preparing your payment. We've been notified. You may want to try again.");
            Helpers.RefreshPage();
        }
    }

    protected void NoChargeCompleteOrderHandler(object sender, EventArgs ea)
    {
        var m = Helpers.GetCurrentUser();
        if (m == null)
            return;

        try
        {
            var o = Controller.Instance.CommerceController.ExecuteNonPosTransaction(m);
            Helpers.AddUserResponse("<b>Thanks!</b> Your order has been processed. You can now download your items.");
            Response.Redirect(Helpers.BuildLink(o));
        }
        catch (TransactionException)
        {
            Helpers.AddUserResponse("<b>Whoops!</b> There was a problem processing your order. We've been notified. You may want to try again.");
            Helpers.RefreshPage();
        }
    }

    protected void SummaryRowCreatedHandler(object sender, GridViewRowEventArgs ea)
    {
        switch (ea.Row.RowType)
        {
            case DataControlRowType.DataRow:
                {
                    var bi = ea.Row.DataItem as IBasketItem;
                    if (bi == null)
                        return;

                    var name = ea.Row.FindControl("_name") as Literal;
                    var license = ea.Row.FindControl("_license") as Literal;
                    var quantity = ea.Row.FindControl("_quantity") as Literal;
                    var rate = ea.Row.FindControl("_rate") as Literal;
                    var licenseLink = ea.Row.FindControl("_licenseLink") as HyperLink;

                    // the only basket-item type at the moment.
                    if (bi.PhotoProduct != null)
                    {
                        if (name != null) name.Text = bi.PhotoProduct.Photo.Name;
                        if (license != null) license.Text = bi.PhotoProduct.License.Name;
                        if (quantity != null) quantity.Text = "1";
                        if (rate != null) rate.Text = bi.PhotoProduct.Rate.ToString("###,###.##");
                        if (licenseLink != null)
                            licenseLink.NavigateUrl = Helpers.BuildLink(bi.PhotoProduct.License);
                    }
                }
                break;
            case DataControlRowType.Footer:
                {
                    var m = Helpers.GetCurrentUser();
                    if (m == null)
                        return;

                    ea.Row.Cells[0].Attributes["style"] = "padding-top: 10px; border-top: solid 1px #333;";
                    ea.Row.Cells[0].Text = "Totals:&nbsp;";
                    ea.Row.Cells[0].ColumnSpan = 3;
                    ea.Row.Cells[0].HorizontalAlign = HorizontalAlign.Right;
                    ea.Row.Cells[1].Attributes["style"] = "padding-top: 10px; border-top: solid 1px #333;";
                    ea.Row.Cells[1].Text = "<h4 style=\"margin: 0px;\">£ " + m.Basket.TotalValue.ToString("###,###.##") + "</h4>";
                    ea.Row.Cells.RemoveAt(2);
                    ea.Row.Cells.RemoveAt(2);
                }
                break;
        }
    }
    #endregion

    #region private methods
    private void RenderView()
    {
        var m = Helpers.GetCurrentUser();
        if (m == null)
            return;

        if (m.Basket.Order != null)
        {
            // an order has already been created for this basket, so we musn't create a new one.
            _checkoutPrompt.Visible = true;
            _checkoutPrompt.InnerHtml = "You already have an order pending for this basket. Check <a href=\"../account/orders/\">your orders</a> for more information.";
            DisableCheckout();
            _blockJS.Visible = false;
        }
        else
        {
            _blockJS.Visible = true;
        }

        if (m.Basket.Items.Count == 0)
        {
            _checkoutPrompt.Visible = true;
            _checkoutPrompt.InnerText = "Your basket is empty!";
            DisableCheckout();
            _noItems.Visible = true;
            return;
        }

        // choose a completion mechanism.
        switch (m.Company.ChargeMethod)
        {
            case ChargeMethod.PointOfSale:
                _pointOfSaleHolder.Visible = true;
                break;

            case ChargeMethod.NoCharge:
                _noChargeHolder.Visible = true;
                break;
        }

        _summaryGrid.DataSource = m.Basket.Items;
        _summaryGrid.DataBind();
    }

    private void DisableCheckout()
    {
        _checkoutBtn.Enabled = false;
    }
    #endregion
}