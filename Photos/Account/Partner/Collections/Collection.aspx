<%@ Page Language="C#" MasterPageFile="~/_masterPages/Regular.master" AutoEventWireup="true" CodeFile="Collection.aspx.cs" Inherits="Account.Partner.Collections.CollectionPage" %>
<%@ Register src="~/_controls/IncomingFolderView.ascx" tagname="IncomingFolderView" tagprefix="uc1" %>
<%@ Import Namespace="MPN.Framework.Files" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
    <script type="text/javascript">
        $(document).ready(function() {
            $("#DetailsToggle").click(function() {
                $("#DetailsBox").toggle("fast");
                return false;
            });
            $("#UploadToggle").click(function() {
                $("#UploadBox").toggle("fast");
                return false;
            });
            $("#PhotosToggle").click(function() {
                $("#PhotosBox").toggle("fast");
                return false;
            });
            $("#photoTilesSortable").sortable({ 
				placeholder: "PhotoTileGhost", 
				revert: true,
				opacity: 0.5
			});
		});
        // TILE VIEW BULK-EDIT FUNCTIONALITY
        var _selectedElements = new Array();
		function beTGL(el) {
			var elID = el.id.substring(3);
			if (beContains(elID)) {
				el.className = 'PhotoTileImageFrame';
				_selectedElements = removeItems(_selectedElements, elID);
			} else {
				el.className = 'PhotoTileImageFrameHighlight';
				_selectedElements.push(elID);
			}
		}
		function beSubmit() {
			var beSelected = document.getElementById('beSelected');
			var beOrder = document.getElementById('beOrder');
			beSelected.value = '';
			beOrder.value = '';

			for (i = 0; i < _selectedElements.length; i++)
				beSelected.value += _selectedElements[i] + ",";
			if (beSelected.value != '')
				beSelected.value = beSelected.value.substring(0, beSelected.value.length -1);

			beOrder.value = $('#photoTilesSortable').sortable('toArray');
			beOrder.value = beOrder.value.replace(/bes-/g, '');
		}
		// TABLE VIEW BULK-EDIT FUNCTIONALITY		
		// GENERIC BULK-EDIT FUNCTIONALITY
		function beContains(elID) {
			for (x in _selectedElements) {
				if (_selectedElements[x] == elID)
					return true;
			}
			return false;
		}
		function removeItems(array, item) {
			var i = 0;
			while (i < array.length) {
				if (array[i] == item)
					array.splice(i, 1);
				else
					i++;
			}
			return array;
		}
    </script>
    <style type="text/css">
        div.TF {
        	padding: 5px; 
        	background-color: black; 
        	text-align: center; 
        	border: solid 3px #333; 
        	border-top: 1px; 
        	font-size: 10px;
        }
        div.TCD {
        	border-width: 1px !important;
        	padding: 5px !important;
        }
        textarea.T {
        	width: 99% !important;
        	height: auto !important;
        }
    </style>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="PageHeading" runat="server">
	<asp:Literal id="_title" runat="server">New Collection</asp:Literal>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PageContentArea" Runat="Server">

    <div style="margin-bottom: 10px;" class="Faint">
        More:
        <img src="/_images/arrow-right.png" style="margin-bottom: 2px;" alt="go" />
        <a href="../collections/" class="FaintU">Collections</a> - 
        <a href="../rates/" class="FaintU">Rates</a> 
        <!--- <a href="../reports/" class="FaintU">Reports</a>-->
    </div>
    <div class="CollapsableHeader" style="margin-top: 10px;">
        <h3>Details</h3>&nbsp;
        <img src="/_images/silk/bullet_arrow_down.png" alt="toggle" /> 
        <a href="#" class="Control" id="DetailsToggle">show/hide</a>
    </div>
    <div class="CollapsableBody" id="DetailsBox" <%= DetailsShowStyle %>>
        <table class="MediumForm" cellspacing="0">
			<tr id="_deleteCollectionRow" runat="server" visible="false">
				<td>
					&nbsp;
				</td>
				<td style="padding-bottom: 5px;">
					<asp:LinkButton 
						runat="server" 
						OnClick="DeleteCollectionHandler" 
						OnClientClick="return confirm('Are you sure?\nThis will delete all photos as well - forever!');" 
						Text="delete whole collection" 
						CssClass="Control" />
				</td>
			</tr>
            <tr>
                <td class="Faint">
                    Name:
                </td>
                <td>
                    <asp:TextBox ID="_name" runat="server" />
                    <asp:RequiredFieldValidator runat="server" ErrorMessage="* A name is required." ControlToValidate="_name" ValidationGroup="Details" />
                </td>
            </tr>
            <tr>
                <td class="Faint" style="padding-right: 20px;" valign="top">
                    Description:
                </td>
                <td>
                    <asp:TextBox TextMode="MultiLine" ID="_description" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="Faint">
                    Status:
                </td>
                <td>
                    <asp:DropDownList ID="_status" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    &nbsp;
                </td>
                <td>
                    <asp:Button runat="server" Text="Update Details" ValidationGroup="Details" OnClick="UpdateDetailsHandler" />
                </td>
            </tr>
        </table>
    </div>
    <asp:PlaceHolder ID="_uploadView" runat="server" Visible="false">
        <div class="CollapsableHeader" style="margin-top: 10px;">
            <h3>Upload</h3>&nbsp;
            <img src="../../../_images/silk/bullet_arrow_down.png" alt="toggle" /> 
            <a href="#" class="Control" id="UploadToggle">show/hide</a>
        </div>
        <div class="CollapsableBody" id="UploadBox" <%= UploadBoxShowStyle %>>
            <table cellpadding="0" cellspacing="0" width="100%" style="margin-top: 10px;">
                <tr>
                    <td style="padding-right: 20px; border-right: solid 1px #333;" class="LineSpaced" valign="top">
                        <h4>Via Web</h4>
                        Use the control below to upload files directly through the browser. Less reliable than FTP but more convenient.
                        <div class="Framed" style="margin-top: 10px; height: 75px;">
                            [ web uploader to come ]
                        </div>
                    </td>
                    <td style="padding-left: 20px; width: 300px;" valign="top" class="LineSpaced">
                        <h4>Via FTP</h4>
                        For large uploads, use the FTP service. Upload your photos into your FTP account and hit the scan button below when done so we can import them.
                        <div style="margin-top: 30px; text-align: center;">
                            <div class="Framed" style="display: inline;"><asp:Button id="_scanFtpBtn" Text="Scan FTP" runat="server" OnClick="ScanFtpHandler" CssClass="BigSize" /></div>
                        </div>
                    </td>
                </tr>
            </table>
             <div class="Highlight" id="_noDefaultRateCard" runat="server" visible="false" style="margin-top: 10px;">
				<img src="../../../_images/silk/error.png" alt="error" />
				You haven't got an active and default Rate Card. You need one so it can be assigned to any photos by
				default when you import some into a collection. Please visit the <a href="../rates/">rate-card area</a> first.
			</div>
        </div>
    </asp:PlaceHolder>
    <div class="FormBox" id="_folderSelectBox" runat="server" visible="false" style="margin-top: 10px; margin-bottom: 10px;">
		<asp:PlaceHolder ID="_folderChoicesView" runat="server" Visible="false">
			<h4>Choices!</h4>
			Select a folder: <br />
			<asp:DropDownList 
				ID="_folderSelectList" 
				runat="server" 
				DataTextField="Name" 
				DataValueField="Name" 
				CssClass="Big" 
				style="margin: 5px 0px 5px 0px;" 
				EnableViewState="true" 
				OnSelectedIndexChanged="SelectFolderHandler" 
				AutoPostBack="true" />
			<hr />
		</asp:PlaceHolder>
		<asp:PlaceHolder ID="_confirmFolderView" runat="server">
			<asp:Button ID="Button1" 
				OnClientClick="if(!confirm('Are you sure?')){return false;}"
				runat="server" 
				Text="okay, import these" 
				OnClick="ImportFtpPhotos" 
				CssClass="Big" />
		</asp:PlaceHolder>
	</div>
	<uc1:IncomingFolderView ID="_incomingFolderView" runat="server" Visible="false" />
	<div class="Highlight" id="_importInProgressBox" runat="server" visible="false" style="margin: 10px 0px 10px 0px;">
		<h4>Import in progress...</h4>
		Refresh this page to see if it's complete.
	</div>
    
    <asp:PlaceHolder ID="_photosView" runat="server" Visible="false">
		<div class="CollapsableHeader" style="margin-top: 10px;">
			<h3>Photos</h3>&nbsp;
			<img src="../../../_images/silk/bullet_arrow_down.png" alt="toggle" /> 
			<a href="#" class="Control" id="PhotosToggle">show/hide</a>
		</div>
		<div class="CollapsableBody" id="PhotosBox" <%= PhotosShowStyle %>>
		    <asp:PlaceHolder ID="_defaultPropertiesView" runat="server">
			    <div class="GridViewContainer" style="margin-bottom: 10px;">
				    <table class="Spacer">
					    <tr>
						    <td valign="top">
							    <h4>Default Properties</h4>
							    <table class="SmallForm" cellspacing="0">
							        <tr>
							            <td style="padding-right: 20px;">
							                Status:
							            </td>
							            <td>
							                <asp:DropDownList ID="_globalStatus" runat="server" />
							            </td>
							        </tr>
							        <tr>
							            <td style="padding-right: 20px;">
							                Photographer:
							            </td>
							            <td>
							                <asp:DropDownList ID="_globalPhotographerList" runat="server" />
							            </td>
							        </tr>
								    <tr>
									    <td>
										    Title:
									    </td>
									    <td>
										    <asp:TextBox ID="_globalTitle" runat="server" />
									    </td>
								    </tr>
								    <tr>
									    <td valign="top">
										    Tags:
									    </td>
									    <td>
										    <asp:TextBox ID="_globalTags" runat="server" TextMode="MultiLine" />
									    </td>
								    </tr>
							    </table>
						    </td>
						    <td valign="top" style="padding: 27px 20px 0px 10px; padding-right: 20px; border-right: solid 1px #333;">
							    If you want to add the same title and tags to all the photos in the collection,
							    then use this form do so. For tags, it will just result in the tags being added,
							    not over-writing any existing ones.
							    <div style="margin-top: 10px;">
							        <input type="hidden" id="beSelected" name="beSelected" />
							        <input type="hidden" id="beOrder" name="beOrder" />
							        <div style="font-weight: bold; margin-bottom: 5px;" class="Small">
							            Save Changes to:
							        </div>
							        <asp:Button runat="server" Text="Selected" OnClick="AssignSpecificPropertiesHandler" OnClientClick="beSubmit();" /> or
							        <asp:Button runat="server" Text="All Photos" OnClick="AssignGlobalPropertiesHandler" OnClientClick="if (confirm('Are you sure you want this to apply to all photos?')) {beSubmit();} else {return false;}" />
							    </div>
						    </td>
						    <td valign="top" style="padding-left: 20px;">
							    <h4>Photo Meta-Data</h4>
							    The best way to get titles and tags onto photos is to enter them when exporting your
							    photos. We can then scan the photos and use this embedded data. Most work-flow apps
							    like <a href="http://www.adobe.com/products/photoshoplightroom/" target="_blank">Adobe Lightroom</a> have this feature built-in.
							    <asp:Button 
							        runat="server" 
							        Text="Import From Photos" 
							        OnClientClick="return confirm('Are you sure?\nThis will lose all current names and tags!');" 
							        OnClick="ImportMetaDataFromPhotosHandler" 
							        style="display: block; margin-top: 10px;" />
						    </td>
					    </tr>
				    </table>
			    </div>
		    </asp:PlaceHolder>
			<asp:PlaceHolder ID="_editPhotoView" runat="server" Visible="false">
			    <div class="Highlight" style="margin-bottom: 10px;">
			        <h4>Edit Photo: <asp:Literal ID="_editPhotoTitle" runat="server" /></h4>
			        <table cellspacing="0" class="SmallForm">
						<tr>
							<td>
								&nbsp;
							</td>
							<td style="padding-bottom: 5px;">
								<asp:LinkButton 
									CssClass="Control" 
									runat="server" 
									OnClick="DeletePhotoHandler" 
									Text="delete photo" 
									OnClientClick="return confirm('Are you sure?\nThis will permenantly delete the photo, forever!');" />
							</td>
							<td rowspan="9" style="padding-left: 20px;">
			                    <div class="SimpleFrame"><asp:Image ID="_editPhotoPreview" runat="server" AlternateText="Preview" /></div>
			                </td>
						</tr>
			            <tr>
			                <td class="Faint">
			                    Status:
			                </td>
			                <td>
			                    <asp:DropDownList ID="_editPhotoStatus" runat="server" />
			                </td>
			            </tr>
			            <tr>
			                <td class="Faint" valign="top" style="padding-right: 20px;">
			                    Rate Card:
			                </td>
			                <td>
			                    <asp:DropDownList ID="_editPhotoRateCard" runat="server" DataTextField="Name" DataValueField="ID" />
			                </td>
			            </tr>
			            <tr>
			                <td class="Faint" valign="top">
			                    Position:
			                </td>
			                <td colspan="3">
			                    <asp:TextBox ID="_editPhotoOrder" runat="server" class="Tiny" />
                                <asp:RequiredFieldValidator 
                                    runat="server" 
                                    ErrorMessage="<br />* Position required." 
                                    ControlToValidate="_editPhotoOrder" 
                                    ValidationGroup="EditPhoto" 
                                    Display="Dynamic" />
                                <asp:RangeValidator 
                                    Display="Dynamic" 
                                    runat="server" 
                                    ErrorMessage="<br />* Choose from 1 upwards." 
                                    ControlToValidate="_editPhotoOrder" 
                                    ValidationGroup="EditPhoto" 
                                    Type="Integer" 
                                    MinimumValue="1" 
                                    MaximumValue="99999" />
			                </td>
			            </tr>
			            <tr>
			                <td class="Faint" valign="top">
			                    Name:
			                </td>
			                <td colspan="3">
			                    <asp:TextBox ID="_editPhotoName" runat="server" />
                                <asp:RequiredFieldValidator 
                                    runat="server" 
                                    Display="Dynamic"
                                    ControlToValidate="_editPhotoName" 
                                    ErrorMessage="<br />* A name is required." 
                                    ValidationGroup="EditPhoto" />
			                </td>
			            </tr>
			            <tr>
							<td class="Faint" valign="top" style="padding-right: 20px;">
								Photographer:
							</td>
							<td>
								<asp:DropDownList ID="_editPhotoPhotographerList" runat="server" />
							</td>
			            </tr>
			            <tr>
			                <td class="Faint" valign="top">
			                    Captured:
			                </td>
			                <td>
			                    <asp:TextBox ID="_editPhotoCaptured" runat="server" />
			                </td>
			            </tr>
			            <tr>
			                <td class="Faint" valign="top">
			                    Comment:
			                </td>
			                <td>
			                    <asp:TextBox TextMode="MultiLine" ID="_editPhotoComment" runat="server" />
			                </td>
			            </tr>
                        <tr>
			                <td class="Faint" valign="top">
			                    Tags:
			                </td>
			                <td>
			                    <asp:TextBox TextMode="MultiLine" ID="_editPhotoTags" runat="server" />
			                </td>
			            </tr>
			            <tr>
			                <td>
			                    &nbsp;
			                </td>
			                <td>
			                    <asp:Button runat="server" Text="Cancel" OnClick="CancelEditPhotoHandler" style="margin-right: 5px;" />
			                    <asp:Button runat="server" Text="Update Photo" OnClick="UpdatePhotoHandler" ValidationGroup="EditPhoto" />
			                </td>
			            </tr>
			        </table>
                </div>
			</asp:PlaceHolder>
			<asp:PlaceHolder id="_photoTilesView" runat="server">
			    <table class="Spacer" cellspacing="0">
			        <tr>
			            <td class="Faint">
			                <b><asp:Literal ID="_photoCount" runat="server" /></b> Photos |
			                <img src="../../../_images/silk/bullet_red.png" alt="Active" /> Active | 
			                <img src="../../../_images/silk/bullet_white.png" alt="New" /> New | 
			                <img src="../../../_images/silk/bullet_black.png" alt="Disabled" /> Disabled
			            </td>
			            <td align="right" class="Faint" style="font-weight: normal;">
			                <img src="../../../_images/silk/images.png" alt="tile view" /> <a href="?id=<%= Request.QueryString["id"] %>&vt=tile">tile view</a> |
			                <img src="../../../_images/silk/text_align_left.png" alt="list view" /> <a href="?id=<%= Request.QueryString["id"] %>&vt=grid">grid view</a>
			            </td>
			        </tr>
			    </table>
			    <hr />
			    <asp:Repeater ID="_photoTiles" runat="server" OnItemCreated="PhotoTileCreatedHandler">
			        <HeaderTemplate>
                        <div class="Clear" />
                        <div>
						    <ul id="photoTilesSortable" style="width: 99%; list-style-position: inside; list-style-type: none; cursor: hand; cursor: pointer; margin: 0px !important; padding: 0px !important;"> 
			        </HeaderTemplate>
				    <ItemTemplate>
						        <li id="bes-<%# Eval("Photo.Id") %>" style="float: left;">
							        <div class="PhotoTile">
								        <div id="be-<%# Eval("Photo.Id") %>" class="PhotoTileImageFrame" onclick="beTGL(this);">
									        <asp:Literal ID="_tilePhoto" runat="server" />
								        </div>
								        <div class="TF Faint">
									        <asp:Literal ID="_statusIcon" runat="server" /> 
									        <a href="preview.aspx?i=<%# Eval("Photo.ID") %>" target="_blank" class="Sub">zoom</a> -
									        <a href="?id=<%# Request.QueryString["id"] %>&e=<%# Eval("Photo.Id") %>&vt=tile" class="Sub">edit</a>
								        </div>
							        </div>
						        </li>
					</ItemTemplate>
				    <FooterTemplate>
						    </ul>
						</div>
                        <div class="Clear" />
				    </FooterTemplate>
			    </asp:Repeater>
			</asp:PlaceHolder>
			
	        <asp:Repeater ID="_photoTable" runat="server" OnItemCreated="PhotoTableItemCreatedHandler">
				<HeaderTemplate>
					<table class="GridView" cellspacing="0">
						<tr class="GridViewHeader">
							<th>&nbsp;</th>
							<th>Position</th>
							<th>Name</th>
							<th>Filesize</th>
							<th>Dimensions</th>
							<th>Photographer</th>
							<th>Rate Card</th>
							<th>Status</th>
							<th>Imported</th>
							<th>&nbsp;</th>
						</tr>
				</HeaderTemplate>
				<ItemTemplate>
						<tr id="_row1" runat="server">
							<td rowspan="2" style="width: 74px;">
								<a href="preview.aspx?i=<%# Eval("Photo.Id") %>" target="_blank"><img src="../../../i.ashx?i=<%# Eval("Photo.Id") %>&d=74" alt="preview" border="0" /></a>
							</td>
							<td class="Faint">
								#<%# Eval("Order") %>
							</td>
							<td>
								<%# Eval("Photo.Name") %>
							</td>
							<td>
								<%# Files.GetFriendlyFilesize((long)Eval("Photo.Filesize")) %>
							</td>
							<td>
								<%# Eval("Photo.Size.Width") + " x " + Eval("Photo.Size.Height") %>
							</td>
							<td>
								<%# (Eval("Photo.Photographer") != null) ? (Eval("Photo.Photographer") as MotoProfessional.Models.Member).GetFullName() : "-" %>
							</td>
							<td>
								<%# Eval("Photo.RateCard.Name") %>
							</td>
							<td>
								<%# Eval("Photo.Status") %>
							</td>
							<td>
								<%# ((DateTime)Eval("Photo.Created")).ToString(System.Configuration.ConfigurationManager.AppSettings["ShortDateTimeFormatString"]) %>
							</td>
							<td style="padding-top: 7px;">
								<img src="../../../_images/silk/text_signature.png" alt="edit" /> 
								<a href="collection.aspx?id=<%# Request.QueryString["ID"] %>&e=<%# Eval("Photo.Id") %>&vt=grid">edit</a>
							</td>
						</tr>
						<tr id="_row2" runat="server">
							<td colspan="9">
								<div class="Framed TCD">
									<textarea name="tblt_<%# Eval("Photo.Id") %>" type="text" class="Borderless T"><%# (Eval("Photo.Tags") as MotoProfessional.Models.TagCollection).ToCsv() %></textarea>
								</div>
							</td>
						</tr>
				</ItemTemplate>
				<FooterTemplate>
					</table>
					<hr />
                    <asp:Button Text="Update Tags" runat="server" style="margin-left: 100px;" OnClick="UpdateTableTagsHandler" />
				</FooterTemplate>
	        </asp:Repeater>
	        
		</div>
	</asp:PlaceHolder>
    
</asp:Content>