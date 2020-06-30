using System.Web;
using MotoProfessional;
using MPN.Framework.Security;

namespace App_Code
{
	/// <summary>
	/// Handles the generation of XML for the PhotoTile widget.
	/// </summary>
	public class PhotoTileHandler : IHttpHandler
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
            RenderLatestPhotosXml();
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
        private void RenderLatestPhotosXml()
        {
            var r = _context.Response;
            r.Write("<PhotoDataConfig>\n");
            r.Write("\t<Images>\n");

            foreach (var p in Controller.Instance.PhotoController.HotPhotos)
            {
                r.Write("\t\t<Image>\n");

                var eParams = _context.Server.UrlEncode(SecurityHelpers.DesEncrypt(string.Format("i={0}&sw=620&sh=230&nw=1", p.Id)));
                r.Write(string.Format("\t\t\t<LargeUrl>i.ashx?e={0}</LargeUrl>\n", eParams));

                r.Write(string.Format("\t\t\t<ThumbUrl>i.ashx?i={0}&d=74</ThumbUrl>\n", p.Id));
                r.Write(string.Format("\t\t\t<Headline>{0}</Headline>\n", p.Name));
				r.Write(string.Format("\t\t\t<LinkUrl>{0}</LinkUrl>\n", Helpers.BuildLink(p)));

                r.Write("\t\t</Image>\n");
            }

            r.Write("\t</Images>\n");
            r.Write("</PhotoDataConfig>");
        }
		#endregion
	}
}