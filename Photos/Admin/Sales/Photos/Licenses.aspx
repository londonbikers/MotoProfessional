<%@ Page Language="C#" MasterPageFile="~/_masterPages/Admin.master" AutoEventWireup="true" CodeFile="Licenses.aspx.cs" Inherits="Admin.Sales.Photos.LicensesPage" Title="MPa: Licenses" %>
<%@ Register tagprefix="tinymce" Namespace="Moxiecode.TinyMCE.Web" Assembly="Moxiecode.TinyMCE" %>

<asp:Content ID="_heading" ContentPlaceHolderID="PageHeading" runat="server">
    Licenses
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="PageContentArea" Runat="Server">
    <div class="LineSpaced">
        Licenses are the virtual product that the customer buys. A customer doesn't buy a photo, they buy a license to use that photo for, or in 
        a particular way. Add, remove or amend the licenses in the system below.
    </div>
    <div class="DottedBox" style="margin-top: 20px; margin-bottom: 10px;">
        <img src="../../../_images/silk/add.png" alt="add" /> <asp:LinkButton runat="server" Text="add license" OnClick="ShowAddLicenseFormHandler" />
    </div>
    <asp:PlaceHolder ID="_editForm" runat="server" Visible="false">
        <div style="padding-bottom: 10px; margin-bottom: 10px;">
            <h3 id="_editFormTitle" runat="server" style="margin: 0px 0px 5px 0px;">Add License</h3>
            <table class="MediumForm">
                <tr>
                    <td class="Faint">
                        Status:
                    </td>
                    <td>
                        <asp:DropDownList ID="_editStatus" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td class="Faint">
                        Name:
                    </td>
                    <td>
                        <asp:TextBox ID="_editName" runat="server" style="margin-right: 5px;" />
                        <asp:RequiredFieldValidator
							runat="server" 
							ErrorMessage="* A name is required." 
							ControlToValidate="_editName" 
							ValidationGroup="EditForm" />
                    </td>
                </tr>
                <tr>
                    <td class="Faint" style="padding-right: 20px;" valign="top">
                        Short Description:
                    </td>
                    <td>
                        <asp:TextBox ID="_editShortDescription" TextMode="MultiLine" runat="server" style="float: left; margin-right: 5px;" />
                        <asp:RequiredFieldValidator 
							runat="server" 
							ErrorMessage="* A short description is required." 
							ControlToValidate="_editShortDescription" 
							ValidationGroup="EditForm" />
                    </td>
                </tr>
                <tr>
                    <td class="Faint" style="padding-right: 20px;" valign="top">
                        Long Description:
                    </td>
                    <td>
                        <tinymce:TextArea Width="100%"
                            id="_editDescription"
		                    theme="advanced"
		                    plugins="spellchecker,pagebreak,style,layer,table,advhr,advimage,advlink,emotions,iespell,inlinepopups,insertdatetime,preview,media,searchreplace,print,contextmenu,paste,directionality,fullscreen,noneditable,visualchars,nonbreaking,xhtmlxtras,template"
		                    theme_advanced_buttons1="spellchecker,save,newdocument,|,bold,italic,underline,strikethrough,|,justifyleft,justifycenter,justifyright,justifyfull,|,styleselect,formatselect,fontselect,fontsizeselect"
		                    theme_advanced_buttons2="cut,copy,paste,pastetext,pasteword,|,replace,|,bullist,numlist,|,outdent,indent,blockquote,|,undo,redo,|,link,unlink,anchor,image,cleanup,help,code,|,insertdate,inserttime,preview,|,forecolor,backcolor"
		                    theme_advanced_buttons3="tablecontrols,|,hr,removeformat,visualaid,|,sub,sup,|,charmap,emotions,iespell,media,advhr,|,print,|,ltr,rtl,|,fullscreen, styleprops"
		                    theme_advanced_toolbar_location="top"
		                    theme_advanced_toolbar_align="left"
		                    theme_advanced_path_location="bottom"
		                    theme_advanced_resizing="true"
		                    runat="server" />
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp;
                    </td>
                    <td style="padding-top: 5px;">
                        <asp:Button runat="server" Text="Cancel" onclick="EditFormCancelHandler" style="margin-right: 5px;" />
                        <asp:Button runat="server" Text="Create" ID="_editBtn" onclick="EditUpdateHandler" ValidationGroup="EditForm" />
                    </td>
                </tr>
            </table>
        </div>
    </asp:PlaceHolder>
    
    <asp:GridView 
        onRowCommand="SetupLicenseEditHandler"
        SkinID="Light"
		ID="_licenseGrid"
		AutoGenerateColumns="false"
		runat="server">
		<Columns>
		    <asp:TemplateField ItemStyle-CssClass="HighlightCell" ItemStyle-Wrap="false" ItemStyle-VerticalAlign="Top" HeaderText="Name">
		        <ItemTemplate>
		            <asp:HiddenField id="_licenseIDField" runat="server" Value='<%# Eval("ID") %>' />
		            <img src="../../../_images/silk/page_white_text.png" alt="license" /> <%# Eval("Name") %>
		        </ItemTemplate>
		    </asp:TemplateField>
			<asp:BoundField DataField="ShortDescription" HeaderText="Short Description" />
			<asp:BoundField DataField="Status" HeaderText="Status" ItemStyle-VerticalAlign="Top" />
			<asp:ButtonField Text="edit" ItemStyle-VerticalAlign="Top" CommandName="EditLicense" />
		</Columns>
	</asp:GridView>
    
</asp:Content>