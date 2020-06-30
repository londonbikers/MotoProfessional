<%@ Page Language="C#" MasterPageFile="~/_masterPages/Regular.master" AutoEventWireup="true" CodeFile="Tag.aspx.cs" Inherits="PhotosTagPage" EnableViewState="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" Runat="Server" />

<asp:Content ID="Content3" ContentPlaceHolderID="PageHeading" runat="server">
	<span class="Faint">Tag:</span> <asp:Literal ID="_title" runat="server" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PageContentArea" Runat="Server">

    <div style="float: right;">
        <asp:HyperLink ID="_rssLink" runat="server" CssClass="H4UF" ToolTip="subscribe to the RSS feed" />
        <img src="<%= Page.ResolveUrl("~/_images/fugue/feed_balloon.png") %>" alt="rss" style="margin-top: 3px;" />
    </div>
    
    Here are the latest photos with a '<asp:Literal ID="_introTagName" runat="server" />' tag.
    <div class="Faint" style="margin-top: 10px; border-bottom: dotted 1px #333; padding-bottom: 10px;">
        If you're unable to find what you're looking for (only 300 photos max can seen here), use our <a href="<%= Page.ResolveUrl("~/") %>photos/search/" class="FaintU">search</a> feature.
    </div>
    <h3 style="margin-bottom: 10px; margin-top: 10px;">Photos</h3>
    <asp:PlaceHolder ID="_tileView" runat="server">
        <div style="padding-bottom: 10px; margin-bottom: 20px; border-bottom: dotted 1px #333;">
            <table class="Spacer" cellspacing="0">
                <tr>
                    <td class="Faint" valign="bottom">
                        <asp:Literal ID="_paginationStats" runat="server" />
                    </td>
                    <td align="right" style="font-size: 14px; font-family: Arial;">
                        <asp:Literal ID="_paginationControls" runat="server" />
                    </td>
                </tr>
            </table>
        </div>
        <asp:Repeater ID="_photosGrid" runat="server" OnItemCreated="PhotoGridItemCreatedHandler">
            <HeaderTemplate>
                <table class="Spacer" cellspacing="0">
                    <tr>
            </HeaderTemplate>
            <ItemTemplate>
                        <td width="33%" valign="top" style="padding-bottom: 20px;<asp:Literal id="_cellStyle" runat="server" />">
                            <table cellpadding="0" cellspacing="0">
                                <tr>
                                    <td valign="top"><asp:Literal ID="_photoCover" runat="server" /></td>
                                    <td valign="top" class="LineSpaced" style="padding-left: 10px;">
                                        <asp:HyperLink ID="_photoTitle" runat="server" CssClass="H4" />
                                        <hr />
                                        <div class="Faint LineSpaced">
                                            Captured: <asp:Literal ID="_photoCaptured" runat="server" Text="-" /><br />
                                        </div>
                                        <div style="margin-top: 5px;" class="Faint LineSpaced">
                                            Collections:
                                            <div class="Small">
                                                <asp:Repeater ID="_photoCollections" runat="server" OnItemCreated="PhotoCollectionItemCreatedHandler">
                                                    <HeaderTemplate>
                                                        <ul class="Custom">
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                            <li>&raquo; <asp:Literal ID="_photoCollectionLink" runat="server" /></li>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        </ul>
                                                    </FooterTemplate>
                                                </asp:Repeater>
                                            </div>
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
        <div style="padding-top: 10px; border-top: dotted 1px #333;">
            <table class="Spacer" cellspacing="0">
                <tr>
                    <td class="Faint" valign="top">
                        <asp:Literal ID="_bottomPaginationStats" runat="server" />
                    </td>
                    <td align="right" style="font-size: 14px; font-family: Arial;">
                        <asp:Literal ID="_bottomPaginationControls" runat="server" />
                    </td>
                </tr>
            </table>
        </div>
    </asp:PlaceHolder>
    
</asp:Content>