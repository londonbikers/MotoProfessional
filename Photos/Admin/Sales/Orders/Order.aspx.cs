using System;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using App_Code;
using MotoProfessional;
using MotoProfessional.Models.Interfaces;
using MPN.Framework.Files;

namespace Admin.Sales.Orders
{
    public partial class OrderPage : System.Web.UI.Page
    {
        #region members
        private IOrder _order;
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            _order = Controller.Instance.CommerceController.GetOrder(Convert.ToInt32(Request.QueryString["i"]));
            if (_order == null)
            {
                Helpers.AddUserResponse("<b>Sorry</b> - No such order exists.");
                Response.Redirect("~/admin/sales/orders/");
            }

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

            var license = ea.Row.FindControl("_license") as Literal;
            var dimensions = ea.Row.FindControl("_dimensions") as Literal;
            var cost = ea.Row.FindControl("_cost") as Literal;
            var filesize = ea.Row.FindControl("_filesize") as Literal;
            var noLink = ea.Row.FindControl("_noLink") as Label;
            var link = ea.Row.FindControl("_link") as HyperLink;
            var productLink = ea.Row.FindControl("_productLink") as HyperLink;

            if (productLink != null)
            {
                productLink.Text = oi.PhotoProduct.Photo.Name;
                productLink.NavigateUrl = Helpers.BuildLink(oi.PhotoProduct.Photo);
            }
            if (license != null) license.Text = oi.PhotoProduct.License.Name;
            if (cost != null) cost.Text = oi.SaleAmount.ToString("N2");

            if (oi.DigitalGood != null)
            {
                if (dimensions != null)
                    dimensions.Text = string.Format("{0} x {1}", oi.DigitalGood.Size.Width, oi.DigitalGood.Size.Height);
                if (filesize != null) filesize.Text = Files.GetFriendlyFilesize(oi.DigitalGood.Filesize);
            }
            else
            {
                if (link != null) link.Visible = false;
                if (noLink != null) noLink.Visible = true;
            }

            if (oi.Order.ChargeStatus != ChargeStatus.Complete && link != null)
                link.Enabled = false;
            else if (link != null) 
                link.NavigateUrl = string.Format("~/download.ashx?o={0}&oi={1}", oi.Order.Id, oi.Id);
        }

        protected void DownloadLogRowCreatedHandler(object sender, GridViewRowEventArgs ea)
        {
            if (ea.Row.RowType != DataControlRowType.DataRow) return;
            var log = ea.Row.DataItem as IDigitalGoodDownloadLog;
            if (log == null)
                return;

            var whoLink = ea.Row.FindControl("_whoLink") as HyperLink;
            var ipLink = ea.Row.FindControl("_ipAddressLink") as HyperLink;
            var what = ea.Row.FindControl("_what") as Literal;
            var when = ea.Row.FindControl("_when") as Literal;
            var referrer = ea.Row.FindControl("_referrer") as Literal;
            var clientBox = ea.Row.FindControl("_clientNameBox") as HtmlGenericControl;

            if (what != null)
                what.Text = string.Format(log.DigitalGood.Type == DigitalGoodType.ZipArchive ? "ZIP ({0})" : "Photo: {0}", log.DigitalGood.Filename);

            var m = Controller.Instance.MemberController.GetMember(log.CustomerUid);
            if (whoLink != null)
            {
                whoLink.Text = m.GetFullName();
                whoLink.NavigateUrl = Helpers.BuildAdminLink(m);
            }
            if (when != null)
                when.Text = log.Created.ToString(ConfigurationManager.AppSettings["ShortDateTimeFormatString"]);
            if (ipLink != null)
            {
                ipLink.Text = log.IpAddress;
                ipLink.NavigateUrl = string.Format("http://private.dnsstuff.com/tools/whois.ch?domain={0}", log.IpAddress);
            }
            if (referrer != null) referrer.Text = log.HttpReferrer;
            if (clientBox != null) clientBox.Attributes["title"] = log.ClientName;
        }

