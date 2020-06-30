using System;
using System.Linq;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using _masterPages;
using App_Code;
using MotoProfessional;
using MotoProfessional.Exceptions;
using MotoProfessional.Models.Interfaces;
using MPN.Framework;

namespace Account.Partner.Collections
{
	public partial class CollectionPage : Page
	{
		#region members
		private int _photoTableRowCounter;
		private ICollection _collection;
		/// <summary>
		/// Default view type.
		/// </summary>
		private PhotoViewType _viewType = PhotoViewType.Tile;
		protected string UploadBoxShowStyle;
		protected string DetailsShowStyle;
		protected string PhotosShowStyle;
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			Helpers.SecurePartnerPage();
			((MasterPagesRegular) Page.Master).SelectedTab = "account";
			if (Page.Master != null)
			{
				(Page.Master as MasterPagesRegular).FluidLayout = true;
				(Page.Master as MasterPagesRegular).ShowJQueryUi = true;
			}

			if (!string.IsNullOrEmpty(Request.QueryString["id"]))
			{
				_collection = Controller.Instance.PhotoController.GetCollection(Convert.ToInt32(Request.QueryString["id"]));
				if (_collection == null)
				{
					Logger.LogWarning("collection.aspx - no such id found: " + Request.QueryString["id"]);
					Helpers.AddUserResponse("<b>Whoops!</b> - No such collection found.");
					Response.Redirect("./");
				}
			}

			try
			{
				if (!string.IsNullOrEmpty(Request.QueryString["vt"]))
					_viewType = (PhotoViewType)Enum.Parse(typeof(PhotoViewType), MPN.Framework.Content.Text.CapitaliseEachWord(Request.QueryString["vt"]));
			}
			catch
			{
			}

			if (!Page.IsPostBack || (!Page.IsPostBack && string.IsNullOrEmpty(Request.QueryString["e"])))
				RenderForm();

			if (!Page.IsPostBack && !string.IsNullOrEmpty(Request.QueryString["e"]))
				RenderEditPhotoView(Convert.ToInt32(Request.QueryString["e"]));
		}

		#region event handlers
		protected void UpdateDetailsHandler(object sender, EventArgs ea)
		{
			if (!Page.IsValid)
				return;

			var isNew = false;
			if (_collection == null)
			{
				_collection = Controller.Instance.PhotoController.NewCollection();
				isNew = true;
			}

			_collection.Name = _name.Text.Trim();
			_collection.Description = _description.Text.Trim();
			_collection.Status = (GeneralStatus)Enum.Parse(typeof(GeneralStatus), _status.SelectedValue);
			_collection.Partner = Helpers.GetCurrentUser().Company.Partner;

			// we need to do a deep-update so all the tag stats for the app get reset.
			// REFACTOR: only update if the status change affects live, live to inactive or something to live.
			Controller.Instance.PhotoController.UpdateCollection(_collection, true);

			Helpers.AddUserResponse(isNew
			                        	? "<b>Created!</b> - Your new collection has been created."
			                        	: "<b>Updated!</b> - The collection has been updated.");

			RefreshPage(true);
		}

		protected void ScanFtpHandler(object sender, EventArgs ea)
		{
			RenderFtpScan();
		}

		protected void SelectFolderHandler(object sender, EventArgs ea)
		{
			if (_folderSelectList.SelectedIndex > 0)
			{
				var path = Helpers.GetPartnerIncomingFolder(Helpers.GetCurrentUser().Company.Partner);
				if (_folderSelectList.SelectedIndex > 1)
					path += _folderSelectList.SelectedValue;

				SelectFolder(path);
				_confirmFolderView.Visible = true;
			}
			else
			{
				_incomingFolderView.Visible = false;
				_confirmFolderView.Visible = false;
			}

			_folderSelectBox.Visible = true;
			DetailsShowStyle = "style=\"display: none;\"";
		}

		protected void ImportFtpPhotos(object sender, EventArgs ea)
		{
			var partner = Helpers.GetCurrentUser().Company.Partner;
			var path = Helpers.GetPartnerIncomingFolder(partner);

			// specific sub-folder?
			if (_folderSelectList.SelectedIndex > 1)
				path += _folderSelectList.SelectedValue;

			try
			{
				_collection.ImportPhotos(path);
				Helpers.AddUserResponse("<b>Underway!</b> - We're currently importing your photos. Refresh this page once in a while to see the progress. Importing may take some time due to the amount of processing done.");
			}
			catch (PhotoImportException pie)
			{
				Helpers.AddUserResponse("<b>Whoops!</b> - " + pie.ClientMessage);
			}
        
			RefreshPage(true);
		}

