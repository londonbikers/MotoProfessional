using System;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using System.Web.UI.WebControls;
using _controls;
using App_Code;
using MotoProfessional;
using MotoProfessional.Models.Interfaces;
using MPN.Framework;
using MPN.Framework.Web;

namespace Admin.Sales.Companies
{
    public partial class CompanyPage : System.Web.UI.Page
    {
        #region members
        private ICompany _company;
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Request.QueryString["i"]))
            {
                Helpers.AddUserResponse("<b>Whoops</b> - No company specified.");
                Response.Redirect("./");
            }

            _company = Controller.Instance.CompanyController.GetCompany(Convert.ToInt32(Request.QueryString["i"]));
            if (_company == null)
            {
                Logger.LogWarning("Admin Company - No such company found for id: " + Request.QueryString["i"]);
                Helpers.AddUserResponse("<b>Whoops</b> - No such company found.");
                Response.Redirect("./");
            }

            _editCompanyEditor.CompanyUpdated += new CompanyDetailsEditor.UpdatedDelegate(CompanyEditedHandler);

            if (!Page.IsPostBack)
                RenderView();
        }

        #region event handlers
        /// <summary>
        /// Handles the company payment terms being changed.
        /// </summary>
        protected void EditPaymentTermsHandler(object sender, EventArgs ea)
        {
            _company.ChargeMethod = (ChargeMethod)byte.Parse(_paymentTermsDropDown.SelectedValue);
            Controller.Instance.CompanyController.UpdateCompany(_company);
            CompanyEditedHandler(_company);
        }

        /// <summary>
        /// Handles the company being updated event from the company details editor control.
        /// </summary>
        protected void CompanyEditedHandler(ICompany company)
        {
            Helpers.AddUserResponse("<b>Updated!</b> - The company details have been updated.");
            Response.Redirect(Request.Url.AbsoluteUri);
        }

        protected void EditCompanyDetailsHandler(object sender, EventArgs ea)
        {
            _confirmedAssociation.Visible = false;
            _editConfirmedCompany.Visible = true;
            _editCompanyEditor.EditExistingCompany(_company.Id, true);
        }

        protected void EmployeeItemCreatedHandler(object sender, RepeaterItemEventArgs ea)
        {
            if (ea.Item.ItemType != ListItemType.Item && ea.Item.ItemType != ListItemType.AlternatingItem) return;
            var ce = ea.Item.DataItem as ICompanyEmployee;
            if (ce == null)
                return;

            var nameLink = ea.Item.FindControl("_employeeNameLink") as HyperLink;
            var status = ea.Item.FindControl("_employeeStatus") as Literal;
            var email = ea.Item.FindControl("_employeeEmail") as HyperLink;

            if (nameLink != null)
            {
                nameLink.Text = ce.Member.GetFullName();
                nameLink.NavigateUrl = Helpers.BuildAdminLink(ce.Member);
            }

            if (ce.Status == CompanyEmployeeStatus.Pending && status != null)
                status.Visible = true;

            if (email == null) return;
            email.Text = ce.Member.MembershipUser.Email;
            email.NavigateUrl = "mailto:" + ce.Member.MembershipUser.Email;
        }

        protected void CancelEditHandler(object sender, EventArgs ea)
        {
            Response.Redirect(Request.Url.AbsoluteUri);
        }

        protected void OrderRowCreatedHandler(object sender, GridViewRowEventArgs ea)
        {
            if (ea.Row.RowType != DataControlRowType.DataRow) return;
            var o = ea.Row.DataItem as IOrder;
            if (o == null)
                return;

            var customerLink = ea.Row.FindControl("_customerLink") as HyperLink;
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
            if (link != null) link.NavigateUrl = Helpers.BuildAdminLink(o);
            if (ordered != null)
                ordered.Text = o.Created.ToString(ConfigurationManager.AppSettings["ShortDateTimeFormatString"]);
            if (items != null) items.Text = o.Items.Count.ToString();
            if (method != null) method.Text = o.ChargeMethod.ToString();
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
            #region details
            _confirmedAssociation.Visible = true;

            // required properties.
            _companyTitle.Text = _company.Name;
            _detailsTelephoneNumber.Text = _company.Telephone;
            _status.Text = _company.Status.ToString();
            Page.Title = "MPa: " + _company.Name;

            // optional properties.
            if (!string.IsNullOrEmpty(_company.Description))
                _detailsDescription.Text = _company.Description;

            if (!string.IsNullOrEmpty(_company.Fax))
                _detailsFaxNumber.Text = _company.Fax;

            if (!string.IsNullOrEmpty(_company.Address))
                _detailsAddress.Text = Common.ToWebFormString(_company.Address);

            if (!string.IsNullOrEmpty(_company.PostalCode))
                _detailsPostalCode.Text = _company.PostalCode;

            if (_company.Country != null)
            {
                _detailsCountry.Text = _company.Country.Name;
                _countryFlag.Visible = true;
                _countryFlag.ImageUrl = Helpers.GetCountryIconUrl(_company.Country);
            }

            if (_company.Url != null)
            {
                _detailsNoWebsite.Visible = false;
                _detailsWebsiteLink.Visible = true;
                _detailsWebsiteLink.NavigateUrl = _company.Url.AbsoluteUri;
                _detailsWebsiteLink.Text = _company.Url.Host;
            }

            switch (_company.ChargeMethod)
            {
                case ChargeMethod.PointOfSale:
                    _paymentTerms.Text = "Point of sale";
                    _paymentTermsDescription.Text = "Payment is required at the checkout by credit/debit-card or Google Checkout account.";
                    break;

                case ChargeMethod.Invoiced:
                    _paymentTerms.Text = "Invoice";
                    _paymentTermsDescription.Text = "Orders will be payable at the end of the calendar month.";
                    break;

                case ChargeMethod.NoCharge:
                    _paymentTerms.Text = "No Charge";
                    _paymentTermsDescription.Text = "As a special client, there are no charge for their orders and they can download straight-away.";
                    break;
            }

            Web.PopulateDropDownFromEnum(_paymentTermsDropDown, _company.ChargeMethod, true);
            _paymentTermsDropDown.SelectedValue = ((byte)_company.ChargeMethod).ToString();
            #endregion

            #region employees
            _employees.DataSource = _company.Employees;
            _employees.DataBind();
            #endregion

            #region orders
            // -- there's no orders collection as such, but we can just group together and order the individual staff orders.
            var orders = new List<IOrder>();
            foreach (var ce in _company.Employees)
                orders.AddRange(ce.Member.Orders);

            if (orders.Count == 0)
                _noOrders.Visible = true;

            var sortedOrders = from o in orders
                               orderby o.Created descending
                               select o;

            _orders.DataSource = sortedOrders.Take(100);
            _orders.DataBind();
            #endregion
        }
        #endregion
    }
}