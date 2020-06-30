<%@ Page Language="C#" MasterPageFile="~/_masterPages/Admin.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="AdminDefaultPage" Title="MPa" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageContentArea" Runat="Server">
    <h3>Welcome</h3>
    <div class="DottedBox">
        Current Stats:
    </div>
    <table>
        <tr>
            <td class="Faint">
                Current active members:
            </td>
            <td>
                <asp:Literal ID="_currentVisitors" runat="server" />
            </td>
        </tr>
    </table>
</asp:Content>