		protected void ImportMetaDataFromPhotosHandler(object sender, EventArgs ea)
		{
			foreach (var cp in _collection.Photos)
			{
				cp.Photo.RetrieveMetaData(false);
				cp.Photo.PopulateFromMetaData();
				Controller.Instance.PhotoController.UpdatePhoto(cp.Photo);
			}

			_collection.RebuildTagStats();
			Helpers.AddUserResponse("<b>Updated!</b> - Each photo has been updated from image meta-data.");
			RefreshPage(true);
		}

		protected void AssignGlobalPropertiesHandler(object sender, EventArgs ea)
		{
			foreach (var cp in _collection.Photos)
			{
				if (_globalStatus.SelectedIndex > 0)
					cp.Photo.Status = (GeneralStatus)Enum.Parse(typeof(GeneralStatus), _globalStatus.SelectedValue);

				if (!string.IsNullOrEmpty(_globalTitle.Text))
					cp.Photo.Name = _globalTitle.Text.Trim();

				if (!string.IsNullOrEmpty(_globalPhotographerList.SelectedValue))
					cp.Photo.Photographer = Controller.Instance.MemberController.GetMember(new Guid(_globalPhotographerList.SelectedValue));

				if (!string.IsNullOrEmpty(_globalTags.Text))
				{
					var newTags = Common.CsvToArray(_globalTags.Text.Trim().ToLower(), ",");
					foreach (var tag in newTags)
						cp.Photo.Tags.Add(tag);
				}
            
				Controller.Instance.PhotoController.UpdatePhoto(cp.Photo);
			}

			if (!string.IsNullOrEmpty(Request.Form["beOrder"]))
			{
				var photoOrderIDs = Request.Form["beOrder"].Split(char.Parse(",")).Select(strPhotoId => Convert.ToInt32(strPhotoId)).ToList();

				if (_collection.OrderPhotos(photoOrderIDs))
					Controller.Instance.PhotoController.UpdateCollection(_collection, true);
			}
        
			Helpers.AddUserResponse("<b>Updated!</b> - Properties and orders set.");
			RefreshPage(true);
		}

		protected void AssignSpecificPropertiesHandler(object sender, EventArgs ea)
		{
			if (!string.IsNullOrEmpty(Request.Form["beSelected"]))
			{
				var selectedPhotosCsv = Request.Form["beSelected"].Split(char.Parse(","));
				foreach (var p in selectedPhotosCsv.Select(photoId => _collection.Photos.Single(qcp => qcp.Photo.Id == Convert.ToInt32(photoId)).Photo))
				{
					if (_globalStatus.SelectedIndex > 0)
						p.Status = (GeneralStatus)Enum.Parse(typeof(GeneralStatus), _globalStatus.SelectedValue);

					if (!string.IsNullOrEmpty(_globalTitle.Text))
						p.Name = _globalTitle.Text.Trim();

					if (!string.IsNullOrEmpty(_globalPhotographerList.SelectedValue))
						p.Photographer = Controller.Instance.MemberController.GetMember(new Guid(_globalPhotographerList.SelectedValue));

					if (!string.IsNullOrEmpty(_globalTags.Text))
					{
						var newTags = Common.CsvToArray(_globalTags.Text.Trim().ToLower(), ",");
						foreach (var tag in newTags)
							p.Tags.Add(tag);
					}

					Controller.Instance.PhotoController.UpdatePhoto(p);
				}
			}

			if (!string.IsNullOrEmpty(Request.Form["beOrder"]))
			{
				var photoOrderIDs = Request.Form["beOrder"].Split(char.Parse(",")).Select(strPhotoId => Convert.ToInt32(strPhotoId)).ToList();
				if (_collection.OrderPhotos(photoOrderIDs))
					Controller.Instance.PhotoController.UpdateCollection(_collection, true);
			}

			_collection.RebuildTagStats();
			Helpers.AddUserResponse("<b>Updated!</b> - Properties and orders set.");
			RefreshPage(true);
		}

