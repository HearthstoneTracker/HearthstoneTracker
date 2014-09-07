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
                    break;
                case LogLevel.Debug:
                    return NLog.LogLevel.Debug;
                    break;
                case LogLevel.Diag:
                    return NLog.LogLevel.Trace;
                    break;
                case LogLevel.Error:
                    return NLog.LogLevel.Error;
                    break;
                case LogLevel.Info:
                    return NLog.LogLevel.Info;
                    break;
                case LogLevel.None:
                    return NLog.LogLevel.Off;
                    break;
                case LogLevel.Warn:
                    return NLog.LogLevel.Warn;
                    break;
                default:
                    return NLog.LogLevel.Info;
            }
        }
    }
}