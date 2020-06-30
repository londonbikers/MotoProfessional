using System;
using System.Reflection;
using log4net;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config", Watch = true)]
namespace MotoProfessional.Logging
{
    public class Logger
    {
        #region members
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region constructors
        internal Logger()
        {
        }
        #endregion

        #region public methods
        public void LogDebug(string message)
        {
            Log.Debug(message);
        }

        public void LogInfo(string message)
        {
            Log.Info(message);
        }

        public void LogWarning(string warning)
        {
            Log.Warn(warning);
        }

        public void LogWarning(string warning, Exception exception)
        {
            Log.Warn(warning, exception);
        }

        public void LogError(string message, Exception exception)
        {
            Log.Error(message, exception);
        }

        public void LogError(string message)
        {
            Log.Error(message);
        }
        #endregion
    }
}