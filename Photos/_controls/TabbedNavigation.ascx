<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TabbedNavigation.ascx.cs" Inherits="_controls_TabbedNavigation" %>
 <style type="text/css">
    ul.tab li { font-family: Arial; font-size: 13px; border: 0px; margin: 0px; padding: 0px; list-style: none; }
    ul.tab { width: auto; text-align: center; margin: 0px; padding: 0px; clear: both; display: inline; height: 29px; margin-bottom: 0px; }
    ul.tab li { float: left; margin-right: 2px; }
    .tab a:link, .tab a:visited {
        background:url(<%= Page.ResolveUrl("~") %>_images/nav/tab-round.png) right 60px;
        color: #666666;
        display: block;
        font-weight: bold;
        line-height: 30px;
        text-decoration: none;
    }
    .tab a span {
        background: url(<%= Page.ResolveUrl("~") %>_images/nav/tab-round.png) left 60px;
        display: block;
        margin-right: 14px;
        padding-left: 14px;
        cursor: pointer;
    }
    .tab a:hover {
        background: url(<%= Page.ResolveUrl("~") %>_images/nav/tab-round.png) right 30px;
        display: block;
        cursor: pointer;
    }
    .tab a:hover span {
        background: url(<%= Page.ResolveUrl("~") %>_images/nav/tab-round.png) left 30px;
        display: block;
        cursor: pointer;
    }
    .active a:link, .active a:visited, .active a:visited, .active a:hover {
        color: #fff;
    } 
</style>
<ul class="tab">
    <asp:Literal ID="_listItems" runat="server" />
</ul>