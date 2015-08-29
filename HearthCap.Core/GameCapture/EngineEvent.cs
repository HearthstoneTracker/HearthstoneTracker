using System;

namespace HearthCap.Core.GameCapture
{
    public class EngineEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public EngineEvent(string message)
        {
            Message = message;
            Date = DateTime.Now;
        }

        public string Message { get; set; }

        public DateTime Date { get; set; }
    }
}
