using System;
using System.Drawing;
using System.Linq;
using MotoProfessional;

public partial class Account_Partner_Collections_makeallpreviews : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(Request.QueryString["pd"]) || 
			string.IsNullOrEmpty(Request.QueryString["s"]) || 
			string.IsNullOrEmpty(Request.QueryString["rw"]))
        {
            Response.Write("Params needed: c=collection; pd=primary dimension; s=square; rw=resize by width");
            Response.End();
            return;
        }

        var dimension = Convert.ToInt32(Request.QueryString["pd"]);
        var square = bool.Parse(Request.QueryString["s"]);
        var resizeByWidth = bool.Parse(Request.QueryString["rw"]);

		var specificSize = Size.Empty;
		if (!string.IsNullOrEmpty(Request.QueryString["sw"]) && !string.IsNullOrEmpty(Request.QueryString["sh"]))
			specificSize = new Size(int.Parse(Request.QueryString["sw"]), int.Parse(Request.QueryString["sh"]));

		try
		{
			foreach (var cp in Controller.Instance.PhotoController.LatestCollections.SelectMany(c => c.Photos))
			{
			    if (specificSize.IsEmpty)
			        cp.Photo.MakePreviewImage(dimension, square, resizeByWidth);
			    else
			        cp.Photo.MakePreviewImage(specificSize);
			}
		}
		catch
		{
		}

        Response.Write("Done!");
        Response.End();
    }
}