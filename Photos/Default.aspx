<%@ Page Language="C#" MasterPageFile="~/_masterPages/Master.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="HomePage" EnableViewState="false" %>
<%@ Register src="_controls/MediaAccreditationBanner.ascx" tagname="MediaBanner" tagprefix="uc1" %>
<%@ Register src="_controls/RegularMasterSearchBox.ascx" tagname="SearchBox" tagprefix="uc1" %>
<%@ Register src="_controls/TabbedNavigation.ascx" tagname="TabbedNavigation" tagprefix="uc1" %>
<%@ Register src="_controls/TagCloud.ascx" tagname="TagCloud" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" Runat="Server" />
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentArea" Runat="Server">
    <div class="SuperContainer" style="margin-top: 20px;">
        <div style="text-align: center; margin: 0px auto;">
            <a href="/" title="Moto Professional"><img src="_images/layout/mp-site-logo.gif" style="width: 514px; height: 58px; border: 0px;" alt="Moto Professional" /></a>
        </div>     
        <table cellpadding="0" cellspacing="0" style="margin: 0px auto; margin-top: 10px;">
            <tr>
                <td align="center">
                    <uc1:TabbedNavigation ID="_tabbedNavigation" runat="server" SelectedTab="home" />
                </td>
            </tr>
        </table>
        <img src="_images/layout/horizontal-div.gif" style="margin-top: -1px; clear: both; display: block; width: 100%; height: 26px;" alt="." />
        <div style="text-align: right;">
			<asp:LoginName ID="LoginName1" runat="server" FormatString="Welcome back {0}! - " />
			<asp:LoginStatus ID="_loginStatus" runat="server" LoginText="" OnLoggedOut="OnLoggedOutHandler" />
        </div>
        <script type="text/javascript">
            var flashVars = {
	            configXML: '',
	            configUrl: '<%= PhotoTileXmlUrl %>',
	            thumbDisplayType: 'diagonal',
	            animDelay: 100,
	            nextImagesDelay: 10000,
	            preloadLargeImages: true
            };
            var flashVarsText = '';
            for (var name in flashVars) {
	            var value = flashVars[name];
	            if (flashVarsText)
		            flashVarsText += '&';
                flashVarsText += name + '=' + value;
            }
            document.write(' \
            <object classid="clsid:d27cdb6e-ae6d-11cf-96b8-444553540000" codebase="http://fpdownload.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=8,0,0,0" width="956" height="375" id="PhotoTile"> \
	            <param name="allowScriptAccess" value="sameDomain" /> \
	            <param name="movie" value="_system/phototile/phototile.swf" /> \
	            <param name="quality" value="high" /> \
	            <param name="bgcolor" value="#000000" /> \
	            <param name="flashvars" value="' + flashVarsText + '" /> \
	            <embed src="_system/phototile/phototile.swf" quality="high" bgcolor="#000000" width="956" height="375" name="PhotoTile" align="middle" allowScriptAccess="sameDomain" type="application/x-shockwave-flash" pluginspage="http://www.macromedia.com/go/getflashplayer" flashvars="' + flashVarsText + '" /> \
            </object>');
        </script>
        <img src="_images/layout/horizontal-div.gif" style="display: block; width: 100%; height: 26px; margin-top: 10px;" alt="." />
        <table style="width: 100%;" cellpadding="0" cellspacing="0">
            <tr>
                <td valign="top" style="width: 33%; padding-right: 10px;" class="LineSpaced">
                    <h2>Who, What?</h2>
					<p>
						Welcome! Moto Professional enables the motorsport industry to get the very best photographs for print, web or promotional use, online
						with no fuss and no delay. There's no crazy prices or confusing licenses. It does exactly what it says on the tin, which is
						how we think it should be; nice and simple.
					</p>
					<p>
						We're very new right now, but we plan on inviting the very best motorsport photographers to join us soon and offer the most comprehensive and useful
						motorsports media service available. Photographers; we have a great offer for you. Watch this space!
					</p>
					<p>
						You can buy any photos we have and download them straight-away. We have a range of licenses available to suit your needs, which make sure
						you don't pay more than you need to for how you want to use the photo.
					</p>
                </td>
                <td valign="top" style="width: 33%; border-left: solid 1px #333; border-right: solid 1px #333; padding-left: 10px; padding-right: 10px;" nowrap="nowrap">
                    <div style="text-align: center;">
						<h2>Find a Photo!</h2>
						<uc1:SearchBox SearchBoxWidth="200" runat="server" />
					</div>
					<hr style="margin-top: 10px;"/>
                    <h2 style=" margin-top: 5px; text-align: center;">Latest Collections</h2>
                    <asp:Repeater ID="_newCollectionsList" runat="server" OnItemCreated="LatestCollectionsItemCreatedHandler">
						<HeaderTemplate>
							<ul style="font-size: 13px; padding-left: 15px; margin-left: 0px;">
						</HeaderTemplate>
						<ItemTemplate>
							    <li><asp:Literal ID="_collectionLink" runat="server" /></li>
						</ItemTemplate>
						<FooterTemplate>
							</ul>
						</FooterTemplate>
                    </asp:Repeater>
                </td>
                <td valign="top" style="width: 33%; padding-left: 10px; text-align: center;">
                    <h2>Latest Tags</h2>
                    <div style="text-align: left !important; margin-bottom: 10px;">
						<uc1:TagCloud id="_tagCloud" runat="server" LinkCssPrefix="HPTC" />
                    </div>
                    <asp:PlaceHolder ID="_memberControls" runat="server">
                        <hr style="margin-bottom: 10px;" />
                        <div class="H2" style="margin-bottom: 10px;">
                            <a href="signin/" class="H2">Sign-In</a> or <a href="register/" class="H2">Register</a>
                        </div>
                        Register to get full access or to buy.
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="_partnerPromotionBox" runat="server" Visible="false">
                        <hr style="margin-bottom: 10px;" />
                        <div class="H2" style="margin-bottom: 10px;">
                            <a href="photographers/" class="H2">Photographers</a>
                        </div>
                        Get your photos on to Moto Professional.
                    </asp:PlaceHolder>
                </td>
            </tr>
        </table>
        <hr style="margin-top: 20px;" />
        <div class="Faint" style="float: left;">
            <a href="about/">about us</a> | 
            <a href="help/faq/">faq</a> | 
            <a href="help/privacy">privacy</a> |
            <a href="http://blog.motoprofessional.com">blog</a>
        </div>
        <div class="Faint" style="float: right; text-align: right;">
            Copyright &copy; <%= DateTime.Now.Year.ToString() %> <a href="http://mediapanther.com">Media Panther Network</a>
        </div>
        <uc1:MediaBanner runat="server" />
    </div>
</asp:Content>