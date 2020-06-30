<%@ Page Language="C#" MasterPageFile="~/_masterPages/Admin.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Admin.Sales.Customers.CustomersPage" Title="MPa: Customers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageHeading" Runat="Server">
    Customers
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PageContentArea" Runat="Server">
    
    <div class="FormBox" style="margin-bottom: 20px;">
		<table class="Spacer">
	        <tr>
	            <td>
	                View:
		            <asp:LinkButton ID="_viewNewestLink" runat="server" OnClick="PerformCustomerSearchByDate" Text="Newest Customers" />
	            </td>
	            <td align="right">
	                Search by name:
	                <asp:TextBox ID="_name" runat="server" style="margin-right: 10px;" />
	                Search by email:
	                <asp:TextBox ID="_email" runat="server" style="margin-right: 10px;" />
	                <asp:Button ID="_customerSearchBtn" runat="server" OnClick="PerformCustomerSearch" Text="Search" />
	            </td>
	        </tr>
	    </table>
	</div>
	
	<asp:GridView 
		ID="_customerGrid"
		SkinID="LightContained"
		AutoGenerateColumns="false"
		runat="server">
		<Columns>
		    <asp:BoundField DataField="UserName" HeaderText="Username" />
		    <asp:TemplateField HeaderText="Email">
                <ItemTemplate>
                    <asp:HyperLink runat="server" Text='<%# Eval("Email") %>' NavigateUrl='<%# Eval("Email", "mailto:{0}") %>' />
                </ItemTemplate>
            </asp:TemplateField>
		    <asp:BoundField DataField="CreationDate" HeaderText="Registered" DataFormatString="{0:dd MMMM \'yy}" />
		    <asp:BoundField DataField="LastLoginDate" HeaderText="Last Sign-in" DataFormatString="{0:dd MMMM \'yy - HH:mm}" />
		    <asp:TemplateField HeaderText="Online?">
		        <ItemTemplate>
		            <%# GridIsOnline(Eval("IsOnline")) %>
		        </ItemTemplate>
		    </asp:TemplateField>
		    <asp:TemplateField HeaderText="Approved?">
		        <ItemTemplate>
		            <%# ((bool)Eval("IsApproved")) ? "Yes" : "<b>*No*</b>" %>
		        </ItemTemplate>
		    </asp:TemplateField>
		    <asp:TemplateField ItemStyle-Font-Bold="true" ItemStyle-ForeColor="Red">
		        <ItemTemplate>
		            <%# GridIsLockedOut(Eval("IsLockedOut")) %>
		        </ItemTemplate>
		    </asp:TemplateField>
		    <asp:TemplateField ItemStyle-Width="115">
		        <ItemTemplate>
					<div class="LinkBox">
						<a href="customer.aspx?uid=<%# Eval("ProviderUserKey") %>">view profile</a> &raquo;
					</div>
		        </ItemTemplate>
		    </asp:TemplateField>
        </Columns>
	</asp:GridView>
    
</asp:Content>