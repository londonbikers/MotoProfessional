using System;

namespace MotoProfessional.Exceptions
{
	public abstract class BaseException : Exception
	{
		public string ClientMessage { get; set; }
	}
}