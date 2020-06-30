using System;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using MotoProfessional.Models;
using App_Code;
using MotoProfessional.Models.Interfaces;

namespace _controls
{
	public partial class BasketViewContent : System.Web.UI.Page
	{
		#region members
		private IBasket _basket;
		private int _currentPhotoId;
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			_basket = Helpers.GetCurrentUser().Basket;
			if (_basket == null)
				return;
		
			if (!Page.IsPostBack)
				RenderView();
		}

		#region event handlers
		protected void ItemsItemCreatedHandler(object sender, RepeaterItemEventArgs ea)
		{
			if (ea.Item.ItemType != ListItemType.Item && ea.Item.ItemType != ListItemType.AlternatingItem) return;
			var bi = ea.Item.DataItem as IBasketItem;
			if (bi == null)
				return;

			// photo products (the only kind for now).
			if (bi.PhotoProduct == null) return;
			var photo = ea.Item.FindControl("_tilePhoto") as Literal;
			var frame = ea.Item.FindControl("_frame") as HtmlGenericControl;
			var rate = ea.Item.FindControl("_rate") as Literal;
			var license = ea.Item.FindControl("_license") as Label;

			if (license != null) license.Text = bi.PhotoProduct.License.Name;
			if (rate != null) rate.Text = bi.PhotoProduct.Rate.ToString("###,###.##");
			if (frame != null)
				frame.Attributes["class"] = (bi.PhotoProduct.Photo.Id == _currentPhotoId) ? "PhotoTileImageFrameHighlight" : "PhotoTileImageFrame";
			if (photo != null)
				photo.Text = string.Format("<a href=\"{0}\" target=\"_top\"><img src=\"{1}i.ashx?i={2}&d=74\" border=\"0\" alt=\"view photo\" /></a>", Helpers.BuildLink(bi.PhotoProduct.Photo), Page.ResolveUrl("~/"), bi.PhotoProduct.Photo.Id);
		}

		protected void RemoveItemHandler(object sender, CommandEventArgs ea)
		{
			if (string.IsNullOrEmpty(ea.CommandArgument.ToString()))
				return;

			var m = Helpers.GetCurrentUser();
			if (m == null)
				return;

			m.Basket.RemoveBasketItem(Convert.ToInt32(ea.CommandArgument));
			_body.Attributes.Add("onload", "parent.location.replace(parent.location);");
			RenderView();
		}
		#endregion

		#region private methods
		private void RenderView()
		{
			if (!string.IsNullOrEmpty(Request.QueryString["i"]))
				_currentPhotoId = Convert.ToInt32(Request.QueryString["i"]);

			var page = 1;
			var paginator = new BasketPaginator
			                	{
			                		UrlFormat = BasketPaginator.PaginationControlsUrlFormat.QueryString,
			                		PageSize = 4,
			                		DataSource = _basket.Items
			                	};

			if (!string.IsNullOrEmpty(Request.QueryString["p"]))
			{
				page = Convert.ToInt32(Request.QueryString["p"]);
				if (page < 1)
					page = 1;
			}

			// now, the clever thing is to pull out a ticker view of the results, i.e. center the current photo and have
			// some before and after it that the user can click on.
        
			_items.DataSource = paginator.GetPage(page);
			_items.DataBind();
			_itemCount.Text = paginator.DataSource.Count.ToString();

			if (paginator.TotalPages == 1)
				_paginationView.Visible = false;
			else
				_paginationControls.Text = paginator.BuildPaginatorControls(Request.Url.AbsoluteUri);
		}
		#endregion
	}
}