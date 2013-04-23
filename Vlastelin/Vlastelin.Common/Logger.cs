using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;


namespace Vlastelin.Common
{
    public enum LogEventType
    {
        /// <summary>
        /// None
        /// </summary>
        None,
        /// <summary>
        /// Debug message
        /// </summary>
        Debug,
        /// <summary>
        /// Info message
        /// </summary>
        Info,
        /// <summary>
        /// Warning message
        /// </summary>
        Warning,
        /// <summary>
        /// Error message
        /// </summary>
        Error
    }

    /// <summary>
    /// Класс обеспечивает логирование всего
    /// </summary>
    public class Logger
    {
        private log4net.ILog _log;
        private string source;

        public Logger()
        {
            _log= log4net.LogManager.GetLogger(Assembly.GetCallingAssembly().FullName);
            log4net.Config.XmlConfigurator.Configure();
            source = Assembly.GetCallingAssembly().GetName().Name;
        }       

        public void LogMessage(LogEventType type, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("message");

            string newMessage = string.Format("{0}: {1}",
                source,
                message);

            switch (type)
            {
                case LogEventType.Debug:
                    _log.Debug(newMessage);
                    break;
                case LogEventType.Info:
                    _log.Info(newMessage);
                    break;
                case LogEventType.Warning:
                    _log.Warn(newMessage);
                    break;
                case LogEventType.Error:
                    _log.Error(newMessage);
                    break;
            }

        }
    }
}
