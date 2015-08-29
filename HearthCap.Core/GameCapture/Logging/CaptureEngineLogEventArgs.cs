using System;

namespace HearthCap.Core.GameCapture.Logging
{
    public class CaptureEngineLogEventArgs : EventArgs
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:System.EventArgs" /> class.
        /// </summary>
        public CaptureEngineLogEventArgs(string message, LogLevel level, DateTime date, object data = null)
        {
            Message = message;
            Level = level;
            Date = date;
            Data = data;
        }

        public string Message { get; protected set; }

        public LogLevel Level { get; protected set; }

        public object Data { get; protected set; }

        public DateTime Date { get; protected set; }
    }
}
