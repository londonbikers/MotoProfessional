using System;
using System.Collections.Generic;
using System.Web.Security;

namespace MotoProfessional.Models.Interfaces
{
    public interface IMember
    {
        /// <summary>
        /// The ASPNET Membership Member UID.
        /// </summary>
        Guid Uid { get; }

        MembershipUser MembershipUser { get; set; }
        IProfile Profile { get; set; }
        ICompany Company { get; set; }
        string IpAddress { get; set; }
        IBasket Basket { get; set; }

        /// <summary>
        /// The top 100 latest orders for this Member.
        /// </summary>
        List<IOrder> Orders { get; set; }

        /// <summary>
        /// Adds the member to a company. This method should be used over using the Company directly as it keeps the member in sync.
        /// </summary>
        void AddToCompany(ICompany company, bool bypassConfirmProcess);

        /// <summary>
        /// Removes the member from their associated company. Use this method instead of working with the Company directly to keep the member in sync.
        /// </summary>
        void RemoveFromCompany();

        /// <summary>
        /// Returns a simplified name for this Member from their Profile. If there's no suitable Profile data then their username is used instead.
        /// </summary>
        string GetFullName();

        /// <summary>
        /// Deletes the current basket.
        /// </summary>
        void EmptyBasket();
    }
}