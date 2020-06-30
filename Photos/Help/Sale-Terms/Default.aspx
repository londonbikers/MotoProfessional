<%@ Page Language="C#" MasterPageFile="~/_masterPages/Regular.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Help_SaleTerms_Default" Title="Sale Terms - MP" EnableViewState="false" %>
<%@ Register src="~/_controls/SaleTerms.ascx" tagname="SaleTermsCtrl" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" Runat="Server" />
<asp:Content ID="Content2" ContentPlaceHolderID="PageHeading" Runat="Server">
    Sale Terms & Conditions
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="PageContentArea" Runat="Server">
    <div style="margin-bottom: 20px;" class="ExplanationBox">
        To make any purchases on Moto Professional, you must first agree to these purchase/license terms and conditions. Failure to do so means 
        the ability to purchase a license is not available. These terms define how the individual photograph licenses can be used.
    </div>
    <uc1:SaleTermsCtrl runat="server" />
</asp:Content>