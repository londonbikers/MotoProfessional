<%@ Page Language="C#" MasterPageFile="~/_masterPages/Admin.master" AutoEventWireup="true" CodeFile="Partner.aspx.cs" Inherits="Admin.Sales.Partners.PartnerPage" Title="MPa: Partner" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" Runat="Server" />
<asp:Content ID="Content2" ContentPlaceHolderID="PageHeading" Runat="Server">
	<span class="Faint">Partner:</span> <asp:Literal ID="_pageHeading" runat="server" Text="New Partner" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="PageContentArea" Runat="Server">

	<div class="GridViewContainer" id="_findCompanyDiv" runat="server">
		<h4>Find a company</h4>
		A Partner needs to be based upon an existing company. If they're not registered, ask them to register first.
		<div style="margin-top: 5px;">
			<asp:TextBox ID="_companySearchBox" runat="server" style="width: 250px;" />
			<asp:Button Text="Find Company" OnClick="FindCompanyHandler" runat="server" />
		</div>
		<asp:PlaceHolder ID="_companySelectionPlaceHolder" runat="server" Visible="false">
			<div style="margin-top: 10px;" class="Faint">
				<asp:DropDownList ID="_companySelection" runat="server" DataTextField="Name" DataValueField="ID" /> &laquo; Choose
			</div>
		</asp:PlaceHolder>
	</div>
	
	<div class="ExplanationBox" style="margin-top: 10px;" id="_commonPropertiesDiv" runat="server">
		<h4>Partner Details</h4>		
		<table class="Form MediumForm">
			<tr>
				<td>
					Company:
				</td>
				<td>
					<asp:HyperLink ID="_companyLink" runat="server" />
					<asp:Literal ID="_noCompanyLabel" runat="server">Choose a company first.</asp:Literal>
				</td>
				<td rowspan="5" style="padding-left: 20px;">
					Logo:
				</td>
				<td rowspan="5">
					<asp:Label ID="_uploadLabel" runat="server" CssClass="SoftHighlight">
						Create the partner first before uploading a logo.
					</asp:Label>
					<asp:PlaceHolder ID="_uploadPlaceHolder" runat="server">
						<div class="UploadBox">
							<asp:FileUpload ID="_logoUploader" runat="server" />
						</div>
					</asp:PlaceHolder>
					<div style="margin-top: 10px;" id="_logoFrame" visible="false" runat="server"><asp:Image ID="_logo" runat="server" tooltip="Partner Logo" /></div>
					<div id="_deleteLinkDiv" runat="server" visible="false" style="margin-top: 5px;">
						<img src="../../../_images/Silk/delete.png" alt="Delete logo..." />
						<asp:LinkButton 
							Text="Delete Logo" 
							ToolTip="Deletes the current logo, so there will be none." 
							OnClick="DeleteLogoHandler" 
							runat="server"
							OnClientClick="return confirm('Are you sure you want to delete this logo?');" />
					</div>
				</td>
			</tr>
			<tr id="_statusRow" runat="server" visible="false">
				<td>Status:</td>
				<td><asp:DropDownList ID="_status" runat="server" /></td>
			</tr>
			<tr>
				<td>Name:</td>
				<td>
					<asp:TextBox ID="_name" runat="server" />
					<asp:RequiredFieldValidator 
						runat="server" 
						ValidationGroup="Partner" 
						Text="*" 
						Display="Dynamic" 
						EnableClientScript="true" 
						ErrorMessage="Name is required." 
						ControlToValidate="_name" />
				</td>
			</tr>
			<tr>
				<td>Description:</td>
				<td>
					<asp:TextBox TextMode="MultiLine" ID="_description" runat="server" />
					<asp:RequiredFieldValidator
						runat="server" 
						ValidationGroup="Partner" 
						Text="*" 
						Display="Dynamic" 
						EnableClientScript="true" 
						ErrorMessage="Description is required." 
						ControlToValidate="_description" />
				</td>
			</tr>
			<tr>
				<td>&nbsp;</td>
				<td>
					<asp:Button ID="_udpateBtn" runat="server" OnClick="UpdatePartnerHandler" ValidationGroup="Partner" />
					<asp:ValidationSummary runat="server" DisplayMode="BulletList" ValidationGroup="Partner" style="margin-top: 10px;"/>
				</td>
			</tr>
		</table>
	</div>
	
	<div class="ExplanationBox" style="margin-top: 10px;">
	    <h4>Statistics</h4>
	    
	    <table cellpadding="0" cellspacing="0">
            <tr>
                <td class="Faint" style="padding-right: 10px;">Joined:</td>
                <td><asp:Literal ID="_joined" runat="server" /></td>
            </tr>
            <tr>
                <td class="Faint">Collections:</td>
                <td><asp:Literal ID="_collectionCount" runat="server" /></td>
            </tr>
            <tr>
                <td class="Faint" style="padding-right: 10px;">Photographs:</td>
                <td><asp:Literal ID="_photoCount" runat="server" /></td>
            </tr>
            <tr>
                <td class="Faint" style="padding-right: 10px;">Photos Sold:</td>
                <td><asp:Literal ID="_photosSold" runat="server" /></td>
            </tr>
            <tr>
                <td class="Faint" style="padding-right: 10px;">Total Sales:</td>
                <td>£ <asp:Literal ID="_totalSales" runat="server" /></td>
            </tr>
        </table>
	    
	</div>
	
</asp:Content>