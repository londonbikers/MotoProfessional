<%@ Page Title="MP: Change your password" Language="C#" MasterPageFile="~/_masterPages/Regular.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Account_PasswordChange_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" Runat="Server" />
<asp:Content ID="Content2" ContentPlaceHolderID="PageHeading" Runat="Server">
	Change Your Password
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="PageContentArea" Runat="Server">
	<div class="ExplanationBox" style="margin-bottom: 10px">
		You can change your Moto Professional password below.
	</div>
	<asp:ChangePassword
		OnChangedPassword="ContinueHandler"
		SuccessPageUrl="~/account/"
		ID="_changePasswordControl" 
		runat="server" 
		style="margin: 0px auto;"
		TitleTextStyle-Height="30" 
		TitleTextStyle-CssClass="Standout" />
</asp:Content>