		protected void PhotoTileCreatedHandler(object sender, RepeaterItemEventArgs ea)
		{
			if (ea.Item.ItemType != ListItemType.Item && ea.Item.ItemType != ListItemType.AlternatingItem) return;
			var cp = ea.Item.DataItem as ICollectionPhoto;
			if (cp == null)
				return;

			// not using an Image control here to reduce the html file-size for a potentially very large page.
			var image = ea.Item.FindControl("_tilePhoto") as Literal;
			var icon = ea.Item.FindControl("_statusIcon") as Literal;

			switch (cp.Photo.Status)
			{
				case GeneralStatus.Active:
					if (icon != null) icon.Text = "<img src=\"../../../_images/silk/bullet_red.png\" alt=\"Active\" />";
					break;
				case GeneralStatus.New:
					if (icon != null) icon.Text = "<img src=\"../../../_images/silk/bullet_white.png\" alt=\"New\" />";
					break;
				case GeneralStatus.Disabled:
					if (icon != null) icon.Text = "<img src=\"../../../_images/silk/bullet_black.png\" alt=\"Disabled\" />";
					break;
			}

			if (!_collection.ImportInProgress)
			{
				if (image != null)
					image.Text = string.Format("<img src=\"{0}i.ashx?i={1}&d=100\" border=\"0\" />", Page.ResolveUrl("~/"), cp.Photo.Id);
			}
			else
			{
				if (image != null)
					image.Text = "<div class=\"LinedBox\" style=\"padding: 5px; display: table;\"><div style=\"width: 100px; height: 100px; text-align: center;\" class=\"Faint Small\"><div style=\"padding-top: 42px;\">importing</div></div></div>";
			}
		}

		protected void CancelEditPhotoHandler(object sender, EventArgs ea)
		{
			RefreshPage(true);
		}

		protected void UpdatePhotoHandler(object sender, EventArgs ea)
		{
			if (!Page.IsValid)
				return;

			var cp = _collection.Photos.SingleOrDefault(qcp => qcp.Photo.Id == Convert.ToInt32(Request.QueryString["e"]));
			if (cp == null)
			{
				Helpers.AddUserResponse("<b>Whoops!</b> - No such photo found.");
				Logger.LogWarning("Partner/Collection.aspx - UpdatePhotoHandler(), no such photo found.");
				RefreshPage(true);
			}

			if (cp != null)
			{
				cp.Photo.Status = (GeneralStatus)Enum.Parse(typeof(GeneralStatus), _editPhotoStatus.SelectedValue);
				cp.Photo.RateCard = _collection.Partner.RateCards.Single(rc => rc.Id == Convert.ToInt32(_editPhotoRateCard.SelectedValue));
				cp.Photo.Name = _editPhotoName.Text.Trim();

				if (!string.IsNullOrEmpty(_editPhotoCaptured.Text) && Common.IsDate(_editPhotoCaptured.Text.Trim()))
					cp.Photo.Captured = DateTime.Parse(_editPhotoCaptured.Text);
				else
					cp.Photo.Captured = DateTime.MinValue;

				cp.Photo.Photographer = !string.IsNullOrEmpty(_editPhotoPhotographerList.SelectedValue) ? Controller.Instance.MemberController.GetMember(new Guid(_editPhotoPhotographerList.SelectedValue)) : null;
				cp.Photo.Comment = _editPhotoComment.Text.Trim();

			    var tagCsv = _editPhotoTags.Text.Trim().ToLower().Split(char.Parse(",")).ToList();
                cp.Photo.Tags.UpdateFrom(tagCsv);

				// persist photo changes.
				Controller.Instance.PhotoController.UpdatePhoto(cp.Photo);

				// photo order change?
				if (!string.IsNullOrEmpty(_editPhotoOrder.Text) && Convert.ToInt32(_editPhotoOrder.Text) != cp.Order)
				{
					_collection.ChangePhotoOrder(cp.Photo, Convert.ToInt32(_editPhotoOrder.Text));
					Controller.Instance.PhotoController.UpdateCollection(_collection, true);
				}
			}

			_collection.RebuildTagStats();
			Helpers.AddUserResponse("<b>Updated!</b> - The photo has been updated.");
			RefreshPage(true);
		}

		protected void DeleteCollectionHandler(object sender, EventArgs ea)
		{
			var wasDeleted = Controller.Instance.PhotoController.DeleteCollection(_collection);
			if (wasDeleted)
			{
				Helpers.AddUserResponse("<b>Done!</b> - The collection was deleted.");
				Response.Redirect("./");
			}
			else
			{
				Helpers.AddUserResponse("<b>Can't do it!</b> - The collection cannot be deleted as it's already in use. Photos are in peoples baskets and/or orders.");
				RefreshPage(true);
			}
		}

