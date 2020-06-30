<%@ Control Language="C#" AutoEventWireup="true" CodeFile="BasketView.ascx.cs" Inherits="BasketView" %>
<div class="Faint" style="margin-bottom: 5px; text-align: center;">
    <div class="CollapsableHeader" style="border-bottom: none;">
        <table class="Spacer" cellspacing="0">
            <tr>
                <td>
                    <img src="<%= Page.ResolveUrl("~/_images/silk/basket.png") %>" alt="basket" />
                    <h4 style="margin-right: 5px; vertical-align: middle; display: inline;">Your Basket (£<asp:Literal ID="_basketTotal" runat="server" />)</h4>
                </td>
                <td align="right" style="vertical-align: middle;">
                    <img src="<%= Page.ResolveUrl("~/_images/arrow-left.png") %>" alt="go" />
                    <a href="<%= Page.ResolveUrl("~/basket/") %>" class="BigFaint">view basket</a> |
                    <a href="<%= Page.ResolveUrl("~/checkout/") %>" class="Big" title="Pay & Download">go to checkout</a>
                    <img src="<%= Page.ResolveUrl("~/_images/arrow-right.png") %>" alt="go" />
                </td>
            </tr>
        </table>
	</div>
	<div id="basketHandle" style="border-left: dotted 1px #333; border-right: dotted 1px #333; border-top: dotted 1px #333;">
	    <iframe frameborder="0" width="100%" height="137" src="<%= Page.ResolveUrl("~/_controls/BasketViewContent.aspx") %>?ni=<%= NewPhotoID %>&i=<%= CurrentPhotoID %>"></iframe>
	</div>
	<img src="<%= Page.ResolveUrl("~/_images/layout/horizontal-div.gif") %>" style="clear: both; display: block; width: 100%; height: 26px;" alt="div" />
</div>