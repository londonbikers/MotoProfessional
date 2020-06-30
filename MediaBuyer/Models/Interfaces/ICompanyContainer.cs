using System.Collections.Generic;

namespace MotoProfessional.Models.Interfaces
{
    public interface ICompanyContainer
    {
        /// <summary>
        /// The number of companies within the container.
        /// </summary>
        int Count { get; }

        ICompany this[int index] { get; }
        void AddCompany(ICompany company);
        void AddCompany(int companyID);
        void RemoveCompany(ICompany company);
        void RemoveCompany(int companyId);
        bool Contains(ICompany company);
        bool Contains(int companyId);
        void Clear();
        IEnumerator<ICompany> GetEnumerator();
    }
}