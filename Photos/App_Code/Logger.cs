using System;
using System.Reflection;
using log4net;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config", Watch = true)]
namespace App_Code
{
    public class Logger
    {
        #region members
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region public methods
        public static void LogDebug(string message)
        {
            Log.Debug(message);
        }

        public static void LogInfo(string message)
        {
            Log.Info(message);
        }

        public static void LogWarning(string warning)
        {
            Log.Warn(warning);
        }

        public static void LogWarning(string warning, Exception exception)
        {
            Log.Warn(warning, exception);
        }

        public static void LogError(string message, Exception exception)
        {
            Log.Error(message, exception);
        }

        public static void LogError(string message)
        {
            Log.Error(message);
        }
        #endregion
    }
}