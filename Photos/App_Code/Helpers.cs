using System;
using System.Configuration;
using System.Web;
using System.Web.UI.WebControls;
using MotoProfessional;
using MotoProfessional.Models.Interfaces;
using MPN.Framework.Web;

namespace App_Code
{
	public class Helpers
	{
		public static string GetCountryIconUrl(ICountry country)
		{
			return country == null ? string.Empty : string.Format("~/_images/flags/{0}.gif", country.Alpha2);
		}

		/// <summary>
		/// If the user is currently logged-in, then a MotoProfession.Models.Member will be returned.
		/// </summary>
		/// <returns>
		/// A Member object if they're authenticated, otherwise null if anonymous. 
		/// For Anonymous users, use the ASPNET Page.User object.
		/// </returns>
		public static IMember GetCurrentUser()
		{
			// this will exist if they're authenticated. otherwise null will be returned.
			if (HttpContext.Current.Session != null)
				return HttpContext.Current.Session["Member"] as IMember;
			return null;
		}

		/// <summary>
		/// Allows a user response message to be shown to the user on the subsequent page view. Useful for if you need to
		/// perform a redirect but still wish to show a message at the resulting page.
		/// </summary>
		public static void AddUserResponse(string response)
		{
			HttpContext.Current.Session["UserResponseContent"] = response;
		}

		/// <summary>
		/// If a user-response has been stored from the previous request then this will be returned.
		/// </summary>
		public static string RenderUserResponse()
		{
			var response = string.Empty;
			if (HttpContext.Current.Session != null && HttpContext.Current.Session["UserResponseContent"] != null)
			{
				response = HttpContext.Current.Session["UserResponseContent"] as string;
				HttpContext.Current.Session.Remove("UserResponseContent");
			}

			return response;
		}

		/// <summary>
		/// Populates a drop-down list with the iso standard country list.
		/// </summary>
		public static void PopulateCountryDropDown(DropDownList list)
		{
			list.Items.Clear();
			list.DataTextField = "Name";
			list.DataValueField = "Id";

			list.DataSource = Controller.Instance.PeripheralController.GetCountries();
			list.DataBind();

			list.SelectedValue = Controller.Instance.PeripheralController.GetCountry(Convert.ToInt32(ConfigurationManager.AppSettings["DefaultCountryID"])).Id.ToString();
		}

		/// <summary>
		/// Ensures that the partner pages are only viewed by those who are part of a partner.
		/// </summary>
		public static void SecurePartnerPage()
		{
			var member = GetCurrentUser();
			if (member.Company != null && member.Company.Partner != null) return;
			AddUserResponse("<b>Naughty!</b> - You can't go there, sorry.");
			HttpContext.Current.Response.Redirect("~/");
		}

		/// <summary>
		/// Returns a virtual url for a particular domain-object within the Admin sub-application.
		/// </summary>
		/// <example>
		/// /admin/customers/customer.aspx?i=43
		/// </example>
		/// <param name="domainObject">A supported domain-object, i.e Collection, Photo, Partner, etc.</param>
		public static string BuildAdminLink(object domainObject)
		{
			var root = HttpContext.Current.Request.ApplicationPath;
			if (root == "/")
				root = string.Empty;

			if (domainObject is IMember)
			{
				var m = domainObject as IMember;
				return string.Format("{0}/admin/sales/customers/customer.aspx?uid={1}", root, m.MembershipUser.ProviderUserKey);
			}
			if (domainObject is IOrder)
			{
				var o = domainObject as IOrder;
				return string.Format("{0}/admin/sales/orders/order.aspx?i={1}", root, o.Id);
			}
			if (domainObject is ICompany)
			{
				var c = domainObject as ICompany;
				return string.Format("{0}/admin/sales/companies/company.aspx?i={1}", root, c.Id);
			}
			if (domainObject is ICollection)
			{
				var c = domainObject as ICollection;
				return string.Format("{0}/admin/sales/photos/collection.aspx?i={1}", root, c.Id);
			}
			if (domainObject is IPhoto)
			{
				var p = domainObject as IPhoto;
				return string.Format("{0}/admin/sales/photos/photo.aspx?i={1}", root, p.Id);
			}
			if (domainObject is IPartner)
			{
				var p = domainObject as IPartner;
				return string.Format("{0}/admin/sales/partners/partner.aspx?i={1}", root, p.Id);
			}

			return string.Empty;
		}

		/// <summary>
		/// Returns a virtual url for a particular domain-object.
		/// </summary>
		/// <example>
		/// /photos/collections/134/bsb-brands-hatch-round-1-rescheduled
		/// </example>
		/// <param name="domainObject">A supported domain-object, i.e Collection, Photo, Partner, etc.</param>
		public static string BuildLink(object domainObject)
		{
			return BuildLink(domainObject, string.Empty);
		}

		/// <summary>
		/// Returns a virtual url for a particular domain-object.
		/// </summary>
		/// <example>
		/// /photos/collections/134/bsb-brands-hatch-round-1-rescheduled
		/// </example>
		/// <param name="domainObject">A supported domain-object, i.e Collection, Photo, Partner, etc.</param>
		/// <param name="isAbsolute">If true, a full http:// url is returned.</param>
		public static string BuildLink(object domainObject, bool isAbsolute)
		{
			return BuildLink(domainObject, string.Empty, isAbsolute);
		}
	
