using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MotoProfessional.Models;
using MotoProfessional.Models.Interfaces;
using MPN.Framework.Caching;
using MPN.Framework.Data;

namespace MotoProfessional.Controllers
{
    public class PeripheralController
    {
        #region constructors
        internal PeripheralController()
        {
        }
        #endregion

        #region retrieval methods
        /// <summary>
        /// Retrieves a collection of all the countries in the system.
        /// </summary>
        public List<ICountry> GetCountries()
        {
            var countries = CacheManager.RetrieveItem("GetCountries()", 0, "Output") as List<ICountry>;
            if (countries == null)
            {
                countries = new List<ICountry>();
                var db = new MotoProfessionalDataContext();
                var dbCountries = from c in db.DbCountries select c;

                foreach (var dbCountry in dbCountries)
                {
                    var country = new Country(ClassMode.Existing)
                    {
                        Id = dbCountry.ID,
                        Name = dbCountry.Name,
                        Numeric = dbCountry.Numeric,
                        Alpha2 = dbCountry.Alpha_2,
                        Alpha3 = dbCountry.Alpha_3
                    };

                    countries.Add(country);
                }

                CacheManager.AddItem(countries, "GetCountries()", 0, "Output");
            }

            return countries;
        }

        /// <summary>
        /// Returns a specific Country by its ID.
        /// </summary>
        public ICountry GetCountry(int id)
        {
        	return id < 1 ? null : GetCountries().Find(c => c.Id == id);
        }
    	#endregion

		#region web methods
		/// <summary>
		/// REFACTOR: The application needs to know about client-application urls from time to time, so
		/// we're centralising all the generation here. Ideally the app should be client agnostic.
		/// </summary>
		public string GetClientUrl(ClientUrlPage page, object subject)
		{
			var c = HttpContext.Current;
			if (page == ClientUrlPage.OrderPage)
			{
				var o = subject as Order;
				if (o != null) return string.Format("http://{0}/account/orders/{1}", c.Request.Url.Host, o.Id);
			}

			return string.Empty;
		}

		/// <summary>
		/// Returns a large collection of items outlining key domain-objects within the application.
		/// </summary>
		public List<ISiteMapItem> GetSiteMapItems()
		{
			var db = new MotoProfessionalDataContext();
			var connection = new SqlConnection(db.Connection.ConnectionString);
			var command = new SqlCommand("GetSiteMapItems", connection) {CommandType = CommandType.StoredProcedure};
			SqlDataReader reader = null;
			var items = new List<ISiteMapItem>();

			try
			{
				// collections.
				connection.Open();
				reader = command.ExecuteReader();
				if (reader != null)
					while (reader.Read())
					{
						var item = new SiteMapItem
			           	{
			           		ItemId = (int) Data.GetValue(typeof (int), reader["ID"]),
			           		Title = Data.GetValue(typeof (string), reader["Title"]) as string,
			           		ContentType = SiteMapItemContentType.Collection,
			           		LastModified = (DateTime) Data.GetValue(typeof (DateTime), reader["LastModified"])
			           	};
						items.Add(item);
					}
			}
			catch (Exception ex)
			{
				Controller.Instance.Logger.LogError("GetSiteMapItems() - Enumeration failed.", ex);

				#if DEBUG
				throw;
				#endif
			}
			finally
			{
				if (reader != null)
					reader.Close();

				if (db.Connection != null)
					db.Connection.Close();
			}

			return items;
		}
		#endregion
	}
}