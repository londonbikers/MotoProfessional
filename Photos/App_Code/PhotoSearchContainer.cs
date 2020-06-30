using System;
using System.Web;
using MotoProfessional;

namespace App_Code
{
	/// <summary>
	/// Holds photo search criteria so it can be stored in session and retrieved by a simple id.
	/// </summary>
	public class PhotoSearchContainer
	{
		#region members
		private int _id;
		private PhotoOrientation _orientation = PhotoOrientation.NotDefined;
		private PhotoSearchProperty _property = PhotoSearchProperty.Tags;
		#endregion

		#region accessors
		/// <summary>
		/// Indicates an inflectional search, i.e. similar words or variations on a phrase.
		/// </summary>
		public bool Inflectional { get; set; }
		public string Term { get; set; }
		/// <summary>
		/// The primary Photo property to search against.
		/// </summary>
		public PhotoSearchProperty Property { get { return _property; } set { _property = value; } }
		public DateTime CapturedFrom { get; set; }
		public DateTime CapturedUntil { get; set; }
		public PhotoOrientation Orientation { get { return _orientation; } set { _orientation = value; } }
		/// <summary>
		/// Contains the actual search results.
		/// </summary>
		public PhotoPaginator Paginator { get; set; }
		/// <summary>
		/// Denotes the current page the user is on within the results.
		/// </summary>
		public int Page { get; set; }
		public string SessionId { get { return GetType() + ":" + Id; } }
		public int Id
		{
			get
			{
				// -- DISABLED, JUST ONE SEARCH AT A TIME NOW.
				//if (_id == 0)
				//	_id = Web.GetNextSessionContainerID(GetType());

				_id = 1;
				return _id;
			}
		}
		#endregion

		#region public methods
		public bool IsValid()
		{
			// need to search for something.
			return !string.IsNullOrEmpty(Term);
		}

		/// <summary>
		/// Builds a url that has all the search parameters in so the user can save or share it.
		/// </summary>
		public string BuildSearchUrl()
		{
			var url = string.Format("~/photos/search/?c={0}&t={1}&f={2}&o={3}", Id, HttpContext.Current.Server.UrlEncode(Term), Property.ToString().ToLower(), Orientation.ToString().ToLower());

			if (CapturedFrom != DateTime.MinValue)
				url += string.Format("&cf={0}", CapturedFrom.ToShortDateString());

			if (CapturedUntil != DateTime.MinValue)
				url += string.Format("&cu={0}", CapturedUntil.ToShortDateString());

			return url;
		}
		#endregion
	}
}