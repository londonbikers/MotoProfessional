using System;
using System.Configuration;
using System.Linq;
using System.IO;
using System.Drawing;
using System.Threading;
using MotoProfessional.Models;
using ICSharpCode.SharpZipLib.Zip;
using MotoProfessional.Models.Interfaces;
using MPN.Framework.Files;
using MPN.Framework.Imaging;

//using SE.Halligang.CsXmpToolkit;
//using SE.Halligang.CsXmpToolkit.Schemas;

namespace MotoProfessional.Controllers
{
    public class DigitalGoodController
	{
		#region accessors
		/// <summary>
		/// The root file-system location for customer Digital-Goods.
		/// </summary>
		public string RootDigitalGoodStore { get { return string.Format(@"{0}Customers\", ConfigurationManager.AppSettings["MediaPath"]); } }
		#endregion

		#region constructors
		internal DigitalGoodController()
        {
        }
        #endregion

        #region digital good methods
		/// <summary>
		/// Initiates the generation of the digital goods in an asyncronous fashion as so not to hold up further work. 
		/// Generation can take a few seconds, depending on server load due to high CPU usage.
		/// </summary>
		/// <remarks>
		/// TODO: To truely scale, this operation needs to be farmed out to a dedicated processing farm.
		/// for now though we can undertake it on the web-server due to the expected abundance of cpu power (quad-core, baby).
		/// </remarks>
        internal void CreateDigitalGoodsAsync(IOrder order)
        {
			var queued = ThreadPool.QueueUserWorkItem(PerformDigitalGoodsCreation, order);
			if (!queued)
				Controller.Instance.Logger.LogError("CreateDigitalGoodsAsync() - Work could not be queued.");
        }

		/// <summary>
		/// Persists any changes to a new or existing DigitalGood object.
		/// </summary>
		internal void UpdateDigitalGood(IDigitalGood digitalGood)
		{
			if (digitalGood == null)
			{
				Controller.Instance.Logger.LogWarning("UpdateDigitalGood() - Null DigitalGood passed in.");
				return;
			}

			var db = new MotoProfessionalDataContext();
		    digitalGood.LastUpdated = DateTime.Now;
			var dbDg = digitalGood.IsPersisted ? db.DbDigitalGoods.Single(dg => dg.ID == digitalGood.Id) : new DbDigitalGood();

            if (digitalGood.Order != null)
            {
                dbDg.OrderID = digitalGood.Order.Id;
                dbDg.OrderItemID = null;
            }
            else
            {
                dbDg.OrderItemID = digitalGood.OrderItem.Id;
                dbDg.OrderID = null;
            }

			dbDg.Type = (byte)digitalGood.Type;
			dbDg.Filename = (!string.IsNullOrEmpty(digitalGood.Filename)) ? digitalGood.Filename : null;

			if (!digitalGood.Size.IsEmpty)
			{
				dbDg.Width = digitalGood.Size.Width;
				dbDg.Height = digitalGood.Size.Height;
			}
			else
			{
				dbDg.Width = null;
				dbDg.Height = null;
			}

			if (digitalGood.Filesize > 0)
				dbDg.Filesize = digitalGood.Filesize;
			else
				dbDg.Filesize = null;

			dbDg.FileExists = digitalGood.FileExists;
			dbDg.Created = digitalGood.Created;
            dbDg.LastUpdated = digitalGood.LastUpdated;

			if (digitalGood.FileCreationDate != DateTime.MinValue)
				dbDg.FileCreationDate = digitalGood.FileCreationDate;
			else
				dbDg.FileCreationDate = null;

			if (!digitalGood.IsPersisted)
				db.DbDigitalGoods.InsertOnSubmit(dbDg);

			try
			{
				db.SubmitChanges();
			}
			catch (Exception ex)
			{
				Controller.Instance.Logger.LogError("UpdateDigitalGood() - Main update failed.", ex);
				return;
			}

			if (digitalGood.IsPersisted) return;
			digitalGood.Id = dbDg.ID;
			digitalGood.IsPersisted = true;
		}

		internal IDigitalGood BuildDigitalGoodObject(DbDigitalGood dbDigitalGood)
		{
			var dg = new DigitalGood(ClassMode.Existing)
         	{
         		Id = dbDigitalGood.ID,
         		Type = (DigitalGoodType) dbDigitalGood.Type,
         		Filename = dbDigitalGood.Filename,
         		FileExists = dbDigitalGood.FileExists,
         		Created = dbDigitalGood.Created
         	};

			if (dbDigitalGood.Width.HasValue && dbDigitalGood.Height.HasValue)
				dg.Size = new Size(dbDigitalGood.Width.Value, dbDigitalGood.Height.Value);

			if (dbDigitalGood.FileCreationDate.HasValue)
				dg.FileCreationDate = dbDigitalGood.FileCreationDate.Value;

			if (dbDigitalGood.Filesize.HasValue)
				dg.Filesize = dbDigitalGood.Filesize.Value;

			return dg;
		}
        #endregion

        #region log methods
		/// <summary>
		/// Persists any changes to a new or existing DigitalGoodDownloadLog object.
		/// </summary>
        internal void UpdateDigitalGoodDownloadLog(IDigitalGoodDownloadLog log)
        {
			if (log == null || !log.IsValid())
			{
				Controller.Instance.Logger.LogWarning("UpdateDigitalGoodDownloadLog() - Null or invalid DigitalGoodDownloadLog passed in.");
				return;
			}

			var db = new MotoProfessionalDataContext();
			var dbLog = log.IsPersisted ? db.DbDigitalGoodsDownloadLogs.Single(ql => ql.ID == log.Id) : new DbDigitalGoodsDownloadLog();

			dbLog.ClientName = (!string.IsNullOrEmpty(log.ClientName)) ? log.ClientName : null;
			dbLog.Created = log.Created;
			dbLog.CustomerUID = log.CustomerUid;
			dbLog.DigitalGoodID = log.DigitalGood.Id;
			dbLog.HttpReferrer = (!string.IsNullOrEmpty(log.HttpReferrer)) ? log.HttpReferrer : null;
            dbLog.IPAddress = (!string.IsNullOrEmpty(log.IpAddress)) ? log.IpAddress : null;

			if (!log.IsPersisted)
				db.DbDigitalGoodsDownloadLogs.InsertOnSubmit(dbLog);

			try
			{
				db.SubmitChanges();
			}
			catch (Exception ex)
			{
				Controller.Instance.Logger.LogError("UpdateDigitalGoodDownloadLog() - Main update failed.", ex);
				return;
			}

			if (log.IsPersisted) return;
			log.Id = dbLog.ID;
			log.IsPersisted = true;
        }

        internal IDigitalGoodDownloadLog BuildDigitalGoodDownloadLogObject(IDigitalGood digitalGood, DbDigitalGoodsDownloadLog dbLog)
        {
            var log = new DigitalGoodDownloadLog(ClassMode.Existing)
          	{
          		Id = dbLog.ID,
          		Created = dbLog.Created,
          		DigitalGood = digitalGood,
          		ClientName = dbLog.ClientName,
          		CustomerUid = dbLog.CustomerUID,
          		HttpReferrer = dbLog.HttpReferrer,
          		IpAddress = dbLog.IPAddress
          	};

        	return log;
        }
        #endregion

		#region production methods
		/// <summary>
		/// Creates the actual digital-good files for each order-item and the order archive and records them against the Order object.
		/// </summary>
		private void PerformDigitalGoodsCreation(object untypedOrder)
		{
			try
			{
				var order = untypedOrder as Order;
				if (order != null)
				{
					var timer = new Timer(string.Format("DigitalGoodsCreation - order: {0}", order.Id));
					order.DigitalGoodsInProduction = true;
					if (!order.IsPersisted || order.ChargeStatus != ChargeStatus.Complete)
					{
						Controller.Instance.Logger.LogWarning("CreateDigitalGoods() - Null, unpersisted or incomplete order passed in.");
						return;
					}

					foreach (var item in order.Items)
						CreateDigitalGood(item);

					// now the individual digital-goods have been created, we can bundle them all up into a single archive.
					// give it ten attempts (found some weird log entry where the master file was in use, so trying this).
					IDigitalGood dg = null;
					var counter = 0;
					while (dg == null || counter < 50)
					{
						dg = CreateMasterDigitalGood(order);
						counter++;
					}

					order.MasterDigitalGood = dg;
					order.DigitalGoodsInProduction = false;
					timer.Stop();
				}
			}
			catch (Exception ex)
			{
				// because this is on another thread, we need to have some general logging in place.
				Controller.Instance.Logger.LogError("PerformDigitalGoodsCreation() - General creation failed.", ex);
			}
		}

		private void CreateDigitalGood(IOrderItem item)
		{
			try
			{
				var dg = new DigitalGood(ClassMode.New) {OrderItem = item};
				if (item.ProductType == ProductType.PhotoProduct)
				{
					dg.Type = DigitalGoodType.Photo;
					dg.Filename = item.PhotoProduct.Photo.Filename;

					// TODO: extend the resizer to embed digital-watermark meta-data.
					var ir = new ImageResizer();
					ir.SaveResizeImage(
						item.PhotoProduct.Photo.FullStorePath,
						dg.FullStorePath,
						item.PhotoProduct.License.PrimaryDimension,
						false,
						ImageResizer.Axis.Undefined,
						null);

					dg.FileCreationDate = DateTime.Now;
					dg.Filesize = Files.GetFileSize(dg.FullStorePath);
					dg.Size = ImageHelper.GetImageDimensions(dg.FullStorePath);
					dg.FileExists = true;
					
					// we can and must do this before persisting so that the file is available in its entirity once persisted.
					WatermarkDigitalGood(dg);
				}

				item.DigitalGood = dg;
				Controller.Instance.DigitalGoodController.UpdateDigitalGood(dg);
			    return;
			}
			catch (Exception ex)
			{
				Controller.Instance.Logger.LogError("CreateDigitalGood() - Creation failed.", ex);
			    return;
			}
		}

		/// <summary>
		/// Creates a DigitalGood that encapsulates all of the OrderItem level DigitalGoods in a Zip archive.
		/// </summary>
		private IDigitalGood CreateMasterDigitalGood(IOrder order)
		{
			try
			{
				var dg = new DigitalGood(ClassMode.New)
	         	{
	         		Type = DigitalGoodType.ZipArchive,
	         		Order = order,
	         		Filename = string.Format("{0}-Order-{1}.zip", ConfigurationManager.AppSettings["ServiceShortName"], order.Id)
	         	};

				#region zip creation
				// 'using' statements gaurantee the stream is closed properly which is a big source
				// of problems otherwise.  Its exception safe as well which is great.
				using (var s = new ZipOutputStream(File.Create(dg.FullStorePath)))
				{
					s.SetLevel(0); // 0 - store only to 9 - means best compression
					var buffer = new byte[4096];

					foreach (var item in order.Items)
					{
						if (item.DigitalGood == null || !item.DigitalGood.IsValid())
						{
							Controller.Instance.Logger.LogWarning("CreateMasterDigitalGood() - item.DigitalGood is null or invalid!");
							continue;
						}

						// Using GetFileName makes the result compatible with XP
						// as the resulting path is not absolute.
						var entry = new ZipEntry(item.DigitalGood.Filename) {DateTime = DateTime.Now};

						// Setup the entry data as required.
						// Crc and size are handled by the library for seakable streams
						// so no need to do them here.
						// Could also use the last write time or similar for the file.
					    s.PutNextEntry(entry);

						using (var fs = File.OpenRead(item.DigitalGood.FullStorePath))
						{
							// Using a fixed size buffer here makes no noticeable difference for output
							// but keeps a lid on memory usage.
							int sourceBytes;
							do
							{
								sourceBytes = fs.Read(buffer, 0, buffer.Length);
								s.Write(buffer, 0, sourceBytes);
							} while (sourceBytes > 0);
						}
					}

					// Finish/Close arent needed strictly as the using statement does this automatically
					// Finish is important to ensure trailing information for a Zip file is appended.  Without this
					// the created file would be invalid.
					s.Finish();

					// Close is important to wrap things up and unlock the file.
					s.Close();
				}
				#endregion

				dg.FileExists = true;
				dg.FileCreationDate = DateTime.Now;
				dg.Filesize = Files.GetFileSize(dg.FullStorePath);

				Controller.Instance.DigitalGoodController.UpdateDigitalGood(dg);
				return dg;
			}
			catch (Exception ex)
			{
				Controller.Instance.Logger.LogError("CreateMasterDigitalGood() - Creation failed.", ex);
				return null;
			}
		}

		/// <summary>
		/// Embeds XMP meta-data into applicable DigitalGood files relating to the derived
		/// good and order information for security tracking purposes.
		/// </summary>
		private void WatermarkDigitalGood(DigitalGood digitalGood)
		{
            //try
            //{
            //    if (digitalGood.Type == DigitalGoodType.Photo)
            //    {
            //        // alias' for convenience.
            //        Photo p = digitalGood.OrderItem.PhotoProduct.Photo;
            //        License l = digitalGood.OrderItem.PhotoProduct.License;

            //        using (Xmp xmp = Xmp.FromFile(digitalGood.FullStorePath, XmpFileMode.ReadWrite))
            //        {
            //            #region dublin core data
            //            DublinCore dc = new DublinCore(xmp);

            //            dc.Subject.Clear();
            //            foreach (string tag in p.Tags)
            //                dc.Subject.Add(tag);

            //            dc.Source = ConfigurationManager.AppSettings["FormalServiceName"];
            //            dc.Title.Clear();
            //            dc.Title.Add(XmpLang.English_UnitedKingdom, p.Name);
            //            dc.Description.Clear();

            //            if (!string.IsNullOrEmpty(p.Comment))
            //                dc.Description.Add(XmpLang.English_UnitedKingdom, p.Comment);

            //            dc.Creator.Clear();
            //            if (p.Photographer != null)
            //                dc.Creator.Add(p.Photographer.GetFullName());
            //            #endregion

            //            #region xmp rights data
            //            XmpRights xmpRights = new XmpRights(xmp);
            //            xmpRights.Owner.Clear();

            //            string license = string.Format("{0}. Licensed to {1}, {2}.", l.Name, digitalGood.OrderItem.Order.Customer.GetFullName(), digitalGood.OrderItem.Order.Customer.Company.Name);
            //            xmpRights.UsageTerms.Clear();
            //            xmpRights.UsageTerms.Add(XmpLang.English_UnitedKingdom, license);
            //            #endregion

            //            #region exif data
            //            ExifAdditional aux = new ExifAdditional(xmp);
            //            aux.SerialNumber = digitalGood.SerialNumber;
            //            #endregion

            //            #region xmp basic data
            //            XmpBasic xmpBasic = new XmpBasic(xmp);
            //            xmpBasic.BaseUrl = ConfigurationManager.AppSettings["FormalServiceUrl"];
            //            #endregion

            //            // See what's about to be stored.
            //            // Set a breakpoint on the following line and
            //            // inspect the xmpDump variable.
            //            xmp.Save();
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Controller.Instance.Logger.LogError(string.Format("WatermarkDigitalGood() - General failure.\nDigitalGood: {0}\nPath: {1}", digitalGood.ID, digitalGood.FullStorePath), ex);
            //}

            return;
		}
		#endregion
    }
}