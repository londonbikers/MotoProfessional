<%@ Page Language="C#" MasterPageFile="~/_masterPages/Regular.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Account.Partner.Collections.CollectionsPage" Title="MP: Partner Collections" %>

<asp:Content ID="Content2" ContentPlaceHolderID="PageHeading" runat="server">
	Partner Collections
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="PageContentArea" Runat="Server">

    <div style="margin-bottom: 10px;" class="Faint">
        More:
        <img src="../../../_images/arrow-right.png" style="margin-bottom: 2px;" alt="go" />
        <a href="../collections/" class="FaintU">Collections</a> - 
        <a href="../rates/" class="FaintU">Rates</a> 
        <!-- - <a href="../reports/" class="FaintU">Reports</a>-->
    </div>
    
    <img src="../../../_images/silk/pictures.png" alt="collection" />
    <a href="collection.aspx" class="H2" title="Upload photos into a new collection.">New Collection</a>
    
    <div class="FormBox" style="margin-top: 20px; margin-bottom: 20px;">
		<table class="Spacer">
	        <tr>
	            <td>
	                View:
		            <a href="./">Newest Collections</a>
	            </td>
	            <td align="right">
	                Search by name:
	                <asp:TextBox ID="_name" runat="server" style="margin-right: 10px;" />
	                Search by tag:
	                <asp:TextBox ID="_tag" runat="server" style="margin-right: 10px;" />
	                <asp:Button ID="_customerSearchBtn" runat="server" Text="Search" OnClick="SearchHandler" />
	            </td>
	        </tr>
	    </table>
	</div>
    
    <asp:GridView
		ID="_collectionsGrid"
		AutoGenerateColumns="false"
		runat="server">
		<Columns>
			<asp:TemplateField ItemStyle-Width="16px">
				<ItemTemplate>
					<img src="../../../_images/silk/pictures.png" alt="collection" />
				</ItemTemplate>
			</asp:TemplateField>
			<asp:TemplateField HeaderText="Name">
				<ItemTemplate>
					<a href="collection.aspx?id=<%# Eval("ID") %>"><%# Eval("Name") %></a>
				</ItemTemplate>
			</asp:TemplateField>
		    <asp:BoundField DataField="Status" HeaderText="Status" ItemStyle-Width="70px" />
		    <asp:TemplateField HeaderText="Created" ItemStyle-Width="140px" ItemStyle-CssClass="Faint">
				<ItemTemplate>
					<%# ((DateTime)Eval("Created")).ToString(System.Configuration.ConfigurationManager.AppSettings["ShortDateTimeFormatString"]) %>
				</ItemTemplate>
		    </asp:TemplateField>
		    <asp:TemplateField HeaderText="Last Updated" ItemStyle-Width="140px">
				<ItemTemplate>
					<%# ((DateTime)Eval("LastUpdated")).ToString(System.Configuration.ConfigurationManager.AppSettings["ShortDateTimeFormatString"]) %>
				</ItemTemplate>
		    </asp:TemplateField>
        </Columns>
	</asp:GridView>
    
</asp:Content>