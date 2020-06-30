<%@ Page Title="" Language="C#" MasterPageFile="~/_masterPages/Regular.master" AutoEventWireup="true" CodeFile="License.aspx.cs" Inherits="PhotosLicensePage" EnableViewState="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" Runat="Server" />
<asp:Content ID="Content2" ContentPlaceHolderID="PageHeading" Runat="Server">
	<span class="Faint">License:</span> <asp:Literal ID="_licenseTitle" runat="server" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="PageContentArea" Runat="Server">
	<div class="ExplanationBox" style="margin-bottom: 10px;">
		When you buy a photo from us, you're actually buying a license to use the photo, not the photo itself. The terms of this license are outlined below 
		and will define how you are permitted (and perhaps not permitted) to use the photograph by your agreement to our terms & conditions at the point of sale. 
		At all times after purchase you are held to the terms of this license. If you break these terms, you may well be the subject of legal action.
	</div>
	<h3>License Details</h3>
	<div class="LineSpaced">
	    <asp:Literal ID="_licenseBody" runat="server" />
	</div>
</asp:Content>