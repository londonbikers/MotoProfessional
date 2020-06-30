<%@ Page Language="C#" MasterPageFile="~/_masterPages/Admin.master" AutoEventWireup="true" CodeFile="Stats.aspx.cs" Inherits="AdminStatsPage" Title="MPa: Basic Stats" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageHeading" Runat="Server">
	Basic Statistics
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContentArea" Runat="Server">

	<div class="ExplanationBox">
		Here's some basic statistics for the service. This is privledged information; do not share.
	</div>
	
	<div class="FormBox" style="margin-bottom: 10px; margin-top: 5px;">
        <img src="../../_images/silk/database_refresh.png" alt="clear cache" />
        <asp:LinkButton runat="server" Text="Refresh Statistics" OnClick="RefreshStatsHandler" />
        <hr />
        <table class="Spacer">
			<tr>
				<td class="Field" style="width: 230px;">
					Number of Partners:
				</td>
				<td class="Control">
					<asp:Literal ID="_partners" runat="server" />
				</td>
			</tr>
			<tr>
				<td class="Field Faint">
					Number of Collections:
				</td>
				<td class="Control Faint">
					<asp:Literal ID="_collections" runat="server" />
				</td>
			</tr>
			<tr>
				<td class="Field">
					Number of Photos:
				</td>
				<td class="Control">
					<asp:Literal ID="_photos" runat="server" />
				</td>
			</tr>
			<tr>
				<td class="Field">
					Size of Photo Archive:
				</td>
				<td class="Control">
					<asp:Literal ID="_photoArchiveSize" runat="server" /> gb
				</td>
			</tr>
			<tr>
				<td class="Field Faint">
					Number of Members:
				</td>
				<td class="Control Faint">
					<asp:Literal ID="_members" runat="server" />
				</td>
			</tr>
			<tr>
				<td class="Field">
					Number of Countries Represented:
				</td>
				<td class="Control">
					<asp:Literal ID="_countriesRepresented" runat="server" />
				</td>
			</tr>
			<tr>
				<td class="Field Faint">
					Number of Baskets:
				</td>
				<td class="Control Faint">
					<asp:Literal ID="_baskets" runat="server" />
				</td>
			</tr>
			<tr>
				<td class="Field">
					Number of Complete Orders:
				</td>
				<td class="Control">
					<asp:Literal ID="_completeOrders" runat="server" />
				</td>
			</tr>
			<tr>
				<td class="Field Faint">
					Number of Incomplete Orders:
				</td>
				<td class="Control Faint">
					<asp:Literal ID="_incompleteOrders" runat="server" />
				</td>
			</tr>
			<tr>
				<td class="Field">
					Number of Downloads:
				</td>
				<td class="Control">
					<asp:Literal ID="_downloads" runat="server" />
				</td>
			</tr>
			<tr>
				<td class="Field Faint">
					Number of Unique Tags:
				</td>
				<td class="Control Faint">
					<asp:Literal ID="_tags" runat="server" />
				</td>
			</tr>
        </table>
	</div>
	
</asp:Content>