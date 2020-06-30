using System.Linq;
using MotoProfessional.Models.Interfaces;

namespace MotoProfessional.Models
{
	public class LatestOrders : OrderContainer, ILatestOrders
	{
		#region members
		private readonly int _maxOrders;
		#endregion

		#region constructors
		internal LatestOrders(int maxOrders)
		{
			_maxOrders = maxOrders;
			Reload();
		}
		#endregion

		#region public methods
		public void Reload()
		{
			Clear();
			var db = new MotoProfessionalDataContext();
			var ids = (from o in db.DbOrders
					   orderby o.Created descending
					   select o.ID).Take(_maxOrders);

			foreach (var id in ids)
				AddOrder(id);
		}
		#endregion
	}
}