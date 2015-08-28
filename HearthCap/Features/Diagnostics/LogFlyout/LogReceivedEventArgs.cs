namespace HearthCap.Features.Diagnostics.LogFlyout
{
    using System;

    public class LogReceivedEventArgs : EventArgs
    {
        public LogMessageModel Message { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.EventArgs"/> class.
        /// </summary>
        public LogReceivedEventArgs(LogMessageModel message)
        {
            this.Message = message;
        }
    }
}