using System;
using System.Xml;
using System.Web;
using System.Text;
using System.Linq;
using System.Configuration;
using MotoProfessional;
using MPN.Framework;
using MPN.Framework.Web;
using MPN.Framework.Content;

namespace App_Code
{
	/// <summary>
	/// Handles the generation of RSS XML.
	/// </summary>
	public class RssHandler : IHttpHandler
	{
		#region members
		private HttpContext _context;
		#endregion

		#region public methods
		/// <summary>
		/// Called by ASP.NET to process the http request. This is the equiv main function.
		/// </summary>
		public void ProcessRequest(HttpContext context)
		{
			_context = context;
			context.Response.Buffer = true;
			context.Response.ContentType = "text/xml";
			var writer = new XmlTextWriter(context.Response.OutputStream, Encoding.UTF8);
			var url = ConfigurationManager.AppSettings["FormalServiceUrl"];

			writer.WriteStartDocument();
			writer.WriteStartElement("rss");
			writer.WriteAttributeString("version", "2.0");
			writer.WriteStartElement("channel");

			writer.WriteElementString("language", "en-Gb");
			writer.WriteElementString("lastBuildDate", Common.DateTimeToRFC822String(DateTime.Now));
			writer.WriteElementString("link", url);
			writer.WriteElementString("description", ConfigurationManager.AppSettings["FormalServiceDescription"]);

			writer.WriteStartElement("image");
			writer.WriteElementString("title", ConfigurationManager.AppSettings["FormalServiceName"]);
			writer.WriteElementString("url", url + "/_images/rss-logo.gif");
			writer.WriteElementString("link", url);
			writer.WriteEndElement();

			// ------------------------------------------------------------
			// build the feed items.

			if (!string.IsNullOrEmpty(_context.Request.QueryString["tag"]))
				RenderLatestTagPhotos(writer, Web.DecodeString(Web.UrlEncodingType.MediaPanther, _context.Request.QueryString["tag"]));
			else
				RenderLatestCollections(writer);

			// ------------------------------------------------------------

			writer.WriteEndElement();
			writer.WriteEndElement();
			writer.WriteEndDocument();
			writer.Close();
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
		private void RenderLatestCollections(XmlWriter writer)
        {
		    writer.WriteElementString("title", "Latest Photo Collections");
			foreach (var c in Controller.Instance.PhotoController.LatestCollections)
			{
				var link = Helpers.BuildLink(c, true);
				var coverUrl = Helpers.BuildLink(string.Format("i.ashx?i={0}&d=100", c.Photos[0].Photo.Id));

				writer.WriteStartElement("item");
				writer.WriteElementString("guid", link);
				writer.WriteElementString("title", c.Name);

				var imageFragment = string.Format("<a href=\"{0}\"><img border=\"0\" src=\"{1}\" style=\"float: left; padding-right: 10px;\" /></a>", link, coverUrl);
				writer.WriteElementString("description", imageFragment + Text.GetFirstParagraph(c.Description));

				writer.WriteElementString("link", link);
				writer.WriteEndElement();
			}
        }

		private void RenderLatestTagPhotos(XmlWriter writer, string rawTag)
		{
		    if (writer == null) throw new ArgumentNullException("writer");
		    var tag = Controller.Instance.PhotoController.GetPhotoTag(rawTag);
			writer.WriteElementString("title", string.Format("Latest {0} Photos", Text.CapitaliseEachWord(tag.Tag)));

			foreach (var p in tag.LatestPhotos.Take(100))
			{
				var link = Helpers.BuildLink(p, true);
				var coverUrl = Helpers.BuildLink(string.Format("i.ashx?i={0}&d=621", p.Id));

				writer.WriteStartElement("item");
				writer.WriteElementString("guid", link);
				writer.WriteElementString("title", p.Name);

				var imageFragment = string.Format("<a href=\"{0}\"><img border=\"0\" src=\"{1}\" style=\"float: left; padding-right: 10px;\" /></a>", link, coverUrl);
				writer.WriteElementString("description", imageFragment + Text.GetFirstParagraph(p.Comment));

				writer.WriteElementString("link", link);
				writer.WriteEndElement();
			}
		}
		#endregion
	}
}