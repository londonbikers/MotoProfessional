using System;
using System.Collections.Generic;
using MotoProfessional.Models.Interfaces;

namespace MotoProfessional.Models
{
	/// <summary>
	/// Holds common media data from that embedded in files.
	/// </summary>
	public class MetaData : IMetaData
	{
		#region members

		#endregion

		#region accessors
		public string Name { get; set; }
		public string Comment { get; set; }
		public string Creator { get; set; }
		public List<string> Tags { get; set; }
		public DateTime Captured { get; set; }
		public IExtendedMetaData ExtendedData { get; set; }
		#endregion

		#region constructors
		internal MetaData()
		{
            Captured = DateTime.MinValue;
		}
		#endregion
	}
}