<%@ Page Language="C#" MasterPageFile="~/_masterPages/Regular.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="CheckoutPage" Title="Checkout - MP" %>
<%@ Register src="~/_controls/SaleTerms.ascx" tagname="SaleTermsCtrl" tagprefix="uc1" %>
<%@ Register TagPrefix="cc1" Namespace="GCheckout.Checkout" Assembly="GCheckout" %>
<%@ Import Namespace="GCheckout.Checkout" %>
<%@ Import Namespace="GCheckout.Util" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" Runat="Server">
	<script src="http://checkout.google.com/files/digital/ga_post.js" type="text/javascript"></script>
    <script type="text/javascript">
        <asp:PlaceHolder id="_blockJS" runat="server">
            $(document).ready(function() {
                $.blockUI({  
                    message: $('#saleTermsBox'),  
                    css: { margin: '0px 0px 0px -8%', border: '3px solid #00a4f1', padding: '10px', width: '572px', backgroundColor: '#000', color: '#acacac', cursor: 'default' }  
                }); 
            });
        </asp:PlaceHolder>
        $(function() {
            $('#agreeTermsBtn').click(function() { 
                $.unblockUI(); 
            }); 
        });
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PageHeading" Runat="Server">
	Checkout
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="PageContentArea" Runat="Server">
	<div class="ExplanationBox" style="margin-bottom: 10px;">
		All payments are handled securely by Google Checkout&trade;, no payment details are shared with us so your details are always safe. 
		You can pay by credit/debit-card. Whilst you don't need a Google Account to pay, if you have one, you can make express purchases
		when you re-visit us.
	</div>
	<a href="../basket/" class="BigHeading">&laquo; Back to Basket</a> <span class="Faint">- make changes</span>
	<hr />
	<div style="padding: 20px 0px 10px 0px; margin: 0px auto; text-align: center;" id="checkoutControls">
		<asp:PlaceHolder ID="_pointOfSaleHolder" runat="server" Visible="false">
			<div id="_checkoutPrompt" runat="server" visible="false" class="Highlight" style="margin-bottom: 20px;" />
			<div style="margin-bottom: 10px;" class="Faint">
				Click below to complete your purchase.
			</div>
			<input type="hidden" name="analyticsdata" value="" />
			<cc1:GCheckoutButton 
				OnClientClick="setUrchinInputCode(pageTracker)" 
				AlternateText="click to pay" 
				ID="_checkoutBtn" 
				OnClick="PostCartToGoogleHandler" 
				runat="server" 
				Background="Transparent" />
		</asp:PlaceHolder>
		<asp:PlaceHolder ID="_noChargeHolder" runat="server" Visible="false">
			<div class="LightFaint" style="margin-bottom: 5px;">
				As a privileged organisation, you can download your order without charge.
			</div>
			<div style="margin-bottom: 10px;" class="Faint">
				Click below to complete your order.
			</div>
			<asp:Button SkinID="Big" OnClick="NoChargeCompleteOrderHandler" Text="Download Order" runat="server" ToolTip="Click to complete your order and download your photos." />
		</asp:PlaceHolder>
	</div>
	<hr />
	<h3>Order Summary</h3>
	<div class="Faint">
		Here's a quick summary of your order.
	</div>
	<asp:Literal ID="_noItems" runat="server" Visible="false">
        <h4 style="margin: 10px 0px 0px 0px;">* No Items!</h4>
    </asp:Literal>
	<asp:GridView
		ShowFooter="true" 
		style="margin-top: 10px;"
        OnRowCreated="SummaryRowCreatedHandler" 
        ID="_summaryGrid"
        AutoGenerateColumns="false"
        runat="server">
        <Columns>
	        <asp:TemplateField HeaderText="Item">
		        <ItemTemplate>
					<img src="<%= Page.ResolveUrl("~/_images/silk/picture.png") %>" alt="Photo" /> <asp:Literal ID="_name" runat="server" />
		        </ItemTemplate>
	        </asp:TemplateField>
	        <asp:TemplateField HeaderText="License">
		        <ItemTemplate>
		            <asp:Literal ID="_license" runat="server" />
		            <span class="Faint">- <asp:HyperLink ID="_licenseLink" runat="server" Text="See full license" class="FaintU" style="text-decoration: underline;" /> &raquo;</span>
		        </ItemTemplate>
	        </asp:TemplateField>
	        <asp:TemplateField HeaderText="Qty">
		        <ItemTemplate>
		            <asp:Literal ID="_quantity" runat="server" />
		        </ItemTemplate>
	        </asp:TemplateField>
	        <asp:TemplateField ItemStyle-CssClass="HighlightCell" HeaderText="Rate" ItemStyle-Width="100px">
		        <ItemTemplate>
		            £ <asp:Literal ID="_rate" runat="server" />
		        </ItemTemplate>
	        </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <div id="saleTermsBox" style="display: none;"> 
        <h4 style="margin-bottom: 5px;">You must agree to our sale terms & conditions first</h4>
        <span class="Faint">(<a href="../help/sale-terms/" class="FaintU" target="_blank" title="see the terms on a whole page">see bigger</a>)</span>
        <div class="LinedBox" style="text-align: left; height: 75px; width: 550px; overflow: auto; margin: 10px 0px 10px 0px;">
            <uc1:SaleTermsCtrl runat="server" />
        </div>
        <input type="button" id="agreeTermsBtn" value="I Agree" />
    </div>
</asp:Content>