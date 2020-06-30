namespace MotoProfessional.Exceptions
{
	public class TransactionException : BaseException
	{
		public TransactionException(string clientMessage)
		{
			ClientMessage = clientMessage;
		}
	}
}