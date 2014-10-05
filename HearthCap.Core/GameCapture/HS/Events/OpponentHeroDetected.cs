// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OpponentHeroDetected.cs" company="">
//   
// </copyright>
// <summary>
//   The opponent hero detected.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Core.GameCapture.HS.Events
{
    /// <summary>
    /// The opponent hero detected.
    /// </summary>
    public class OpponentHeroDetected : GameEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpponentHeroDetected"/> class.
        /// </summary>
        /// <param name="hero">
        /// The hero.
        /// </param>
        public OpponentHeroDetected(string hero)
            : base(string.Format("Detected opponent hero: " + hero))
        {
            this.Hero = hero;
        }

        /// <summary>
        /// Gets or sets the hero.
        /// </summary>
        public string Hero { get; protected set; }
    }
}