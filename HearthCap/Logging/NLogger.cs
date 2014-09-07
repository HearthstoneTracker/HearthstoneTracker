namespace HearthCap.Logging
{
    using System;

    using Caliburn.Micro;

    using NLog;

    public class NLogger : ILog
    {
        private Logger logger;

        public NLogger(Type type)
        {
            this.logger = NLog.LogManager.GetLogger(type.Name);
        }

        /// <summary>
        /// Logs the message as info.
        /// </summary>
        /// <param name="format">A formatted message.</param><param name="args">Parameters to be injected into the formatted message.</param>
        public void Info(string format, params object[] args)
        {
            this.logger.Info(String.Format(format, args));
        }

        /// <summary>
        /// Logs the message as a warning.
        /// </summary>
        /// <param name="format">A formatted message.</param><param name="args">Parameters to be injected into the formatted message.</param>
        public void Warn(string format, params object[] args)
        {
            this.logger.Warn(String.Format(format, args));
        }

        /// <summary>
        /// Logs the exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public void Error(Exception exception)
        {
            this.logger.Error(exception);
        }
    }
}