		/// <summary>
		/// Returns a virtual URL for a particular domain-object.
		/// </summary>
		/// <example>
		/// /photos/collections/134/bsb-brands-hatch-round-1-rescheduled
		/// </example>
		/// <param name="domainObject">A supported domain-object, i.e Collection, Photo, Partner, etc.</param>
		/// <param name="tag">Builds a link to a tag for the domain-object if supplied.</param>
		public static string BuildLink(object domainObject, string tag)
		{
			return BuildLink(domainObject, tag, false);
		}

		/// <summary>
		/// Returns a virtual URL for a particular domain-object.
		/// </summary>
		/// <example>
		/// /photos/collections/134/bsb-brands-hatch-round-1-rescheduled
		/// </example>
		/// <param name="domainObject">A supported domain-object, i.e Collection, Photo, Partner, etc.</param>
		/// <param name="tag">Builds a link to a tag for the domain-object if supplied.</param>
		/// <param name="isAbsolute">If true, a full http:// url is returned.</param>
		public static string BuildLink(object domainObject, string tag, bool isAbsolute)
		{
			string root;
			if (isAbsolute)
			{
				root = ConfigurationManager.AppSettings["FormalServiceUrl"];
				if (root.EndsWith("/"))
					root = root.Substring(0, root.Length - 1);
			}
			else
			{
				root = HttpContext.Current.Request.ApplicationPath;
				if (root == "/")
					root = string.Empty;
			}

			if (domainObject is ICollection)
			{
				var c = domainObject as ICollection;
				return string.Format("{0}/photos/collections/{1}/{2}", root, c.Id, Web.EncodeString(Web.UrlEncodingType.MediaPanther, c.Name));
			}
			if (domainObject is IPhoto)
			{
				var p = domainObject as IPhoto;
				return !string.IsNullOrEmpty(tag) ? string.Format("{0}/photos/tags/{1}", root, Web.EncodeString(Web.UrlEncodingType.MediaPanther, Web.EncodedStringMode.Compliant, tag)) : string.Format("{0}/photos/{1}/{2}", root, p.Id, Web.EncodeString(Web.UrlEncodingType.MediaPanther, p.Name));
			}
			if (domainObject is IPartner)
			{
				var p = domainObject as IPartner;
				return string.Format("{0}/partners/{1}/{2}", root, p.Id, Web.EncodeString(Web.UrlEncodingType.MediaPanther, p.Name));
			}
			if (domainObject is IOrder)
			{
				var o = domainObject as IOrder;
				return string.Format("{0}/account/orders/{1}", root, o.Id);
			}
			if (domainObject is ILicense)
			{
				var l = domainObject as ILicense;
				return string.Format("{0}/help/licenses/{1}/{2}", root, l.Id, Web.EncodeString(Web.UrlEncodingType.MediaPanther, l.Name));
			}

			// unknown domain-object.
			return string.Empty;
		}

		/// <summary>
		/// Returns an absolute URL for a given path, i.e. "http://domain.com/endPath".
		/// </summary>
		/// <param name="endPath">The complete path off the domain, i.e. "endPath" for "http://domain.com/endPath".</param>
		public static string BuildLink(string endPath)
		{
			return string.Format("{0}/{1}", ConfigurationManager.AppSettings["FormalServiceUrl"], endPath);
		}

		/// <summary>
		/// Returns the physical path of a partner's incoming-media folder.
		/// </summary>
		public static string GetPartnerIncomingFolder(IPartner partner)
		{
			return string.Format("{0}{1}\\incoming\\", ConfigurationManager.AppSettings["MediaPath"], partner.Id);
		}

		/// <summary>
		/// Attempts to retrieve the lates PhotoSearchContainer from the users Session.
		/// </summary>
		public static PhotoSearchContainer GetLatestPhotoSearchContainer()
		{
			if (HttpContext.Current.Session == null)
				return null;

			var nextId = Web.GetNextSessionContainerID(typeof(PhotoSearchContainer));
			if (nextId == 1)
				return null;

			return HttpContext.Current.Session[string.Format("{0}:{1}", typeof(PhotoSearchContainer), nextId - 1)] as PhotoSearchContainer;
		}

		/// <summary>
		/// Refreshes the current page, but takes care to use the cleanest url possible.
		/// </summary>
		public static void RefreshPage()
		{
			var u = HttpContext.Current.Request.Url.AbsoluteUri.ToLower();
		
			// trim any trailing default document part.
			if (u.EndsWith("/default.aspx"))
				u = u.Substring(0, u.LastIndexOf("/default.aspx"));

			// rewriting bug puts querystring params on rewritten urls. remove it.
			if (u.Contains("?") && !u.Contains("aspx?"))
				u = u.Substring(0, u.LastIndexOf("?"));

			HttpContext.Current.Response.Redirect(u, true);
		}
	}
}