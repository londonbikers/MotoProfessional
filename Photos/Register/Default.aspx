<%@ Page Language="C#" MasterPageFile="~/_masterPages/Regular.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="RegisterPage" Title="Register - MP" %>
<%@ Register src="~/_controls/RegistrationTerms.ascx" tagname="RegistrationTermsCtrl" tagprefix="uc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="PageHeading" runat="server">
	Register
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="PageContentArea" Runat="Server">
    
    <div class="ExplanationBox" style="margin-bottom: 20px;">
		Register now, for free, to be able to browse the site in full, get higher-resolution previews, receive notifications of updates you're
		interested in and to be able to buy straight away.
    </div>
    
    <table border="0" style="margin: 0px auto;">
        <tr>
            <td align="center" colspan="2">
                <h3>Give us some details...</h3>
            </td>
        </tr>
        <tr>
            <td align="right">
                <label for="_username">User Name:</label>
            </td>
            <td>
                <asp:TextBox ID="_username" runat="server" />
                <asp:RequiredFieldValidator 
                    ControlToValidate="_username" 
                    ErrorMessage="User Name is required." 
                    ID="UserNameRequired" 
                    runat="server" 
                    ToolTip="User Name is required." 
                    ValidationGroup="_createUserStep" 
                    Text="*" />
            </td>
        </tr>
        <tr>
            <td align="right">
                <label for="_password">Password:</label>
            </td>
            <td>
                <asp:TextBox ID="_password" runat="server" TextMode="Password" />
                <asp:RequiredFieldValidator 
                    ControlToValidate="_password" 
                    ErrorMessage="Password is required."
                    ID="PasswordRequired" 
                    runat="server" 
                    ToolTip="Password is required." 
                    ValidationGroup="_createUserStep"
                    Text="*" />
            </td>
        </tr>
        <tr>
            <td align="right">
                <label for="ConfirmPassword">Confirm Password:</label>
            </td>
            <td>
                <asp:TextBox ID="_confirmPassword" runat="server" TextMode="Password" />
                <asp:RequiredFieldValidator 
                    ControlToValidate="_confirmPassword" 
                    ErrorMessage="Confirm Password is required."
                    ID="ConfirmPasswordRequired" 
                    runat="server" 
                    ToolTip="Confirm Password is required."
                    ValidationGroup="_createUserStep"
                    Text="*" />
            </td>
        </tr>
        <tr>
            <td align="right">
                <label for="_email">E-mail:</label>
            </td>
            <td>
                <asp:TextBox ID="_email" runat="server" />
                <asp:RequiredFieldValidator 
                    ControlToValidate="_email" 
                    ErrorMessage="E-mail is required."
                    ID="EmailRequired" 
                    runat="server" 
                    ToolTip="E-mail is required." 
                    ValidationGroup="_createUserStep"
                    Text="*" />
            </td>
        </tr>
        <tr>
            <td align="center" colspan="2">
                <asp:CompareValidator 
                    ControlToCompare="_password" 
                    ControlToValidate="_confirmPassword"
                    Display="Dynamic" 
                    ErrorMessage="The Password and Confirmation Password must match."
                    ID="PasswordCompare" 
                    runat="server" 
                    ValidationGroup="_createUserStep" />
            </td>
        </tr>
        <tr>
            <td colspan="2" style="text-align: center;">
                <h5 style="margin-top: 10px; margin-bottom: 10px;">Terms & Conditions</h5>
                <div style="text-align: left; overflow: auto; height: 100px; width: 550px; margin: 0px auto;" class="Highlight">
                    <uc1:RegistrationTermsCtrl runat="server" />
                </div>
                <div style="margin-top: 10px;">
                    <asp:CheckBox
                        ID="_acceptTerms" 
                        runat="server" 
                        Text="You accept the <a href='../help/registration-terms/' class='FaintU' target='_blank'>terms & conditions</a>." 
                        Checked="true" 
                        CssClass="Faint" />
                </div>
            </td>
        </tr>
        <tr>
            <td align="center" colspan="2" style="color: red;">
                <asp:ValidationSummary 
                    ValidationGroup="_createUserStep" 
                    runat="server" 
                    HeaderText="Please correct the following:">
                </asp:ValidationSummary>
                <asp:Literal 
                    ID="_customErrorMessage" 
                    runat="server" 
                    Visible="false" />
            </td>
        </tr>
        <tr>
            <td colspan="2" align="center">
                <asp:Button 
                    OnClick="RegisterMemberHandler" 
                    Text="Register!" 
                    ValidationGroup="_createUserStep" 
                    runat="server" 
                    CssClass="BigSize" />
            </td>
        </tr>
    </table>
               
</asp:Content>