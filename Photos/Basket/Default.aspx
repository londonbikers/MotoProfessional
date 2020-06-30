<%@ Page Language="C#" MasterPageFile="~/_masterPages/Regular.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="BasketPage" Title="Your Basket - MP" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" Runat="Server" />
<asp:Content ID="Content2" ContentPlaceHolderID="PageHeading" Runat="Server">
	Your Basket
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="PageContentArea" Runat="Server">
	
	<div class="ExplanationBox" style="margin-bottom: 20px;">
		Here's the contents of your shopping basket. You can add or remove items, or just go straight on to the checkout to make your purchases.
	</div>
	
	<div style="text-align: center;" id="_noItemsDiv" runat="server" visible="false">
	    <h4>* No Items!</h4>
	    <a href="<%= Page.ResolveUrl("~/photos/") %>">(browse the photos)</a>
	</div>
	
	<asp:PlaceHolder ID="_basketControlsView" runat="server">
		<div style="float: left;">
			<img src="<%= Page.ResolveUrl("~/_images/fugue/cross.png") %>" alt="clear basket" />
			<asp:LinkButton runat="server" text="Empty Basket" CssClass="FaintU" ToolTip="Empty your basket completely (hides it from view as well)" OnClick="EmptyBasketHandler" />
		</div>
		<div style="text-align: right; margin-bottom: 10px;">
			<a href="../checkout/" class="BigHeading" title="pay and download...">Go to Checkout &raquo;</a>
		</div>
		<div class="Clear" />
	</asp:PlaceHolder>	
	
	<asp:Repeater
		id="_basketContents"
		runat="server"
		OnItemCreated="ListItemCreatedHandler"
		OnItemCommand="ListCommandHandler">
		<HeaderTemplate>
			<hr />
			<table cellspacing="0" width="100%" cellpadding="0" style="margin-top: 10px;">
		</HeaderTemplate>
		<ItemTemplate>
				<tr>
					<td style="width: 125px;">
						<div class="SimpleFrame" style="width: 100px;">
							<asp:HyperLink ID="_thumbLink" runat="server" ToolTip="Return to photo" />
						</div>
					</td>
					<td class="LineSpaced">
						<h3><asp:Literal ID="_photoName" runat="server" /></h3>
						<asp:Label CssClass="Faint" ID="_photoDescription" runat="server" style="display: block;" />
						License: <asp:Literal ID="_licenseName" runat="server" />
						<div title="The size (along the longest side) of the photo you will download when you purchase this photo/license." class="LightFaint">
							Download Size: <asp:Literal id="_dimensions" runat="server" />.
						</div>
						<div class="Faint">
						    <asp:Literal ID="_licenseDescription" runat="server"/>
						</div>
						<span class="Faint">
						    <asp:HyperLink ID="_licenseLink" runat="server" Text="See full license" class="H5Faint" style="text-decoration: underline;" /> &raquo;
						</span>
						
					</td>
					<td class="Standout" style="padding-right: 10px;">
						£<asp:Literal ID="_rate" runat="server" />
					</td>
					<td style="border-left: dotted 1px #333; padding: 0px 10px 0px 10px; text-align: center;" class="LineSpaced">
						Qty.<br />
						<asp:Literal ID="_quantity" runat="server" />
					</td>
					<td class="Faint" style="width: 75px; border-left: dotted 1px #333; padding-left: 10px;">
						<asp:ImageButton ID="_removeBtn" runat="server" AlternateText="remove item" ImageUrl="~/_images/silk/cross.png" CommandName="remove" CommandArgument='<%# Eval("ID") %>' />
						Remove
					</td>
				</tr>
		</ItemTemplate>
		<SeparatorTemplate>
			<tr>
				<td colspan="5">
					<hr style="margin: 10px 0px 10px 0px;"/>
				</td>
			</tr>
		</SeparatorTemplate>
		<FooterTemplate>
				<tr>
					<td colspan="5">
						<hr style="margin: 10px 0px 10px 0px;"/>
					</td>
				</tr>
				<tr>
					<td colspan="2" align="right" style="padding-right: 10px;">
						<h4 style="margin: 0px;">Basket Total:</h4>
					</td>
					<td>
						<h4 style="margin: 0px;">£<asp:Literal ID="_totalValue" runat="server" /></h4>
					</td>
					<td align="right" colspan="2">
					    <a href="../checkout/" class="BigHeading" title="pay and download...">Go to Checkout &raquo;</a>
					</td>
				</tr>
			</table>
		</FooterTemplate>
	</asp:Repeater>
	
</asp:Content>