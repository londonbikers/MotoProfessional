<%@ Page Language="C#" MasterPageFile="~/_masterPages/Regular.master" AutoEventWireup="true" CodeFile="Collection.aspx.cs" Inherits="PhotoCollectionPage" EnableViewState="false" %>
<%@ Register src="~/_controls/TagCloud.ascx" tagname="TagCloud" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" Runat="Server">
    <script type="text/javascript">
        $(document).ready(function() {
            $("#expandTags").click(function(){
                $("#<%= this._tagsContainer.ClientID %>").animate({ height: "100%" }, 1000);
                $("#<%= this._expandTagsBox.ClientID %>").css({ display: "none" });
                return false;
            });
        });
    </script>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="PageHeading" runat="server">
	<span class="Faint">Collection:</span> <asp:Literal ID="_title" runat="server" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PageContentArea" Runat="Server">
    <div class="ExplanationBox MedGradient" style="padding-top: 5px; background-color: Transparent;">
        <div class="Faint" style="margin-bottom: 5px;">
            Description:
        </div>
	    <asp:Literal ID="_description" runat="server" />
    </div>            
    <table class="Spacer" cellspacing="0" style="border-bottom: solid 1px #333;">
        <tr>
            <td valign="top" style="padding-top: 10px; padding-right: 10px; width: 160px;">
                <table style="width: 100%;" cellspacing="0">
			        <tr>
			            <td class="Faint LineSpaced" style="width: 60px;" valign="top">
			                Created:<br />
			                Photos:<br />
			                Tags:
			            </td>
			            <td class="LineSpaced" valign="top" nowrap="nowrap">
			                <asp:Literal ID="_created" runat="server" /><br />
			                <asp:Literal ID="_photoCount" runat="server" /><br />
			                <asp:Literal ID="_uniqueTags" runat="server" />
			            </td>
			        </tr>
			    </table>
                <div class="LineSpaced Faint" style="margin: 10px 0px 10px 0px;">
                    Click a tag to see just those photos in the collection.
                    <img src="<%= Page.ResolveUrl("~/") %>_images/arrow-right.png" alt="right" />
                </div>
            </td>
            <td style="padding: 10px 0px 10px 10px; border-left: dotted 1px #333;" valign="top">
                <h3 style="margin-bottom: 5px;">Tags</h3>
				<div id="_tagsContainer" runat="server" style="height: 125px; overflow: hidden;">
				    <asp:Literal ID="_noTags" runat="server" Visible="false">None, yet!</asp:Literal>
				    <uc1:TagCloud id="_tagCloud" runat="server" LinkCssPrefix="SMLTC" />
				</div>
				<div id="_expandTagsBox" runat="server" style="text-align: center; border-top: solid 1px #333; padding-top: 5px; margin-top: 2px;">
				    <a href="#" id="expandTags" class="H5">&laquo; view all &raquo;</a>
                </div>
				
				
            </td>
        </tr>
    </table>
    
    <div style="margin: 10px 0px 5px 0px;" id="_filterBox" runat="server" visible="false">
        <h3 id="_filterHeading" runat="server" style="vertical-align: middle; display: inline; margin-right: 10px;" />
        <asp:HyperLink ID="_filterNoneLink" runat="server" CssClass="H4U" Text="(View All)" />
    </div>
    <asp:PlaceHolder ID="_noFilterSeparator" runat="server">
        <div style="margin-top: 10px;">&nbsp;</div>
    </asp:PlaceHolder>
    
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
                                        <span class="Faint">Captured: <asp:Literal ID="_photoCaptured" runat="server" Text="-" /></span>
                                        <div style="margin-top: 5px;" class="Small Faint">
                                            <asp:Literal ID="_photoTags" runat="server" />
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