namespace HearthCap.Core.GameCapture.HS.Events
{
    using System;

    public class GameEvent
    {
        public GameEvent(string message)
        {
            this.Date = DateTime.Now;
            this.Message = message;
        }
        
        public DateTime Date { get; set; }

        public string Message { get; set; }
    }
}