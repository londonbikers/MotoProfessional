using System.Collections.Generic;

namespace MotoProfessional.Models.Interfaces
{
    public interface IPartnerContainer : IEnumerable<IPartner>
    {
        /// <summary>
        /// The number of companies within the container.
        /// </summary>
        int Count { get; }

        IPartner this[int index] { get; }
        void AddPartner(IPartner partner);
        void AddPartner(int partnerId);
        void RemovePartner(IPartner partner);
        void RemovePartner(int partnerId);
        bool Contains(IPartner partner);
        bool Contains(int partnerId);
        void Clear();
    }
}