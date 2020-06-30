<%@ Page Title="Password Reset - MP" Language="C#" MasterPageFile="~/_masterPages/Regular.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="ResetPasswordPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" Runat="Server" />
<asp:Content ID="Content2" ContentPlaceHolderID="PageHeading" Runat="Server">
	Password Reset
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="PageContentArea" Runat="Server">
	<div class="ExplanationBox" style="margin-bottom: 10px;">
		If you've lost your password, you can request a new to be emailed out to you. Once you're signed-in again, you can change your password to one of your choice.
		We'll only send the new password out to the email address we have on record for you.
	</div>
	<asp:PasswordRecovery 
		style="margin: 0px auto;"
		ID="_passwordRecovery" 
		runat="server" 
		TitleTextStyle-Height="30" 
		TitleTextStyle-CssClass="Standout">
		<MailDefinition 
			IsBodyHtml="false" 
			Subject="Your New Moto Professional Password" 
			BodyFileName="~/_system/email/passwordreset.txt" />
	</asp:PasswordRecovery>
</asp:Content>