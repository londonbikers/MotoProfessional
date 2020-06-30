using MotoProfessional.Models.Interfaces;

namespace MotoProfessional.Models
{
	/// <summary>
	/// Represents an amount for a specific license on a rate-card.
	/// </summary>
	public class RateCardItem : CommonBase, IRateCardItem
	{
		#region accessors
		public IRateCard RateCard { get; set; }
		public ILicense License { get; set; }
		public decimal Amount { get; set; }
		#endregion

		#region static accessors
		public static DomainObject DomainObjectType { get { return DomainObject.RateCardItem; } }
		#endregion

		#region constructors
		internal RateCardItem(ClassMode mode)
        {
            if (mode == ClassMode.Existing)
                base.IsPersisted = true;

			DomainObject = DomainObjectType;
        }
		#endregion
	}
}