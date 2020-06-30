using System;
using System.Collections.Generic;
using App_Code;
using MotoProfessional;
using VRK.Controls;

public partial class TagCloud : System.Web.UI.UserControl
{
	#region members
	private List<CloudItem> _items = new List<CloudItem>();
	private string _linkCssPrefix = "TC";
	#endregion

	#region accessors
	/// <summary>
	/// Allows the default datasource of the Photos.HotPhotos to be over-ridden.
	/// </summary>
	public List<CloudItem> Items { get { return _items; } set { _items = value; } }
	public string LinkCssPrefix { get { return _linkCssPrefix; } set { _linkCssPrefix = value; } }
	#endregion

	protected void Page_Load(object sender, EventArgs e)
	{
		// make sure the cloud hasn't already been populated by a custom data-source.
		if (_items.Count > 0)
			return;

		// set-up a default data-source for the cloud.
		_items.Clear();

		// we need a photo, any photo, for the BuildLink() helper method.
		var p = Controller.Instance.PhotoController.HotPhotos[0];
		foreach (var ts in Controller.Instance.PhotoController.Tags.LatestTags)
			_items.Add(new CloudItem(ts.Tag, Convert.ToDouble(ts.Count), Helpers.BuildLink(p, ts.Tag)));

		RenderCloud();
	}

	#region public methods
	public void RenderCloud()
	{
		_cloud.ItemCssClassPrefix = _linkCssPrefix;
		_cloud.DataSource = _items;
		_cloud.DataHrefField = "Href";
		_cloud.DataTextField = "Text";
		_cloud.DataWeightField = "Weight";
		_cloud.DataBind();
	}
	#endregion
}