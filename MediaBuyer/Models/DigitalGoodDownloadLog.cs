using System;
using MotoProfessional.Models.Interfaces;

namespace MotoProfessional.Models
{
	/// <summary>
	/// Represents a log of a customer attempt to download a DigitalGood from the service.
	/// </summary>
	public class DigitalGoodDownloadLog : CommonBase, IDigitalGoodDownloadLog
	{
		#region accessors
		public IDigitalGood DigitalGood { get; set; }
        public Guid CustomerUid { get; set; }
        public string IpAddress { get; set; }
		public string HttpReferrer { get; set; }
		public string ClientName { get; set; }
		#endregion

		#region constructors
        internal DigitalGoodDownloadLog(ClassMode mode)
        {
            if (mode == ClassMode.Existing)
                base.IsPersisted = true;
        }
		#endregion

        #region public methods
        public bool IsValid()
        {
            if (DigitalGood == null)
                return false;

			if (!DigitalGood.IsPersisted)
				return false;

            return CustomerUid != Guid.Empty;
        }
        #endregion
    }
}