<%@ Page Language="C#" AutoEventWireup="true" CodeFile="BasketViewContent.aspx.cs" Inherits="_controls.BasketViewContent" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
	<head runat="server">
		<title>Basket Contents</title>
	</head>
	<body style="padding: 0px; margin: 0px; background-color: #000;" id="_body" runat="server">
		<form runat="server">
			<table cellspacing="0" cellpadding="0" style="margin: 0px auto;">
				<tr>
					<td align="center" style="text-align: center;">
						<div style="margin: 5px 0px 5px 0px;">
							<span class="Faint">Basket Contents (<asp:Literal ID="_itemCount" runat="server" /> items)</span>
						</div>
						<asp:PlaceHolder ID="_paginationView" runat="server">
							<div>
								<img src="<%= Page.ResolveUrl("~/_images/arrow-left.png") %>" alt="previous" style="vertical-align: bottom;" />
								<asp:Literal ID="_paginationControls" runat="server"/>
								<img src="<%= Page.ResolveUrl("~/_images/arrow-right.png") %>" alt="next" style="vertical-align: bottom;" />
							</div>
						</asp:PlaceHolder>
						<asp:Repeater ID="_items" runat="server" OnItemCreated="ItemsItemCreatedHandler">
							<ItemTemplate>
								<table cellpadding="0" cellspacing="0" style="float: left; margin: 5px;">
									<tr>
										<td>
											<div id="_frame" runat="server" style="float: left;">
												<asp:Literal ID="_tilePhoto" runat="server" />
											</div>
										</td>
										<td style="width: 100px; vertical-align: top; padding: 5px; border-top: dotted 1px #333; border-right: dotted 1px #333; border-bottom: dotted 1px #333; text-align: left;">
											<asp:Label CssClass="Faint" ID="_license" runat="server" />
											<div class="Standout" style="margin: 5px 0px 5px 0px;">£<asp:Literal ID="_rate" runat="server" /></div>
											<asp:ImageButton ID="_removeBtn" runat="server" ImageUrl="~/_images/silk/cross.png" ToolTip="remove item" OnCommand="RemoveItemHandler" CommandArgument='<%# Eval("ID") %>' />
										</td>
									</tr>
								</table>
							</ItemTemplate>
						</asp:Repeater>
					</td>
				</tr>
			</table>
        </form>
	</body>
</html>