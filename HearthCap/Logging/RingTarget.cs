namespace HearthCap.Logging
{
    using System.Threading;

    using NLog;
    using NLog.Targets;

    public class RingTarget : TargetWithLayout
    {
        private const int logSize = 1024;

        private readonly static string[] buffer;

        private static int nextLogEntry;

        static RingTarget()
        {
            RingTarget.buffer = new string[logSize];
        }

        protected override void Write(LogEventInfo logEvent)
        {
            RingTarget.buffer[Interlocked.Increment(ref RingTarget.nextLogEntry) % logSize] = this.Layout.Render(logEvent);
        }
    }
}