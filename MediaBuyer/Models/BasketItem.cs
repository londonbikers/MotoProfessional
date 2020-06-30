using MotoProfessional.Models.Interfaces;

namespace MotoProfessional.Models
{
    public class BasketItem : CommonBase, IBasketItem
    {
        #region accessors
    	public IPhotoProduct PhotoProduct { get; set; }
    	#endregion

        #region static accessors
		public static DomainObject DomainObjectType { get { return DomainObject.BasketItem; } }
		#endregion

		#region constructors
		internal BasketItem(ClassMode mode)
        {
            if (mode == ClassMode.Existing)
                base.IsPersisted = true;

            DomainObject = DomainObjectType;
        }
		#endregion
    }
}