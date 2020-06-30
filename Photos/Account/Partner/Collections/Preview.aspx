<%@ Page Language="C#" MasterPageFile="~/_masterPages/Master.master" AutoEventWireup="true" CodeFile="Preview.aspx.cs" Inherits="Account.Partner.Collections.CollectionsPreviewPage" Title="Photo Preview" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" Runat="Server"></asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContentArea" Runat="Server">
    <table cellpadding="0" cellspacing="0" style="margin: 0px auto; text-align: center;">
        <tr>
            <td>
                <asp:Image ID="_photo" runat="server" AlternateText="Partner Preview" style="border: solid 5px #333;" />
                <div style="border: dotted 1px #333; padding: 10px; width: 95%; margin: 0px auto; margin-top: 10px; margin-bottom: 10px;">
                    <h4><span class="Faint">Name:</span> <asp:Literal ID="_name" runat="server" /></h4>
                    <span class="Faint">Size:</span> <asp:Literal ID="_dimensions" runat="server" /><br />
                    <span class="Faint">Status:</span> <asp:Literal ID="_status" runat="server" /><br />
                    <span class="Faint">Comment:</span> <asp:Literal ID="_description" runat="server" Text="-" /><br />
                    <span class="Faint">Tags:</span> <asp:Literal ID="_tags" runat="server" Text="-" />
                </div>
                <span class="Faint">protected area - don't share this photo</span>
            </td>
        </tr>
    </table>
</asp:Content>