        protected void TransactionLogRowCreatedHandler(object sender, GridViewRowEventArgs ea)
        {
            if (ea.Row.RowType != DataControlRowType.DataRow) return;
            var ot = ea.Row.DataItem as IOrderTransaction;
            if (ot == null)
                return;

            var when = ea.Row.FindControl("_when") as Literal;
            var type = ea.Row.FindControl("_type") as Literal;
            var gon = ea.Row.FindControl("_googleOrderNumber") as Literal;
            var content = ea.Row.FindControl("_content") as Literal;

            if (when != null)
                when.Text = ot.Created.ToString(ConfigurationManager.AppSettings["LongDateTimeFormatString"]);
            if (!string.IsNullOrEmpty(ot.GoogleOrderNumber) && gon != null)
                gon.Text = ot.GoogleOrderNumber;
            if (type != null) type.Text = ot.Type.ToString();

            switch (ot.Type)
            {
                case OrderTransactionType.GC_ChargeAmount:
                    if (content != null) content.Text = "£ " + ot.GcChargeAmount.ChargedAmount.ToString("N2");
                    break;
                case OrderTransactionType.GC_ChargebackAmount:
                    if (content != null) content.Text = "£ " + ot.GcChargebackAmount.ChargebackAmount.ToString("N2");
                    break;
                case OrderTransactionType.GC_NewOrder:
                    if (content != null) content.Text = "-";
                    break;
                case OrderTransactionType.GC_OrderStateChange:
                    if (content != null)
                        content.Text = string.Format("NewFinanceState: {1}<br /><span class=\"Faint\">PrevFinanceState: {0}</span><br />NewFulfillmentState: {3}<br /><span class=\"Faint\">PrevFulfillmentState: {2}</span>", ot.GcOrderStateChange.PrevFinanceState, ot.GcOrderStateChange.NewFinanceState, ot.GcOrderStateChange.PrevFulfillmentState, ot.GcOrderStateChange.NewFulfillmentState);
                    break;
                case OrderTransactionType.GC_RefundAmount:
                    if (content != null) content.Text = "£ " + ot.GcRefundAmount.RefundedAmount.ToString("N2");
                    break;
                case OrderTransactionType.GC_RiskInformation:
                    if (content != null)
                        content.Text = string.Format("AVS: {0}<br />CVN: {1}", ot.GcRiskInformation.Avs, ot.GcRiskInformation.Cvn);
                    break;
                case OrderTransactionType.Generic:
                    if (content != null) content.Text = ot.Operation;
                    break;
            }
        }

        protected void UpdateOrderStatusHandler(object sender, EventArgs ea)
        {
            _order.ChargeStatus = (ChargeStatus)Enum.Parse(typeof(ChargeStatus), _status.SelectedValue);
            Controller.Instance.CommerceController.UpdateOrder(_order);

            // log this action.
            var ot = _order.NewOrderTransaction();
            ot.Type = OrderTransactionType.Generic;
            ot.Member = Helpers.GetCurrentUser();
            ot.Operation = "Order status changed to: " + _order.ChargeStatus.ToString();
            _order.AddTransaction(ot);
		
            Helpers.AddUserResponse("<b>Updated!</b> - The order status has been updated.");
            Response.Redirect(Request.Url.AbsoluteUri);
        }

        protected void CreateDownloadsHandler(object sender, EventArgs ea)
        {
            // a full update will trigger the digital-goods creation.
            Controller.Instance.CommerceController.UpdateOrder(_order, true);
            Helpers.AddUserResponse("<b>Underway!</b> - The downloads are being created. If they're not available straight-away, refresh this page in a moment.");
            Response.Redirect(Helpers.BuildAdminLink(_order));
        }
        #endregion

        #region private methods
        private void RenderView()
        {
            #region basic details
            _customerLink.Text = _order.Customer.GetFullName();
            _customerLink.NavigateUrl = Helpers.BuildAdminLink(_order.Customer);

            if (_order.Customer.Company != null)
            {
                _companyLink.Text = _order.Customer.Company.Name;
                _companyLink.NavigateUrl = Helpers.BuildAdminLink(_order.Customer.Company);
                _companyLink.Enabled = true;
            }

            _ourReference.Text = _order.Id.ToString();
            _created.Text = _order.Created.ToString(ConfigurationManager.AppSettings["ShortDateTimeFormatString"]);
            _method.Text = _order.ChargeMethod.ToString();
            _total.Text = _order.ChargeAmount.ToString("N2");

            _status.DataSource = Enum.GetNames(typeof(ChargeStatus));
            _status.DataBind();
            _status.SelectedValue = _order.ChargeStatus.ToString();
            #endregion

            #region items
            if (_order.ChargeStatus == ChargeStatus.Complete)
            {
                // do the DigitalGoods exist?
                if (_order.HasDigitalGoods)
                {
                    _zipDiv.Attributes["class"] = "Highlight";
                    _zipFileLink.Enabled = true;
                    _noDownloads.Visible = false;

                    // merge all the download-logs into one list.
                    var logs = new List<IDigitalGoodDownloadLog>();
                    if (_order.MasterDigitalGood != null && _order.MasterDigitalGood.Logs != null)
                        logs.AddRange(_order.MasterDigitalGood.Logs);

                    logs.AddRange(_order.Items.Where(item => item.DigitalGood != null).SelectMany(item => item.DigitalGood.Logs));

                    if (logs.Count > 0)
                    {
                        _downloadLogs.DataSource = logs.OrderBy(q => q.Created);
                        _downloadLogs.DataBind();
                    }
                    else
                    {
                        _noDownloadLogs.Visible = true;
                    }
                }
                else
                {
                    _noDownloadLogs.Visible = true;

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

            #region transactions
            if (_order.Transactions.Count > 0)
            {
                _transactionLogs.DataSource = _order.Transactions;
                _transactionLogs.DataBind();
            }
            else
            {
                _noTransactionLogs.Visible = true;
            }
            #endregion
        }
        #endregion
    }
}