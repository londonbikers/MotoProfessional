using System;
using MotoProfessional.Models.Interfaces;

namespace MotoProfessional.Models
{
	public class TopPartners : PartnerContainer, ITopPartners
	{
		#region members
		private readonly int _maxObjects;
        private DateTime _lastRefresh;
		#endregion

		#region constructors
        internal TopPartners(int maxObjects)
		{
			_maxObjects = maxObjects;
            _lastRefresh = DateTime.MinValue;
			Reset();
		}
		#endregion

		#region public methods
		public void Reset()
		{
			if (_lastRefresh != DateTime.MinValue && DateTime.Now - _lastRefresh < TimeSpan.FromHours(12D)) return;
			_lastRefresh = DateTime.Now;
			Clear();
			var db = new MotoProfessionalDataContext();
			foreach (var result in db.GetCurrentTopPartners(_maxObjects))
				AddPartner(result.PartnerID);
		}
		#endregion
	}
}