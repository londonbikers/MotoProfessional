using System;
using System.Linq;
using System.IO;
using App_Code;
using MotoProfessional;
using MotoProfessional.Models;
using MotoProfessional.Models.Interfaces;
using MPN.Framework.Web;

namespace Admin.Sales.Partners
{
    public partial class PartnerPage : System.Web.UI.Page
    {
        #region members
        private IPartner _partner;
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Request.QueryString["i"]))
                _partner = Controller.Instance.PartnerController.GetPartner(Convert.ToInt32(Request.QueryString["i"]));

            if (!Page.IsPostBack)
                RenderView();
        }

        #region event handler
        protected void FindCompanyHandler(object sender, EventArgs ea)
        {
            if (string.IsNullOrEmpty(_companySearchBox.Text.Trim()))
                return;

            var companies = Controller.Instance.CompanyController.GetCompaniesByLooseName(_companySearchBox.Text.Trim(), 100).Where(q => q.Partner == null);
            if (companies.Count() > 0)
            {
                _companySelection.DataSource = companies;
                _companySelection.DataBind();
                _companySelectionPlaceHolder.Visible = true;
                ToggleCommonPropertiesForm(true);
            }
            else 
            {
                _companySelection.Items.Clear();
                _companySelection.Items.Add("None found!");
                ToggleCommonPropertiesForm(false);
            }
        }

        protected void UpdatePartnerHandler(object sender, EventArgs ea)
        {
            if (!Page.IsValid)
                return;

            if (_partner == null)
                _partner = Controller.Instance.PartnerController.NewPartner();

            if (!_partner.IsPersisted)
                _partner.Company = Controller.Instance.CompanyController.GetCompany(Convert.ToInt32(_companySelection.SelectedValue));

            _partner.Name = _name.Text.Trim();
            _partner.Description = _description.Text.Trim();

            if (_partner.IsPersisted)
                _partner.Status = (GeneralStatus)byte.Parse(_status.SelectedValue);

            #region logo file
            if (_logoUploader.HasFile)
            {
                if (!_logoUploader.PostedFile.FileName.ToLower().EndsWith(".jpg") &&
                    !_logoUploader.PostedFile.FileName.ToLower().EndsWith(".jpeg") &&
                    !_logoUploader.PostedFile.FileName.ToLower().EndsWith(".gif") &&
                    !_logoUploader.PostedFile.FileName.ToLower().EndsWith(".png"))
                {
                    Helpers.AddUserResponse("<b>Oops!</b> - That logo isn't a valid image (jpeg, gif or png's accepted).");
                    return;
                }

                // delete any previous image.
                if (!string.IsNullOrEmpty(_partner.LogoFilename))
                {
                    try
                    {
                        File.Delete(_partner.GetFullLogoFilePath());
                    } catch {}
                }

                // save the image to the file-system.
                _logoUploader.PostedFile.SaveAs(Partner.GetFullLogoPath(_partner, _logoUploader.PostedFile.FileName.ToLower()));
                _partner.LogoFilename = Path.GetFileName(_logoUploader.PostedFile.FileName);
            }
            #endregion

            Controller.Instance.PartnerController.UpdatePartner(_partner);
            Helpers.AddUserResponse("<b>Updated!</b> The partner details have been saved.");
            Response.Redirect(Helpers.BuildAdminLink(_partner));
        }

        protected void DeleteLogoHandler(object sender, EventArgs ea)
        {
            _partner.DeleteLogoFile();
            Helpers.AddUserResponse("<b>Deleted!</b> - The logo has been deleted.");
            Response.Redirect(Helpers.BuildAdminLink(_partner));
        }
        #endregion

        #region private methods
        private void ToggleCommonPropertiesForm(bool enableForm)
        {
            _commonPropertiesDiv.Visible = enableForm;
        }

        private void RenderView()
        {
            ToggleCommonPropertiesForm(false);
            if (_partner != null)
            {
                ToggleCommonPropertiesForm(true);
                _findCompanyDiv.Visible = false;
                _statusRow.Visible = true;

                _udpateBtn.Text = "Update Partner";
                _name.Text = _partner.Name;
                _pageHeading.Text = _partner.Name;
                _description.Text = _partner.Description;
                _uploadLabel.Visible = false;
                _uploadPlaceHolder.Visible = true;
                _noCompanyLabel.Visible = false;
                _companyLink.Text = _partner.Company.Name;
                _companyLink.NavigateUrl = Helpers.BuildAdminLink(_partner.Company);

                _joined.Text = _partner.Created.ToString("f");
                _collectionCount.Text = _partner.Statistics.Collections.ToString("#,###");
                if (string.IsNullOrEmpty(_collectionCount.Text))
                    _collectionCount.Text = "0";

                _photoCount.Text = _partner.Statistics.Photos.ToString("###,###");
                if (string.IsNullOrEmpty(_photoCount.Text))
                    _photoCount.Text = "0";

                _photosSold.Text = _partner.Statistics.PhotosSold.ToString("###,###");
                if (string.IsNullOrEmpty(_photosSold.Text))
                    _photosSold.Text = "0";

                _totalSales.Text = _partner.Statistics.PhotosSoldValue.ToString("###,###.##");
                if (string.IsNullOrEmpty(_totalSales.Text))
                    _totalSales.Text = "-";

                Web.PopulateDropDownFromEnum(_status, _partner.Status, true);
                _status.SelectedValue = ((byte)_partner.Status).ToString();

                if (!string.IsNullOrEmpty(_partner.LogoFilename))
                {
                    _logoFrame.Visible = true;
                    _logo.ImageUrl = string.Format("~/i.ashx?l=pr&i={0}&t={1}", _partner.Id, DateTime.Now.Ticks); // the tick is just to create a unique url and break any client-side caching.
                    _deleteLinkDiv.Visible = true;
                }
            }
            else
            {
                _findCompanyDiv.Visible = true;
                _udpateBtn.Text = "Create Partner";
                _uploadLabel.Visible = true;
                _uploadPlaceHolder.Visible = false;
                _noCompanyLabel.Visible = true;
                _companyLink.Visible = false;
            }
        }
        #endregion
    }
}