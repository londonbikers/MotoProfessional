using System;
using System.Linq;
using System.Web.UI;
using System.IO;
using App_Code;
using MPN.Framework.Files;

/// <summary>
/// Shows the contents of a partners incoming folder in summary.
/// </summary>
public partial class IncomingFolderView : UserControl
{
	protected void Page_Load(object sender, EventArgs e)
	{
	}

	#region public methods
	/// <summary>
	/// Provides a summary view of all the photos within a specified folder.
	/// </summary>
	public void LoadPath(string path)
	{
		if (!Directory.Exists(path))
		{
			_nothingFound.Visible = true;
			_mediaFound.Visible = false;
			return;
		}

		// look for supported image formats, including the new HD Photo .hdp format.
		var filenames = from f in Directory.GetFiles(path)
						where f.ToLower().EndsWith(".jpg") ||
						f.ToLower().EndsWith(".jpeg") ||
						f.ToLower().EndsWith(".png") ||
						f.ToLower().EndsWith(".gif") ||
						f.ToLower().EndsWith("bmp") ||
						f.ToLower().EndsWith("hdp")
						select f;

		if (filenames.Count() == 0)
		{
			_nothingFound.Visible = true;
			_mediaFound.Visible = false;
		}
		else
		{
			_nothingFound.Visible = false;
			_mediaFound.Visible = true;
			_photoCount.Text = filenames.Count().ToString();

			// filesize?
			long filesize = 0;
			foreach (var filePath in filenames)
			{
				var fi = new FileInfo(filePath);
				if (fi != null)
					filesize += fi.Length;
				else
					Logger.LogInfo("IncomingFolderView.ascx - Could not get FileInfo for: " + filePath);
			}

			_photoTotalSize.Text = Files.GetFriendlyFilesize(filesize);
		}
	}
	#endregion
}