using System;

namespace HearthCap.Core.GameCapture.HS.Events
{
    public class ArenaSessionEnded : GameEvent
    {
        public DateTime Started { get; set; }

        public DateTime Ended { get; set; }

        public string HeroKey { get; set; }

        public int Wins { get; set; }

        public int Losses { get; set; }

        public ArenaSessionEnded(DateTime started, DateTime ended, string heroKey, int wins, int losses)
            : base("Arena ended")
        {
            Started = started;
            Ended = ended;
            HeroKey = heroKey;
            Wins = wins;
            Losses = losses;
        }
    }
}
