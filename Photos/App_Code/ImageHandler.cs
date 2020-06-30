using System;
using System.Collections.Generic;
using System.Web;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using MotoProfessional;
using MotoProfessional.Models.Interfaces;
using MPN.Framework.Imaging;
using MPN.Framework.Security;

namespace App_Code
{
	/// <summary>
	/// Handles all image-manipulation tasks for the application, i.e. resizing, watermarking, etc.
	/// </summary>
	public class ImageHandler : IHttpHandler
	{
		#region members
		private HttpContext _context;
        private Watermark _watermark = Watermark.Heavy;
		private ImageLocation _imageLocation = ImageLocation.Library;
        private const ImageWatermarker.WatermarkLocation WatermarkLocation = ImageWatermarker.WatermarkLocation.Center;
		private string _fileType;
		private string _imagePath = string.Empty;
		private IPhoto _photo;
		private IPartner _partner;
        private int _primaryDimensionSize;
        private bool _constrainToSquare;
        private bool _resizeByHeight;
        private bool _resizeByWidth;
        private bool _specificDimensions;
        private int _specificWidth;
        private int _specificHeight;
		#endregion

        #region enums
        public enum Watermark
        {
            None,
            Regular,
            Heavy
        }

		public enum ImageLocation
		{
			Library,
			PartnerRoot
		}
        #endregion

        #region public methods
        /// <summary>
		/// Called by ASP.NET to process the http request. This is the equiv main function.
		/// </summary>
		public void ProcessRequest(HttpContext context)
		{
			_context = context;
			if (!ProcessParametersAndValidate())
				return;

			ResizeAndRenderImage();
		}

		/// <summary>
		/// Determines is subsequent requests can reuse this handler.
		/// </summary>
		public bool IsReusable
		{
			get { return false; }
		}
		#endregion

		#region private methods
		/// <summary>
		/// Resizes an image from disk and renders it to the output stream.
		/// </summary>
		private void ResizeAndRenderImage()
		{
			Image image;
			var resizer = new ImageResizer();

            // ensure the output is cachable by the client.
            _context.Response.Cache.SetExpires(DateTime.Now.AddYears(10));
            _context.Response.Cache.SetCacheability(HttpCacheability.Private);
            _context.Response.Cache.SetValidUntilExpires(true);

			#region determine source image path
			switch (_imageLocation)
			{
			    case ImageLocation.Library:
			        _imagePath = !IsRequestForPreviewImage() ? _photo.FullStorePath : _photo.GetPreviewImagePath(_primaryDimensionSize);
			        break;
			    case ImageLocation.PartnerRoot:
			        _imagePath = _partner.GetFullLogoFilePath();
			        break;
			}
			#endregion

			// set the output format/quality (100% quality).
			var info = ImageCodecInfo.GetImageEncoders();
			var encParams = new EncoderParameters(1);
			encParams.Param[0] = new EncoderParameter(Encoder.Quality, 100L);

			try
			{
				image = Image.FromFile(_imagePath);
			}
			catch (Exception ex)
			{
				Logger.LogWarning(string.Format("ImageHandler.ResizeAndRenderImage() - Cannot open Image from path '{0}'.", _imagePath), ex);
				return;
			}

            GetContentTypeFromFile();
            _context.Response.ContentType = _fileType;

            // only resize non-preview images.
            if ((!IsRequestForPreviewImage() && _imageLocation == ImageLocation.Library) || _imageLocation == ImageLocation.PartnerRoot)
            {
                if (_primaryDimensionSize < 1)
                    _primaryDimensionSize = (image.Width > image.Height) ? image.Width : image.Height;

                if (_specificDimensions)
                    image = resizer.ResizeImage(image, new Size(_specificWidth, _specificHeight));
                else if (_resizeByHeight)
                    image = resizer.ResizeImage(image, _primaryDimensionSize, ImageResizer.Axis.Vertical);
                else if (_resizeByWidth)
                    image = resizer.ResizeImage(image, _primaryDimensionSize, ImageResizer.Axis.Horizontal);
                else
                    image = resizer.ResizeImage(image, _primaryDimensionSize, _constrainToSquare);
            }

			if (_watermark != Watermark.None)
			    image = new ImageWatermarker().WatermarkImage(image, GetWatermarkImage(_watermark), WatermarkLocation);

			image.Save(_context.Response.OutputStream, info[1], encParams);
			image.Dispose();
		}

		/// <summary>
		/// Attempts to produce a mime content-type for a file/path.
		/// </summary>
		private void GetContentTypeFromFile()
		{
			// we may already have a mime-type as part of the params.
			if (!string.IsNullOrEmpty(_fileType))
				return;

			// otherwise determine the type from the path extension.
			var extension = Path.GetExtension(_imagePath).ToLower();

			switch (extension)
			{
			    case ".jpe":
			    case ".jpeg":
			    case ".jpg":
			        _fileType = "image/jpeg";
			        break;
			    case ".jps":
			        _fileType = "image/x-jps";
			        break;
			    case ".gif":
			        _fileType = "image/gif";
			        break;
			    case ".png":
			        _fileType = "image/png";
			        break;
			    case ".bm":
			    case ".bmp":
			        _fileType = "image/bmp";
			        break;
			    case ".htm":
			    case ".html":
			        _fileType = "text/html";
			        break;
			    case ".txt":
			        _fileType = "text/plain";
			        break;
			    case ".css":
			        _fileType = "text/css";
			        break;
			    case ".js":
			        _fileType = "application/x-javascript";
			        break;
			    case ".avi":
			        _fileType = "video/avi";
			        break;
			    case ".mp3":
			        _fileType = "audio/mpeg3";
			        break;
			    case ".flv":
			        _fileType = "video/x-flv";
			        break;
			    case ".swf":
			        _fileType = "application/x-shockwave-flash";
			        break;
			    case ".pdf":
			        _fileType = "application/pdf";
			        break;
			    default:
			        _fileType = "application/octet-stream";
			        break;
			}
		}

