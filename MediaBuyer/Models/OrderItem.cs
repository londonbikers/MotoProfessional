using MotoProfessional.Models.Interfaces;

namespace MotoProfessional.Models
{
    public class OrderItem : CommonBase, IOrderItem
    {
        #region members

    	#endregion

        #region accessors
    	public IOrder Order { get; set; }
    	public IPhotoProduct PhotoProduct { get; set; }
    	public decimal SaleAmount { get; set; }
    	public OrderItemStatus Status { get; set; }
    	public ProductType ProductType { get; set; }
    	public IDigitalGood DigitalGood { get; set; }
    	#endregion

        #region static accessors
        public static DomainObject DomainObjectType { get { return DomainObject.OrderItem; } }
        #endregion

        #region constructors
        internal OrderItem(ClassMode mode)
        {
            if (mode == ClassMode.Existing)
                base.IsPersisted = true;

            DomainObject = DomainObjectType;
			ProductType = ProductType.PhotoProduct;
        }
        #endregion
    }
}