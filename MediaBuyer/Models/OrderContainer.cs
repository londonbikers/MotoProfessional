using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MotoProfessional.Models.Interfaces;

namespace MotoProfessional.Models
{
	/// <summary>
	/// Provides a lightweight means to hold a collection of orders. Internally only the ID's are stored
	/// and only upon retrieval is a whole Order object retrieved (from cache or db).
	/// </summary>
	public class OrderContainer : IEnumerable<IOrder>, IOrderContainer
	{
		#region members
		private readonly List<int> _ids;
		#endregion

		#region accessors
		/// <summary>
		/// The number of orders within the container.
		/// </summary>
		public int Count { get { return _ids.Count; } }
		#endregion

		#region constructors
		internal OrderContainer()
		{
			_ids = new List<int>();
		}
		#endregion

		#region indexers
		public IOrder this[int index]
		{
			get
			{
				if (index < 0 || index > _ids.Count - 1)
					throw new ArgumentOutOfRangeException();

				return Controller.Instance.CommerceController.GetOrder(_ids[index]);
			}
		}
		#endregion

		#region public methods
		public void AddOrder(IOrder order)
		{
			if (order == null)
				throw new ArgumentNullException("order");

			if (!_ids.Contains(order.Id))
				_ids.Add(order.Id);
		}

		public void AddOrder(int orderId)
		{
			if (orderId < 1)
				throw new ArgumentOutOfRangeException("orderId");

			if (!_ids.Contains(orderId))
				_ids.Add(orderId);
		}

		public void RemoveOrder(IOrder order)
		{
			if (order == null)
				throw new ArgumentNullException("order");

			_ids.Remove(order.Id);
		}

		public void RemoveOrder(int orderId)
		{
			if (orderId < 1)
				throw new ArgumentOutOfRangeException("orderId");

			_ids.Remove(orderId);
		}

		public bool Contains(IOrder order)
		{
			return _ids.Contains(order.Id);
		}

		public bool Contains(int orderId)
		{
			return _ids.Contains(orderId);
		}

		public void Clear()
		{
			_ids.Clear();
		}
		#endregion

		#region IEnumerable methods
        public IEnumerator<IOrder> GetEnumerator()
        {
        	return _ids.Select(id => Controller.Instance.CommerceController.GetOrder(id)).GetEnumerator();
        }

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _ids.Select(id => Controller.Instance.CommerceController.GetOrder(id)).GetEnumerator();
		}
		#endregion
	}
}