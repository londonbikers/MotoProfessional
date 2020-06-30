using System;
using System.Drawing;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using MotoProfessional.Models.Interfaces;

namespace MotoProfessional.Models
{
    public class DigitalGood : CommonBase, IDigitalGood
    {
        #region members
    	private List<IDigitalGoodDownloadLog> _logs;
        #endregion

        #region accessors
        public DigitalGoodType Type { get; set; }
        public string Filename { get; set; }
    	public Size Size { get; set; }
    	public long Filesize { get; set; }
        public bool FileExists { get; set; }
        public DateTime FileCreationDate { get; set; }
		/// <summary>
		/// The Order that this good may relate to.
		/// </summary>
		public IOrder Order { get; set; }
		/// <summary>
		/// The OrderItem that this good may relate to.
		/// </summary>
		public IOrderItem OrderItem { get; set; }
		public List<IDigitalGoodDownloadLog> Logs
		{
			get
			{
				if (_logs == null)
					RetrieveLogs();

				return _logs;
			}
		}
		/// <summary>
		/// The full file or unc path to the DigitalGood file. Access type determined by application configuration.
		/// </summary>
		public string FullStorePath { get { return RootStorePath + Filename; } }
		/// <summary>
		/// Provides a unique key that identifies the order and order-item (if apt).
		/// </summary>
		public string SerialNumber
		{
			get
			{
				return Type == DigitalGoodType.Photo ? string.Format("{0}.{1}", OrderItem.Order.Id, OrderItem.Id) : Order.Id.ToString();
			}
		}
		/// <summary>
		/// The root local or UNC containing folder where the DigitalGood file is stored.
		/// </summary>
		internal string RootStorePath
		{
			get
			{
				var orderId = (Order != null) ? Order.Id : OrderItem.Order.Id;
				var customerUid = (Order != null) ? (Guid)Order.Customer.MembershipUser.ProviderUserKey : (Guid)OrderItem.Order.Customer.MembershipUser.ProviderUserKey;
				return string.Format(@"{0}Customers\{1}\{2}\", ConfigurationManager.AppSettings["MediaPath"], customerUid, orderId);
			}
		}
	    #endregion

        #region static accessors
        public static DomainObject DomainObjectType { get { return DomainObject.DigitalGood; } }
        #endregion

        #region constructors
		internal DigitalGood(ClassMode mode)
        {
            if (mode == ClassMode.Existing)
                base.IsPersisted = true;

            DomainObject = DomainObjectType;
        }
		#endregion

        #region public methods
        public IDigitalGoodDownloadLog NewLog()
        {
            var log = new DigitalGoodDownloadLog(ClassMode.New) {DigitalGood = this};
        	return log;
        }

        /// <summary>
        /// Adds a DigitalGoodDownloadLog to the download logs collection and persists it.
        /// </summary>
        public void AddLog(IDigitalGoodDownloadLog downloadLog)
        {
            if (downloadLog == null || Logs.Exists(ql => ql.Id == downloadLog.Id))
                return;

            if (!IsPersisted)
                throw new Exception("DigitalGood must be persisted first before adding a download log!");

			if (!downloadLog.IsPersisted)
				Controller.Instance.DigitalGoodController.UpdateDigitalGoodDownloadLog(downloadLog);

			Logs.Add(downloadLog);
        }

		/// <summary>
		/// Determines whether or not this object is valid for use and persistence.
		/// </summary>
		public bool IsValid()
		{
			return Order != null || OrderItem != null;
		}
    	#endregion

        #region private methods
        private void RetrieveLogs()
		{
			_logs = new List<IDigitalGoodDownloadLog>();
			var db = new MotoProfessionalDataContext();
			var logs = from l in db.DbDigitalGoodsDownloadLogs
					   where l.DigitalGoodID == Id
					   orderby l.Created
					   select l;

            foreach (var dbLog in logs)
                _logs.Add(Controller.Instance.DigitalGoodController.BuildDigitalGoodDownloadLogObject(this, dbLog));
		}
		#endregion
	}
}