<%@ Page Language="C#" MasterPageFile="~/_masterPages/Regular.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Help_RegistrationTerms_Default" Title="Registration Terms - MP" EnableViewState="false" %>
<%@ Register src="~/_controls/RegistrationTerms.ascx" tagname="RegistrationTermsCtrl" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" Runat="Server" />
<asp:Content ID="Content2" ContentPlaceHolderID="PageHeading" Runat="Server">
    Registration Terms & Conditions
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="PageContentArea" Runat="Server">
    <div style="margin-bottom: 20px;" class="ExplanationBox">
        To make any purchases on Moto Professional, you need to have a membership account. To register with us you are required to accept these terms 
        and conditions.
    </div>
    <uc1:RegistrationTermsCtrl runat="server" />
</asp:Content>