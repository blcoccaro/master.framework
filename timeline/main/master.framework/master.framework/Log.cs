using log4net;
using System;
using System.Runtime.CompilerServices;

namespace master.framework
{
    /// <summary>
    /// Class that implement log4net
    /// </summary>
    public static class Log
    {
        private static ILog logger;
        private static void setLogger(Type classType)
        {
            logger = LogManager.GetLogger(classType);
            log4net.Config.XmlConfigurator.Configure();
        }

        /// <summary>
        /// Call record log
        /// </summary>
        /// <param name="logType"></param>
        /// <param name="classType"></param>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        /// <param name="methodName"></param>
        /// <param name="lineNumber"></param>
        private static void log(Enumerators.LogType logType, object classType, string message = null, Exception ex = null, string methodName = null, int lineNumber = 0)
        {
            string errorMsg = string.Empty;
            string recordMsg = ".{0} at line number: {1} with message: {2} {3}";
            setLogger(classType.GetType());
            if (ex != null)
            {
                dto.DataLogging dl = ex?.ToDataLogging();
                if (dl != null)
                {
                    errorMsg = string.Format("- |-| - Exception Source: {0} -- Message: {1} -- StackTrace: {2}", dl.Source, dl.Message, dl.StackTrace);
                }
            }

            recordMsg = string.Format(recordMsg, methodName, lineNumber, string.IsNullOrWhiteSpace(message) ? "-" : message, errorMsg);

            switch (logType)
            {
                case Enumerators.LogType.Debug:
                    logger.Debug(recordMsg);
                    break;
                case Enumerators.LogType.Info:
                    logger.Info(recordMsg);
                    break;
                case Enumerators.LogType.Warn:
                    logger.Warn(recordMsg);
                    break;
                case Enumerators.LogType.Fatal:
                    logger.Fatal(recordMsg);
                    break;
                case Enumerators.LogType.Error:
                    logger.Error(recordMsg);
                    break;
            }


        }


        /// <summary>
        /// Call log with level DEBUG
        /// </summary>
        /// <param name="classType">Current Class Type</param>
        /// <param name="message">Message to Log</param>
        public static void logDebug(this object classType, string message, [CallerMemberName]string memberName = "",
            [CallerLineNumberAttribute]int lineNumber = 0)
        {
            log(Enumerators.LogType.Debug, classType, message: message, methodName: memberName, lineNumber: lineNumber);
        }
        /// <summary>
        /// Call log with level ERROR
        /// </summary>
        /// <param name="classType">Current Class Type</param>
        /// <param name="customMessage">Message to Log</param>
        /// <param name="ex">Exception to Log</param>
        public static void logError(this object classType, string message = null, Exception ex = null, [CallerMemberName]string memberName = "",
            [CallerLineNumberAttribute]int lineNumber = 0)
        {
            log(Enumerators.LogType.Error, classType, message: message, ex: ex, methodName: memberName, lineNumber: lineNumber);
        }
        /// <summary>
        /// Call log with level FATAL
        /// </summary>
        /// <param name="classType">Current Class Type</param>
        /// <param name="customMessage">Message to Log</param>
        /// <param name="ex">Exception to Log</param>
        public static void logFatal(this object classType, string message = null, Exception ex = null, [CallerMemberName]string memberName = "",
            [CallerLineNumberAttribute]int lineNumber = 0)
        {
            log(Enumerators.LogType.Fatal, classType, message: message, ex: ex, methodName: memberName, lineNumber: lineNumber);
        }
        /// <summary>
        /// Call log with level INFO
        /// </summary>
        /// <param name="classType">Current Class Type</param>
        /// <param name="message">Message to Log</param>
        public static void logInfo(this object classType, string message, [CallerMemberName]string memberName = "",
            [CallerLineNumberAttribute]int lineNumber = 0)
        {
            log(Enumerators.LogType.Info, classType, message: message, methodName: memberName, lineNumber: lineNumber);

        }
        /// <summary>
        /// Call log with level WARN
        /// </summary>
        /// <param name="classType">Current Class Type</param>
        /// <param name="message">Message to Log</param>
        public static void logWarn(this object classType, string message, [CallerMemberName]string memberName = "",
        [CallerLineNumberAttribute]int lineNumber = 0)
        {
            log(Enumerators.LogType.Warn, classType, message: message, methodName: memberName, lineNumber: lineNumber);

        }
    }
}
