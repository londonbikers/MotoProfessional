<%@ Page Language="C#" MasterPageFile="~/_masterPages/Admin.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="AdminSystemPage" Title="MPa: System" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageHeading" Runat="Server">
	System Status
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PageContentArea" Runat="Server">

	<a href="http://logs.mediapanther.com" target="_blank">Logging</a> | 
	<a href="emailtester.aspx">Email Tester</a> |
	<a href="stats.aspx">Basic Statistics</a>
	<hr />

	Top 100 Cache Contents:
    <div class="FormBox" style="margin-bottom: 10px; margin-top: 5px;">
        <img src="../../_images/silk/database_refresh.png" alt="clear cache" />
        <asp:LinkButton ID="_clearCacheBtn" runat="server" Text="Clear Cache" OnClick="ClearCacheHandler" />
        <hr />
        <table class="Spacer">
            <tr>
                <td class="Field">
                    Items in cache:
                </td>
                <td class="Control">
                    <asp:Literal ID="_itemsCount" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="Field">
                    Cache capacity:
                </td>
                <td class="Control">
                    <asp:Literal ID="_cacheCapacity" runat="server" />
                </td>
            </tr>
        </table>
	</div>

	<asp:GridView 
		ID="_cacheItemsGrid"
		AutoGenerateColumns="false"
		runat="server">
		<Columns>
			<asp:BoundField DataField="RequestCount" HeaderText="Uses" />
			<asp:TemplateField HeaderText="Name">
                <ItemTemplate>
					<%# GetItemTitle(Eval("Item")) %>
                </ItemTemplate>
            </asp:TemplateField>
			<asp:BoundField DataField="TypeIdentifier" HeaderText="Type" />
			<asp:BoundField DataField="PrimaryKey" HeaderText="Primary Key" />
			<asp:BoundField DataField="SecondaryKey" HeaderText="Secondary Key" />
			<asp:BoundField DataField="Created" HeaderText="Added" />
		</Columns>
	</asp:GridView>

</asp:Content>