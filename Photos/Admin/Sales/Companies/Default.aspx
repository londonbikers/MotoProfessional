<%@ Page Language="C#" MasterPageFile="~/_masterPages/Admin.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Admin.Sales.Companies.CompaniesPage" Title="MPa: Companies" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageHeading" Runat="Server">
    Companies
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PageContentArea" Runat="Server">
    <div class="ExplanationBox" style="margin-bottom: 20px;">
		Here's a list of the latest companies in the system. Alternatively you can search for a specific company by using the controls below.
    </div>
    
	<asp:GridView
        OnRowCreated="CompanyRowCreatedHandler" 
        SkinID="LightContained"
        ID="_companies"
        AutoGenerateColumns="false"
        runat="server">
        <Columns>
			<asp:TemplateField HeaderText="Name">
		        <ItemTemplate>
		            <asp:Literal ID="_name" runat="server" />
		        </ItemTemplate>
	        </asp:TemplateField>
	        <asp:TemplateField HeaderText="Website">
		        <ItemTemplate>
		            <asp:HyperLink ID="_website" runat="server" CssClass="Faint" Text="-" Enabled="false" Target="_blank" />
		        </ItemTemplate>
	        </asp:TemplateField>
	        <asp:TemplateField HeaderText="Payment Terms">
		        <ItemTemplate>
		            <asp:Literal ID="_chargeMethod" runat="server" />
		        </ItemTemplate>
	        </asp:TemplateField>
	        <asp:TemplateField HeaderText="Country">
		        <ItemTemplate>
		            <asp:Image ID="_countryIcon" runat="server" />
		        </ItemTemplate>
	        </asp:TemplateField>
	        <asp:TemplateField HeaderText="Partner?">
		        <ItemTemplate>
		            <asp:Literal ID="_partner" runat="server">
		                <span class="Faint">-</span>
		            </asp:Literal>
		        </ItemTemplate>
	        </asp:TemplateField>
	        <asp:TemplateField HeaderText="Staff">
		        <ItemTemplate>
		            <asp:Literal ID="_staff" runat="server" />
		        </ItemTemplate>
	        </asp:TemplateField>
	        <asp:TemplateField HeaderText="Status">
		        <ItemTemplate>
		            <asp:Literal ID="_status" runat="server" />
		        </ItemTemplate>
	        </asp:TemplateField>
	        <asp:TemplateField HeaderText="Joined">
		        <ItemTemplate>
		            <asp:Literal ID="_joined" runat="server" />
		        </ItemTemplate>
	        </asp:TemplateField>
	        <asp:TemplateField ItemStyle-HorizontalAlign="Right" ItemStyle-Width="125">
		        <ItemTemplate>
					<div class="LinkBox">
						<asp:HyperLink ID="_link" runat="server" Text="view company" /> &raquo;
		            </div>
		        </ItemTemplate>
	        </asp:TemplateField>
        </Columns>
    </asp:GridView>
    
</asp:Content>