<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SearchResultsBarContent.aspx.cs" Inherits="_controls.SearchResultsBarContent" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
	<head runat="server">
		<title>Search Results</title>
	</head>
	<body style="padding: 0px; margin: 0px; background-color: #000;">
	    <table cellspacing="0" cellpadding="0" style="margin: 0px auto;">
		    <tr>
		        <td align="center" style="text-align: center;">
		            <div style="margin: 5px 0px 5px 0px;">
		                <asp:PlaceHolder ID="_paginationView" runat="server">
	                        <img src="<%= Page.ResolveUrl("~/_images/arrow-left.png") %>" alt="previous" style="vertical-align: bottom;" />
	                        <asp:Literal ID="_paginationControls" runat="server"/>
	                        <img src="<%= Page.ResolveUrl("~/_images/arrow-right.png") %>" alt="next" style="vertical-align: bottom;" />
		                </asp:PlaceHolder>
		                <asp:PlaceHolder ID="_noPaginationView" runat="server" Visible="false">
                            <span class="Faint">showing all <asp:Literal ID="_noPaginationItemCount" runat="server" /> results</span>
		                </asp:PlaceHolder>
		            </div>
		            <asp:Repeater ID="_results" runat="server" OnItemCreated="ResultItemCreatedHandler">
			            <ItemTemplate><div class="PhotoTile"><div id="_frame" runat="server"><asp:Literal ID="_tilePhoto" runat="server" /></div></div></ItemTemplate>
		            </asp:Repeater>
		        </td>
		    </tr>
        </table>
	</body>
</html>