using System;
using MotoProfessional;
using MPN.Framework.Security;

namespace Account.Partner.Collections
{
    public partial class CollectionsPreviewPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Request.QueryString["i"]))
                return;

            var p = Controller.Instance.PhotoController.GetPhoto(Convert.ToInt32(Request.QueryString["i"]));
            if (p == null)
                return;

            var eParams = Server.UrlEncode(SecurityHelpers.DesEncrypt(string.Format("i={0}&d=800&nw=1", p.Id)));
            _photo.ImageUrl = string.Format("{0}i.ashx?e={1}", Page.ResolveUrl("~/"), eParams);
            _name.Text = p.Name;
            _status.Text = p.Status.ToString();
            _dimensions.Text = string.Format("{0} x {1}", p.Size.Width, p.Size.Height);
            _tags.Text = p.Tags.ToCsv();
        
            if (!string.IsNullOrEmpty(p.Comment))
                _description.Text = p.Comment;
        }
    }
}