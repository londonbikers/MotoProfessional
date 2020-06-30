using System;
using System.Configuration;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using _masterPages;
using App_Code;
using MotoProfessional;
using MotoProfessional.Models.Interfaces;

namespace Account.Partner.Rates
{
    public partial class PartnerRatesPage : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Helpers.SecurePartnerPage();
            ((MasterPagesRegular) Page.Master).SelectedTab = "account";

            RenderForm();

            // nested controls mean events aren't firing. not got the time to research a fix, so going old-school.
            if (!Page.IsPostBack && !string.IsNullOrEmpty(Request.QueryString["erc"]))
                PopulateEditRateCardForm(Convert.ToInt32(Request.QueryString["erc"]));
        }

        #region event handlers
        protected void ShowEditRateCardView(object sender, EventArgs ea)
        {
            RenderEditRateCardForm();
        }

        protected void CancelEditHandler(object sender, EventArgs ea)
        {
            Response.Redirect("./");
        }

        protected void UpdateRateCardHandler(object sender, EventArgs ea)
        {
            if (!Page.IsValid)
                return;

            // custom validation.
            if (!IsRateCardItemEditsValid())
            {
                _invalidRCIs.Visible = true;
                return;
            }

            var rc = ViewState["EditRateCardID"] == null ? Controller.Instance.LicenseController.NewRateCard() : Controller.Instance.LicenseController.GetRateCard(Convert.ToInt32(ViewState["EditRateCardID"]));
            var member = Helpers.GetCurrentUser();
            rc.Name = _editRcName.Text.Trim();
            rc.IsDefault = _editRcIsDefault.Checked;
            rc.Status = (GeneralStatus)Enum.Parse(typeof(GeneralStatus), _editRcStatus.SelectedValue);
            rc.Partner = member.Company.Partner;

            // rate-card-items. -- ensure we have all the items. new liceneses may have been added since 
            // the item was first created if this is an edit, not a create.
            rc.PopulateRateCardItems();
            var names = Request.Form.AllKeys.Where(s => s.Contains("_editRcLicense:"));
            foreach (var name in names)
            {
                var pos = name.IndexOf("_editRcLicense:") + 15;
                var licenseId = int.Parse(name.Substring(pos));
                rc.Items.Single(rci => rci.License.Id == licenseId).Amount = decimal.Parse(Request.Form[name]);
            }

            Controller.Instance.LicenseController.UpdateRateCard(rc);
            member.Company.Partner.AddRateCard(rc);

            Helpers.AddUserResponse(string.Format("<b>{0}!</b> - Your changes have been saved.", (rc.IsPersisted) ? "Updated" : "Added"));
            Response.Redirect("./");
        }

        protected void RateCardsItemCreatedHandler(object sender, RepeaterItemEventArgs ea)
        {
            if (ea.Item.ItemType != ListItemType.Item && ea.Item.ItemType != ListItemType.AlternatingItem) return;
            var rc = ea.Item.DataItem as IRateCard;
            if (rc == null)
                return;

            var gv = ea.Item.FindControl("_rateCardItemGrid") as GridView;
            var title = ea.Item.FindControl("_rcName") as HtmlGenericControl;
            var status = ea.Item.FindControl("_rcStatus") as Label;
            var editLink = ea.Item.FindControl("_editLink") as HyperLink;

            if (title != null) title.InnerHtml = string.Format("<span class=\"Faint\">Name:</span> {0}", rc.Name);

            if (status != null)
            {
                status.Text = string.Format(rc.Status == GeneralStatus.Active ? "<span class=\"Outline\">{{{0}}}</span>" : "{{{0}}}", rc.Status);
                if (rc.IsDefault)
                    status.Text += " Default";
            }

            if (editLink != null) editLink.NavigateUrl = string.Format("./?erc={0}", rc.Id);

            if (gv == null) return;
            gv.DataSource = rc.Items.Where(qrci => qrci.License.Status == GeneralStatus.Active).OrderByDescending(rci => rci.Amount);
            gv.DataBind();
        }

        protected void RcEditFormLicenseCreatedHandler(object sender, RepeaterItemEventArgs ea)
        {
            if (ea.Item.ItemType != ListItemType.Item && ea.Item.ItemType != ListItemType.AlternatingItem) return;
            var license = ea.Item.DataItem as ILicense;
            if (license == null)
                return;

            foreach (var ctl in ea.Item.Controls.OfType<TextBox>())
                ctl.ID = "_editRcLicense:" + license.Id;
        }

        protected void RateCardItemRowCreatedHandler(object sender, GridViewRowEventArgs ea)
        {
            if (ea.Row.RowType != DataControlRowType.DataRow) return;
            var rci = ea.Row.DataItem as IRateCardItem;
            if (rci == null)
                return;

            var licenseName = ea.Row.FindControl("_licenseName") as Literal;
            var rate = ea.Row.FindControl("_rate") as Literal;
            var lastUpdated = ea.Row.FindControl("_lastUpdated") as Literal;

            if (licenseName != null) licenseName.Text = rci.License.Name;
            if (rate != null) rate.Text = rci.Amount.ToString("N2");
            if (lastUpdated != null)
                lastUpdated.Text = rci.LastUpdated.ToString(ConfigurationManager.AppSettings["ShortDateTimeFormatString"]);
        }
        #endregion

        #region private methods
        private void RenderForm()
        {
            var member = Helpers.GetCurrentUser();
            _rateCards.DataSource = member.Company.Partner.RateCards;
            _rateCards.DataBind();
            _noRateCards.Visible = (member.Company.Partner.RateCards.Exists(rc => rc.Status == GeneralStatus.Active && rc.IsDefault)) ? false : true;
        }

        private void RenderEditRateCardForm()
        {
            _editRateCardView.Visible = true;
            _showEditFormDiv.Visible = false;

            _editRcStatus.DataSource = Enum.GetNames(typeof(GeneralStatus));
            _editRcStatus.DataBind();

            _editRcLicenses.DataSource = Controller.Instance.LicenseController.GetActiveLicenses();
            _editRcLicenses.DataBind();
        }

        private void PopulateEditRateCardForm(int rateCardID)
        {
            RenderEditRateCardForm();
            var rc = Helpers.GetCurrentUser().Company.Partner.RateCards.Single(qRc => qRc.Id == rateCardID);

            _editRcTitle.InnerText = "Edit Rate Card";
            _editRcBtn.Text = "Update";
            _editRcName.Text = rc.Name;
            _editRcIsDefault.Checked = rc.IsDefault;
            _editRcStatus.SelectedValue = rc.Status.ToString();

            foreach (var rci in rc.Items)
            {
                foreach (Control control in _editRcLicenses.Controls)
                {
                    foreach (Control riCtl in control.Controls)
                    {
                        if (riCtl.ID == "_editRcLicense:" + rci.License.Id)
                            ((TextBox) riCtl).Text = rci.Amount.ToString("N2");
                    }
                }
            }

            ViewState["EditRateCardID"] = rc.Id.ToString();
        }

        /// <summary>
        /// Determines whether or not the rate-card-items in a rate-card edit form are valid.
        /// </summary>
        private bool IsRateCardItemEditsValid()
        {
            var names = Request.Form.AllKeys.Where(s => s.StartsWith("_editRcLicense:"));
            foreach (var val in names.Select(name => Request.Form[name]))
            {
                if (string.IsNullOrEmpty(val))
                    return false;

                decimal d;
                if (!decimal.TryParse(val, out d))
                    return false;
            }

            return true;
        }
        #endregion
    }
}