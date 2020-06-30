using System;
using System.Collections.Generic;
using System.Drawing;

namespace MotoProfessional.Models.Interfaces
{
    public interface IDigitalGood
    {
        DigitalGoodType Type { get; set; }
        string Filename { get; set; }
        Size Size { get; set; }
        long Filesize { get; set; }
        bool FileExists { get; set; }
        DateTime FileCreationDate { get; set; }

        /// <summary>
        /// The Order that this good may relate to.
        /// </summary>
        IOrder Order { get; set; }

        /// <summary>
        /// The OrderItem that this good may relate to.
        /// </summary>
        IOrderItem OrderItem { get; set; }

        List<IDigitalGoodDownloadLog> Logs { get; }

        /// <summary>
        /// The full file or unc path to the DigitalGood file. Access type determined by application configuration.
        /// </summary>
        string FullStorePath { get; }

        /// <summary>
        /// Provides a unique key that identifies the order and order-item (if apt).
        /// </summary>
        string SerialNumber { get; }

        /// <summary>
        /// The identifier for the object.
        /// </summary>
        int Id { get; set; }

        /// <summary>
        /// If set, denotes the type of object the implementer is.
        /// </summary>
        DomainObject DomainObject { get; set; }

        DateTime Created { get; set; }
        DateTime LastUpdated { get; set; }

        /// <summary>
        /// Denotes whether or not the object has been persisted to the database.
        /// </summary>
        bool IsPersisted { get; set; }

        IDigitalGoodDownloadLog NewLog();

        /// <summary>
        /// Adds a DigitalGoodDownloadLog to the download logs collection and persists it.
        /// </summary>
        void AddLog(IDigitalGoodDownloadLog downloadLog);

        /// <summary>
        /// Determines whether or not this object is valid for use and persistence.
        /// </summary>
        bool IsValid();
    }
}