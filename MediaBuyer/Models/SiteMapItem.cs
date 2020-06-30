using System;
using MotoProfessional.Models.Interfaces;

namespace MotoProfessional.Models
{
	public class SiteMapItem : ISiteMapItem
	{
		#region accessors
		public int ItemId { get; set; }
		public string Title { get; set; }
		public DateTime LastModified { get; set; }
		public string Keywords { get; set; }
		public SiteMapItemContentType ContentType { get; set; }
		#endregion

		#region constructors
		internal SiteMapItem()
		{
		}
		#endregion
	}
}