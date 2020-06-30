using System;
using System.Web.Security;

public partial class _system_Admin_roles : System.Web.UI.Page
{
	protected string AddRoleResponse;
	protected string AddRoleToUserResponse;

	protected void Page_Load(object sender, EventArgs e)
	{
		if (!Page.IsPostBack)
			RenderView();
	}

	protected void CreateRoleHandler(object sender, EventArgs ea)
	{
		Roles.CreateRole(_roleName.Text.Trim());
		AddRoleResponse = _roleName.Text.Trim() + " role created!";
		RenderView();
	}

	protected void AssignRoleHandler(object sender, EventArgs ea)
	{
		Roles.AddUserToRole(_username.Text.Trim(), _roleList.SelectedValue);
		AddRoleToUserResponse = _roleList.SelectedValue + " added to " + _username.Text + "!";
		RenderView();
	}

	private void RenderView()
	{
		_roleList.DataSource = Roles.GetAllRoles();
		_roleList.DataBind();
	}
}