using System;

namespace HearthCap.Core.GameCapture.HS.Events
{
    public class ArenaSessionStarted : GameEvent
    {
        public DateTime Started { get; set; }

        public string HeroKey { get; set; }

        public int Wins { get; set; }

        public int Losses { get; set; }

        public ArenaSessionStarted(DateTime started, string heroKey, int wins, int losses)
            : base("Arena started")
        {
            Started = started;
            HeroKey = heroKey;
            Wins = wins;
            Losses = losses;
        }
    }
}
