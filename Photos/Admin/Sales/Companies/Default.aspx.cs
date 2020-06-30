using System;
using System.Configuration;
using System.Web.UI.WebControls;
using App_Code;
using MotoProfessional;
using MotoProfessional.Models.Interfaces;

namespace Admin.Sales.Companies
{
    public partial class CompaniesPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RenderView();
        }

        #region event handlers
        protected void CompanyRowCreatedHandler(object sender, GridViewRowEventArgs ea)
        {
            if (ea.Row.RowType != DataControlRowType.DataRow) return;
            var c = ea.Row.DataItem as ICompany;
            if (c == null)
                return;

            var link = ea.Row.FindControl("_link") as HyperLink;
            var website = ea.Row.FindControl("_website") as HyperLink;
            var name = ea.Row.FindControl("_name") as Literal;
            var partner = ea.Row.FindControl("_partner") as Literal;
            var staff = ea.Row.FindControl("_staff") as Literal;
            var chargeMethod = ea.Row.FindControl("_chargeMethod") as Literal;
            var status = ea.Row.FindControl("_status") as Literal;
            var joined = ea.Row.FindControl("_joined") as Literal;
            var country = ea.Row.FindControl("_countryIcon") as Image;

            if (link != null) link.NavigateUrl = Helpers.BuildAdminLink(c);

            if (c.Url != null)
            {
                if (website != null)
                {
                    website.Enabled = true;
                    website.Text = c.Url.Host;
                    website.NavigateUrl = c.Url.AbsoluteUri;
                }
            }

            if (name != null) name.Text = c.Name;
            if (staff != null) staff.Text = (c.Employees != null) ? c.Employees.Count.ToString() : "0";
            if (status != null) status.Text = c.Status.ToString();
            if (joined != null)
                joined.Text = c.Created.ToString(ConfigurationManager.AppSettings["ShortDateFormatString"]);
            if (chargeMethod != null)
                chargeMethod.Text = MPN.Framework.Content.Text.SplitCamelCaseWords(c.ChargeMethod.ToString());
            if (country != null)
            {
                country.AlternateText = c.Country.Name;
                country.ImageUrl = Helpers.GetCountryIconUrl(c.Country);
            }

            if (c.Partner != null && partner != null)
                partner.Text = c.Partner.Name;
        }
        #endregion

        #region private methods
        private void RenderView()
        {
            _companies.DataSource = Controller.Instance.CompanyController.LatestCompanies;
            _companies.DataBind();
        }
        #endregion
    }
}