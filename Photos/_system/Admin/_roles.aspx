<%@ Page Language="C#" AutoEventWireup="true" CodeFile="_roles.aspx.cs" Inherits="_system_Admin_roles" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Role Management</title>
</head>
<body>
    <form id="form1" runat="server">
    
		<b>add a role</b><br />
		Role Name: <asp:TextBox ID="_roleName" runat="server" /><br />
		<asp:Button Text="create role" runat="server" OnClick="CreateRoleHandler" /><br />
		<%= AddRoleResponse %>
		
		<hr />
		
		<b>add role to user</b><br />
		Username: <asp:TextBox ID="_username" runat="server" /><br />
		Role: <asp:DropDownList ID="_roleList" runat="server" />
		<asp:Button Text="assign role" runat="server" OnClick="AssignRoleHandler" /><br />
		<%= AddRoleToUserResponse %>
    
    </form>
</body>
</html>