using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MotoProfessional.Models.Interfaces;

namespace MotoProfessional.Models
{
	/// <summary>
	/// Provides a lightweight means to hold a collection of companies. Internally only the ID's are stored
	/// and only upon retrieval is a whole Partner object retrieved (from cache or db).
	/// </summary>
	public class PartnerContainer : IEnumerable<IPartner>, IPartnerContainer
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
        internal PartnerContainer()
		{
			_ids = new List<int>();
		}
		#endregion

		#region indexers
		public IPartner this[int index]
		{
			get
			{
				if (index < 0 || index > _ids.Count - 1)
					throw new ArgumentOutOfRangeException();

				return Controller.Instance.PartnerController.GetPartner(_ids[index]);
			}
		}
		#endregion

		#region public methods
		public void AddPartner(IPartner partner)
		{
			if (partner == null)
				throw new ArgumentNullException("partner");

			lock (_ids)
			{
				if (!_ids.Contains(partner.Id))
					_ids.Add(partner.Id);
			}
		}

		public void AddPartner(int partnerId)
		{
			if (partnerId < 1)
				throw new ArgumentOutOfRangeException("partnerId");

			lock (_ids)
			{
				if (!_ids.Contains(partnerId))
					_ids.Add(partnerId);
			}
		}

		public void RemovePartner(IPartner partner)
		{
			if (partner == null)
				throw new ArgumentNullException("partner");

			lock (_ids)
				_ids.Remove(partner.Id);
		}

		public void RemovePartner(int partnerId)
		{
			if (partnerId < 1)
				throw new ArgumentOutOfRangeException("partnerId");

			lock (_ids)
				_ids.Remove(partnerId);
		}

		public bool Contains(IPartner partner)
		{
			return _ids.Contains(partner.Id);
		}

		public bool Contains(int partnerId)
		{
			return _ids.Contains(partnerId);
		}

		public void Clear()
		{
			lock (_ids)
				_ids.Clear();
		}
		#endregion

		#region IEnumerable methods
        public IEnumerator<IPartner> GetEnumerator()
        {
        	return _ids.Select(id => Controller.Instance.PartnerController.GetPartner(id)).GetEnumerator();
        }

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _ids.Select(id => Controller.Instance.PartnerController.GetPartner(id)).GetEnumerator();
		}
		#endregion
	}
}