<%@ Page Language="C#" MasterPageFile="~/_masterPages/Admin.master" AutoEventWireup="true" CodeFile="Company.aspx.cs" Inherits="Admin.Sales.Companies.CompanyPage" %>
<%@ Register src="~/_controls/CompanyDetailsEditor.ascx" tagname="CompanyEditor" tagprefix="uc1" %>

<asp:Content ContentPlaceHolderID="Head" runat="server">
	<script type="text/javascript">
		$(document).ready(function(){
			$("#EditPaymentTermsLnk").click(function () {
			  $("#EditPaymentTermsDiv").toggle("slow");
			});   
		});
	</script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="PageHeading" Runat="Server">
    <span class="Faint">Company:</span> <asp:Literal ID="_companyTitle" runat="server" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PageContentArea" Runat="Server">
    <div class="ExplanationBox" style="margin-bottom: 10px;">
        Here's a summary of the company. You can edit the details below, and see any of their staff's recent orders.
    </div>
    <asp:PlaceHolder runat="server" ID="_confirmedAssociation" Visible="false">
	    <h3>Details</h3>
	    <table cellspacing="0">
	        <tr>
	            <td valign="top" style="padding-right: 20px;">
	                <div class="DottedBox">
	                    <img src="../../../_images/silk/text_signature.png" alt="edit" /> 
	                    <asp:LinkButton runat="server" Text="edit" OnClick="EditCompanyDetailsHandler" ToolTip="edit company details" />
	                </div>
	                <table cellspacing="0" class="Form">
	                    <tr>
	                        <td class="Faint">
	                            Status:
	                        </td>
	                        <td>
	                            <asp:Literal ID="_status" runat="server" />
	                        </td>
	                    </tr>
	                    <tr>
	                        <td class="Faint" valign="top">
	                            Description:
	                        </td>
	                        <td class="LineSpaced">
	                            <asp:Literal ID="_detailsDescription" runat="server" Text="-" />
	                        </td>
	                    </tr>
	                    <tr>
	                        <td class="Faint">
	                            Website:
	                        </td>
	                        <td>
	                            <asp:Literal ID="_detailsNoWebsite" runat="server" Text="-" />
	                            <asp:HyperLink ID="_detailsWebsiteLink" runat="server" Target="_blank" Visible="false" />
	                        </td>
	                    </tr>
	                    <tr>
	                        <td class="Faint" style="padding-right: 20px;" nowrap="nowrap">
	                            Telephone Number:
	                        </td>
	                        <td>
	                            <asp:Literal ID="_detailsTelephoneNumber" runat="server" />
	                        </td>
	                    </tr>
	                    <tr>
	                        <td class="Faint">
	                            Fax Number:
	                        </td>
	                        <td>
	                            <asp:Literal ID="_detailsFaxNumber" runat="server" Text="-" />
	                        </td>
	                    </tr>
	                    <tr>
	                        <td colspan="2">
	                            <hr />
	                        </td>
	                    </tr>
	                    <tr>
	                        <td class="Faint" valign="top">
	                            Address:
	                        </td>
	                        <td class="LineSpaced">
	                            <asp:Literal ID="_detailsAddress" runat="server" Text="-" />
	                        </td>
	                    </tr>
	                    <tr>
	                        <td class="Faint">
	                            Postal Code:
	                        </td>
	                        <td>
	                            <asp:Literal ID="_detailsPostalCode" runat="server" Text="-" />
	                        </td>
	                    </tr>
	                    <tr>
	                        <td class="Faint">
	                            Country:
	                        </td>
	                        <td>
	                            <asp:Image ID="_countryFlag" runat="server" Visible="false" style="margin-right: 5px;" />
	                            <asp:Literal ID="_detailsCountry" runat="server" Text="-" />
	                        </td>
	                    </tr>
	                </table>
	            </td>
	            <td valign="top" style="padding-left: 20px; border-left: solid 1px #333;">
					<h4>Payment Terms</h4>
					<div class="LineSpaced">
						<span class="Highlight"><asp:Literal ID="_paymentTerms" runat="server" /></span> - <asp:Literal ID="_paymentTermsDescription" runat="server" />
						<a id="EditPaymentTermsLnk" href="#">Edit</a>
						<div id="EditPaymentTermsDiv" style="margin-top: 10px; display: none;">
							<asp:DropDownList ID="_paymentTermsDropDown" runat="server" /><br />
							<asp:Button OnClick="EditPaymentTermsHandler" runat="server" Text="Change" style="margin-top: 5px;" />
						</div>
					</div>
					
					<hr style="margin: 10px 0px 10px 0px;" />	            
	                <h4>Staff</h4>
	                <div style="margin-bottom: 20px;" class="LineSpaced">
	                    The following people are part of the company.
	                </div>
	                <asp:Repeater ID="_employees" runat="server" OnItemCreated="EmployeeItemCreatedHandler">
	                    <HeaderTemplate>
	                        <table class="Spacer" cellspacing="0">
	                    </HeaderTemplate>
	                    <ItemTemplate>
	                            <tr>
	                                <td nowrap="nowrap">
	                                    <img src="../../../_images/silk/user.png" alt="person" />
	                                    <asp:HyperLink ID="_employeeNameLink" runat="server" CssClass="Big" />
	                                    <asp:Literal ID="_employeeStatus" runat="server" Visible="false" Text="Pending" /> -
	                                    <asp:HyperLink ID="_employeeEmail" runat="server" CssClass="Faint" />
	                                </td>
	                            </tr>
	                    </ItemTemplate>
	                    <FooterTemplate>
	                        </table>
	                    </FooterTemplate>
	                </asp:Repeater>
	            </td>
	        </tr>
	    </table>
    </asp:PlaceHolder>
    
    <asp:PlaceHolder ID="_editConfirmedCompany" runat="server" Visible="false">
        <h3>Update Company Details</h3>
        <p style="margin: 0px 0px 20px 0px;">
            <asp:LinkButton Text="&laquo; cancel" runat="server" OnClick="CancelEditHandler" /> - Update the company details below.
        </p>
        <uc1:CompanyEditor ID="_editCompanyEditor" runat="server" />
    </asp:PlaceHolder>
    
    <h3 style="margin-top: 20px;">Orders</h3>
    Here's the latest 100 orders from members of this company.
    <div style="margin-top: 10px; font-weight: bold;" id="_noOrders" runat="server" visible="false">* No orders.</div>
    <asp:GridView 
        style="margin-top: 10px;"
        OnRowCreated="OrderRowCreatedHandler" 
        SkinID="LightContained"
        ID="_orders"
        AutoGenerateColumns="false"
        runat="server">
        <Columns>
			<asp:BoundField DataFormatString="#{0}" DataField="ID" />
			<asp:TemplateField HeaderText="Customer">
		        <ItemTemplate>
		            <asp:HyperLink ID="_customerLink" runat="server" />
		        </ItemTemplate>
	        </asp:TemplateField>
	        <asp:TemplateField HeaderText="Ordered">
		        <ItemTemplate>
		            <asp:Literal ID="_ordered" runat="server" />
		        </ItemTemplate>
	        </asp:TemplateField>
	        <asp:TemplateField HeaderText="Items">
		        <ItemTemplate>
		            <asp:Literal ID="_items" runat="server" />
		        </ItemTemplate>
	        </asp:TemplateField>
	        <asp:TemplateField HeaderText="Total Amount">
		        <ItemTemplate>
		            £ <asp:Literal ID="_total" runat="server" />
		        </ItemTemplate>
	        </asp:TemplateField>
	        <asp:TemplateField HeaderText="Charge Method">
		        <ItemTemplate>
		            <asp:Literal ID="_method" runat="server" />
		        </ItemTemplate>
	        </asp:TemplateField>
	        <asp:TemplateField ItemStyle-CssClass="HighlightCell" HeaderText="State">
		        <ItemTemplate>
		            <asp:Literal ID="_state" runat="server" />
		        </ItemTemplate>
	        </asp:TemplateField>
	        <asp:TemplateField ItemStyle-HorizontalAlign="Right">
		        <ItemTemplate>
		            <b><asp:HyperLink ID="_link" runat="server" Text="view order" /> &raquo;</b>
		        </ItemTemplate>
	        </asp:TemplateField>
        </Columns>
    </asp:GridView>
    
</asp:Content>