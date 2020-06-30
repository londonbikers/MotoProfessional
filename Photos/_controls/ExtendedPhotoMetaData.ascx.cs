using System;
using MotoProfessional.Models.Interfaces;

namespace _controls
{
	public partial class ExtendedPhotoMetaData : System.Web.UI.UserControl
	{
		#region members
		private IPhoto _photo;
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
		}

		#region public methods
		public bool IsExtendedMetaDataEmpty()
		{
			if (_photo == null || _photo.MetaData == null || _photo.MetaData.ExtendedData == null)
				return true;

			var m = _photo.MetaData.ExtendedData;

			return string.IsNullOrEmpty(m.CreatorTool) &&
			       string.IsNullOrEmpty(m.ExposureTime) &&
			       string.IsNullOrEmpty(m.FlashFired) &&
			       string.IsNullOrEmpty(m.FlashFunction) &&
			       string.IsNullOrEmpty(m.FlashMode) &&
			       string.IsNullOrEmpty(m.FlashRedEyeMode) &&
			       string.IsNullOrEmpty(m.FlashReturn) &&
			       string.IsNullOrEmpty(m.FNumber) &&
			       string.IsNullOrEmpty(m.FocalLength) &&
			       string.IsNullOrEmpty(m.Iso) &&
			       string.IsNullOrEmpty(m.Lens) &&
			       string.IsNullOrEmpty(m.Model) &&
			       string.IsNullOrEmpty(m.XResolution) &&
			       string.IsNullOrEmpty(m.YResolution);
		}

		public void LoadData(IPhoto p)
		{
			_photo = p;

			if (p.MetaData == null || p.MetaData.ExtendedData == null)
				p.RetrieveMetaData(true);

			if (p.MetaData == null || p.MetaData.ExtendedData == null)
				return;

			var m = p.MetaData.ExtendedData;

			if (!string.IsNullOrEmpty(m.Model))
				_cameraModel.Text = m.Model;
			else
				_cameraModelRow.Visible = false;

			if (!string.IsNullOrEmpty(m.Lens))
				_lens.Text = m.Lens;
			else
				_lensRow.Visible = false;

			if (!string.IsNullOrEmpty(m.Iso))
				_iso.Text = m.Iso;
			else
				_isoRow.Visible = false;

			if (!string.IsNullOrEmpty(m.XResolution) && !string.IsNullOrEmpty(m.YResolution))
			{
				var x = (m.XResolution.Contains("/")) ? m.XResolution.Split(char.Parse("/"))[0] : m.XResolution;
				var y = (m.YResolution.Contains("/")) ? m.YResolution.Split(char.Parse("/"))[0] : m.YResolution;
				_resolution.Text = string.Format("{0} dpi x {1} dpi", x, y);
			}
			else
			{
				_resolutionRow.Visible = false;
			}

			if (!string.IsNullOrEmpty(m.ExposureTime))
				_exposureTime.Text = m.ExposureTime;
			else
				_exposureTimeRow.Visible = false;

			if (!string.IsNullOrEmpty(m.FNumber))
				_fNumber.Text = m.FNumber;
			else
				_fNumberRow.Visible = false;

			if (!string.IsNullOrEmpty(m.FocalLength))
				_focalLength.Text = m.FocalLength;
			else
				_focalLengthRow.Visible = false;

			if (!string.IsNullOrEmpty(m.FlashFired))
				_flashFired.Text = (m.FlashFired.ToLower() == "true") ? "Yes" : "No";
			else
				_flashFiredRow.Visible = false;
		}
		#endregion
	}
}