<%@ Page Language="C#" MasterPageFile="~/_masterPages/Regular.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Account.Partner.Rates.PartnerRatesPage" Title="MP: Partner Rates" %>

<asp:Content ID="Content2" ContentPlaceHolderID="PageHeading" runat="server">
	Partner Rates
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="PageContentArea" Runat="Server">

    <div style="margin-bottom: 10px;" class="Faint">
        More:
        <img src="../../../_images/arrow-right.png" style="margin-bottom: 2px;" alt="go" />
        <a href="../collections/" class="FaintU">Collections</a> - 
        <a href="../rates/" class="FaintU">Rates</a> 
        <!-- - <a href="../reports/" class="FaintU">Reports</a>-->
    </div>
    
    <div class="ExplanationBox">
		Rates are required before any of your work can go on display and sale. Rates work by assigning prices to licenses. A collection of rates for licenses
		is called a Rate Card. You can have multiple Rate Cards to allow you to easily scale the price up or down for a particular photo or even collections.
		This gives you the power to charge more for higher-value work, or less for more general-public work.
    </div>
    
    <asp:PlaceHolder ID="_editRateCardView" runat="server" Visible="false">
        <div class="DottedBox" style="margin-top: 10px;">
		    <h3 style="margin-bottom: 10px;" id="_editRcTitle" runat="server">Add Rate Card</h3>
		    <table class="Form" cellspacing="0">
			    <tr>
				    <td class="Faint">
					    Name:
				    </td>
				    <td>
					    <asp:TextBox ID="_editRcName" runat="server" />
					    <asp:RequiredFieldValidator runat="server" ErrorMessage="* A name is required." Display="Dynamic" ControlToValidate="_editRcName" ValidationGroup="EditRC" />
				    </td>
			    </tr>
			    <tr>
				    <td class="Faint">
					    Is Default?
				    </td>
				    <td>
					    <asp:CheckBox ID="_editRcIsDefault" runat="server" Checked="true" />
				    </td>
			    </tr>
			    <tr>
				    <td class="Faint">
					    Status:
				    </td>
				    <td>
					    <asp:DropDownList ID="_editRcStatus" runat="server" />
				    </td>
			    </tr>
			    <tr>
				    <td colspan="2" style="padding-top: 10px; padding-bottom: 20px;">
					    <hr />
					    <b>License Rates</b>&nbsp;
					    <span class="Faint">e.g. '9.99' or '10'</span>
					    <asp:Label 
						    style="color: red; display: block; margin-top: 10px;" 
						    ID="_invalidRCIs" 
						    runat="server" 
						    Text="* Invalid amounts, please make sure they're numeric, i.e. '9.99' or '10'." 
						    Visible="false" />
				    </td>
			    </tr>
			    <asp:Repeater ID="_editRcLicenses" runat="server" OnItemCreated="RcEditFormLicenseCreatedHandler">
				    <ItemTemplate>
					    <tr>
						    <td class="Faint" style="padding-right: 20px; text-align: right;">
							    <%# Eval("Name") %>:
						    </td>
						    <td>
							    £ <asp:TextBox runat="server" />
						    </td>
					    </tr>
				    </ItemTemplate>
			    </asp:Repeater>
			    <tr>
				    <td>
					    &nbsp;
				    </td>
				    <td>
				        <asp:Button Text="Cancel" runat="server" OnClick="CancelEditHandler" style="margin-left: 10px; margin-right: 5px;" />
				        <asp:Button ID="_editRcBtn" runat="server" OnClick="UpdateRateCardHandler" Text="Add Rate-Card" ValidationGroup="EditRC" />
				    </td>
			    </tr>
		    </table>
        </div>
    </asp:PlaceHolder>
    
    <div class="DottedBox" style="margin-top: 20px; margin-bottom: 10px;" id="_showEditFormDiv" runat="server">
        <img src="../../../_images/silk/add.png" alt="add" /> 
        <asp:LinkButton runat="server" Text="add rate card" OnClick="ShowEditRateCardView" />
    </div>
    
    <h3>Rate Cards</h3>
    
    <div class="Highlight" id="_noRateCards" runat="server" visible="false" style="margin-bottom: 10px;">
        <img src="../../../_images/silk/error.png" alt="error" style="float: left; margin-bottom: 10px; margin-right: 10px; margin-top: 9px;" />
		You've got no active Rate Cards. You need at least one before you can put any photos live.
		Also, at least one needs to be set as the default. Photos cannot be imported otherwise.
    </div>
    
    <asp:Repeater id="_rateCards" runat="server" OnItemCreated="RateCardsItemCreatedHandler">
		<ItemTemplate>
	        <div class="GridViewContainer">
	            <table width="100%" cellpadding="0" cellspacing="0" style="margin-bottom: 5px;">
	                <tr>
	                    <td>
                            <h4 id="_rcName" runat="server" style="display: inline; margin-right: 5px;"/> <asp:Label ID="_rcStatus" runat="server" CssClass="Faint" />	                    
	                    </td>
	                    <td align="right">
	                        <asp:HyperLink ID="_editLink" runat="server" Text="edit" />
	                        <img src="../../../_images/silk/text_signature.png" alt="edit" />
	                    </td>
	                </tr>
	            </table>
		        <asp:GridView
		            OnRowCreated="RateCardItemRowCreatedHandler" 
			        SkinID="LightContained"
			        ID="_rateCardItemGrid"
			        AutoGenerateColumns="false"
			        runat="server">
			        <Columns>
				        <asp:TemplateField HeaderText="License">
					        <ItemTemplate>
					            <asp:Literal ID="_licenseName" runat="server" />
					        </ItemTemplate>
				        </asp:TemplateField>
				        <asp:TemplateField ItemStyle-CssClass="HighlightCell" HeaderText="Rate" ItemStyle-Width="100px">
					        <ItemTemplate>
					            <b>£ <asp:Literal ID="_rate" runat="server" /></b>
					        </ItemTemplate>
				        </asp:TemplateField>
				        <asp:TemplateField ItemStyle-CssClass="HighlightCell" ItemStyle-Wrap="false" HeaderText="Last Updated" ItemStyle-Width="130px">
					        <ItemTemplate>
					            <asp:Literal ID="_lastUpdated" runat="server" />
					        </ItemTemplate>
				        </asp:TemplateField>
			        </Columns>
		        </asp:GridView>
		    </div>
		</ItemTemplate>
		<SeparatorTemplate>
		    <div style="margin-bottom: 5px;">&nbsp;</div>
		</SeparatorTemplate>
    </asp:Repeater>
    
</asp:Content>