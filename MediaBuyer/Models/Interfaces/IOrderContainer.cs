using System.Collections.Generic;

namespace MotoProfessional.Models.Interfaces
{
    public interface IOrderContainer
    {
        /// <summary>
        /// The number of orders within the container.
        /// </summary>
        int Count { get; }

        IOrder this[int index] { get; }
        void AddOrder(IOrder order);
        void AddOrder(int orderId);
        void RemoveOrder(IOrder order);
        void RemoveOrder(int orderId);
        bool Contains(IOrder order);
        bool Contains(int orderId);
        void Clear();
        IEnumerator<IOrder> GetEnumerator();
    }
}