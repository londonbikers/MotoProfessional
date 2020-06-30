using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MotoProfessional.Models.Interfaces;

namespace MotoProfessional.Models
{
	/// <summary>
	/// Provides a lightweight means to hold a collection of companies. Internally only the ID's are stored
	/// and only upon retrieval is a whole Company object retrieved (from cache or db).
	/// </summary>
	public class CompanyContainer : IEnumerable<ICompany>, ICompanyContainer
	{
		#region members
		private readonly List<int> _ids;
		#endregion

		#region accessors
		/// <summary>
		/// The number of companies within the container.
		/// </summary>
		public int Count { get { return _ids.Count; } }
		#endregion

		#region constructors
        internal CompanyContainer()
		{
			_ids = new List<int>();
		}
		#endregion

		#region indexers
		public ICompany this[int index]
		{
			get
			{
				if (index < 0 || index > _ids.Count - 1)
					throw new ArgumentOutOfRangeException();

				return Controller.Instance.CompanyController.GetCompany(_ids[index]);
			}
		}
		#endregion

		#region public methods
		public void AddCompany(ICompany company)
		{
			if (company == null)
				throw new ArgumentNullException("company");

			lock (_ids)
			{
				if (!_ids.Contains(company.Id))
					_ids.Add(company.Id);
			}
		}

		public void AddCompany(int companyID)
		{
			if (companyID < 1)
				throw new ArgumentOutOfRangeException("companyID");

			lock (_ids)
			{
				if (!_ids.Contains(companyID))
					_ids.Add(companyID);
			}
		}

		public void RemoveCompany(ICompany company)
		{
			if (company == null)
				throw new ArgumentNullException("company");

			lock (_ids)
				_ids.Remove(company.Id);
		}

		public void RemoveCompany(int companyId)
		{
			if (companyId < 1)
				throw new ArgumentOutOfRangeException("companyId");

			lock (_ids)
				_ids.Remove(companyId);
		}

		public bool Contains(ICompany company)
		{
			return _ids.Contains(company.Id);
		}

		public bool Contains(int companyId)
		{
			return _ids.Contains(companyId);
		}

		public void Clear()
		{
			lock (_ids)
				_ids.Clear();
		}
		#endregion

		#region IEnumerable methods
        public IEnumerator<ICompany> GetEnumerator()
        {
        	return _ids.Select(id => Controller.Instance.CompanyController.GetCompany(id)).GetEnumerator();
        }

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _ids.Select(id => Controller.Instance.CompanyController.GetCompany(id)).GetEnumerator();
		}
		#endregion
	}
}