namespace MotoProfessional.Exceptions
{
	public class PhotoImportException : BaseException
    {
		public PhotoImportException(string clientMessage)
		{
			ClientMessage = clientMessage;
		}
    }
}