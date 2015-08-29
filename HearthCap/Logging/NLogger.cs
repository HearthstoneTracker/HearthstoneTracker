using System;
using Caliburn.Micro;
using LogManager = NLog.LogManager;

namespace HearthCap.Logging
{
    public class NLogger : ILog
    {
        private readonly NLog.Logger logger;

        public NLogger(Type type)
        {
            logger = LogManager.GetLogger(type.Name);
        }

        /// <summary>
        ///     Logs the message as info.
        /// </summary>
        /// <param name="format">A formatted message.</param>
        /// <param name="args">Parameters to be injected into the formatted message.</param>
        public void Info(string format, params object[] args)
        {
            logger.Info(format, args);
        }

        /// <summary>
        ///     Logs the message as a warning.
        /// </summary>
        /// <param name="format">A formatted message.</param>
        /// <param name="args">Parameters to be injected into the formatted message.</param>
        public void Warn(string format, params object[] args)
        {
            logger.Warn(format, args);
        }

        /// <summary>
        ///     Logs the exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public void Error(Exception exception)
        {
            logger.Error(exception);
        }
    }
}
