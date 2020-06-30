<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ExtendedPhotoMetaData.ascx.cs" Inherits="_controls.ExtendedPhotoMetaData" %>

<h3 style="margin-top: 0px;">Technical Details</h3>
<div class="Faint LineSpaced">
    Want some trade secrets on how this is done? Here's some technical details we have for the photo.
</div>
<hr style="margin: 10px 0px 10px 0px;" />
<table cellpadding="0" cellspacing="0" class="Form">
	<tr id="_cameraModelRow" runat="server">
		<td class="Faint" style="padding-right: 20px;">
			Camera Model:
		</td>
		<td>
			<asp:Literal ID="_cameraModel" runat="server" />
		</td>
	</tr>
	<tr id="_lensRow" runat="server">
		<td class="Faint" style="padding-right: 20px;">
			Lens:
		</td>
		<td>
			<asp:Literal ID="_lens" runat="server" />
		</td>
	</tr>
	<tr id="_isoRow" runat="server">
		<td class="Faint" style="padding-right: 20px;">
			ISO:
		</td>
		<td>
			<asp:Literal ID="_iso" runat="server" />
		</td>
	</tr>
	<tr id="_resolutionRow" runat="server">
		<td class="Faint" style="padding-right: 20px;">
			Resolution:
		</td>
		<td>
			<asp:Literal ID="_resolution" runat="server" />
		</td>
	</tr>
	<tr id="_exposureTimeRow" runat="server">
		<td class="Faint" style="padding-right: 20px;">
			Exposure Time:
		</td>
		<td>
			<asp:Literal ID="_exposureTime" runat="server" />
		</td>
	</tr>
	<tr id="_fNumberRow" runat="server">
		<td class="Faint" valign="top">
			F-Number:
		</td>
		<td class="LineSpaced">
			<asp:Literal ID="_fNumber" runat="server" />
		</td>
	</tr>
	<tr id="_focalLengthRow" runat="server">
		<td class="Faint" valign="top">
			Focal Length:
		</td>
		<td class="LineSpaced">
			<asp:Literal ID="_focalLength" runat="server" />
		</td>
	</tr>
	<tr id="_flashFiredRow" runat="server">
		<td class="Faint" valign="top">
			Flash Fired?
		</td>
		<td class="LineSpaced">
			<asp:Literal ID="_flashFired" runat="server" />
		</td>
	</tr>
</table>