using System;
using System.Configuration;
using System.Web.UI.WebControls;
using _masterPages;
using App_Code;
using MotoProfessional;
using MotoProfessional.Models.Interfaces;

namespace Account.Orders
{
    public partial class OrdersPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // basic page setup.
            var masterPage = Page.Master as MasterPagesRegular;
            if (masterPage != null) masterPage.SelectedTab = "account";

            RenderView();
        }

        #region event handlers
        protected void OrderRowCreatedHandler(object sender, GridViewRowEventArgs ea)
        {
            if (ea.Row.RowType != DataControlRowType.DataRow) return;
            var o = ea.Row.DataItem as IOrder;
            if (o == null)
                return;

            var link = ea.Row.FindControl("_link") as HyperLink;
            var ordered = ea.Row.FindControl("_ordered") as Literal;
            var items = ea.Row.FindControl("_items") as Literal;
            var total = ea.Row.FindControl("_total") as Literal;
            var method = ea.Row.FindControl("_method") as Literal;
            var state = ea.Row.FindControl("_state") as Literal;

            if (link != null) link.NavigateUrl = Helpers.BuildLink(o);
            if (ordered != null)
                ordered.Text = o.Created.ToString(ConfigurationManager.AppSettings["ShortDateTimeFormatString"]);
            if (items != null) items.Text = o.Items.Count.ToString();
            if (method != null)
                method.Text = MPN.Framework.Content.Text.SplitCamelCaseWords(o.ChargeMethod.ToString());
            if (total != null) total.Text = o.ChargeAmount.ToString("N2");

            switch (o.ChargeStatus)
            {
                case ChargeStatus.Complete:
                    if (state != null) state.Text = ChargeStatus.Complete.ToString();
                    break;
                case ChargeStatus.Refunded:
                    if (state != null) state.Text = ChargeStatus.Refunded.ToString();
                    break;
                case ChargeStatus.Outstanding:
                    if (state != null) state.Text = "In Progress";
                    break;
            }
        }
        #endregion

        #region private methods
        private void RenderView()
        {
            var m = Helpers.GetCurrentUser();
            if (m == null)
                Response.Redirect("~/");

            if (m.Orders.Count > 0)
            {
                _orders.DataSource = m.Orders;
                _orders.DataBind();
            }
            else
            {
                _orders.Visible = false;
                _noOrders.Visible = true;
            }
        }
        #endregion
    }
}