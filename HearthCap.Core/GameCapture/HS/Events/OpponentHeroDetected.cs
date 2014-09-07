namespace HearthCap.Core.GameCapture.HS.Events
{
    using System;

    public class OpponentHeroDetected : GameEvent
    {
        public OpponentHeroDetected(string hero)
            : base(String.Format("Detected opponent hero: " + hero))
        {
            this.Hero = hero;
        }

        public string Hero { get; protected set; }
    }
}