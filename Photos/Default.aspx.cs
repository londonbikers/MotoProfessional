using System;
using System.Linq;
using System.Web.UI.WebControls;
using App_Code;
using MotoProfessional;
using MotoProfessional.Models.Interfaces;
using MPN.Framework;

public partial class HomePage : System.Web.UI.Page
{
	#region members
	protected string PhotoTileXmlUrl;
	#endregion

	protected void Page_Load(object sender, EventArgs e)
	{
		Page.EnableViewState = false;
		RenderForm();
	}

	#region event handlers
	protected void OnLoggedOutHandler(object sender, EventArgs ea)
	{
		Session.Abandon();
	}

	protected void LatestCollectionsItemCreatedHandler(object sender, RepeaterItemEventArgs ea)
	{
	    if (ea.Item.ItemType != ListItemType.Item && ea.Item.ItemType != ListItemType.AlternatingItem) return;
	    var c = ea.Item.DataItem as ICollection;
	    var collectionLink = ea.Item.FindControl("_collectionLink") as Literal;
	    if (c == null)
	        return;

	    if (collectionLink != null)
	        collectionLink.Text = string.Format("<a href=\"{0}\">{1}</a>", Helpers.BuildLink(c), Common.ToShortString(c.Name, 40));
	}
	#endregion

	#region private methods
	private void RenderForm()
	{
		if (User.Identity.IsAuthenticated)
		{
			_memberControls.Visible = false;
			var member = Helpers.GetCurrentUser();
			if (member.Company != null && member.Company.Partner != null)
				_partnerPromotionBox.Visible = false;
		}

		_newCollectionsList.DataSource = Controller.Instance.PhotoController.LatestCollections.Take(10);
		_newCollectionsList.DataBind();

		// this isn't a physical url, it'll be re-written by isapi_rewrite. this is to break SWF caching.
		PhotoTileXmlUrl = string.Format("phototile-{0}.ashx", DateTime.Now.Ticks);
	}
	#endregion
}