<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SubNavigation.ascx.cs" Inherits="SubNavigation" %>

<asp:PlaceHolder ID="_headerView" runat="server" Visible="false">
    <div style="margin-bottom: 10px; padding-bottom: 3px;" class="DottedBottom">
        Options:
        <img src="<%= Page.ResolveUrl("~/_images/silk/bullet_go.png") %>" style="margin-bottom: 2px;" alt="go" />
</asp:PlaceHolder>

<asp:PlaceHolder ID="_accountOptionsView" runat="server" Visible="false">
		<b><a href="<%= Page.ResolveUrl("~/account/") %>">Your Details</a></b> -
        <b><a href="<%= Page.ResolveUrl("~/account/orders/") %>">Your Orders</a></b> - 
        <b><a href="<%= Page.ResolveUrl("~/account/company/") %>">Your Company</a></b>
        <asp:PlaceHolder ID="_partnerCPView" runat="server" Visible="false">
            - <b><a href="<%= Page.ResolveUrl("~/account/partner/") %>">Partner Control Panel</a></b>
        </asp:PlaceHolder>
</asp:PlaceHolder>

<asp:PlaceHolder ID="_footerView" runat="server" Visible="false">
    </div>
</asp:PlaceHolder>