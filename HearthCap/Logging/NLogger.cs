// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NLogger.cs" company="">
//   
// </copyright>
// <summary>
//   The n logger.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Logging
{
    using System;

    using Caliburn.Micro;

    using NLog;

    using LogManager = NLog.LogManager;

    /// <summary>
    /// The n logger.
    /// </summary>
    public class NLogger : ILog
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private Logger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="NLogger"/> class.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        public NLogger(Type type)
        {
            this.logger = LogManager.GetLogger(type.Name);
        }

        /// <summary>
        /// Logs the message as info.
        /// </summary>
        /// <param name="format">
        /// A formatted message.
        /// </param>
        /// <param name="args">
        /// Parameters to be injected into the formatted message.
        /// </param>
        public void Info(string format, params object[] args)
        {
            this.logger.Info(string.Format(format, args));
        }

        /// <summary>
        /// Logs the message as a warning.
        /// </summary>
        /// <param name="format">
        /// A formatted message.
        /// </param>
        /// <param name="args">
        /// Parameters to be injected into the formatted message.
        /// </param>
        public void Warn(string format, params object[] args)
        {
            this.logger.Warn(string.Format(format, args));
        }

        /// <summary>
        /// Logs the exception.
        /// </summary>
        /// <param name="exception">
        /// The exception.
        /// </param>
        public void Error(Exception exception)
        {
            this.logger.Error(exception);
        }
    }
}