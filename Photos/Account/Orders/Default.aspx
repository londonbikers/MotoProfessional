<%@ Page Language="C#" MasterPageFile="~/_masterPages/Regular.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Account.Orders.OrdersPage" Title="MP: Your Orders" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" Runat="Server" />
<asp:Content ID="Content2" ContentPlaceHolderID="PageHeading" Runat="Server">
	Your Orders
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="PageContentArea" Runat="Server">

	<asp:PlaceHolder ID="_newOrdersView" runat="server">
	    <h3>Recent Orders</h3>
	    <div class="ExplanationBox" style="margin-bottom: 10px;">
		    Here's a list of your recent orders. You can download any digital media through the orders as well.
	    </div>
	    
	    <h4 id="_noOrders" runat="server" visible="false">You haven't bought anything from us yet. Please do!</h4>
	    
	    <asp:GridView
            OnRowCreated="OrderRowCreatedHandler" 
	        SkinID="LightContained"
	        ID="_orders"
	        AutoGenerateColumns="false"
	        runat="server">
	        <Columns>
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
		        <asp:TemplateField ItemStyle-HorizontalAlign="Right">
			        <ItemTemplate>
			            <b><asp:HyperLink ID="_link" runat="server" Text="view order" /> &raquo;</b>
			        </ItemTemplate>
		        </asp:TemplateField>
	        </Columns>
        </asp:GridView>
	    
    </asp:PlaceHolder>

</asp:Content>