		protected void DeletePhotoHandler(object sender, EventArgs ea)
		{
			var cp = _collection.Photos.SingleOrDefault(qcp => qcp.Photo.Id == Convert.ToInt32(Request.QueryString["e"]));
			if (cp == null)
			{
				Helpers.AddUserResponse("<b>Whoops!</b> - No such photo found.");
				Logger.LogWarning("Partner/Collection.aspx - DeletePhotoHandler(), no such photo found.");
				RefreshPage(true);
			}

			if (cp != null)
			{
				var wasDeleted = Controller.Instance.PhotoController.DeletePhoto(cp.Photo);
				if (wasDeleted)
				{
					_collection.RebuildTagStats();
					Helpers.AddUserResponse("<b>Done!</b> - The photo was deleted.");
				}
				else
				{
					Helpers.AddUserResponse("<b>Can't do it!</b> - The photo cannot be deleted as it's already in use in peoples baskets and/or orders.");
				}
			}

			RefreshPage(true);
		}

		protected void PhotoTableItemCreatedHandler(object sender, RepeaterItemEventArgs ea)
		{
			if (ea.Item.ItemType != ListItemType.Item && ea.Item.ItemType != ListItemType.AlternatingItem) return;
			if (ea.Item.DataItem == null)
				return;

			var row1 = ea.Item.FindControl("_row1") as HtmlTableRow;
			var row2 = ea.Item.FindControl("_row2") as HtmlTableRow;

			if (_photoTableRowCounter == 0)
			{
				if (row1 != null) row1.Attributes["class"] = "GridViewRow";
				if (row2 != null) row2.Attributes["class"] = "GridViewRow";
				_photoTableRowCounter++;
			}
			else
			{
				if (row1 != null) row1.Attributes["class"] = "GridViewAlternatingRow";
				if (row2 != null) row2.Attributes["class"] = "GridViewAlternatingRow";
				_photoTableRowCounter = 0;
			}
		}

		protected void UpdateTableTagsHandler(object sender, EventArgs ea)
		{
			foreach (var cp in _collection.Photos)
			{
				var originalTags = cp.Photo.Tags.ToCsv();
				var rawTags = Request.Form[string.Format("tblt_{0}", cp.Photo.Id)];
				cp.Photo.Tags.UpdateFrom(rawTags);

				// only update if there's a change.
				if (cp.Photo.Tags.ToCsv() != originalTags)
					Controller.Instance.PhotoController.UpdatePhoto(cp.Photo);
			}

			_collection.RebuildTagStats();
			Helpers.AddUserResponse("<b>Updated!</b> - The image tags have been updated.");
			RefreshPage(true);
		}
		#endregion

		#region private methods
		private void RenderForm()
		{
			_status.DataSource = Enum.GetNames(typeof(GeneralStatus));
			_status.DataBind();

			// default view setup.
			// -- if new, don't show upload.
			// -- if persisted by no photos, show upload view only
			// -- if persisted and has photos, don't show details or upload views.

			if (_collection == null)
			{
				_status.Enabled = false;
				_status.SelectedValue = GeneralStatus.New.ToString();
				_uploadView.Visible = false;
				_deleteCollectionRow.Visible = false;
			}
			else
			{
				_deleteCollectionRow.Visible = true;
				_uploadView.Visible = true;
				DetailsShowStyle = "style=\"display: none;\"";
				_photoCount.Text = _collection.Photos.Count.ToString();

				if (_collection.Photos.Count > 0)
				{
					UploadBoxShowStyle = "style=\"display: none;\"";
					_photosView.Visible = true;
				}
				else
				{
					PhotosShowStyle = "style=\"display: none;\"";
					_photosView.Visible = false;
				}

				_importInProgressBox.Visible = _collection.ImportInProgress;
				PopulateForm();
			}

			// got a default rate-card?
			if (Controller.Instance.BusinessRuleController.CanPhotosBeImported(Helpers.GetCurrentUser().Company.Partner))
				return;

			_noDefaultRateCard.Visible = true;
			_scanFtpBtn.Enabled = false;
		}

