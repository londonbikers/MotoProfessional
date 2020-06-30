using System;
using App_Code;

namespace _masterPages
{
    public partial class AdminMaster : System.Web.UI.MasterPage
    {
        #region accessors
        /// <summary>
        /// Pass-through for the MPMasterPage. Sets the default form button. Supply the ID of the button to be the form default.
        /// </summary>
        public string DefaultFormButton { get { return ((MpMasterPage) Master).DefaultFormButton; } set { ((MpMasterPage) Master).DefaultFormButton = value; } }
        public bool ShowJQueryAutoGrow { get { return ((MpMasterPage) Master).ShowJQueryAutoGrow; } set { ((MpMasterPage) Master).ShowJQueryAutoGrow = value; } }
        public bool ShowJQueryUi { get { return ((MpMasterPage) Master).ShowJQueryUi; } set { ((MpMasterPage) Master).ShowJQueryUi = value; } }
        public bool ShowJQueryBlockUi { get { return ((MpMasterPage) Master).ShowJQueryBlockUi; } set { ((MpMasterPage) Master).ShowJQueryBlockUi = value; } }
        public bool ShowJQueryAlphanumeric { get { return ((MpMasterPage) Master).ShowJQueryAlphanumeric; } set { ((MpMasterPage) Master).ShowJQueryAlphanumeric = value; } }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            _responseBox.InnerHtml = Helpers.RenderUserResponse();
            if (!string.IsNullOrEmpty(_responseBox.InnerHtml))
                _responseHolder.Visible = true;
        }
    }
}