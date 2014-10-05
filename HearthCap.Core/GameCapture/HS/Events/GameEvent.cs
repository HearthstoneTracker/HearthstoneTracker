// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GameEvent.cs" company="">
//   
// </copyright>
// <summary>
//   The game event.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Core.GameCapture.HS.Events
{
    using System;

    /// <summary>
    /// The game event.
    /// </summary>
    public class GameEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GameEvent"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public GameEvent(string message)
        {
            this.Date = DateTime.Now;
            this.Message = message;
        }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string Message { get; set; }
    }
}