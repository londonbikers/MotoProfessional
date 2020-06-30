<%@ Page Language="C#" MasterPageFile="~/_masterPages/Admin.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Admin.Sales.Orders.OrdersPage" Title="MPa: Orders" %>

<asp:Content ContentPlaceHolderID="Head" runat="server">
	<script type="text/javascript">
		$(document).ready(function() {
			$("#order-id").numeric();
		});
	</script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="PageHeading" Runat="Server">
    Orders
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PageContentArea" Runat="Server">
    <div class="ExplanationBox" style="margin-bottom: 20px;">
		Here's a list of the latest orders in the system. Alternatively you can search for a specific order by using the controls below.
    </div>
    
    <div class="GridViewContainer" style="margin-bottom: 20px;">
		Order ID: 
		<input type="text" id="order-id" class="SmallBox" />
		<input type="button" value="View" onclick="document.location='order.aspx?i=' + document.getElementById('order-id').value;" />
    </div>
    
	<asp:GridView
        OnRowCreated="OrderRowCreatedHandler" 
        SkinID="LightContained"
        ID="_orders"
        AutoGenerateColumns="false"
        runat="server">
        <Columns>
			<asp:BoundField DataFormatString="#{0}" DataField="ID" />
			<asp:TemplateField HeaderText="Customer">
		        <ItemTemplate>
		            <asp:HyperLink ID="_customerLink" runat="server" />
		        </ItemTemplate>
	        </asp:TemplateField>
	        <asp:TemplateField HeaderText="Company">
		        <ItemTemplate>
		            <asp:HyperLink ID="_companyLink" runat="server" CssClass="Faint" Text="-" />
		        </ItemTemplate>
	        </asp:TemplateField>
	        <asp:TemplateField HeaderText="Ordered">
		        <ItemTemplate>
		            <asp:Literal ID="_ordered" runat="server" />
		        </ItemTemplate>
	        </asp:TemplateField>
	        <asp:TemplateField HeaderText="Items">
		        <ItemTemplate>
		            <asp:Literal ID="_items" runat="server" />
		        </ItemTemplate>
	        </asp:TemplateField>
	        <asp:TemplateField HeaderText="Total Amount">
		        <ItemTemplate>
		            £ <asp:Literal ID="_total" runat="server" />
		        </ItemTemplate>
	        </asp:TemplateField>
	        <asp:TemplateField HeaderText="Charge Method">
		        <ItemTemplate>
		            <asp:Literal ID="_method" runat="server" />
		        </ItemTemplate>
	        </asp:TemplateField>
	        <asp:TemplateField ItemStyle-CssClass="HighlightCell" HeaderText="State">
		        <ItemTemplate>
		            <asp:Literal ID="_state" runat="server" />
		        </ItemTemplate>
	        </asp:TemplateField>
	        <asp:TemplateField ItemStyle-HorizontalAlign="Right" ItemStyle-Width="105">
		        <ItemTemplate>
					<div class="LinkBox">
						<asp:HyperLink ID="_link" runat="server" Text="view order" /> &raquo;
					</div>
		        </ItemTemplate>
	        </asp:TemplateField>
        </Columns>
    </asp:GridView>
    
</asp:Content>