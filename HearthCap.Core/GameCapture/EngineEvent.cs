// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EngineEvent.cs" company="">
//   
// </copyright>
// <summary>
//   The engine event.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Core.GameCapture
{
    using System;

    /// <summary>
    /// The engine event.
    /// </summary>
    public class EngineEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EngineEvent"/> class. 
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public EngineEvent(string message)
        {
            this.Message = message;
            this.Date = DateTime.Now;
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        public DateTime Date { get; set; }
    }
}