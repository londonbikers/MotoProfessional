using System.Web;
using MotoProfessional;
using MPN.Framework;
using MPN.Framework.Web;

namespace App_Code
{
	/// <summary>
	/// Handles the generation of SiteMap XML.
	/// </summary>
	public class SiteMapHandler : IHttpHandler
	{
		#region public methods
		/// <summary>
		/// Called by ASP.NET to process the http request. This is the equiv main function.
		/// </summary>
		public void ProcessRequest(HttpContext context)
		{
			RenderSiteMap(context);
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
		private void RenderSiteMap(HttpContext context)
		{
			context.Response.ContentType = "text/XML";
			context.Response.Write("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n");
			context.Response.Write("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">\n");

			var items = Controller.Instance.PeripheralController.GetSiteMapItems();
			var url = string.Empty;
			foreach (var item in items)
			{
				if (item.ContentType == SiteMapItemContentType.Collection)
				{
					// not ideal, this is custom url generation, which we don't want to do!
					url = Helpers.BuildLink(string.Format("photos/collections/{0}/{1}", item.ItemId, Web.EncodeString(Web.UrlEncodingType.MediaPanther, item.Title)));
				}

				context.Response.Write("<url>\n");
				context.Response.Write(string.Format("<loc>{0}</loc>\n", url));
				context.Response.Write(string.Format("<lastmod>{0}</lastmod>\n", Common.DateTimeToIso8601String(item.LastModified)));
				context.Response.Write("</url>\n");
			}

			context.Response.Write("</urlset>");
		}
		#endregion
	}
}