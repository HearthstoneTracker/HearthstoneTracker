using System;

namespace HearthCap.Core.GameCapture.HS.Events
{
    public class GameEvent
    {
        public GameEvent(string message)
        {
            Date = DateTime.Now;
            Message = message;
        }

        public DateTime Date { get; set; }

        public string Message { get; set; }
    }
}
