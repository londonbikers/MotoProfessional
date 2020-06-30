<%@ Page Language="C#" MasterPageFile="~/_masterPages/Regular.master" AutoEventWireup="true" CodeFile="Order.aspx.cs" Inherits="Account.Orders.OrderPage" Title="MP: Your Order" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" Runat="Server" />
<asp:Content ID="Content2" ContentPlaceHolderID="PageHeading" Runat="Server">
	Your Order
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="PageContentArea" Runat="Server">
	<div class="ExplanationBox" style="margin-bottom: 10px;">
		<h4>Thanks for buying from us!</h4>
		The details of your order are below, where you can also download your purchases. You can download these at any time again as well.
	</div>
	<h3>Details</h3>
	<table class="Form" cellspacing="0">
		<tr>
			<td class="Faint">
				Reference:
			</td>
			<td>
				#<asp:Literal ID="_ourReference" runat="server" />
			</td>
		</tr>
		<tr>
			<td class="Faint">
				Created:
			</td>
			<td>
				<asp:Literal ID="_created" runat="server" />
			</td>
		</tr>
		<tr>
			<td class="Faint">
				Status:
			</td>
			<td>
				<asp:Literal ID="_status" runat="server" />
			</td>
		</tr>
		<tr>
			<td class="Faint" style="padding-right: 20px;">
				Charge Method:
			</td>
			<td>
				<asp:Literal ID="_method" runat="server" />
			</td>
		</tr>
		<tr>
			<td class="Faint" style="padding-right: 20px;">
				Total Charged:
			</td>
			<td>
				£ <asp:Literal ID="_total" runat="server" />
			</td>
		</tr>
	</table>
	
	<h3 style="margin-top: 20px;">Downloads</h3>
	<h4 id="_downloadsNotAvailable" runat="server" visible="false">* Order incomplete, downloads not yet available.</h4>
	
	<div style="margin-bottom: 10px;" id="_noDownloads" runat="server" visible="false">
	    <h4>Wait! Your downloads aren't ready!</h4>
	    It's been some time since they were bought so we need to re-create them. It won't take a moment.
	    <div style="margin-top: 5px;">
	        <img src="<%= Page.ResolveUrl("~/_images/silk/cog_go.png") %>" alt="create" />
	        <asp:LinkButton runat="server" ID="_createDownloadsBtn" Text="Create Downloads" CssClass="H4U" OnClick="CreateDownloadsHandler" ToolTip="Wait whilst your downloads are created..." />
	    </div>
	</div>
	
	<div style="margin-bottom: 10px;" id="_zipDiv" runat="server">
		For convenience, you can also download a Zip file with all of the items in your order here:
		<div style="margin-top: 5px;">
			<img src="<%= Page.ResolveUrl("~/_images/silk/picture_save.png") %>" alt="download" /> 
			<asp:HyperLink ID="_zipFileLink" runat="server" class="H4U" Text="Download Whole Order" />
		</div>
	</div>
	
	<asp:GridView
		OnRowCreated="OrderItemRowCreatedHandler" 
		SkinID="LightContained"
		ID="_orderItems"
		AutoGenerateColumns="false"
		runat="server">
		<Columns>
			<asp:TemplateField HeaderText="Product">
				<ItemTemplate>
					<asp:Literal ID="_name" runat="server" />
				</ItemTemplate>
			</asp:TemplateField>
			<asp:TemplateField HeaderText="License">
				<ItemTemplate>
					<asp:Literal ID="_license" runat="server" />
				</ItemTemplate>
			</asp:TemplateField>
			<asp:TemplateField HeaderText="Cost">
				<ItemTemplate>
					£ <asp:Literal ID="_cost" runat="server" />
				</ItemTemplate>
			</asp:TemplateField>
			<asp:TemplateField HeaderText="Dimensions">
				<ItemTemplate>
					<asp:Literal ID="_dimensions" runat="server" Text="-" />
				</ItemTemplate>
			</asp:TemplateField>
			<asp:TemplateField HeaderText="Filesize">
				<ItemTemplate>
					<asp:Literal ID="_filesize" runat="server" Text="-" />
				</ItemTemplate>
			</asp:TemplateField>
			<asp:TemplateField HeaderText="Download">
				<ItemTemplate>
					<asp:HyperLink ID="_link" runat="server" Text="download photo" />
					<asp:Label ID="_noLink" runat="server" Text="-" Visible="false" />
				</ItemTemplate>
			</asp:TemplateField>
		</Columns>
	</asp:GridView>
	
</asp:Content>