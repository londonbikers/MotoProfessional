<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SearchResultsBarCtrl.ascx.cs" Inherits="_controls.SearchResultsBarCtrl" %>
<script type="text/javascript">
    $(document).ready(function() {
        $("#searchBoxToggle").click(function() {
            $("#seachBoxHandle").slideToggle("fast");
            return false;
        });
    });
</script>
<div class="Faint" style="margin-bottom: 5px; text-align: center;">
    <div class="CollapsableHeader" style="border-bottom: none;">
        <table class="Spacer" cellspacing="0">
            <tr>
                <td>
                    <h4 style="margin: 0px; display: inline; margin-right: 5px;"><asp:Literal ID="_term" runat="server" /> Search Results</h4> 
                    - (<asp:HyperLink ID="_back" runat="server" Text="back to search" Target="_top" CssClass="Control" />)
                </td>
                <td align="right">
                    <asp:LinkButton ID="LinkButton1" Text="finish search" runat="server" CssClass="Control" OnClick="CancelSearchHandler" /> | <a href="#" id="searchBoxToggle" class="Control">show/hide</a>
                    <img src="<%= Page.ResolveUrl("~/_images/silk/bullet_arrow_down.png") %>" alt="toggle" />
                </td>
            </tr>
        </table>
	</div>
	<div id="seachBoxHandle" style="border-left: dotted 1px #333; border-right: dotted 1px #333; border-top: dotted 1px #333;">
	    <iframe id="_iframe" runat="server" frameborder="0" width="100%" height="120"></iframe>
	</div>
	<img src="<%= Page.ResolveUrl("~/_images/layout/horizontal-div.gif") %>" style="clear: both; display: block; width: 100%; height: 26px;" alt="div" />
</div>