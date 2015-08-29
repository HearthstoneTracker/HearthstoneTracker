using System.Diagnostics;

namespace HearthCap.Core.GameCapture
{
    internal class TraceLogger
    {
        private readonly NLog.Logger log;

        public TraceLogger(NLog.Logger log)
        {
            this.log = log;
        }

        [Conditional("DEBUG")]
        public void Log(string message, params object[] args)
        {
            log.Trace(message, args);
        }
    }
}
