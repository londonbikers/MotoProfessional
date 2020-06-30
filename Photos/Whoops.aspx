<%@ Page Language="C#" MasterPageFile="~/_masterPages/Regular.master" AutoEventWireup="true" CodeFile="Whoops.aspx.cs" Inherits="Whoops" EnableViewState="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" Runat="Server" />
<asp:Content ID="Content2" ContentPlaceHolderID="PageHeading" Runat="Server">
	Whoops!
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="PageContentArea" Runat="Server">
	<div class="ExplanationBox">
		Unfortunately there was a problem at our end showing you what you wanted. The staff have been notified of this problem and will endevour to fix
		it as soon as possible. You can try refreshing the page to see if it will work now.
	</div>
	<div class="Highlight" id="_detailBox" runat="server" visible="false" style="margin-top: 20px; overflow: scroll;">
	    <h3>Exception Details</h3>
		<pre><asp:Literal ID="_exception" runat="server" /></pre>
	</div>
</asp:Content>