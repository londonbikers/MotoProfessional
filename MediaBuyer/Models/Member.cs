using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using MotoProfessional.Models.Interfaces;

namespace MotoProfessional.Models
{
    /// <summary>
    /// Container class for the presentation layer to hold things together for the customer/partner.
    /// </summary>
    public class Member : IMember
    {
        #region members
    	private IBasket _basket;
		private List<IOrder> _orders;
        #endregion

        #region accessors
		/// <summary>
		/// The ASPNET Membership Member UID.
		/// </summary>
		public Guid Uid { get { return (Guid)MembershipUser.ProviderUserKey; } }
    	public MembershipUser MembershipUser { get; set; }
    	public IProfile Profile { get; set; }
    	public ICompany Company { get; set; }
    	public string IpAddress { get; set; }
		public IBasket Basket 
		{ 
			get 
			{
                if (_basket == null)
                    RetrieveBasket();

                // if there's an order associated here and it's begun its order-process then
                // we need to drop the basket so the member has a clean purchase slate.
                if (_basket != null && _basket.Order != null && _basket.Order.HasTransactionBegun)
					EmptyBasket();

				return _basket;
			}
			set
			{
				_basket = value;
			}
		}
		/// <summary>
		/// The top 100 latest orders for this Member.
		/// </summary>
		public List<IOrder> Orders
		{
			get
			{
				if (_orders == null)
					RetrieveOrders();

				return _orders;
			}
			set { _orders = value; } 
		}
        #endregion

		#region static accessors
		public static DomainObject DomainObjectType { get { return DomainObject.Member; } }
		#endregion

        #region constructors
        internal Member()
        {
            Profile = new Profile(ClassMode.New);
        }
        #endregion

        #region public methods
        /// <summary>
        /// Adds the member to a company. This method should be used over using the Company directly as it keeps the member in sync.
        /// </summary>
        public void AddToCompany(ICompany company, bool bypassConfirmProcess)
        {
            var status = (bypassConfirmProcess) ? CompanyEmployeeStatus.Confirmed : CompanyEmployeeStatus.Pending;
            company.AddEmployee(this, status);
            Company = company;
            Controller.Instance.CompanyController.UpdateCompany(company);
        }

        /// <summary>
        /// Removes the member from their associated company. Use this method instead of working with the Company directly to keep the member in sync.
        /// </summary>
        public void RemoveFromCompany()
        {
            Company.RemoveEmployee(this);
            Controller.Instance.CompanyController.UpdateCompany(Company);
            Company = null;
        }

		/// <summary>
		/// Returns a simplified name for this Member from their Profile. If there's no suitable Profile data then their username is used instead.
		/// </summary>
		public string GetFullName()
		{
			var name = Profile.ToFullName();
			if (string.IsNullOrEmpty(name))
				name = MembershipUser.UserName;

			return name;
		}

		/// <summary>
		/// Deletes the current basket.
		/// </summary>
		public void EmptyBasket()
		{
			Controller.Instance.CommerceController.DeleteBasket(_basket);
			_basket = null;
			RetrieveBasket();
		}
        #endregion

		#region internal methods
		/// <summary>
		/// Retrieves the top 100 latest orders for this Member.
		/// </summary>
		/// <remarks>
		/// Internal so controllers can refresh the order collection at will.
		/// </remarks>
		internal void RetrieveOrders()
		{
			_orders = new List<IOrder>();
			var db = new MotoProfessionalDataContext();
			var ids = (from o in db.DbOrders
							where o.CustomerUID == (Guid)MembershipUser.ProviderUserKey
							orderby o.Created descending
							select o.ID).Take(100);

            foreach (var id in ids)
                _orders.Add(Controller.Instance.CommerceController.GetOrder(id));
		}
		#endregion

		#region private methods
		private void RetrieveBasket()
		{
			_basket = Controller.Instance.CommerceController.GetCurrentBasket(this);
		}
		#endregion
	}
}