<%@ Page Language="C#" MasterPageFile="~/_masterPages/Regular.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Account_Partner_Default" Title="MP: Partner" %>

<asp:Content ContentPlaceHolderID="PageHeading" runat="server">
    Partner Control Panel
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="PageContentArea" Runat="Server">
    
    <div style="margin-bottom: 10px;" class="Faint">
        More:
        <img src="../../_images/arrow-right.png" style="margin-bottom: 2px;" alt="go" />
        <a href="collections/" class="FaintU">Collections</a> - 
        <a href="rates/" class="FaintU">Rates</a> 
        <!-- - <a href="reports/" class="FaintU">Reports</a>-->
    </div>
    
    Choose an option from above.
    
</asp:Content>