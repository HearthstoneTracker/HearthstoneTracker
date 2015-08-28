namespace HearthCap.Core.GameCapture
{
    using System.Diagnostics;

    using NLog;

    internal class TraceLogger
    {
        private readonly Logger log;

        public TraceLogger(Logger log)
        {
            this.log = log;
        }

        [Conditional("DEBUG")]
        public void Log(string message, params object[] args)
        {
            this.log.Trace(message, args);
        }
    }
}