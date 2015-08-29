using System;

namespace HearthCap.Core.GameCapture.Logging
{
    /// <summary>The log event.</summary>
    public class LogEvent
    {
        public string Message { get; set; }

        public LogLevel Level { get; set; }

        public DateTime Date { get; set; }

        public LogEvent(string message, LogLevel level = LogLevel.Info)
        {
            Message = message;
            Level = level;
            Date = DateTime.Now;
        }

        public static LogEvent Info(string msg)
        {
            return new LogEvent(msg, LogLevel.Info);
        }

        public static LogEvent Warn(string msg)
        {
            return new LogEvent(msg, LogLevel.Warn);
        }

        public static LogEvent Error(string msg)
        {
            return new LogEvent(msg, LogLevel.Error);
        }

        public static LogEvent Diag(string msg)
        {
            return new LogEvent(msg, LogLevel.Diag);
        }

        public static LogEvent Debug(string msg)
        {
            return new LogEvent(msg, LogLevel.Debug);
        }

        public class WithData : LogEvent
        {
            public WithData(string message, object data, LogLevel level = LogLevel.Info)
                : base(message, level)
            {
                Data = data;
            }

            public object Data { get; set; }
        }

        public class WithData<T> : LogEvent
        {
            public WithData(string message, T data, LogLevel level = LogLevel.Info)
                : base(message, level)
            {
                Data = data;
            }

            public T Data { get; set; }
        }
    }
}
