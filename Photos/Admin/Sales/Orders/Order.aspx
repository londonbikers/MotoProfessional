<%@ Page Language="C#" MasterPageFile="~/_masterPages/Admin.master" AutoEventWireup="true" CodeFile="Order.aspx.cs" Inherits="Admin.Sales.Orders.OrderPage" Title="MPa: Order" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageHeading" Runat="Server">
    Customer Order
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PageContentArea" Runat="Server">
    <div class="ExplanationBox" style="margin-bottom: 20px;">
		Here's the details of the customer order. If you need to make changes you can, though this is not advised, it should done as a last-resort.
    </div>
    
	<h3>Details</h3>
	<table class="Form" cellspacing="0">
		<tr>
			<td class="Faint">
				Customer:
			</td>
			<td>
				<asp:HyperLink ID="_customerLink" runat="server" />
			</td>
		</tr>
		<tr>
			<td class="Faint">
				Company:
			</td>
			<td>
				<asp:HyperLink ID="_companyLink" runat="server" Text="-" Enabled="false" />
			</td>
		</tr>
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
				<asp:DropDownList ID="_status" runat="server" style="margin-right: 5px; vertical-align: middle;" /><asp:Button id="_changeStatusBtn" runat="server" Text="update" OnClick="UpdateOrderStatusHandler" />
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
	<b id="_downloadsNotAvailable" runat="server" visible="false">* Order incomplete, downloads not yet available.</b>
	
	<div style="margin-bottom: 10px;" id="_noDownloads" runat="server" visible="false">
	    <h4>Downloads need to be created!</h4>
	    It's been some time since they were bought so we need to re-create them. It won't take a moment.
	    <div style="margin-top: 5px;">
	        <img src="<%= Page.ResolveUrl("~/_images/silk/cog_go.png") %>" alt="create" />
	        <asp:LinkButton runat="server" ID="_createDownloadsBtn" Text="Create Downloads" CssClass="H4U" OnClick="CreateDownloadsHandler" ToolTip="Wait whilst the downloads are created..." />
	    </div>
	</div>
	
	<div style="margin-bottom: 10px;" id="_zipDiv" runat="server">
		For convenience, you can also download a Zip file with all of the items in the order here:
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
					<asp:HyperLink ID="_productLink" runat="server" Target="_blank" />
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
	
	<h3 style="margin-top: 20px;">Download Logs</h3>
	<b id="_noDownloadLogs" runat="server" visible="false">* No downloads logs.</b>
    <asp:GridView
		OnRowCreated="DownloadLogRowCreatedHandler" 
		SkinID="LightContained"
		ID="_downloadLogs"
		AutoGenerateColumns="false"
		runat="server">
		<Columns>
			<asp:TemplateField HeaderText="What">
				<ItemTemplate>
					<asp:Literal ID="_what" runat="server" />
				</ItemTemplate>
			</asp:TemplateField>
			<asp:TemplateField HeaderText="When">
				<ItemTemplate>
					<asp:Literal ID="_when" runat="server" />
				</ItemTemplate>
			</asp:TemplateField>
			<asp:TemplateField HeaderText="Who">
				<ItemTemplate>
					<asp:HyperLink ID="_whoLink" runat="server" />
				</ItemTemplate>
			</asp:TemplateField>
			<asp:TemplateField HeaderText="IP Address">
				<ItemTemplate>
					<asp:HyperLink ID="_ipAddressLink" runat="server" Target="_blank" />
				</ItemTemplate>
			</asp:TemplateField>
			<asp:TemplateField HeaderText="Referrer">
				<ItemTemplate>
					<asp:Literal ID="_referrer" runat="server" Text="-" />
				</ItemTemplate>
			</asp:TemplateField>
			<asp:TemplateField HeaderText="Client">
				<ItemTemplate>
					<div id="_clientNameBox" runat="server">
						(hover for details)
					</div>
				</ItemTemplate>
			</asp:TemplateField>
		</Columns>
	</asp:GridView>
	
	<h3 style="margin-top: 20px;">Transaction Logs</h3>
	<b id="_noTransactionLogs" runat="server" visible="false">* No transaction logs.</b>
	<asp:GridView
		OnRowCreated="TransactionLogRowCreatedHandler" 
		SkinID="LightContained"
		ID="_transactionLogs"
		AutoGenerateColumns="false"
		runat="server">
		<Columns>
			<asp:TemplateField HeaderText="When" ItemStyle-VerticalAlign="Top">
				<ItemTemplate>
					<asp:Literal ID="_when" runat="server" />
				</ItemTemplate>
			</asp:TemplateField>
			<asp:TemplateField HeaderText="Type" ItemStyle-VerticalAlign="Top">
				<ItemTemplate>
					<asp:Literal ID="_type" runat="server" />
				</ItemTemplate>
			</asp:TemplateField>
			<asp:TemplateField HeaderText="Google Order Number" ItemStyle-VerticalAlign="Top">
				<ItemTemplate>
					<asp:Literal ID="_googleOrderNumber" runat="server" Text="-" />
				</ItemTemplate>
			</asp:TemplateField>
			<asp:TemplateField HeaderText="Content">
				<ItemTemplate>
					<asp:Literal ID="_content" runat="server" />
				</ItemTemplate>
			</asp:TemplateField>
		</Columns>
	</asp:GridView>
	
</asp:Content>