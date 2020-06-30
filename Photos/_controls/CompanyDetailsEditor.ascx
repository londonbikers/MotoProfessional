<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CompanyDetailsEditor.ascx.cs" Inherits="_controls.CompanyDetailsEditor" %>

<table class="MediumForm">
    <tr id="_statusRow" runat="server" visible="false">
        <td class="Faint">
            Status:
        </td>
        <td>
            <asp:DropDownList ID="_status" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="Faint">
            Name:
        </td>
        <td class="Faint">
            <asp:TextBox ID="_name" runat="server" />
            <img src="<%= Page.ResolveUrl("~/_images/silk/bullet_star.png") %>" alt="required" /> <b>Required</b>
            <asp:RequiredFieldValidator runat="server" ErrorMessage="- A name is required!" ControlToValidate="_name" ValidationGroup="CDE" />
        </td>
    </tr>
    <tr>
        <td class="Faint">
            Telephone:
        </td>
        <td class="Faint">
            <asp:TextBox ID="_telephone" runat="server" />
            <img src="<%= Page.ResolveUrl("~/_images/silk/bullet_star.png") %>" alt="required" /> <b>Required</b>
            <asp:RequiredFieldValidator runat="server" ErrorMessage="- A telephone number is required!" ControlToValidate="_telephone" ValidationGroup="CDE" />
        </td>
    </tr>
    <tr>
        <td class="Faint">
            Fax:
        </td>
        <td>
            <asp:TextBox ID="_fax" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="Faint" valign="top">
            Description:
        </td>
        <td class="Faint">
            <asp:TextBox TextMode="MultiLine" style="float: left; margin-right: 10px;" ID="_description" runat="server"  />
            <img src="<%= Page.ResolveUrl("~/_images/silk/arrow_left.png") %>" alt="<" />
            Optionally tell us a little about your company or team.
        </td>
    </tr>
    <tr>
        <td class="Faint" valign="top" style="padding-right: 10px;">
            Postal Address:
        </td>
        <td>
            <asp:TextBox TextMode="MultiLine" ID="_postalAddress" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="Faint">
            Postal Code:
        </td>
        <td>
            <asp:TextBox ID="_postalCode" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="Faint">
            Country:
        </td>
        <td>
            <asp:DropDownList ID="_countryList" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="Faint">
            Website:
        </td>
        <td>
            <asp:TextBox ID="_website" runat="server" />
        </td>
    </tr>
    <tr>
        <td>
            &nbsp;
        </td>
        <td>
            <asp:Button ID="_updateBtn" runat="server" Text="Register!" OnClick="UpdateCompanyHandler" ValidationGroup="CDE" />
        </td>
    </tr>
</table>