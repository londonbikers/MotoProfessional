using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using MotoProfessional.Models.Interfaces;

namespace App_Code
{
	/// <summary>
	/// Responsible for simplifying the process of paginating large amounts of CollectionPhoto items.
	/// </summary>
	public class PhotoPaginator
	{
		#region members
		private int _pageSize;
		private IPhotoContainer _dataSource;
		#endregion

		#region enums
		/// <summary>
		/// Describes what type of url format to use for pagination controls.
		/// </summary>
		public enum PaginationControlsUrlFormat
		{
			/// <summary>
			/// Pagination control instructions will be displayed in a url-rewritten format, i.e. '/photos/tags/honda/page/2'.
			/// </summary>
			ReWritten,
			/// <summary>
			/// Pagination control instructions will be dislayed in a traditional querystring format, i.e. '/photos/search/s.aspx?t=honda&p=2'.
			/// </summary>
			QueryString
		}
		#endregion

		#region accessors
		/// <summary>
		/// Determines the number of items to be returned in each page.
		/// </summary>
		public int PageSize { get { return _pageSize; } set { _pageSize = value; } }
		/// <summary>
		/// The total number of pages that exist in the collection.
		/// </summary>
		public int TotalPages 
		{ 
			get 
			{
				var pages = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(DataSource.Count()) / Convert.ToDouble(PageSize)));
				if (pages == 0)
					pages = 1;

				return pages;
			} 
		}

		/// <summary>
		/// Indicates which the last page to be requested from the Paginator.
		/// </summary>
		public int CurrentPage { get; private set; }

		/// <summary>
		/// The data to be paginated.
		/// </summary>
		public IPhotoContainer DataSource
		{
			get { return _dataSource; }
			set
			{
				_dataSource = value;
				InitialisePaginator();
			}
		}

		public PaginationControlsUrlFormat UrlFormat { get; set; }
		#endregion

		#region constructors
		/// <summary>
		/// Create a new PhotoPaginator object.
		/// </summary> 
		public PhotoPaginator()
		{
			// defaults.
			PageSize = 50;
			CurrentPage = 1;
			UrlFormat = PaginationControlsUrlFormat.ReWritten;
		}
		#endregion

		#region public methods
		/// <summary>
		/// Returns the data in a collection for a specific page.
		/// </summary>
		/// <param name="pageNumber">The page of data to return, i.e. page 2.</param>
		public List<IPhoto> GetPage(int pageNumber)
		{
			if (pageNumber > TotalPages)
				pageNumber = TotalPages;

			var page = new List<IPhoto>();
			if (DataSource.Count() == 0)
				return page;

			var start = (pageNumber - 1) * _pageSize;
			var end = start + _pageSize;

			// ensure the end doesn't go out of bounds.
			if (end > DataSource.Count())
				end = DataSource.Count();

			CurrentPage = pageNumber;

			// build the new sub-collection.			
			for (var i = start; i < end; i++)
				page.Add(DataSource[i]);

			return page;
		}
		/// <summary>
		/// Builds a set of href controls to allow the user to navigate the paged data.
		/// </summary>
		/// <param name="baseUrl">An optional base Url to base the pagination Url's from. Applicable only to the QueryString url format for now.</param>
		public string BuildPaginatorControls(string baseUrl)
		{
			return UrlFormat == PaginationControlsUrlFormat.ReWritten ? BuildRewrittenUrlPaginatorControls() : BuildQueryStringUrlPaginatorControls(baseUrl);
		}
		#endregion

		#region private methods
		/// <summary>
		/// When a new datasource is supplied, this method will configure the class.
		/// </summary>
		private void InitialisePaginator()
		{
			CurrentPage = 1;
		}

		private string BuildRewrittenUrlPaginatorControls()
		{
			if (TotalPages == 1)
				return String.Empty;

			var control = new StringBuilder();

			// determine the base url.
			var url = HttpContext.Current.Request.FilePath;
			if (!url.EndsWith("/"))
				url += "/";

			if (url.IndexOf("/page/") > -1)
				url = url.Substring(0, url.IndexOf("/page/")) + "/";

			int outerPage;

			// previous page control.
			if (CurrentPage > 1)
			{
				if (CurrentPage == 2)
					control.AppendFormat("<b><a href=\"{0}\">previous</a></b> | ", url.Substring(0, url.Length - 1));
				else
					control.AppendFormat("<b><a href=\"{0}page/{1}\">previous</a></b> | ", url, (CurrentPage - 1));
			}
			else
			{
				control.Append("<span class=\"Faint\">previous</span> | ");
			}

			// show the leading pages.
			if (CurrentPage > 1)
			{
				if (CurrentPage < 5)
					outerPage = 1;
				else
					outerPage = CurrentPage - 3;

				for (var i = outerPage; i < CurrentPage; i++)
				{
					if (i == 1)
						control.AppendFormat("<a href=\"{0}\">{1}</a>, ", url.Substring(0, url.Length - 1), i);
					else
						control.AppendFormat("<a href=\"{0}page/{1}\">{1}</a>, ", url, i);
				}
			}

			// current page isn't a control.
			control.Append(CurrentPage);

			// show the trailing pages.
			if (CurrentPage > (TotalPages - 4))
				outerPage = TotalPages;
			else
				outerPage = CurrentPage + 3;

			for (var i = CurrentPage + 1; i <= outerPage; i++)
				control.AppendFormat(", <a href=\"{0}page/{1}\">{1}</a>", url, i);

			// next page control.
			if (CurrentPage < TotalPages)
				control.AppendFormat(" | <b><a href=\"{0}page/{1}\">next</a></b>", url, (CurrentPage + 1));
			else
				control.Append(" | <span class=\"Faint\">next</span>");

			return control.ToString();
		}

		private string BuildQueryStringUrlPaginatorControls(string baseUrl)
		{
			if (TotalPages == 1)
				return String.Empty;

			var control = new StringBuilder();

			// determine the base url.
			var url = (!string.IsNullOrEmpty(baseUrl)) ? baseUrl : HttpContext.Current.Request.Url.AbsoluteUri;

			if (url.IndexOf("&p=") > -1)
				url = url.Substring(0, url.IndexOf("&p="));

			int outerPage;

			// previous page control.
			if (CurrentPage > 1)
				control.AppendFormat("<b><a href=\"{0}&p={1}\">previous</a></b> | ", url, (CurrentPage - 1));
			else
				control.Append("<span class=\"Faint\">previous</span> | ");

			// show the leading pages.
			if (CurrentPage > 1)
			{
				if (CurrentPage < 5)
					outerPage = 1;
				else
					outerPage = CurrentPage - 3;

				for (var i = outerPage; i < CurrentPage; i++)
					control.AppendFormat("<a href=\"{0}&p={1}\">{1}</a>, ", url, i);
			}

			// current page isn't a control.
			control.Append(CurrentPage);

			// show the trailing pages.
			if (CurrentPage > (TotalPages - 4))
				outerPage = TotalPages;
			else
				outerPage = CurrentPage + 3;

			for (var i = CurrentPage + 1; i <= outerPage; i++)
				control.AppendFormat(", <a href=\"{0}&p={1}\">{1}</a>", url, i);

			// next page control.
			if (CurrentPage < TotalPages)
				control.AppendFormat(" | <b><a href=\"{0}&p={1}\">next</a></b>", url, (CurrentPage + 1));
			else
				control.Append(" | <span class=\"Faint\">next</a>");

			return control.ToString();
		}
		#endregion
	}
}