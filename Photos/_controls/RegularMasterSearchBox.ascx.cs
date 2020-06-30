using System;
using System.Web.UI;

public partial class _controls_RegularMasterSearchBox : UserControl
{
	#region members
	protected string _searchBoxWidth;
	#endregion

	#region accessors
	public int SearchBoxWidth
	{
		set
		{
			_searchBoxWidth = string.Format("width: {0}px; ", value);
		}
	}
	#endregion

	protected void Page_Load(object sender, EventArgs e)
	{
	}

	#region event handlers
	protected void SearchHandler(object sender, ImageClickEventArgs ea)
	{
		if (!string.IsNullOrEmpty(Request.Form["searchBoxTerm"]))
			Response.Redirect(string.Format("{0}photos/search/?t={1}", Page.ResolveUrl("~/"), Server.UrlEncode(Request.Form["searchBoxTerm"])));
	}
	#endregion
}