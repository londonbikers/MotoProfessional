<%@ Control Language="C#" AutoEventWireup="true" CodeFile="RegularMasterSearchBox.ascx.cs" Inherits="_controls_RegularMasterSearchBox" %>

<script type="text/javascript">
    <!--
    function SubmitRegularMasterSearchBox(myfield,e)
    {
        var keycode;
        if (window.event) keycode = window.event.keyCode;
        else if (e) keycode = e.which;
        else return true;

        if (keycode == 13) {
            var btn = document.getElementById("<%= _searchBoxBtn.ClientID %>");
            btn.click();
            return false;
        } else
           return true;
        }
     //-->
</script>
<img src="<%= Page.ResolveUrl("~/_images/silk/zoom.png") %>" alt="Search" />
<input id="regularMasterSearchBox" type="text" name="searchBoxTerm" class="SearchBox" style="<%= _searchBoxWidth %>margin-right: 5px;" onkeypress="return SubmitRegularMasterSearchBox(this,event)" />
<asp:ImageButton ImageUrl="~/_images/forms/search.gif" OnClick="SearchHandler" runat="server" ID="_searchBoxBtn" />