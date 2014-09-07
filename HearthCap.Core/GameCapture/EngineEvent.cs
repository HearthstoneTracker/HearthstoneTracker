namespace HearthCap.Core.GameCapture
{
    using System;

    public class EngineEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public EngineEvent(string message)
        {
            this.Message = message;
            this.Date = DateTime.Now;
        }

        public string Message { get; set; }

        public DateTime Date { get; set; }
    }
}