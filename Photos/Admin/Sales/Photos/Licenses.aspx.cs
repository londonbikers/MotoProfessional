using System;
using System.Web.UI.WebControls;
using App_Code;
using MotoProfessional;
using MotoProfessional.Models.Interfaces;

namespace Admin.Sales.Photos
{
    public partial class LicensesPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            _editForm.Visible = false;
            if (Page.IsPostBack) return;
            _licenseGrid.DataSource = Controller.Instance.LicenseController.GetLicenses();
            _licenseGrid.DataBind();
        }

        #region event handlers
        protected void ShowAddLicenseFormHandler(object sender, EventArgs ea)
        {
            RenderEditForm();
            _editFormTitle.InnerText = "Add License";
        }

        protected void EditFormCancelHandler(object sender, EventArgs ea)
        {
            _editForm.Visible = false;
            Response.Redirect(Request.Url.AbsoluteUri);
        }

        protected void EditUpdateHandler(object sender, EventArgs ea)
        {
            if (!Page.IsValid)
                return;

            var isNew = false;
            ILicense l;
            if (ViewState["LicenseID"] == null)
            {
                l = Controller.Instance.LicenseController.NewLicense();
                isNew = true;
            }
            else
            {
                l = Controller.Instance.LicenseController.GetLicense(Convert.ToInt32(ViewState["LicenseID"]));
            }

            l.Name = _editName.Text.Trim();
            l.ShortDescription = _editShortDescription.Text.Trim();
            l.Description = _editDescription.Value.Trim();
            l.Status = (GeneralStatus)Enum.Parse(typeof(GeneralStatus), _editStatus.SelectedValue);

            Controller.Instance.LicenseController.UpdateLicense(l);
        	Helpers.AddUserResponse(isNew ? "<b>Added!</b> - New license added." : "<b>Updated!</b> - License updated.");
        	Response.Redirect(Request.Url.AbsoluteUri);
        }

        protected void SetupLicenseEditHandler(object sender, GridViewCommandEventArgs ea)
        {
            var index = Convert.ToInt32(ea.CommandArgument);
            var selectedRow = ((GridView) ea.CommandSource).Rows[index];
            var hf = selectedRow.FindControl("_licenseIDField") as HiddenField;

            RenderEditForm();
            _editFormTitle.InnerText = "Edit License";
            _editBtn.Text = "Update";

            if (hf == null) return;
            var l = Controller.Instance.LicenseController.GetLicense(Convert.ToInt32(hf.Value));
            _editName.Text = l.Name;
            _editShortDescription.Text = l.ShortDescription;
            _editDescription.Value = l.Description;
            _editStatus.SelectedValue = l.Status.ToString();
            ViewState["LicenseID"] = l.Id;
        }
        #endregion

        #region private methods
        private void RenderEditForm()
        {
            _editForm.Visible = true;
            _editStatus.DataSource = Enum.GetNames(typeof(GeneralStatus));
            _editStatus.DataBind();
        }
        #endregion
    }
}