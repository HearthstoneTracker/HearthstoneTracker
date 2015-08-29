using System;
using HearthCap.Data;

namespace HearthCap.Core.GameCapture.HS.Events
{
    /// <summary>The game ended.</summary>
    public class GameEnded : GameEvent
    {
        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public bool? Victory { get; set; }

        public bool? GoFirst { get; set; }

        public string Hero { get; set; }

        public string OpponentHero { get; set; }

        public GameMode GameMode { get; set; }

        public int Turns { get; set; }

        public bool Conceded { get; set; }

        public string Deck { get; set; }

        public GameEnded()
            : base("Detected end of game")
        {
        }
    }
}
