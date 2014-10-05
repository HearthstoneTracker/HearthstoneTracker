// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogEvent.cs" company="">
//   
// </copyright>
// <summary>
//   The log event.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Core.GameCapture.Logging
{
    using System;

    /// <summary>The log event.</summary>
    public class LogEvent
    {
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the level.
        /// </summary>
        public LogLevel Level { get; set; }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogEvent"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="level">
        /// The level.
        /// </param>
        public LogEvent(string message, LogLevel level = LogLevel.Info)
        {
            this.Message = message;
            this.Level = level;
            this.Date = DateTime.Now;
        }

        /// <summary>
        /// The info.
        /// </summary>
        /// <param name="msg">
        /// The msg.
        /// </param>
        /// <returns>
        /// The <see cref="LogEvent"/>.
        /// </returns>
        public static LogEvent Info(string msg) { return new LogEvent(msg, LogLevel.Info); }

        /// <summary>
        /// The warn.
        /// </summary>
        /// <param name="msg">
        /// The msg.
        /// </param>
        /// <returns>
        /// The <see cref="LogEvent"/>.
        /// </returns>
        public static LogEvent Warn(string msg) { return new LogEvent(msg, LogLevel.Warn); }

        /// <summary>
        /// The error.
        /// </summary>
        /// <param name="msg">
        /// The msg.
        /// </param>
        /// <returns>
        /// The <see cref="LogEvent"/>.
        /// </returns>
        public static LogEvent Error(string msg) { return new LogEvent(msg, LogLevel.Error); }

        /// <summary>
        /// The diag.
        /// </summary>
        /// <param name="msg">
        /// The msg.
        /// </param>
        /// <returns>
        /// The <see cref="LogEvent"/>.
        /// </returns>
        public static LogEvent Diag(string msg) { return new LogEvent(msg, LogLevel.Diag); }

        /// <summary>
        /// The debug.
        /// </summary>
        /// <param name="msg">
        /// The msg.
        /// </param>
        /// <returns>
        /// The <see cref="LogEvent"/>.
        /// </returns>
        public static LogEvent Debug(string msg) { return new LogEvent(msg, LogLevel.Debug); }

        /// <summary>
        /// The with data.
        /// </summary>
        public class WithData : LogEvent
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="WithData"/> class.
            /// </summary>
            /// <param name="message">
            /// The message.
            /// </param>
            /// <param name="data">
            /// The data.
            /// </param>
            /// <param name="level">
            /// The level.
            /// </param>
            public WithData(string message, object data, LogLevel level = LogLevel.Info)
                : base(message, level)
            {
                this.Data = data;
            }

            /// <summary>
            /// Gets or sets the data.
            /// </summary>
            public object Data { get; set; }
        }

        /// <summary>
        /// The with data.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        public class WithData<T> : LogEvent
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="WithData{T}"/> class.
            /// </summary>
            /// <param name="message">
            /// The message.
            /// </param>
            /// <param name="data">
            /// The data.
            /// </param>
            /// <param name="level">
            /// The level.
            /// </param>
            public WithData(string message, T data, LogLevel level = LogLevel.Info)
                : base(message, level)
            {
                this.Data = data;
            }

            /// <summary>
            /// Gets or sets the data.
            /// </summary>
            public T Data { get; set; }
        }
    }
}