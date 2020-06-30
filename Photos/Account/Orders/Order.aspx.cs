using System;
using System.Linq;
using System.Configuration;
using System.Web.UI.WebControls;
using _masterPages;
using App_Code;
using MotoProfessional;
using MotoProfessional.Models.Interfaces;
using MPN.Framework.Content;
using MPN.Framework.Files;

namespace Account.Orders
{
    public partial class OrderPage : System.Web.UI.Page
    {
        #region members
        private IOrder _order;
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Request.QueryString["id"]))
            {
                Helpers.AddUserResponse("<b>Whoops!</b> - No order specified.");
                Response.Redirect("~/account/orders/");
            }

            _order = Controller.Instance.CommerceController.GetOrder(Convert.ToInt32(Request.QueryString["id"]));
            if (_order == null)
            {
                Logger.LogWarning("Order.aspx - Retrieved order is null. ID: " + Request.QueryString["id"]);
                Helpers.AddUserResponse("<b>Whoops!</b> - That order couldn't be retrieved.");
                Response.Redirect("~/account/orders/");
            }

            // make sure this user is an employee of the company that the customer ordered it for.
            if (_order.Customer.Company.Employees.Count(q => q.Member.Uid == (Guid)Helpers.GetCurrentUser().MembershipUser.ProviderUserKey) == 0)
            {
                Logger.LogInfo(string.Format("Order.aspx - Order doesn't belong to user's company. ID: {0}, user: {1}", Request.QueryString["id"], Helpers.GetCurrentUser().MembershipUser.ProviderUserKey));
                Helpers.AddUserResponse("<b>Whoops!</b> - That order couldn't be retrieved.");
                Response.Redirect("~/account/orders/");
            }

            // basic page setup.
            var masterPage = Page.Master as MasterPagesRegular;
            masterPage.CustomBreadCrumbs = string.Format("<a href=\"{0}account/orders/\">Your Orders</a> > Order", Page.ResolveUrl("~/"));
            masterPage.SelectedTab = "account";

            if (!Page.IsPostBack)
                RenderView();
        }

        #region event handlers
        protected void OrderItemRowCreatedHandler(object sender, GridViewRowEventArgs ea)
        {
            if (ea.Row.RowType != DataControlRowType.DataRow) return;
            var oi = ea.Row.DataItem as IOrderItem;
            if (oi == null)
                return;
			
            var name = ea.Row.FindControl("_name") as Literal;
            var license = ea.Row.FindControl("_license") as Literal;
            var dimensions = ea.Row.FindControl("_dimensions") as Literal;
            var cost = ea.Row.FindControl("_cost") as Literal;
            var filesize = ea.Row.FindControl("_filesize") as Literal;
            var noLink = ea.Row.FindControl("_noLink") as Label;
            var link = ea.Row.FindControl("_link") as HyperLink;

            if (name != null) name.Text = oi.PhotoProduct.Photo.Name;
            if (license != null) license.Text = oi.PhotoProduct.License.Name;
            if (cost != null) cost.Text = oi.SaleAmount.ToString("N2");

            if (oi.DigitalGood != null)
            {
                if (dimensions != null)
                    dimensions.Text = string.Format("{0} x {1}", oi.DigitalGood.Size.Width, oi.DigitalGood.Size.Height);
                if (filesize != null) 
                    filesize.Text = Files.GetFriendlyFilesize(oi.DigitalGood.Filesize);
            }
            else
            {
                if (link != null) 
                    link.Visible = false;
                if (noLink != null) 
                    noLink.Visible = true;
            }

            if (oi.Order.ChargeStatus != ChargeStatus.Complete)
            {
                if (link != null) 
                    link.Enabled = false;
            }
            else if (link != null) 
            {
                link.NavigateUrl = string.Format("~/download.ashx?o={0}&oi={1}", oi.Order.Id, oi.Id);
            }
        }

        protected void CreateDownloadsHandler(object sender, EventArgs ea)
        {
            // a full update will trigger the digital-goods creation.
            Controller.Instance.CommerceController.UpdateOrder(_order, true);
            Helpers.AddUserResponse("<b>Underway!</b> - Your downloads are being created. If they're not available straight-away, refresh this page in a moment.");
            Response.Redirect(Helpers.BuildLink(_order));
        }
        #endregion

        #region private methods
        private void RenderView()
        {
            var downloadsAvailable = false;

            // BIZ-RULES TO DEFINE WHO CAN DOWNLOAD
            // *****************************************************************************
            if (_order.ChargeStatus == ChargeStatus.Complete)
                downloadsAvailable = true;
            else if (Helpers.GetCurrentUser().Company.ChargeMethod == ChargeMethod.Invoiced)
                downloadsAvailable = true;
            // *****************************************************************************

            if (downloadsAvailable)
            {
                _downloadsNotAvailable.Visible = false;
                _zipDiv.Visible = true;
            }

            // basic details.
            _ourReference.Text = _order.Id.ToString();
            _created.Text = _order.Created.ToString(ConfigurationManager.AppSettings["ShortDateTimeFormatString"]);
            _status.Text = _order.ChargeStatus.ToString();
            _method.Text = Text.SplitCamelCaseWords(_order.ChargeMethod.ToString());
            _total.Text = Helpers.GetCurrentUser().Company.ChargeMethod == ChargeMethod.NoCharge ? "-" : _order.ChargeAmount.ToString("N2");

            // order-items/downloads.
            if (_order.ChargeStatus != ChargeStatus.Complete) return;

            // do the DigitalGoods exist?
            if (_order.HasDigitalGoods)
            {
                _zipDiv.Attributes["class"] = "Highlight";
                _zipFileLink.Enabled = true;
                _noDownloads.Visible = false;
            }
            else
            {
                _zipDiv.Attributes["class"] = "LightContentBox Faint";
                _zipFileLink.CssClass = "H4";
                _zipFileLink.Enabled = false;
                _noDownloads.Visible = true;
                _noDownloads.Attributes["class"] = "Highlight";

                // if the goods are in production, disable the creation control.
                if (_order.DigitalGoodsInProduction)
                {
                    _noDownloads.Attributes["class"] = "LightContentBox Faint";
                    _createDownloadsBtn.Enabled = false;
                }
            }

            // no an expected case but we need to cover it just in case the master-dg doesn't exist.
            _zipFileLink.Enabled = _order.MasterDigitalGood != null;
            _zipFileLink.NavigateUrl = string.Format("~/download.ashx?o={0}", _order.Id);
            _orderItems.DataSource = _order.Items;
            _orderItems.DataBind();
        }
        #endregion
    }
}