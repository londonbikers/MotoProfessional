using System.Linq;

namespace MotoProfessional.Controllers.Application
{
	/// <summary>
	/// Some basic application statistics.
	/// </summary>
	public class Statistics
	{
		#region accessors
		public int Partners { get; internal set; }
		public int Collections { get; internal set; }
		public int Photos { get; internal set; }
		public decimal PhotosSizeGigabytes { get; internal set; }
		public int Members { get; internal set; }
		public int CountriesRepresented { get; internal set; }
		public int Baskets { get; internal set; }
		public int CompleteOrders { get; internal set; }
		public int IncompleteOrders { get; internal set; }
		public int Downloads { get; internal set; }
		public int Tags { get; internal set; }
		#endregion

		#region constructors
		internal Statistics()
		{
			RebuildStatistics();
		}
		#endregion

		#region public methods
		public void RebuildStatistics()
		{
			var db = new MotoProfessionalDataContext();

			Partners = db.DbPartners.Count();
			Collections = db.DbCollections.Count();
			Photos = db.DbPhotos.Count();
			PhotosSizeGigabytes = (decimal)db.DbPhotos.Sum(p => p.Filesize) / (decimal)1073741824;
			CountriesRepresented = db.DbProfiles.GroupBy(p => p.BillingCountryID).Count();
			Baskets = db.DbBaskets.Count();
			CompleteOrders = db.DbOrders.Count(o => o.ChargeStatus == (byte)ChargeStatus.Complete);
			IncompleteOrders = db.DbOrders.Count(o => o.ChargeStatus == (byte)ChargeStatus.Outstanding);
			Downloads = db.DbDigitalGoodsDownloadLogs.Count();
			Members = db.aspnet_Users.Count();
			//Tags = 
		}
		#endregion
	}
}