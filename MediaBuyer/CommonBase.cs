using System;

namespace MotoProfessional
{
	public abstract class CommonBase
	{
		#region public accessors
		/// <summary>
		/// The identifier for the object.
		/// </summary>
		public int Id { get; set; }
		/// <summary>
		/// If set, denotes the type of object the implementer is.
		/// </summary>
		public DomainObject DomainObject { get; set; }
		public DateTime Created { get; set; }
		public DateTime LastUpdated { get; set; }
		/// <summary>
		/// Denotes whether or not the object has been persisted to the database.
		/// </summary>
		public bool IsPersisted { get; set; }
		#endregion

		#region constructors
		internal CommonBase()
		{
			Id = -1;
			IsPersisted = false;
			DomainObject = DomainObject.Unknown;
			Created = DateTime.Now;
			LastUpdated = DateTime.Now;
		}
		#endregion
	}
}