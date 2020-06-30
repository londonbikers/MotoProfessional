<%@ Control Language="C#" AutoEventWireup="true" CodeFile="IncomingFolderView.ascx.cs" Inherits="IncomingFolderView" %>

<div class="Highlight" id="_nothingFound" runat="server">
	No photos found!
</div>

<div class="Highlight" id="_mediaFound" runat="server">
	<b><asp:Literal ID="_photoCount" runat="server" /></b> Photos Found!
	<div>
		Totalling <asp:Literal ID="_photoTotalSize" runat="server" />
	</div>
</div>