using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using App_Code;
using MotoProfessional;
using MotoProfessional.Models.Interfaces;
using MPN.Framework;

public partial class BasketPage : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
            RenderView();
    }

    #region event handlers
    protected void ListCommandHandler(object sender, CommandEventArgs ea)
    {
        if (ea.CommandName == "remove")
            RemoveItem(Convert.ToInt32(ea.CommandArgument));
    }

    protected void ListItemCreatedHandler(object sender, RepeaterItemEventArgs ea)
    {
        switch (ea.Item.ItemType)
        {
            case ListItemType.AlternatingItem:
            case ListItemType.Item:
                {
                    var bi = ea.Item.DataItem as IBasketItem;
                    if (bi == null)
                        return;

                    // only photo products are supported for now.
                    if (bi.PhotoProduct != null)
                    {
                        var thumb = ea.Item.FindControl("_thumbLink") as HyperLink;
                        var licenseLink = ea.Item.FindControl("_licenseLink") as HyperLink;
                        var name = ea.Item.FindControl("_photoName") as Literal;
                        var license = ea.Item.FindControl("_licenseName") as Literal;
                        var rate = ea.Item.FindControl("_rate") as Literal;
                        var quantity = ea.Item.FindControl("_quantity") as Literal;
                        var licenseDescription = ea.Item.FindControl("_licenseDescription") as Literal;
                        var dimensions = ea.Item.FindControl("_dimensions") as Literal;
                        var photoDescription = ea.Item.FindControl("_photoDescription") as Label;
                        var removeBtn = ea.Item.FindControl("_removeBtn") as ImageButton;

                        if (thumb != null)
                        {
                            thumb.NavigateUrl = Helpers.BuildLink(bi.PhotoProduct.Photo);
                            thumb.ImageUrl = string.Format("~/i.ashx?i={0}&d=100", bi.PhotoProduct.Photo.Id);
                        }
                        if (name != null) name.Text = bi.PhotoProduct.Photo.Name;
                        if (license != null) license.Text = bi.PhotoProduct.License.Name;
                        if (licenseDescription != null)
                            licenseDescription.Text = Common.ToWebFormString(MPN.Framework.Content.Text.GetFirstParagraph(bi.PhotoProduct.License.ShortDescription));
                        if (licenseLink != null)
                            licenseLink.NavigateUrl = Helpers.BuildLink(bi.PhotoProduct.License);
                        if (photoDescription != null)
                            photoDescription.Text = Common.ToWebFormString(MPN.Framework.Content.Text.GetFirstParagraph(bi.PhotoProduct.Photo.Comment));
                        if (rate != null) rate.Text = bi.PhotoProduct.Rate.ToString("N2");
                        if (quantity != null) quantity.Text = "1";
                        if (removeBtn != null) removeBtn.CommandName = "remove";

                        if (dimensions != null)
                            dimensions.Text = bi.PhotoProduct.License.PrimaryDimension < 9999 ? string.Format("{0} px {1}", bi.PhotoProduct.License.PrimaryDimension, (bi.PhotoProduct.Photo.Orientation == PhotoOrientation.Portrait) ? "tall" : "wide") : "Original";
                    }
                }
                break;
            case ListItemType.Footer:
                {
                    var m = Helpers.GetCurrentUser();
                    if (m == null)
                        return;

                    var total = ea.Item.FindControl("_totalValue") as Literal;
                    if (total != null) total.Text = m.Basket.TotalValue.ToString("N2");
                }
                break;
        }
    }

    protected void EmptyBasketHandler(object sender, EventArgs ea)
    {
        Helpers.GetCurrentUser().EmptyBasket();
        Helpers.AddUserResponse("<b>Done!</b> - Your basket has been emptied.");
        Response.Redirect("./");
    }
    #endregion

    #region private methods
    private void RenderView()
    {
        var m = Helpers.GetCurrentUser();
        if (m == null)
            Response.Redirect("~/");

        if (m.Basket.Items.Count > 0)
        {
            _basketContents.DataSource = m.Basket.Items;
            _basketContents.DataBind();
            _basketControlsView.Visible = true;
        }
        else
        {
            _noItemsDiv.Visible = true;
            _basketControlsView.Visible = false;
            _basketContents.Visible = false;
        }
    }

    private void RemoveItem(int basketItemId)
    {
        var m = Helpers.GetCurrentUser();
        if (m == null)
            return;

        m.Basket.RemoveBasketItem(basketItemId);
        Helpers.AddUserResponse("<b>Photo Removed!</b>");
        Response.Redirect(Request.Url.AbsoluteUri);
    }
    #endregion
}