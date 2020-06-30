<%@ Page Language="C#" MasterPageFile="~/_masterPages/Regular.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="PhotoSearchPage" Title="MP: Photo Search" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" Runat="Server">
    <script type="text/javascript">
        $(document).ready(function() {
            $("#SearchBoxToggle").click(function() {
                $("#SearchBox").toggle("fast");
                return false;
            });
        });
    </script>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="PageHeading" runat="server">
	Photo Search
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PageContentArea" Runat="Server">

    <div class="CollapsableHeader" style="margin-top: 10px;">
        <h3>New Search</h3>&nbsp;
        <img src="../../_images/silk/bullet_arrow_down.png" alt="toggle" /> 
        <a href="#" class="Control" id="SearchBoxToggle">show/hide</a>
    </div>
    <div class="CollapsableBody" id="SearchBox" style="text-align: left;">
        <div style="padding-bottom: 10px; border-bottom: solid 1px #333; margin-bottom: 10px;">
            <asp:TextBox CssClass="SuperSize" ID="_term" runat="server" ValidationGroup="Search" />
            <asp:Button CssClass="BigSize" runat="server" Text="Search" ID="_searchBtn" OnClick="SearchHandler" ValidationGroup="Search" style="margin: 0px !important; padding-top: 5px !important;"/>
            <asp:RequiredFieldValidator runat="server" ErrorMessage="* Mmmm?" ControlToValidate="_term" ValidationGroup="Search" />
        </div>
        <table cellspacing="0" cellpadding="0">
            <tr>
                <td style="padding-right: 20px; border-right: dotted 1px #333;">
                    <div class="Faint" style="margin-bottom: 5px;">
                        Photo Captured <span class="Small">(dd/mm/yy)</span>
                    </div>
                    from: <asp:TextBox ID="_capturedFrom" runat="server" style="margin-right: 5px;" CssClass="SmallBox" />
                    to: <asp:TextBox ID="_capturedUntil" runat="server" CssClass="SmallBox" />                
                </td>
                <td style="padding-left: 20px; padding-right: 20px; border-right: dotted 1px #333;">
                    <div class="Faint" style="margin-bottom: 5px;">
                        Orientation
                    </div>
                    <asp:RadioButton ID="_orientationAll" runat="server" Text="All" GroupName="Orientation" Checked="true" />
                    <span class="Faint" style="margin-left: 5px;">or</span>
                    <asp:RadioButton ID="_orientationLandscape" runat="server" Text="Landscape" GroupName="Orientation" />
                    <span class="Faint" style="margin-left: 5px;">or</span>
                    <asp:RadioButton ID="_orientationPortrait" runat="server" Text="Portrait" GroupName="Orientation" />
                    <span class="Faint" style="margin-left: 5px;">or</span>
                    <asp:RadioButton ID="_orientationSquare" runat="server" Text="Square" GroupName="Orientation" />
                </td>
                <td style="padding-left: 20px;">
                    <div class="Faint" style="margin-bottom: 5px;">
                        Search Against
                    </div>
                    <asp:RadioButton ID="_tagsChecked" runat="server" GroupName="Field" Text="Tags" Checked="true" />
                    <span class="Faint" style="margin-left: 5px;">or</span>
                    <asp:RadioButton ID="_nameCheckBox" runat="server" GroupName="Field" Text="Names" /> 
                    <span class="Faint" style="margin-left: 5px;">or</span>
                    <asp:RadioButton ID="_commentCheckBox" runat="server" GroupName="Field" Text="Comments" />
                </td>
            </tr>
        </table>
    </div>
    
    <asp:PlaceHolder ID="_noResultsView" runat="server" Visible="false">
        <div class="Highlight" style="margin-top: 10px;">
            <h4 style="margin-top: 0px;">No Results</h4>
            <img src="<%= Page.ResolveUrl("~/_images/silk/information.png") %>" alt="info" />
            Nothing was found for your search. Try expanding the search.
        </div>
    </asp:PlaceHolder>
    
    <asp:PlaceHolder ID="_photosView" runat="server" Visible="false">
        <h3 style="margin: 20px 0px 10px 0px;"><asp:Literal ID="_searchResultsTitleTerm" runat="server" /> Search Results</h3>
        <div style="padding-bottom: 10px; margin-bottom: 10px; border-bottom: dotted 1px #333;">
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
                                        <div class="Small Faint LineSpaced">
                                            Captured: <br />
                                            <asp:Literal ID="_photoCaptured" runat="server" Text="-" /><br />
                                        </div>
                                        <div style="margin-top: 5px;" class="Small Faint LineSpaced">
                                            Collections:
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