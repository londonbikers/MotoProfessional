<%@ Page Language="C#" MasterPageFile="~/_masterPages/Regular.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Account.AccountPage" Title="MP: My Account" %>
<%@ Register src="~/_controls/MemberDetailsEditor.ascx" tagname="MemberEditor" tagprefix="uc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="PageHeading" runat="server">
	My Account
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="PageContentArea" Runat="Server">
    
    <div class="Highlight" style="margin-top: 10px; margin-bottom: 20px;" id="_noCompanyBox" runat="server" visible="false">
        You haven't registered your self with a company or team yet. Moto Professional is a trade service only currently. To buy anything you need to let us know about your affiliation.
        <a href="company/">Please do so here</a> before continuing.
    </div>
    
    <asp:PlaceHolder ID="_summaryView" runat="server">
        <h3>Your Details</h3>
        <table cellspacing="0" class="Form">
            <tr>
                <td colspan="2">
                    <div class="DottedBox">
                        <img src="../_images/silk/text_signature.png" alt="edit" /> 
                        <asp:LinkButton runat="server" Text="edit" OnClick="EditAccountHandler" ToolTip="edit your personal details" />
                        -  <a href="passwordchange/" class="FaintU">change your password</a>
                    </div>
                </td>
            </tr>
            <tr>
                <td class="Faint">
                    Username:
                </td>
                <td>
                    <asp:Literal ID="_username" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="Faint" style="padding-right: 20px;">
                    Email Address:
                </td>
                <td>
                    <asp:Literal ID="_email" runat="server" />
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <hr />
                </td>
            </tr>
            <tr>
                <td class="Faint">
                    Name:
                </td>
                <td>
                    <asp:Literal ID="_name" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="Faint">
                    Sex:
                </td>
                <td>
                    <asp:Literal ID="_sex" runat="server" Text="-" />
                </td>
            </tr>
            <tr>
                <td class="Faint">
                    Job Title:
                </td>
                <td>
                    <asp:Literal ID="_jobTitle" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="Faint">
                    Telephone:
                </td>
                <td>
                    <asp:Literal ID="_telephone" runat="server" />
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <hr />
                </td>
            </tr>
            <tr>
                <td class="Faint" valign="top">
                    Address:
                </td>
                <td class="LineSpaced">
                    <asp:Literal ID="_billingAddress" runat="server">-</asp:Literal>
                </td>
            </tr>
            <tr>
                <td class="Faint" valign="top" style="padding-right: 20px;">
                    Postal Code:
                </td>
                <td>
                    <asp:Literal ID="_billingPostalCode" runat="server" Text="-" />
                </td>
            </tr>
            <tr>
                <td class="Faint">
                    Country:
                </td>
                <td>
                    <asp:Image ID="_countryFlag" runat="server" Visible="false" style="margin-right: 5px;" />
                    <asp:Literal ID="_billingCountry" runat="server" Text="-" />
                </td>
            </tr>
        </table>
    </asp:PlaceHolder>
    
    <asp:PlaceHolder ID="_editView" runat="server" Visible="false">
        <h3>Update Details</h3>
        <div class="ExplanationBox" style="margin-bottom: 20px;">
           <asp:LinkButton ID="LinkButton2" Text="&laquo; cancel" runat="server" OnClick="CancelEditHandler" /> 
           - Update your details below. Your details are private and won't be shared with anyone.
        </div>
        <uc1:MemberEditor ID="_memberEditor" runat="server" />
    </asp:PlaceHolder>
    
</asp:Content>