namespace HearthCap.Core.GameCapture.HS.Events
{
    using System;

    public class ArenaSessionStarted : GameEvent
    {
        public DateTime Started { get; set; }

        public string HeroKey { get; set; }

        public int Wins { get; set; }

        public int Losses { get; set; }

        public ArenaSessionStarted(DateTime started, string heroKey, int wins, int losses)
            : base("Arena started")
        {
            this.Started = started;
            this.HeroKey = heroKey;
            this.Wins = wins;
            this.Losses = losses;
        }
    }
}