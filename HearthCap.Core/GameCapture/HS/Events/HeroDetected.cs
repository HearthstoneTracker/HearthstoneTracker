namespace HearthCap.Core.GameCapture.HS.Events
{
    using System;

    public class HeroDetected : GameEvent
    {
        public HeroDetected(string hero)
            : base(String.Format("Detected your hero: " + hero))
        {
            this.Hero = hero;
        }

        public string Hero { get; protected set; }
    }
}