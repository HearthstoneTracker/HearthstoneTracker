// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RingTarget.cs" company="">
//   
// </copyright>
// <summary>
//   The ring target.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Logging
{
    using System.Threading;

    using NLog;
    using NLog.Targets;

    /// <summary>
    /// The ring target.
    /// </summary>
    public class RingTarget : TargetWithLayout
    {
        /// <summary>
        /// The log size.
        /// </summary>
        private const int logSize = 1024;

        /// <summary>
        /// The buffer.
        /// </summary>
        private readonly static string[] buffer;

        /// <summary>
        /// The next log entry.
        /// </summary>
        private static int nextLogEntry;

        /// <summary>
        /// Initializes static members of the <see cref="RingTarget"/> class.
        /// </summary>
        static RingTarget()
        {
            buffer = new string[logSize];
        }

        /// <summary>
        /// The write.
        /// </summary>
        /// <param name="logEvent">
        /// The log event.
        /// </param>
        protected override void Write(LogEventInfo logEvent)
        {
            buffer[Interlocked.Increment(ref nextLogEntry) % logSize] = this.Layout.Render(logEvent);
        }
    }
}