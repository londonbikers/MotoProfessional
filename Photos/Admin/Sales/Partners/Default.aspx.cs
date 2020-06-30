using System;
using System.Configuration;
using System.Web.UI.WebControls;
using App_Code;
using MotoProfessional;
using MotoProfessional.Models.Interfaces;

namespace Admin.Sales.Partners
{
    public partial class PartnersPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RenderView();
        }

        #region event handlers
        protected void PartnerRowCreatedHandler(object sender, GridViewRowEventArgs ea)
        {
            if (ea.Row.RowType != DataControlRowType.DataRow) return;
            var p = ea.Row.DataItem as IPartner;
            if (p == null)
                return;

            var link = ea.Row.FindControl("_link") as HyperLink;
            var company = ea.Row.FindControl("_company") as HyperLink;
            var website = ea.Row.FindControl("_website") as HyperLink;
            var name = ea.Row.FindControl("_name") as Literal;
            var status = ea.Row.FindControl("_status") as Literal;
            var joined = ea.Row.FindControl("_joined") as Literal;
            var country = ea.Row.FindControl("_countryIcon") as Image;

            if (link != null) link.NavigateUrl = Helpers.BuildAdminLink(p);
            if (company != null)
            {
                company.Text = p.Company.Name;
                company.NavigateUrl = Helpers.BuildAdminLink(p.Company);
            }

            if (name != null) name.Text = p.Name;
            if (status != null) status.Text = p.Status.ToString();
            if (joined != null)
                joined.Text = p.Created.ToString(ConfigurationManager.AppSettings["ShortDateFormatString"]);
            if (country != null)
            {
                country.AlternateText = p.Company.Country.Name;
                country.ImageUrl = Helpers.GetCountryIconUrl(p.Company.Country);
            }

            if (p.Company.Url == null) return;
            if (website == null) return;
            website.Enabled = true;
            website.Text = p.Company.Url.Host;
            website.NavigateUrl = p.Company.Url.AbsoluteUri;
        }
        #endregion

        #region private methods
        private void RenderView()
        {
            _partners.DataSource = Controller.Instance.PartnerController.LatestPartners;
            _partners.DataBind();
        }
        #endregion
    }
}