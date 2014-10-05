// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TraceLogger.cs" company="">
//   
// </copyright>
// <summary>
//   The trace logger.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Core.GameCapture
{
    using System.Diagnostics;

    using NLog;

    /// <summary>
    /// The trace logger.
    /// </summary>
    internal class TraceLogger
    {
        /// <summary>
        /// The log.
        /// </summary>
        private readonly Logger log;

        /// <summary>
        /// Initializes a new instance of the <see cref="TraceLogger"/> class.
        /// </summary>
        /// <param name="log">
        /// The log.
        /// </param>
        public TraceLogger(Logger log)
        {
            this.log = log;
        }

        /// <summary>
        /// The log.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        [Conditional("DEBUG")]
        public void Log(string message, params object[] args)
        {
            this.log.Trace(message, args);
        }
    }
}