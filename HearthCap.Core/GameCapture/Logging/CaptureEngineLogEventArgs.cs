// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CaptureEngineLogEventArgs.cs" company="">
//   
// </copyright>
// <summary>
//   The capture engine log event args.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Core.GameCapture.Logging
{
    using System;

    /// <summary>
    /// The capture engine log event args.
    /// </summary>
    public class CaptureEngineLogEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CaptureEngineLogEventArgs"/> class. 
        /// Initializes a new instance of the <see cref="T:System.EventArgs"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="level">
        /// The level.
        /// </param>
        /// <param name="date">
        /// The date.
        /// </param>
        /// <param name="data">
        /// The data.
        /// </param>
        public CaptureEngineLogEventArgs(string message, LogLevel level, DateTime date, object data = null)
        {
            this.Message = message;
            this.Level = level;
            this.Date = date;
            this.Data = data;
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string Message { get; protected set; }

        /// <summary>
        /// Gets or sets the level.
        /// </summary>
        public LogLevel Level { get; protected set; }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        public object Data { get; protected set; }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        public DateTime Date { get; protected set; }
    }
}