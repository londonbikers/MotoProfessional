<%@ Page Language="C#" MasterPageFile="~/_masterPages/Admin.master" AutoEventWireup="true" CodeFile="Customer.aspx.cs" Inherits="Admin.Sales.Customers.CustomerPage" %>
<%@ Register src="~/_controls/MemberDetailsEditor.ascx" tagname="MemberEditor" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageHeading" Runat="Server">
    <asp:Label ID="_titlePrefix" CssClass="Faint" runat="server" /> <asp:Literal ID="_customerHeadingName" runat="server" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PageContentArea" Runat="Server">
    
    <hr />
    <table cellpadding="0" cellspacing="0" style="width: 100%; margin-top: 10px;">
        <tr>
            <td valign="top" style="padding-right: 10px;">
                <asp:PlaceHolder id="_basicDetailsView" runat="server">
                    <h3 style="margin-bottom: 10px;">Basic Details</h3>
                    <table cellspacing="0" class="Form" width="100%">
                        <tr>
                            <td colspan="2">
                                <div class="DottedBox">
                                    <img src="../../../_images/silk/text_signature.png" alt="edit" /> 
                                    <asp:LinkButton runat="server" Text="edit" OnClick="EditMembershipUserHandler" ToolTip="edit basic details" />
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td class="Faint">
                                User Name:
                            </td>
                            <td>
                                <asp:Literal ID="_basicUsername" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td class="Faint">
                                Email:
                            </td>
                            <td>
                                <img src="../../../_images/silk/email.png" alt="email" />
                                <asp:HyperLink ID="_basicEmail" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td class="Faint">
                                Company:
                            </td>
                            <td>
                                <asp:HyperLink ID="_companyLink" runat="server" Text="-" Enabled="false" />
                            </td>
                        </tr>
                         <tr>
							<td class="Faint">
								Is Staff?
							</td>
							<td>
								<asp:Literal ID="_basicIsStaff" runat="server" />
							</td>
                        </tr>
                        <tr>
                            <td class="Faint">
                                Is Locked Out?
                            </td>
                            <td>
                                <asp:Literal ID="_basicIsLockedOut" runat="server" Text="No" />
                                <asp:LinkButton ID="_basicLockOutLinkBtn" runat="server" Text="unlock" OnClick="UnlockAccountHandler" Visible="false" />
                            </td>
                        </tr>
                        <tr>
                            <td class="Faint">
                                Is Approved?
                            </td>
                            <td>
                                <asp:Literal ID="_isApproved" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td class="Faint" style="padding-right: 20px; width: 115px;">
                                Staff Comment:
                            </td>
                            <td>
                                <asp:Literal ID="_basicComment" runat="server" Text="-" />
                            </td>
                        </tr>
                    </table>
                </asp:PlaceHolder>
                
                <asp:PlaceHolder ID="_basicDetailsEditView" runat="server" Visible="false">
                    <h3 style="margin-bottom: 10px;">Update Basic Details</h3>
                    <p style="margin: 0px 0px 10px 0px;">
                       <asp:LinkButton Text="&laquo; cancel" runat="server" OnClick="CancelEditHandler" /> 
                    </p>
                    <table cellspacing="0" class="MediumForm" width="100%">
                        <tr>
                            <td class="Faint">
                                Email:
                            </td>
                            <td>
                                <asp:TextBox ID="_editBasicEmail" runat="server" />
                                <asp:RequiredFieldValidator 
                                    Display="Dynamic"
                                    style="margin-left: 5px;"
                                    runat="server" 
                                    ErrorMessage="* Email required." 
                                    ControlToValidate="_editBasicEmail" 
                                    ValidationGroup="UpdateBasicDetails" />
                            </td>
                        </tr>
                        <tr>
                            <td class="Faint" style="padding-right: 20px; width: 115px;" valign="top">
                                Staff Comment:
                            </td>
                            <td>
                                <asp:TextBox TextMode="MultiLine" ID="_editBasicComment" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td class="Faint">
                                Is Staff?
                            </td>
                            <td>
                                <asp:CheckBox ID="_editBasicIsStaff" runat="server" />
                            </td>
                        </tr>
                        <tr>
							<td class=Faint>
								Is Approved?
							</td>
							<td>
								<asp:DropDownList ID="_editIsApproved" runat="server">
									<asp:ListItem>Yes</asp:ListItem>
									<asp:ListItem>No</asp:ListItem>
								</asp:DropDownList>
							</td>
                        </tr>
						<tr>
                            <td>
                                &nbsp;
                            </td>
                            <td>
                                <asp:Button Text="Update" OnClick="UpdateBasicDetailsHandler" runat="server" ValidationGroup="UpdateBasicDetails" />
                            </td>
                        </tr>
                    </table>
                </asp:PlaceHolder>
                
                <asp:PlaceHolder ID="_profileDetailsView" runat="server">
                    <h3 style="margin: 20px 0px 10px 0px;">Profile</h3>
                    <table cellspacing="0" class="Form" width="100%">
                        <tr>
                            <td colspan="2">
                                <div class="DottedBox">
                                    <img src="../../../_images/silk/text_signature.png" alt="edit" /> 
                                    <asp:LinkButton runat="server" Text="edit" OnClick="EditProfileHandler" ToolTip="edit profile details" />
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td class="Faint">
                                Name:
                            </td>
                            <td>
                                <asp:Literal ID="_profileName" runat="server" Text="-" />
                            </td>
                        </tr>
                        <tr>
                            <td class="Faint">
                                Sex:
                            </td>
                            <td>
                                <asp:Literal ID="_profileSex" runat="server" Text="-" />
                            </td>
                        </tr>
                        <tr>
                            <td class="Faint">
                                Job Title:
                            </td>
                            <td>
                                <asp:Literal ID="_profileJobTitle" runat="server" Text="-" />
                            </td>
                        </tr>
                        <tr>
                            <td class="Faint">
                                Telephone:
                            </td>
                            <td>
                                <asp:Literal ID="_profileTelephone" runat="server" Text="-" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <hr />
                            </td>
                        </tr>
                        <tr>
                            <td class="Faint" valign="top">
                                Billing Address:
                            </td>
                            <td class="LineSpaced">
                                <asp:Literal ID="_profileBillingAddress" runat="server" Text="-" />
                            </td>
                        </tr>
                        <tr>
                            <td class="Faint" valign="top" style="padding-right: 20px; width: 115px;">
                                Billing Postal Code:
                            </td>
                            <td>
                                <asp:Literal ID="_profileBillingPostalCode" runat="server" Text="-" />
                            </td>
                        </tr>
                        <tr>
                            <td class="Faint">
                                Billing Country:
                            </td>
                            <td>
                                <asp:Image ID="_profileCountryFlag" runat="server" Visible="false" style="margin-right: 5px;" />
                                <asp:Literal ID="_profileBillingCountry" runat="server" Text="-" />
                            </td>
                        </tr>
                    </table>
                </asp:PlaceHolder>
                
                <asp:PlaceHolder ID="_profileEditView" runat="server" Visible="false">
					<h3 style="margin-bottom: 10px;">Update Profile</h3>
					<p style="margin: 0px 0px 20px 0px;">
					   <asp:LinkButton Text="&laquo; cancel" runat="server" OnClick="CancelEditHandler"  />
					</p>
					<uc1:MemberEditor ID="_memberEditor" runat="server" />
                </asp:PlaceHolder>
    
            </td>
            <td valign="top" style="padding-left: 10px; border-left: solid 1px #333;">
                <h3>Account Summary</h3>
                <table cellspacing="0" class="Form">
                    <tr>
                        <td class="Faint">
                            Active Now?
                        </td>
                        <td>
                           <asp:Literal ID="_signedIn" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="Faint" style="padding-right: 20px; width: 115px;">
                            Last Sign-In:
                        </td>
                        <td>
                           <asp:Literal ID="_lastSignIn" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="Faint">
                            Registered:
                        </td>
                        <td>
                           <asp:Literal ID="_registeredDate" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="Faint" valign="top">
                            Basket:
                        </td>
                        <td>
                           <asp:Literal ID="_basketContents" runat="server" Text="Empty" />
                        </td>
                    </tr>
                </table>
                
                <h3 style="margin: 20px 0px 10px 0px;">Order History</h3>
                <asp:Literal ID="_noOrders" runat="server">* No orders yet.</asp:Literal>
                <asp:GridView
					OnRowCreated="OrderRowCreatedHandler" 
					SkinID="LightContained"
					ID="_orders"
					AutoGenerateColumns="false"
					runat="server">
					<Columns>
						<asp:BoundField DataFormatString="#{0}" DataField="ID" />
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
						<asp:TemplateField HeaderText="Total">
							<ItemTemplate>
								£ <asp:Literal ID="_total" runat="server" />
							</ItemTemplate>
						</asp:TemplateField>
						<asp:TemplateField HeaderText="Method">
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
								<b><asp:HyperLink ID="_link" runat="server" Text="view" /> &raquo;</b>
							</ItemTemplate>
						</asp:TemplateField>
					</Columns>
				</asp:GridView>
            </td>
        </tr>
    </table>

</asp:Content>