<%@ Page Language="C#" MasterPageFile="~/_masterPages/Regular.master" AutoEventWireup="true" CodeFile="Photo.aspx.cs" Inherits="PhotoPage" %>
<%@ Register src="~/_controls/ExtendedPhotoMetaData.ascx" tagname="ExtendedMetaData" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" Runat="Server">
	<script type="text/javascript">
		$(document).ready(function() {
			$("#<%= this._staticPreview.ClientID %>").bind("contextmenu", function(e){
				return false;
			});
		});
	</script>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="PageHeading" runat="server">
	<span class="Faint">Photo:</span> <asp:Literal ID="_title" runat="server" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PageContentArea" Runat="Server">
	<table class="Spacer" cellspacing="0">
		<tr>
			<td valign="top" style="padding-right: 5px;">
				<div class="LightContentBox">
					<h3>Photo Details</h3>
					<table cellpadding="0" cellspacing="0" class="Form">
					    <tr>
					        <td class="Faint">
					            Taken By:
					        </td>
					        <td>
					            <asp:HyperLink ID="_partner" runat="server" CssClass="FaintU" />
					        </td>
					    </tr>
						<tr id="_viewCountRow" runat="server" visible="false">
							<td class="Faint" style="padding-right: 20px; white-space: nowrap;">
								Views:
							</td>
							<td>
								<asp:Literal ID="_views" runat="server" />
							</td>
						</tr>
						<tr>
							<td class="Faint" style="padding-right: 20px; white-space: nowrap;">
								Date Captured:
							</td>
							<td>
								<asp:Literal ID="_dateCaptured" runat="server" Text="-" />
							</td>
						</tr>
						<tr>
							<td class="Faint" style="padding-right: 20px;">
								Orientation:
							</td>
							<td>
								<asp:Literal ID="_orientation" runat="server" />
							</td>
						</tr>
						<tr>
							<td class="Faint" style="padding-right: 20px;">
								Original Size:
							</td>
							<td>
								<asp:Literal ID="_dimensions" runat="server" />
							</td>
						</tr>
						<tr>
							<td class="Faint" style="padding-right: 20px;">
								Photographer:
							</td>
							<td>
								<asp:Literal ID="_photographer" runat="server" Text="-" />
							</td>
						</tr>
						<tr>
							<td class="Faint" valign="top">
								Comment:
							</td>
							<td class="LineSpaced">
								<asp:Literal ID="_comment" runat="server" Text="-" />
							</td>
						</tr>
					</table>
					<div style="padding-bottom: 10px; border-bottom: dotted 1px #333; margin-top: 20px; margin-bottom: 10px;">
						<img src="<%= Page.ResolveUrl("~/_images/silk/tag_green.png") %>" alt="tags" /> <h4 style="display: inline;">Tags</h4>
					</div>
					<div class="Faint LineSpaced">
						<asp:Literal ID="_tags" runat="server" Text="No tags yet." />
					</div>
					<div style="border-top: dotted 1px #333; padding-top: 10px; margin-top: 10px;">
						<script type="text/javascript">addthis_pub  = 'Amethi';</script>
						<a href="http://www.addthis.com/bookmark.php" onmouseover="return addthis_open(this, '', '[URL]', '[TITLE]')" onmouseout="addthis_close()" onclick="return addthis_sendto()"><img src="<%= Page.ResolveUrl("~/_images/thirdparty/share-btn.gif") %>" width="83" height="16" border="0" alt="" /></a><script type="text/javascript" src="http://s7.addthis.com/js/152/addthis_widget.js"></script>
					</div>
				</div>
				
				<div class="LightContentBox" style="margin-top: 10px;">
					<h3 style="margin-top: 0px;">Collections</h3>
					<div style="margin-bottom: 10px;" class="Faint">
					    This photo belongs to the following collections:
                    </div>
					<asp:Repeater ID="_collections" runat="server" OnItemCreated="CollectionsItemCreated">
					    <ItemTemplate>
					        <img src="<%# Page.ResolveUrl("~/_images/silk/pictures.png") %>" alt="Collection" />
					        <asp:HyperLink ID="_collectionTitleLink" runat="server" CssClass="H4U" />
					        <span class="Faint">- <asp:Literal ID="_collectionStats" runat="server" /></span>
					        <hr style="margin-top: 10px;" />
					        <table>
								<tr>
									<td style="width: 16px;" id="_prevCell1" runat="server">
										<img src="<%# Page.ResolveUrl("~/_images/arrow-left.png") %>" alt="previous" />
									</td>
									<td>
									    <div class="LinedBox" style="padding: 5px; display: table;" id="_startThumbGhost" runat="server" visible="false">
									        <div style="width: 100px; height: 100px; text-align: center;" class="Faint Small">
									            <div style="padding-top: 42px;">at start</div>
									        </div>
									    </div>
										<div class="SimpleFrame" style="display: table;" id="_frame1" runat="server"><asp:HyperLink ID="_previousThumb" runat="server" ToolTip="Previous" /></div>
									</td>
									<td>
										<div class="SimpleFrame" style="display: table;" id="_frame2" runat="server"><asp:HyperLink ID="_nextThumb" runat="server" ToolTip="Next" /></div>
										<div class="LinedBox" style="padding: 5px; display: table;" id="_endThumbGhost" runat="server" visible="false">
									        <div style="width: 100px; height: 100px; text-align: center;" class="Faint Small">
									            <div style="padding-top: 42px;">at end</div>
									        </div>
									    </div>
									</td>
									<td style="width: 16px;" id="_nextCell1" runat="server">
										<img src="<%# Page.ResolveUrl("~/_images/arrow-right.png") %>" alt="next" />
									</td>
								</tr>
					        </table>
					    </ItemTemplate>
					</asp:Repeater>
				</div>
				
				<asp:PlaceHolder ID="_extendedMetaDataView" runat="server">
				    <div class="LightContentBox" style="margin-top: 10px;">
					    <uc1:ExtendedMetaData id="_extendedMetaData" runat="server" />
                    </div>
                </asp:PlaceHolder>
			</td>
			<td valign="top" style="padding-left: 5px; width: 5%;">
				<div class="CollapsableHeader Small Faint" style="padding-top: 10px; padding-bottom: 10px; text-align: right;"><asp:Image ID="_staticPreview" runat="server" /></div>
				<div class="LinedBox" style="border-top: none; padding: 0px;">
				    <div style="background-image: url(<%= Page.ResolveUrl("~/_images/layout/row-header-gradient.gif") %>); background-repeat: repeat-x; padding: 10px;">
				        <table class="Spacer" cellspacing="0">
				            <tr>
				                <td>
				                    <h3 style="margin-top: 0px;">Purchase Options</h3>
				                </td>
				                <td align="right" valign="top" class="Faint Small">
				                    preview image not for use
				                </td>
				            </tr>
				        </table>
				    </div>
				    <div style="padding: 10px;">
					
						<div style="padding-bottom: 10px; border-bottom: dotted 1px #333; margin-bottom: 10px;" class="LineSpaced">
							When you buy a photo, you buy a license to use it, not the photo itself. When bought, you download a photo to accompany the license as well.
							Choose a license that matches your need and size. 
							<br />
							<br />
							The license also determines how big the photo is you download. If you're unsure, 
							<a href="<%= Page.ResolveUrl("~/") %>contact/">contact</a> us for help. Read our <a href="<%= Page.ResolveUrl("~/help/faq/") %>" title="Frequently Asked Questions">FAQ</a> as 
							well for more information. 
						</div>
						<asp:Repeater ID="_purchaseOptions" runat="server" OnItemCreated="PoItemCreatedHandler">
							<HeaderTemplate>
								<table class="Spacer" cellspacing="0">
							</HeaderTemplate>
							<ItemTemplate>
									<tr>
									    <td style="width: 50px; vertical-align: top;" class="Faint">
									        <asp:Image ID="_sizeIndicator" runat="server" style="margin-right: 10px; float: left;" AlternateText="size indicator" />
									    </td>
										<td>
											<h4 style="margin: 0px;"><asp:Literal ID="_poTitle" runat="server" /></h4>
											<div style="margin: 10px 0px 5px 0px; font-weight: bold;" title="The size (along the longest side) of the photo you will download if you purchase this photo/license." class="LightFaint">
												Download Size: <asp:Literal id="_poDimensions" runat="server" />.
											</div>
											<div class="Faint LineSpaced">
												<asp:Literal ID="_poShortDescription" runat="server" />
												<div style="margin-top: 5px;">
												    <asp:HyperLink ID="_licenseLink" runat="server" Text="See full license" class="H5Faint" style="text-decoration: underline;" /> &raquo;
												</div>
											</div>
										</td>
										<td valign="top" align="right" style="padding-left: 10px;" nowrap="nowrap">
											<div style="margin-bottom: 10px;" class="Standout">
												£<asp:Literal ID="_poPrice" runat="server" />
											</div>
											<asp:HyperLink ID="_addBtn" runat="server" ImageUrl="~/_images/forms/add-to-basket.gif" ToolTip="add to basket" />
											<asp:PlaceHolder Visible="false" ID="_noAddPlaceHolder" runat="server">
											    <img src="<%# Page.ResolveUrl("~/_images/forms/add-to-basket-inactive.gif") %>" alt="sign-in to add to basket" />
											    <div style="margin-top: 10px;" class="LineSpaced">
											        <asp:HyperLink ID="_signInLink" runat="server" Text="sign-in" CssClass="H5" /> or <a href="<%# Page.ResolveUrl("~/register/") %>" class="H5">register</a><br />
											        <span class="Faint">to buy something</span>
											    </div>
											</asp:PlaceHolder>
										</td>
									</tr>
							</ItemTemplate>
							<SeparatorTemplate>
								<tr>
									<td colspan="3">
										<div style="border-bottom: dotted 1px #333; margin-bottom: 10px;" class="Small">&nbsp;</div>
									</td>
								</tr>
							</SeparatorTemplate>
							<FooterTemplate>
								</table>
							</FooterTemplate>
						</asp:Repeater>
												
						<asp:PlaceHolder ID="_noPurchaseOptionsNoCompanyView" runat="server" Visible="false">
							<div class="LineSpaced">
								You need to let us know about your company before you can make a purchase. 
								<h4 style="margin: 10px 0px 10px 0px;">Not registered or found your company? <a href="<%= Page.ResolveUrl("~/account/company/") %>">Do it now!</a></h4>
								<div class="Faint">
									Registration of a company only takes a minute.
								</div>
							</div>
						</asp:PlaceHolder>
					
					</div>
				</div>
			</td>
		</tr>
	</table>
</asp:Content>