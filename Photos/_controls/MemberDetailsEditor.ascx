<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MemberDetailsEditor.ascx.cs" Inherits="_controls.MemberDetailsEditor" %>

<table class="MediumForm">
    <tr>
        <td class="Faint">
            Title:
        </td>
        <td class="Faint">
            <asp:TextBox ID="_title" runat="server" CssClass="Tiny" />
        </td>
    </tr>
    <tr>
        <td class="Faint">
            First Name:
        </td>
        <td class="Faint">
            <asp:TextBox ID="_firstName" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="Faint">
            Middle Name:
        </td>
        <td>
            <asp:TextBox ID="_middleName" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="Faint">
            Last Name:
        </td>
        <td>
            <asp:TextBox ID="_lastName" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="Faint">
            E-Mail:
        </td>
        <td>
            <asp:TextBox ID="_email" runat="server" ValidationGroup="EditPersonalDetails" />
            <asp:RequiredFieldValidator 
                ControlToValidate="_email" 
                ValidationGroup="EditPersonalDetails" 
                runat="server" 
                Text="*" 
                ErrorMessage="Please supply a valid e-mail address." 
                Display="Dynamic" />
        </td>
    </tr>
    <tr>
        <td class="Faint" valign="top">
            Sex:
        </td>
        <td class="Faint">
            <asp:DropDownList ID="_sex" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="Faint" valign="top" style="padding-right: 10px;">
            Job Title:
        </td>
        <td>
            <asp:TextBox ID="_jobTitle" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="Faint" style="padding-right: 20px;">
            Telephone Number:
        </td>
        <td>
            <asp:TextBox ID="_telephone" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="Faint" valign="top">
            Address:
        </td>
        <td>
            <asp:TextBox TextMode="MultiLine" ID="_billingAddress" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="Faint">
            Postal Code:
        </td>
        <td>
            <asp:TextBox ID="_billingPostalCode" runat="server" CssClass="Tiny" />
        </td>
    </tr>
    <tr>
        <td class="Faint">
            Country:
        </td>
        <td>
            <asp:DropDownList ID="_billingCountryList" runat="server" />
        </td>
    </tr>
    <tr>
        <td>
            &nbsp;
        </td>
        <td>
            <asp:Button ID="_updateBtn" runat="server" Text="Update" OnClick="UpdateHandler" ValidationGroup="EditPersonalDetails" />
            <div style="margin-top: 10px;">
                <asp:ValidationSummary 
                    HeaderText="Please correct the following first:" 
                    ValidationGroup="EditPersonalDetails" 
                    runat="server" 
                    DisplayMode="BulletList" />
            </div>
        </td>
    </tr>
</table>