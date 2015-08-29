using System;

namespace HearthCap.Core.GameCapture.HS.Events
{
    public class OpponentHeroDetected : GameEvent
    {
        public OpponentHeroDetected(string hero)
            : base(String.Format("Detected opponent hero: " + hero))
        {
            Hero = hero;
        }

        public string Hero { get; protected set; }
    }
}
