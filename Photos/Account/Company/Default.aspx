<%@ Page Language="C#" MasterPageFile="~/_masterPages/Regular.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Account.Company.CompanyPage" Title="MP: Your Company or Team" %>
<%@ Register src="~/_controls/CompanyDetailsEditor.ascx" tagname="CompanyEditor" tagprefix="uc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="PageHeading" runat="server">
	Your Company or Team
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="PageContentArea" Runat="Server">

    <asp:PlaceHolder runat="server" ID="_noAssociation" Visible="false">
	    <div class="ExplanationBox" style="margin-bottom: 20px;">
		    Moto Professional is business-only service. We do not currently sell to the general-public. We must have your company or team
		    details recorded below before you can make a purchase.
		    <br />
		    <br />
		    Please search for your company first to see if it's already registered with us. If it is, you can apply to be associated with
		    it. One of your collegues who are already customers will be sent a notification and can authorise you.
		    If your company is not registered already, please do so!
	    </div>
		
	    <h3>Search For a Company or Team</h3>
	    <img src="../../_images/silk/zoom.png" alt="search" />
	    <asp:TextBox ID="_searchBox" runat="server" class="BigTextBox" style="width: 250px; margin-right: 10px;" />
	    <asp:Button class="BigButton" runat="server" Text="Search" OnClick="CompanySearchHandler" ID="_companySearchBtn" />
        <asp:RequiredFieldValidator runat="server" ErrorMessage="* Please supply all, or part of a company name." ControlToValidate="_searchBox" style="margin-left: 10px;" />
	    <asp:PlaceHolder ID="_searchResults" Visible="false" runat="server">
	        <hr style="margin: 10px 0px 10px 0px;"/>
	        <h3 style="margin: 0px;">Search Results</h3>
	        <div style="margin-bottom: 20px;" class="Faint">
	            Maximum of 50
	        </div>
	        <asp:GridView
	            OnRowCommand="JoinCompanyHandler" 
	            SkinID="CompactLarge"
                ID="_resultsGrid"
                AutoGenerateColumns="false"
                runat="server">
                <Columns>
	                <asp:TemplateField>
		                <ItemTemplate>
			                <img src="../../_images/silk/group.png" alt="Company/Team" />
			                <asp:HiddenField ID="_companyID" Value='<%# Eval("ID") %>' runat="server" />
		                </ItemTemplate>
	                </asp:TemplateField>
                    <asp:BoundField DataField="Name" HeaderText="Name" />
                    <asp:ButtonField Text="That's Mine!" ButtonType="Button" CommandName="JoinCompany" />
                </Columns>
            </asp:GridView>
            <asp:Label CssClass="Standout" ID="_noResults" runat="server" Visible="false" Text="* No Results!" EnableViewState="false" />
	    </asp:PlaceHolder>
    </asp:PlaceHolder>
    
    <asp:PlaceHolder ID="_pendingAssociation" runat="server" Visible="false">
        <div class="LineSpaced">
            We've got your company below, but before we can continue, one of your collegues needs to confirm  We've sent an email
            letting them know. If you've made a mistake, you can leave the company using the form below.
            <hr />
            <h3>Pending Company</h3>
            <img src="../../_images/silk/group.png" alt="Company/Team" />
            <asp:Label ID="_pendingCompanyName" runat="server" CssClass="Standout" style="margin: 0px 10px 0px 10px;" />
            <asp:Button OnClick="RemovePendingCompanyHandler" runat="server" Text="Cancel Join" OnClientClick="return confirm('Are you sure?');" />
        </div>
    </asp:PlaceHolder>
	
    <asp:PlaceHolder runat="server" ID="_confirmedAssociation" Visible="false">
	    <div class="ExplanationBox" style="margin-bottom: 20px;">
		    Moto Professional is business-only service. We don't currently sell to the general public. We must have your company or team
		    details recorded below before you can make a purchase.
	    </div>
	    <h3>Company or Team Details</h3>
	    <table cellspacing="0">
	        <tr>
	            <td valign="top" style="padding-right: 20px;">
	                <div class="DottedBox">
	                    <img src="../../_images/silk/text_signature.png" alt="edit" /> 
	                    <asp:LinkButton runat="server" Text="edit" OnClick="EditCompanyDetailsHandler" ToolTip="edit your company or team details" />
	                </div>
	                <table cellspacing="0" class="Form">
	                    <tr>
	                        <td class="Faint">
	                            Name:
	                        </td>
	                        <td>
	                            <h5 style="margin: 0px;"><asp:Literal ID="_detailsName" runat="server" /></h5>
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
					<div class="LineSpaced" style="margin-bottom: 10px;">
						<span class="Highlight"><asp:Literal ID="_paymentTerms" runat="server" /></span> - <asp:Literal ID="_paymentTermsDescription" runat="server" />
						To see about changing this, please <a href="<%= Page.ResolveUrl("~/contact/") %>">contact us</a>.
					</div>
					<hr />
	                <h4 style="margin-top: 10px;">Your Collegues</h4>
	                <div style="margin-bottom: 20px;" class="LineSpaced">
	                    The following people are part of your company or team and can purchase on its behalf. Any pending collegues must be allowed or 
	                    refused before they can buy on behalf of your company though.
	                </div>
	                <asp:Repeater ID="_employees" runat="server" OnItemCreated="EmployeeItemCreatedHandler">
	                    <HeaderTemplate>
	                        <table class="Spacer" cellspacing="0">
	                    </HeaderTemplate>
	                    <ItemTemplate>
	                            <tr>
	                                <td nowrap="nowrap">
	                                    <img src="../../_images/silk/user.png" alt="person" />
	                                    <asp:Label CssClass="Standout" ID="_employeeName" runat="server" />
	                                    <asp:PlaceHolder ID="_authControlView" runat="server" Visible="false">
											(Pending) -
											<asp:LinkButton OnCommand="ProcessPendingUserHandler" CommandName="Confirm" ID="_confirmAuthBtn" runat="server" Text="Yes" CssClass="H4U" ToolTip="Confirm this person belongs to your company/team." /> <img src="<%# Page.ResolveUrl("~/_images/silk/tick.png") %>" alt="yes" /> -
											<asp:LinkButton OnCommand="ProcessPendingUserHandler" CommandName="Refuse" ID="_refuseAuthBtn" runat="server" Text="No" CssClass="H4U" ToolTip="Refuse this person as belonging to your company/team." /> <img src="<%# Page.ResolveUrl("~/_images/silk/cross.png") %>" alt="no" />
	                                    </asp:PlaceHolder>
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
        <h3>Update Your Company or Team Details</h3>
        <p style="margin: 0px 0px 20px 0px;">
            <asp:LinkButton Text="&laquo; cancel" runat="server" OnClick="CancelEditHandler" /> - Update your company or team details below.
        </p>
        <uc1:CompanyEditor ID="_editCompanyEditor" runat="server" />
    </asp:PlaceHolder>
    
    <asp:PlaceHolder ID="_newCompany" runat="server" Visible="false">
        <hr style="margin: 10px 0px 10px 0px;"/>
        <h3>New Company or Team!</h3>
        <p style="margin: 0px 0px 20px 0px;">
            Okay, no company or team found. You must be the first of your kind. Please supply your details below.
        </p>
        <uc1:CompanyEditor ID="_companyEditor" runat="server" />
    </asp:PlaceHolder>
	
</asp:Content>