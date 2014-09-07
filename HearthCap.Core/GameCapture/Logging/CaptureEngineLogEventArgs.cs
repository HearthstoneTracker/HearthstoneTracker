namespace HearthCap.Core.GameCapture.Logging
{
    using System;

    public class CaptureEngineLogEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.EventArgs"/> class.
        /// </summary>
        public CaptureEngineLogEventArgs(string message, LogLevel level, DateTime date, object data = null)
        {
            this.Message = message;
            this.Level = level;
            this.Date = date;
            this.Data = data;
        }

        public string Message { get; protected set; }
        
        public LogLevel Level { get; protected set; }

        public object Data { get; protected set; }

        public DateTime Date { get; protected set; }
    }
}