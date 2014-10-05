// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogReceivedEventArgs.cs" company="">
//   
// </copyright>
// <summary>
//   The log received event args.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Diagnostics.LogFlyout
{
    using System;

    /// <summary>
    /// The log received event args.
    /// </summary>
    public class LogReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public LogMessageModel Message { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogReceivedEventArgs"/> class. 
        /// Initializes a new instance of the <see cref="T:System.EventArgs"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public LogReceivedEventArgs(LogMessageModel message)
        {
            this.Message = message;
        }
    }
}