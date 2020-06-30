using System.Linq;
using MotoProfessional.Models.Interfaces;

namespace MotoProfessional.Models
{
	public class LatestCollections : CollectionContainer, ILatestCollections
	{
		#region members
		private readonly int _maxCollections;
		private readonly int _partnerId;
		#endregion

		#region constructors
		internal LatestCollections(int maxCollections)
		{
			_maxCollections = maxCollections;
			Reload();
		}

		internal LatestCollections(int maxCollections, int partnerId)
		{
			_partnerId = partnerId;
			_maxCollections = maxCollections;
			Reload();
		}
		#endregion

		#region public methods
		public void Reload()
		{
			Clear();
			var db = new MotoProfessionalDataContext();

			if (_partnerId > 0)
			{
				// partner collections - all, irrespective of status. filter at the consumer level.
				var ids = (from c in db.DbCollections
                           where c.PartnerID == _partnerId
						   orderby c.Created descending
						   select c.ID).Take(_maxCollections);

				foreach (var id in ids)
					AddCollection(id);
			}
			else
			{
				// global collections - live only.
				var ids = (from c in db.DbCollections
						   where c.Status == (byte)GeneralStatus.Active
						   orderby c.Created descending
						   select c.ID).Take(_maxCollections);

				foreach (var id in ids)
					AddCollection(id);
			}
		}
		#endregion
	}
}