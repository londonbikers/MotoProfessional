using System;

public partial class Help_Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // no content for now.
        Response.Redirect("faq/");
    }
}