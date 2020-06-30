<%@ Page Language="C#" MasterPageFile="~/_masterPages/Admin.master" AutoEventWireup="true" CodeFile="EmailTester.aspx.cs" Inherits="AdminEmailTesterPage" Title="MPa: Email Tester" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageHeading" Runat="Server">
	Email Tester
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContentArea" Runat="Server">
	<div class="ExplanationBox" style="margin-bottom: 10px;">
		Use this form to test outgoing email functionality. It can be used to test whether or not there is a problem with the configured
		SMTP server for example.
	</div>
	
	<h3>Configured Details</h3>
	<table class="Form" cellspacing="0">
		<tr>
			<td class="Faint" style="padding-right: 20px;">
				MediaPanther.Framework.Email.TemplatePath:
			</td>
			<td>
				<asp:Literal ID="_templatePath" runat="server" />
			</td>
		</tr>
		<tr>
			<td class="Faint" style="padding-right: 20px;">
				MediaPanther.Framework.Email.TemplatePathIsWebPath:
			</td>
			<td>
				<asp:Literal ID="_templatePathIsWebPath" runat="server" />
			</td>
		</tr>
		<tr>
			<td class="Faint" style="padding-right: 20px;">
				MediaPanther.Framework.Email.FromAddress:
			</td>
			<td>
				<asp:Literal ID="_fromAddress" runat="server" />
			</td>
		</tr>
		<tr>
			<td class="Faint" style="padding-right: 20px;">
				MediaPanther.Framework.Email.SmtpServer:
			</td>
			<td>
				<asp:Literal ID="_smtpServer" runat="server" />
			</td>
		</tr>
	</table>
	
	<h3 style="margin-top: 20px;">Send an Email</h3>
	<table class="MediumForm" cellspacing="0">
		<tr>
			<td class="Faint" style="padding-right: 20px;">
				Recipient:
			</td>
			<td>
				<asp:TextBox ID="_sendRecipient" runat="server" />
			</td>
		</tr>
		<tr>
			<td class="Faint" style="padding-right: 20px; vertical-align: top;">
				Custom Message:
			</td>
			<td>
				<asp:TextBox TextMode="MultiLine" Height="75px" ID="_sendFormCustomMessage" runat="server" />
			</td>
		</tr>
		<tr>
			<td>
				&nbsp;
			</td>
			<td>
				<asp:Button Text="Send Email" runat="server" OnClick="SendEmailHandler" />
			</td>
		</tr>
	</table>
	
</asp:Content>