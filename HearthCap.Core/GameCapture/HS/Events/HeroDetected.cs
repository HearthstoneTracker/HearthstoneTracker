using System;

namespace HearthCap.Core.GameCapture.HS.Events
{
    public class HeroDetected : GameEvent
    {
        public HeroDetected(string hero)
            : base(String.Format("Detected your hero: " + hero))
        {
            Hero = hero;
        }

        public string Hero { get; protected set; }
    }
}
