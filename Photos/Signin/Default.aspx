<%@ Page Language="C#" MasterPageFile="~/_masterPages/Regular.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Signin_Default" Title="Sign-In - MP" %>

<asp:Content ID="Content2" ContentPlaceHolderID="PageHeading" runat="server">
	Sign-in!
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="PageContentArea" Runat="Server">
	
	<div class="ExplanationBox">
		If you're already registered with us you need to sign-in below to use the site properly. 
		If you're not registered, please <a href="../register/">register now</a> for free!
    </div>
    
    <asp:Login
		LoginButtonText="Sign-In" 
		TitleText="Sign-In" 
		RememberMeText="Remember me" 
		TitleTextStyle-Height="30" 
		TitleTextStyle-CssClass="Standout"
		PasswordRecoveryIconUrl="~/_images/fugue/question_frame.png" 
		PasswordRecoveryText=" Forgotten your password?" 
		PasswordRecoveryUrl="reset/" 
		style="margin-top: 10px; margin: 0px auto;" 
		ID="_login" 
		runat="server" 
		OnLoggedIn="OnLoginHandler" 
		RememberMeSet="true" />
		
</asp:Content>