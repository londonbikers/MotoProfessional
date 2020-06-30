using System;
using System.Configuration;
using System.Web.UI.WebControls;
using _masterPages;
using App_Code;
using MotoProfessional;
using MotoProfessional.Models.Interfaces;

namespace Admin.Sales.Orders
{
    public partial class OrdersPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RenderView();
        }

        #region event handlers
        protected void OrderRowCreatedHandler(object sender, GridViewRowEventArgs ea)
        {
            if (ea.Row.RowType != DataControlRowType.DataRow) return;
            var o = ea.Row.DataItem as IOrder;
            if (o == null)
                return;

            var customerLink = ea.Row.FindControl("_customerLink") as HyperLink;
            var companyLink = ea.Row.FindControl("_companyLink") as HyperLink;
            var link = ea.Row.FindControl("_link") as HyperLink;
            var ordered = ea.Row.FindControl("_ordered") as Literal;
            var items = ea.Row.FindControl("_items") as Literal;
            var total = ea.Row.FindControl("_total") as Literal;
            var method = ea.Row.FindControl("_method") as Literal;
            var state = ea.Row.FindControl("_state") as Literal;

            if (customerLink != null)
            {
                customerLink.Text = o.Customer.GetFullName();
                customerLink.NavigateUrl = Helpers.BuildAdminLink(o.Customer);
            }

            if (o.Customer.Company != null && companyLink != null)
            {
                companyLink.Text = o.Customer.Company.Name;
                companyLink.NavigateUrl = Helpers.BuildAdminLink(o.Customer.Company);
            }
            else
            {
                if (companyLink != null) companyLink.Enabled = false;
            }

            if (link != null) link.NavigateUrl = Helpers.BuildAdminLink(o);
            if (ordered != null)
                ordered.Text = o.Created.ToString(ConfigurationManager.AppSettings["ShortDateTimeFormatString"]);
            if (items != null) items.Text = o.Items.Count.ToString();
            if (method != null)
                method.Text = MPN.Framework.Content.Text.SplitCamelCaseWords(o.ChargeMethod.ToString());
            if (total != null) total.Text = o.ChargeAmount.ToString("N2");

            switch (o.ChargeStatus)
            {
                case ChargeStatus.PartialRefund:
                    if (state != null) state.Text = "Partial Refund";
                    break;
                case ChargeStatus.Outstanding:
                    if (state != null) state.Text = "In Progress";
                    break;
                default:
                    if (state != null) state.Text = o.ChargeStatus.ToString();
                    break;
            }
        }
        #endregion

        #region private methods
        private void RenderView()
        {
            var master = Page.Master as AdminMaster;
            if (master != null) master.ShowJQueryAlphanumeric = true;

            _orders.DataSource = Controller.Instance.CommerceController.LatestOrders;
            _orders.DataBind();
        }
        #endregion
    }
}