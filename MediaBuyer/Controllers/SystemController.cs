using System.Collections.Generic;
using MotoProfessional.Controllers.Application;
using MPN.Framework.Caching;

namespace MotoProfessional.Controllers
{
	public class SystemController
	{
		#region members
		private Statistics _stats;
		#endregion

		#region accessors
		public Statistics BasicStatistics
		{
			get
			{
				return _stats ?? (_stats = new Statistics());
			}
		}
		#endregion

		#region constructors
		internal SystemController()
		{
		}
		#endregion

		#region caching methods
		/// <summary>
		/// Clears the application cache to ensure all content is re-loaded from the database.
		/// </summary>
		public void ClearCache()
		{
			CacheManager.FlushCache();
			Controller.Instance.CommerceController.LatestOrders = null;
			Controller.Instance.PhotoController.LatestCollections = null;
			Controller.Instance.PhotoController.HotPhotos = null;
		}

		/// <summary>
		/// For administration purposes, this method can be used to get the most popular items currently in the cache.
		/// </summary>
		/// <param name="max">The maximum number of cache items to return.</param>
		public List<CacheItem> GetTopCacheItems(int max)
		{
			return CacheManager.RetrieveTopItems(max);
		}

		/// <summary>
		/// Returns the total number of items in the cache.
		/// </summary>
		public int GetCacheSize()
		{
			return CacheManager.ItemCount;
		}

		/// <summary>
		/// The maximum numbers of items the cache is configured to accept before removing unpopular items.
		/// </summary>
		public int GetCacheCeiling()
		{
			return CacheManager.ItemCeiling;
		}

		/// <summary>
		/// Returns the percentage of how much cache is used. This will often reach 100% fairly quickly.
		/// </summary>
		public decimal GetCacheCapacityUsed()
		{
			return CacheManager.CacheCapacityUsed;
		}
		#endregion
	}
}