		private void PopulateForm()
		{
			_title.Text = string.Format("Collection: <span class=\"Faint\">{0}</span>", _collection.Name);
			_status.Enabled = true;
			_name.Text = _collection.Name;
			_description.Text = _collection.Description;
			_status.SelectedValue = _collection.Status.ToString();

			if (_viewType == PhotoViewType.Tile)
			{
				_photoTiles.DataSource = _collection.Photos;
				_photoTiles.DataBind();
				_photoTable.Visible = false;
			}
			else
			{
				_photoTable.DataSource = _collection.Photos;
				_photoTable.DataBind();
				_photoTiles.Visible = false;
			}

			_globalStatus.DataSource = Enum.GetNames(typeof(GeneralStatus));
			_globalStatus.DataBind();
			_globalStatus.Items.Insert(0, new ListItem("-- Not Set", string.Empty));

			_globalPhotographerList.Items.Clear();
			_globalPhotographerList.Items.Insert(0, new ListItem("-- Not Set", string.Empty));
			foreach (var ce in Helpers.GetCurrentUser().Company.Employees)
				_globalPhotographerList.Items.Add(new ListItem(ce.Member.GetFullName(), ce.Member.MembershipUser.ProviderUserKey.ToString()));
		}

		/// <summary>
		/// Once either the root incoming folder or one ot it's sub-folders has been selected, this
		/// will load the contents into the summary control.
		/// </summary>
		private void SelectFolder(string path)
		{
			_incomingFolderView.LoadPath(path);
			_incomingFolderView.Visible = true;
		}

		private void RenderFtpScan()
		{
			// folder has sub-folders?
			// -- yes = show folder select list.
			// -- no  = show file summary

			var path = Helpers.GetPartnerIncomingFolder(Helpers.GetCurrentUser().Company.Partner);
			if (!Directory.Exists(path))
			{
				Helpers.AddUserResponse("<b>Whoops!</b> - FTP site not found!");
				Logger.LogError("Collection.aspx - FTP site not found: " + path);
				RefreshPage(true);
			}

			var di = new DirectoryInfo(path);
			var subs = di.GetDirectories();
			if (subs.Length > 0)
			{
				// more than one folder so need to select one, including the root.
				_folderSelectBox.Visible = true;
				_folderChoicesView.Visible = true;
				_folderSelectList.DataSource = subs;
				_folderSelectList.DataBind();
				_folderSelectList.Items.Insert(0, new ListItem("Choose..."));
				_folderSelectList.Items.Insert(1, new ListItem("Incoming"));
			}
			else
			{
				// no sub-folders, so select this one.
				_folderSelectBox.Visible = true;
				SelectFolder(path);
			}

			RenderForm();
		}

		private void RenderEditPhotoView(int photoId)
		{
			var cp = _collection.Photos.SingleOrDefault(qcp => qcp.Photo.Id == photoId);
			if (cp == null)
				return;

			_editPhotoView.Visible = true;
			_defaultPropertiesView.Visible = false;
			_editPhotoOrder.Text = cp.Order.ToString();
			_editPhotoTitle.Text = cp.Photo.Name;
			_editPhotoPreview.ImageUrl = string.Format("~/i.ashx?i={0}&d=350&rh=1", cp.Photo.Id);

			_editPhotoStatus.DataSource = Enum.GetNames(typeof(GeneralStatus));
			_editPhotoStatus.DataBind();
			_editPhotoStatus.SelectedValue = cp.Photo.Status.ToString();
        
			_editPhotoRateCard.DataSource = _collection.Partner.LiveRateCards;
			_editPhotoRateCard.DataBind();
			_editPhotoRateCard.SelectedValue = cp.Photo.RateCard.Id.ToString();
        
			_editPhotoName.Text = cp.Photo.Name;
			_editPhotoComment.Text = cp.Photo.Comment;
			_editPhotoTags.Text = cp.Photo.Tags.ToCsv();

			if (cp.Photo.Captured != DateTime.MinValue)
				_editPhotoCaptured.Text = cp.Photo.Captured.ToString();

			_editPhotoPhotographerList.Items.Clear();
			_editPhotoPhotographerList.Items.Insert(0, new ListItem("-- Not Set", string.Empty));
			foreach (var ce in Helpers.GetCurrentUser().Company.Employees)
				_editPhotoPhotographerList.Items.Add(new ListItem(ce.Member.GetFullName(), ce.Member.MembershipUser.ProviderUserKey.ToString()));

			if (cp.Photo.Photographer != null)
				_editPhotoPhotographerList.SelectedValue = cp.Photo.Photographer.MembershipUser.ProviderUserKey.ToString();
		}

		private void RefreshPage(bool initialState)
		{
			string url;
			if (initialState)
			{
				url = "collection.aspx?id=" + _collection.Id;
				if (!string.IsNullOrEmpty(Request.QueryString["vt"]))
					url += "&vt=" + Request.QueryString["vt"];
			}
			else
			{
				url = Request.Url.AbsoluteUri;
			}

			Response.Redirect(url);
		}
		#endregion
	}
}