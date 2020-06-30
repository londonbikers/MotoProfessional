<%@ Page Language="C#" MasterPageFile="~/_masterPages/Admin.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Admin.Sales.Partners.PartnersPage" Title="MPa: Partners" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageHeading" Runat="Server">
    Partners
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PageContentArea" Runat="Server">
    <div class="ExplanationBox" style="margin-bottom: 10px;">
		Here's a list of all the partners in the system.
    </div>
    <div style="margin-bottom: 20px; padding-bottom: 10px;" class="DottedBottom">
		<span class="Faint">Options:</span> <img src="../../../_images/silk/group_add.png" alt="create a partner" /> <a href="partner.aspx">Create a Partner</a>
	</div>
    
	<asp:GridView
        OnRowCreated="PartnerRowCreatedHandler" 
        SkinID="LightContained"
        ID="_partners"
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
	        <asp:TemplateField HeaderText="Company">
		        <ItemTemplate>
		            <asp:HyperLink ID="_company" runat="server" Text="view company" />
		        </ItemTemplate>
	        </asp:TemplateField>
	        <asp:TemplateField HeaderText="Country">
		        <ItemTemplate>
		            <asp:Image ID="_countryIcon" runat="server" />
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
	        <asp:TemplateField ItemStyle-HorizontalAlign="Right" ItemStyle-Width="120">
		        <ItemTemplate>
					<div class="LinkBox">
						<asp:HyperLink ID="_link" runat="server" Text="view partner" /> &raquo;
					</div>
		        </ItemTemplate>
	        </asp:TemplateField>
        </Columns>
    </asp:GridView>
</asp:Content>