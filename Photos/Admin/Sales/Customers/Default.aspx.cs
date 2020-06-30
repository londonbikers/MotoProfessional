using System;
using System.Web.Security;
using _masterPages;

namespace Admin.Sales.Customers
{
    public partial class CustomersPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
                PerformDefaultSearch();

            var master = Page.Master as AdminMaster;
            if (master != null) master.DefaultFormButton = _customerSearchBtn.UniqueID;
        }

        #region event handlers
        /// <summary>
        /// Handles the searching for a specific user or set of users by either email or name.
        /// </summary>
        protected void PerformCustomerSearch(object sender, EventArgs ea)
        {
            if (string.IsNullOrEmpty(_name.Text) && string.IsNullOrEmpty(_email.Text))
                PerformDefaultSearch();

            var records = 100;
            if (_name.Text != null && _name.Text.Trim() != String.Empty)
            {
                _customerGrid.DataSource = Membership.FindUsersByName(_name.Text.Trim(), 0, 100, out records);
                _customerGrid.DataBind();
            } 
            else if (_email.Text != null && _email.Text.Trim() != String.Empty)
            {
                _customerGrid.DataSource = Membership.FindUsersByEmail(_email.Text.Trim(), 0, 100, out records);
                _customerGrid.DataBind();
            }
        }

        /// <summary>
        /// Handles the default date-descending user search.
        /// </summary>
        protected void PerformCustomerSearchByDate(object sender, EventArgs ea)
        {
            PerformDefaultSearch();
        }
        #endregion

        #region protected methods
        protected string GridIsLockedOut(object value)
        {
            if ((bool)value)
                return "<img src=\"../../../_images/silk/lock.png\" alt=\"locked out\" /> locked out";
            return string.Empty;
        }

        protected string GridIsOnline(object value)
        {
            if ((bool)value)
                return "<img src=\"../../../_images/silk/user.png\" alt=\"online\" /> Yes";
            return "-";
        }
        #endregion

        #region private methods
        /// <summary>
        /// Returns the newest members to the site.
        /// </summary>
        private void PerformDefaultSearch()
        {
            var membersReturned = 100;
            _customerGrid.DataSource = Membership.GetAllUsers(0, 100, out membersReturned);
            _customerGrid.DataBind();
        }
        #endregion
    }
}