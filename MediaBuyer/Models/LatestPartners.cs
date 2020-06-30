using System.Linq;
using MotoProfessional.Models.Interfaces;

namespace MotoProfessional.Models
{
	public class LatestPartners : PartnerContainer, ILatestPartners
	{
		#region members
		private readonly int _maxObjects;
		#endregion

		#region constructors
		internal LatestPartners(int maxObjects)
		{
			_maxObjects = maxObjects;
			Reload();
		}
		#endregion

		#region public methods
		public void Reload()
		{
			Clear();
			var db = new MotoProfessionalDataContext();

			var ids = (from c in db.DbPartners
					   orderby c.Created descending
					   select c.ID).Take(_maxObjects);

			foreach (var id in ids)
				AddPartner(id);
		}
		#endregion
	}
}