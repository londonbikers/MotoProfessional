<%@ Page Language="C#" MasterPageFile="~/_masterPages/Regular.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="PhotosDefaultPage" Title="MP: Latest Photos" EnableViewState="false" %>
<%@ Register src="~/_controls/TagCloud.ascx" tagname="TagCloud" tagprefix="uc1" %>

<asp:Content ContentPlaceHolderID="PageHeading" runat="server">
	The Latest Photos
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="PageContentArea" Runat="Server">
	<table class="Spacer" cellspacing="0" style="margin-top: 10px;">
	    <tr>
	        <td style="padding-right: 20px; width: 200px;" valign="top">
	            <div class="LineSpaced DottedBottom">
	                All of our photos are grouped into collections. You can see, browse and buy from the latest collections here.
	            </div>
	            <div style="margin-top: 10px;" class="DottedBottom">
    	            <h3 style="margin-top: 10px;">Latest Tags</h3>
    	            <div class="Faint LineSpaced" style="margin-bottom: 10px;">
    	                You can also browse by tag, i.e. keywords associated with a photo.
    	            </div>
    	            <uc1:TagCloud runat="server" />
    	        </div>
    	        <div class="DottedBottom" style="padding-top: 10px;">
    	            <h3>Top Events</h3>
    	            <div class="Faint LineSpaced" style="margin-bottom: 10px;">
    	                Here's our speciality events.
    	            </div>
    	            <ul class="Pinned">
    	                <li>
    	                    <b><a href="tags/british-superbikes">British Superbikes</a></b>
                            <ul>
	                            <li><a href="tags/british-supersport">British Supersport</a></li>
	                            <li><a href="tags/superstock-1000">Superstock 1000</a></li>
	                            <li><a href="tags/superstock-600">Superstock 600</a></li>
	                            <li><a href="tags/ktm-rc8-super-cup">KTM RC8 Super Cup</a></li>
	                            <li><a href="tags/125gp">125GP</a></li>
	                        </ul>
                        </li>
    	                <li><b><a href="tags/world-superbikes">World Superbikes</a></b></li>
    	                <li><b><a href="tags/motogp">MotoGP</a></b></li>
    	                <li><b><a href="tags/british-mx">British MX</a></b></li>
    	                <li><b><a href="tags/world-mx">World MX</a></b></li>
    	            </ul>
    	        </div>
    	        <div class="DottedBottom" style="padding-top: 10px;">
    	            Previous Events:
                    <ul style="margin-top: 10px;">
                        <li><a href="tags/ktm-superduke-cup">KTM Superduke Cup</a></li>
                        <li><a href="tags/r1-cup">Henderson R1 Cup</a></li>
                    </ul>
    	        </div>
                <div style="padding-top: 10px;">
	                <h3>Our Partners</h3>
	                <div class="Faint LineSpaced" style="margin-bottom: 10px;">
	                    Our library is made up by the production of our partners. <a href="/partners/" class="FaintU">Click here</a> to see all the partners.
	                </div>
	                Current Top Partners:
	                <ul class="Pinned">
	                    <asp:Repeater ID="_topPartners" runat="server" OnItemCreated="TopPartnersItemCreatedHandler">
	                        <ItemTemplate>
	                            <li><b><asp:HyperLink ID="_partnerLink" runat="server" CssClass="Faint" /></b></li>
	                        </ItemTemplate>
	                    </asp:Repeater>
	                </ul>
	            </div>
	        </td>
	        <td style="padding-left: 20px; border-left: dotted 1px #333;" valign="top">
    	        <h3>Latest Collections</h3>
    	        <div class="DottedBottom" style="margin-bottom: 10px;">
    	            <asp:Repeater ID="_collectionsGrid" runat="server" OnItemCreated="CollectionGridItemCreatedHandler">
    	                <HeaderTemplate>
    	                    <table class="Spacer" cellspacing="0">
    	                        <tr>
    	                </HeaderTemplate>
    	                <ItemTemplate>
    	                            <td width="50%" valign="top" style="padding-bottom: 20px;<asp:Literal id="_cellStyle" runat="server" />">
	                                    <table class="Spacer" cellspacing="0">
	                                        <tr>
	                                            <td style="width: 120px;" valign="top">
	                                                <div class="SimpleFrame" style="width: 100px;"><asp:Literal ID="_collectionCover" runat="server" /></div>
	                                            </td>
	                                            <td valign="top" class="LineSpaced">
	                                                <asp:HyperLink ID="_collectionTitle" runat="server" CssClass="H4" />
	                                                <hr />
	                                                <span class=""><asp:Literal ID="_created" runat="server" /> - <asp:Literal ID="_photoCount" runat="server" /> photos</span>
	                                                <div style="margin-top: 5px;" class="Faint Small">
	                                                    <asp:Literal ID="_collectionDescription" runat="server" />
	                                                </div>
	                                            </td>
	                                        </tr>
	                                    </table>
                                    </td>
                                    <asp:Literal ID="_rowSeparator" runat="server" Visible="false">
                                        </tr>
                                        <tr>
                                    </asp:Literal>
    	                </ItemTemplate>
    	                <FooterTemplate>
    	                        </tr>
    	                    </table>
    	                </FooterTemplate>
    	            </asp:Repeater>
    	        </div>
    	        
    	        <h3>More Collections</h3>
    	        <asp:Repeater ID="_moreCollections" runat="server" OnItemCreated="MoreCollectionsItemCreatedHandler">
    	            <HeaderTemplate>
    	                <ul class="Black">
    	            </HeaderTemplate>
    	            <ItemTemplate>
    	                    <li><asp:HyperLink ID="_link" runat="server" /> <span class="Faint">| <asp:Literal ID="_meta" runat="server" /></span></li>
    	            </ItemTemplate>
    	            <FooterTemplate>
    	                </ul>
    	            </FooterTemplate>
    	        </asp:Repeater>
    	        
    	        To view photos older than this, please use our <a href="search/" class="FaintU">search feature</a>.
	        </td>
        </tr>
	</table>
</asp:Content>