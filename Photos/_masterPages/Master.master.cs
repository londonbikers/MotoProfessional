using System;
using System.Web.UI;

namespace _masterPages
{
    public partial class MpMasterPage : MasterPage
    {
        #region accessors
        /// <summary>
        /// The ID of the control to have as the default button for the form, i.e. when the enter button is hit.
        /// </summary>
        public string DefaultFormButton { get { return _form.DefaultButton; } set { _form.DefaultButton = value; } }
        public bool ShowJQueryUi { get { return _jQueryUI.Visible; } set { _jQueryUI.Visible = value; } }
        public bool ShowJQueryAutoGrow { get { return _jQueryAutoGrow.Visible; } set { _jQueryAutoGrow.Visible = value; } }
        public bool ShowJQueryBlockUi { get { return _jQueryBlockUI.Visible; } set { _jQueryBlockUI.Visible = value; } }
        public bool ShowJQueryAlphanumeric { get { return _jQueryAlphanumeric.Visible; } set { _jQueryAlphanumeric.Visible = value; } }
        /// <summary>
        /// Gets or sets the url to a cover image for the page's content. Used by Facebook and Digg for preview images.
        /// </summary>
        public string MetaImageUrl { set { _metaImageUrl.Text = string.Format("<link rel=\"image_src\" href=\"{0}\" />", value); } }
        /// <summary>
        /// Gets or sets the meta tag for the content title. Distinct from the TITLE element which is a general site descriptor, not a content descriptor.
        /// </summary>
        public string MetaTitle
        {
            get
            {
                return _metaTitle.Attributes["content"];
            }
            set
            {
                _metaTitle.Visible = true;
                _metaTitle.Attributes["content"] = value;
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            // you can't do this dynamically on the html. something to do with script tags and <%= or <%# statements.
            _jQuery.Text = string.Format("<script type=\"text/javascript\" src=\"{0}_system/jquery-1.2.6.min.js\"></script>", ResolveUrl("~/"));
            _jQueryUI.Text = string.Format("<script type=\"text/javascript\" src=\"{0}_system/jquery-ui-personalized-1.5.2.min.js\"></script>", ResolveUrl("~/"));
            _jQueryAutoGrow.Text = string.Format("<script type=\"text/javascript\" src=\"{0}_system/jquery.autogrow.js\"></script>", ResolveUrl("~/"));
            _jQueryBlockUI.Text = string.Format("<script type=\"text/javascript\" src=\"{0}_system/jquery.blockUI.js\"></script>", ResolveUrl("~/"));
            _jQueryAlphanumeric.Text = string.Format("<script type=\"text/javascript\" src=\"{0}_system/jquery.alphanumeric.pack.js\"></script>", ResolveUrl("~/"));
        }
    }
}