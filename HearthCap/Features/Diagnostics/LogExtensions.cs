namespace HearthCap.Features.Diagnostics
{
    using HearthCap.Core.GameCapture.HS.Events;
    using HearthCap.Core.GameCapture.Logging;

    public static class LogExtensions
    {
        public static LogMessageModel ToMessageModel(this LogEvent logEvent)
        {
            return new LogMessageModel(logEvent.Message, GetLevel(logEvent.Level), logEvent.Date);
        }

        public static LogMessageModel ToMessageModel(this CaptureEngineLogEventArgs logEvent)
        {
            return new LogMessageModel(logEvent.Message, GetLevel(logEvent.Level), logEvent.Date);
        }

        public static LogMessageModel ToMessageModel(this GameEvent logEvent, string message, LogLevel level = LogLevel.Info)
        {
            return new LogMessageModel(message, GetLevel(level), logEvent.Date);
        }

        private static NLog.LogLevel GetLevel(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.All:
                    return NLog.LogLevel.Trace;
                case LogLevel.Debug:
                    return NLog.LogLevel.Debug;
                case LogLevel.Diag:
                    return NLog.LogLevel.Trace;
                case LogLevel.Error:
                    return NLog.LogLevel.Error;
                case LogLevel.Info:
                    return NLog.LogLevel.Info;
                case LogLevel.None:
                    return NLog.LogLevel.Off;
                case LogLevel.Warn:
                    return NLog.LogLevel.Warn;
                default:
                    return NLog.LogLevel.Info;
            }
        }
    }
}