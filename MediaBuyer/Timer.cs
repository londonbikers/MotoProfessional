using System;

namespace MotoProfessional
{
	/// <summary>
	/// Logs how long operations take within the application.
	/// </summary>
	public class Timer
	{
		#region members
		private readonly DateTime _start;
		private readonly string _name;
		#endregion

		#region constructors
		internal Timer(string name)
		{
			_name = name;
			_start = DateTime.Now;
		}
		#endregion

		#region public methods
		public void Stop()
		{
			var runTime = DateTime.Now - _start;
			Controller.Instance.Logger.LogInfo(string.Format("Timer ({0}) - Runtime: {1}", _name, runTime));
		}
		#endregion
	}
}