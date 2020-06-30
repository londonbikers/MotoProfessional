using System.Linq;
using MotoProfessional.Models.Interfaces;

namespace MotoProfessional.Models
{
	public class LatestCompanies : CompanyContainer, ILatestCompanies
	{
		#region members
		private readonly int _maxCompanies;
		#endregion

		#region constructors
        internal LatestCompanies(int maxCompanies)
		{
			_maxCompanies = maxCompanies;
			Reload();
		}
		#endregion

		#region public methods
		public void Reload()
		{
			Clear();
			var db = new MotoProfessionalDataContext();
			var ids = (from c in db.DbCompanies
					   orderby c.Created descending
					   select c.ID).Take(_maxCompanies);

			foreach (var id in ids)
				AddCompany(id);
		}
		#endregion
	}
}