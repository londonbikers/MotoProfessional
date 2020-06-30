<%@ Page Language="C#" MasterPageFile="~/_masterPages/Regular.master" AutoEventWireup="true" CodeFile="Partner.aspx.cs" Inherits="PartnerPage" %>
<%@ Register src="~/_controls/TagCloud.ascx" tagname="TagCloud" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" Runat="Server" />

<asp:Content ID="Content2" ContentPlaceHolderID="PageHeading" Runat="Server">
    <span class="Faint">Partner:</span> <asp:Literal ID="_name" runat="server" />
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="PageContentArea" Runat="Server">
    <table class="Spacer" cellspacing="0" style="margin-top: 10px;">
	    <tr>
	        <td style="padding-right: 20px; width: 200px;" valign="top">
	            <asp:Image ID="_logo" runat="server" Visible="false" style="margin-bottom: 10px;" />
	            <div class="LineSpaced" style="margin-bottom: 10px;">
	                <asp:Literal ID="_description" runat="server" />
	                <asp:PlaceHolder ID="_websitePlaceHolder" runat="server" Visible="false">
	                    <p>
	                        <span class="Faint">Website:</span><br />
	                        <asp:HyperLink ID="_websiteLink" runat="server" Target="_blank" />
	                    </p>
	                </asp:PlaceHolder>
	                <table cellpadding="0" cellspacing="0">
	                    <tr>
	                        <td class="Faint">Joined:</td>
	                        <td><asp:Literal ID="_joined" runat="server" /></td>
	                    </tr>
	                    <tr>
	                        <td class="Faint">Collections:</td>
	                        <td><asp:Literal ID="_collectionCount" runat="server" /></td>
	                    </tr>
	                    <tr>
	                        <td class="Faint" style="padding-right: 10px;">Photographs:</td>
	                        <td><asp:Literal ID="_photoCount" runat="server" /></td>
	                    </tr>
	                </table>
	            </div>
	            <hr />
	            <div class="LineSpaced Faint" style="margin-top: 10px;">
	                MP publishes the best photography by the best photographers. These photographers are our partners. Here's the latest work by
	                our partner <asp:Literal ID="_smallName" runat="server" />. 
	                
	                <p>To ask about becoming a partner, sign-up and then <a href="/contact/" class="FaintU">contact-us</a>.</p>
	            </div>
	        </td>
	        <td style="padding-left: 20px; border-left: dotted 1px #333;" valign="top">
	            <asp:PlaceHolder ID="_mainContent" runat="server">
	                <h3 style="margin-bottom: 5px;">Hot Photos</h3> 
	                <div class="Faint" style="margin-bottom: 10px;"><asp:Literal ID="_hotPhotosMode" runat="server" /></div>
	                <div class="Clear"></div>
    	            
	                <div class="Gradient" style="margin-bottom: 20px;">
	                    <div class="Clear"></div>
	                    <div style="float: left; width: 30px;">&nbsp;</div>
	                    <asp:Repeater ID="_hotPhotosGrid" runat="server" OnItemCreated="PhotoGridItemCreatedHandler">
    	                    <ItemTemplate>
    	                        <div class="SimpleFrame" style="width: 100px; float: left; margin-right: 15px;"><asp:Literal ID="_photoThumb" runat="server" /></div>
    	                    </ItemTemplate>
    	                </asp:Repeater>
    	                <div class="Clear"></div>
	                </div>
    	        
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
    	        
    	        </asp:PlaceHolder>
    	        <asp:PlaceHolder id="_noContent" runat="server" Visible="false">
    	            <div class="Highlight">
    	                <h2>Hold on a minute!</h2>
    	                This partner has no work published yet. Please check back again later.
    	            </div>
    	        </asp:PlaceHolder>
	        </td>
        </tr>
	</table>
</asp:Content>