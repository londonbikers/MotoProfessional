<%@ Page Language="C#" MasterPageFile="~/_masterPages/Regular.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Calendar_Default" Title="Calendar - MP" EnableViewState="false" %>

<asp:Content ID="Content2" ContentPlaceHolderID="PageHeading" runat="server">
	Calendar
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="PageContentArea" Runat="Server">
    <p class="LineSpaced" style="margin-bottom: 20px;">
        Here's our motorsport calendar. We're accredited media with the BSB, WSB, MotoGP, British MX and World MX series' and at all UK events, but 
        <a href="../contact/">speak to us</a> if you need us to be somewhere for certain. Chances are we have plans or can accomodate you.
    </p>
    
    <div style="padding: 10px; background-color: White; border: solid 6px #ccc;">
        <iframe src="http://www.google.com/calendar/embed?height=700&amp;wkst=1&amp;bgcolor=%23ffffff&amp;src=gmas606di9scj0i48vp29d9je0%40group.calendar.google.com&amp;color=%23B1365F&amp;ctz=Europe%2FLondon" style=" border-width:0 " width="915" height="700" frameborder="0" scrolling="no"></iframe>
    </div>
</asp:Content>