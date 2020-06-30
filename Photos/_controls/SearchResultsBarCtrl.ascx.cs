using System;
using System.Web.UI;
using App_Code;

namespace _controls
{
	public partial class SearchResultsBarCtrl : UserControl
	{
		#region accessors
		public int CurrentPhotoId { get; set; }
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			RenderView();
		}

		#region event handlers
		protected void CancelSearchHandler(object sender, EventArgs ea)
		{
			Session.Remove(Helpers.GetLatestPhotoSearchContainer().SessionId);
			Helpers.RefreshPage();
		}
		#endregion

		#region public methods
		public void SetPhoto()
		{
			RenderView();
		}
		#endregion

		#region private methods
		private void RenderView()
		{
			var container = Helpers.GetLatestPhotoSearchContainer();
			if (container == null)
				return;

			_term.Text = MPN.Framework.Content.Text.CapitaliseEachWord(container.Term);
			_iframe.Attributes["src"] = string.Format("{0}_controls/SearchResultsBarContent.aspx?i={1}", Page.ResolveUrl("~/"), CurrentPhotoId);
			_back.NavigateUrl = container.BuildSearchUrl();
		}
		#endregion
	}
}