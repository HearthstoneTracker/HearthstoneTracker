using System.Threading;
using NLog;
using NLog.Targets;

namespace HearthCap.Logging
{
    public class RingTarget : TargetWithLayout
    {
        private const int logSize = 1024;

        private static readonly string[] buffer;

        private static int nextLogEntry;

        static RingTarget()
        {
            buffer = new string[logSize];
        }

        protected override void Write(LogEventInfo logEvent)
        {
            buffer[Interlocked.Increment(ref nextLogEntry) % logSize] = Layout.Render(logEvent);
        }
    }
}
