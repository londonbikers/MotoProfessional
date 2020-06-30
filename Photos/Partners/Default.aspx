<%@ Page Language="C#" MasterPageFile="~/_masterPages/Regular.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="PartnersPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" Runat="Server" />
<asp:Content ID="Content2" ContentPlaceHolderID="PageHeading" Runat="Server">
    Our Partners
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="PageContentArea" Runat="Server">
  <table class="Spacer" cellspacing="0" style="margin-top: 10px;">
	    <tr>
	        <td style="padding-right: 20px; width: 200px;" valign="top">
	            <div class="LineSpaced Faint">
	                MP publishes the best photography by the best photographers. These photographers are our partners. Here's the latest work by
	                our partner <asp:Literal ID="_smallName" runat="server" />. 
	                
	                <p>To ask about becoming a partner, sign-up and then <a href="/contact/" class="FaintU">contact-us</a>.</p>
	            </div>
	        </td>
	        <td style="padding-left: 20px; border-left: dotted 1px #333;" valign="top">
            
                <asp:PlaceHolder ID="_topPartnersArea" runat="server" Visible="false">
                    <!-- disabled whilst we build up a partner portfolio. -->
                    <h3 style="margin-bottom: 5px;">Top Partners</h3> 
                    <div class="Clear"></div>
                    <div class="Gradient" style="margin-bottom: 20px;">
                        <div class="Clear"></div>
                        <div style="float: left; width: 30px;">&nbsp;</div>
                        <asp:Repeater ID="_topPartnersGrid" runat="server" OnItemCreated="TopPartnerItemCreatedHandler">
	                        <ItemTemplate>
	                            <div class="SimpleFrame" style="width: 100px; float: left; margin-right: 15px;"><asp:Literal ID="_photoThumb" runat="server" /></div>
	                        </ItemTemplate>
	                    </asp:Repeater>
	                    <div class="Clear"></div>
                    </div>
                </asp:PlaceHolder>
	        
	            <h3>Our Partners</h3>
	            <div class="DottedBottom" style="margin-bottom: 10px;">
	                <asp:Repeater ID="_partnersGrid" runat="server" OnItemCreated="PartnersItemCreatedHandler">
	                    <HeaderTemplate>
	                        <table class="Spacer" cellspacing="0">
	                            <tr>
	                    </HeaderTemplate>
	                    <ItemTemplate>
	                                <td width="50%" valign="top" style="padding-bottom: 20px;<asp:Literal id="_cellStyle" runat="server" />">
                                        <table class="Spacer" cellspacing="0">
                                            <tr>
                                                <td style="width: 120px;" valign="top">
                                                    <div class="SimpleFrame" style="width: 100px;"><asp:Literal ID="_partnerCover" runat="server" /></div>
                                                </td>
                                                <td valign="top" class="LineSpaced">
                                                    <asp:HyperLink ID="_partnerName" runat="server" CssClass="H4" />
                                                    <hr />
                                                    <span class="">Joined: <asp:Literal ID="_partnerCreated" runat="server" /></span>
                                                    <div style="margin-top: 5px;" class="Faint Small">
                                                        <asp:Literal ID="_partnerCollections" runat="server" />
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
    	        
    	        <asp:PlaceHolder ID="_morePartnersArea" runat="server" Visible="false">
	                <h3>More Partners</h3>
	                <asp:Repeater ID="_morePartners" runat="server" OnItemCreated="MorePartnersItemCreatedHandler">
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
	            </asp:PlaceHolder>
    	        
	        </td>
        </tr>
	</table>
</asp:Content>