		/// <summary>
		/// Assigns the right class members and validates the parameters to ensure we have everything we need.
		/// </summary>
		private bool ProcessParametersAndValidate()
		{
            // plain or encyrpted parameters?
            if (!string.IsNullOrEmpty(_context.Request.QueryString["e"]))
            {
                #region encrypted params
                var eParams = DeserialiseParameters(_context.Server.UrlDecode(_context.Request.QueryString["e"]));

                // identifier required.
                if (!eParams.ContainsKey("i"))
                    return false;

                // decode params.
                _resizeByHeight = (eParams.ContainsKey("rh")) ? true : false;
                _resizeByWidth = (eParams.ContainsKey("rw")) ? true : false;
                _constrainToSquare = (eParams.ContainsKey("s")) ? true : false;
                _primaryDimensionSize = (eParams.ContainsKey("d")) ? Convert.ToInt32(eParams["d"]) : 0;

                if (eParams.ContainsKey("nw"))
                    _watermark = Watermark.None;

				// different location than the library (default)?
				if (eParams.ContainsKey("l") && eParams["l"] == "pr")
				{
					_imageLocation = ImageLocation.PartnerRoot;
					_watermark = Watermark.None;
					_partner = Controller.Instance.PartnerController.GetPartner(Convert.ToInt32(eParams["i"]));
					if (_partner == null)
						return false;
				}
				else
				{
					// require a primary dimension, unless a specific size is supplied.
					if (!eParams.ContainsKey("d") && !eParams.ContainsKey("sw"))
						return false;

					_photo = Controller.Instance.PhotoController.GetPhoto(Convert.ToInt32(eParams["i"]));
					if (_photo == null)
						return false;
				}

                // is this a crop/specific sizing request?
                if (eParams.ContainsKey("sw") && eParams.ContainsKey("sh"))
                {
                    _specificDimensions = true;
                    _specificWidth = Convert.ToInt32(eParams["sw"]);
                    _specificHeight = Convert.ToInt32(eParams["sh"]);
                }
                #endregion
            }
            else
            {
                #region plain params
                // identifier required.
                if (string.IsNullOrEmpty(_context.Request.QueryString["i"]))
                    return false;

                // decode params.
                _resizeByHeight = (!string.IsNullOrEmpty(_context.Request.QueryString["rh"])) ? true : false;
                _resizeByWidth = (!string.IsNullOrEmpty(_context.Request.QueryString["rw"])) ? true : false;
                _constrainToSquare = (!string.IsNullOrEmpty(_context.Request.QueryString["s"])) ? true : false;
                _primaryDimensionSize = Convert.ToInt32(_context.Request.QueryString["d"]);

				// different location than the library (default)?
				if (!string.IsNullOrEmpty(_context.Request.QueryString["l"]) && _context.Request.QueryString["l"] == "pr")
				{
					_imageLocation = ImageLocation.PartnerRoot;
					_watermark = Watermark.None;
					_partner = Controller.Instance.PartnerController.GetPartner(Convert.ToInt32(_context.Request.QueryString["i"]));
					if (_partner == null)
						return false;
				}
				else
				{
					// require a primary dimension, unless a specific size is supplied.
					if (string.IsNullOrEmpty(_context.Request.QueryString["d"]) && string.IsNullOrEmpty(_context.Request.QueryString["sw"]))
						return false;

					// assign.
					_photo = Controller.Instance.PhotoController.GetPhoto(Convert.ToInt32(_context.Request.QueryString["i"]));
					if (_photo == null)
						return false;
				}

                // is this a crop/specific sizing request?
                if (!string.IsNullOrEmpty(_context.Request.QueryString["sw"]) && !string.IsNullOrEmpty(_context.Request.QueryString["sh"]))
                {
                    _specificDimensions = true;
                    _specificWidth = Convert.ToInt32(_context.Request.QueryString["sw"]);
                    _specificHeight = Convert.ToInt32(_context.Request.QueryString["sh"]);
                }
                #endregion
            }

            // dimensions of 150px or smaller don't have watermarks applied.
            if (_primaryDimensionSize <= 150)
                _watermark = Watermark.None;
            
			return true;
		}

        /// <summary>
        /// Preview images are served from disc, not resized. They exist as files already.
        /// </summary>
        private bool IsRequestForPreviewImage()
        {
			var previewImagePrimaryDimensions = new[] { 74, 100, 150, 620, 621 };
			return previewImagePrimaryDimensions.Contains(_primaryDimensionSize);
        }

        /// <summary>
        /// Decrypts the DES encrypted parameters.
        /// </summary>
        private Dictionary<string, string> DeserialiseParameters(string desEncryptedParameters)
        {
            var decryptedString = SecurityHelpers.DesDecrypt(desEncryptedParameters);
            var groups = decryptedString.Split(char.Parse("&"));
            return groups.Select(t => t.Split(char.Parse("="))).Where(pair => pair.Length == 2).ToDictionary(pair => pair[0], pair => pair[1]);
        }

		private Image GetWatermarkImage(Watermark watermark)
		{
			if (watermark == Watermark.None)
				return null;

			try
			{
				return Image.FromFile(HttpContext.Current.Server.MapPath(string.Format("~/_images/watermarks/{0}.png", watermark)));
			}
			catch (Exception ex)
			{
				Logger.LogWarning("Watermark image could not be loaded. Type: " + watermark, ex);
				return null;
			}
		}
		#endregion
	}
}