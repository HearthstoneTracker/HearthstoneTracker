// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HeroDetected.cs" company="">
//   
// </copyright>
// <summary>
//   The hero detected.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Core.GameCapture.HS.Events
{
    /// <summary>
    /// The hero detected.
    /// </summary>
    public class HeroDetected : GameEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HeroDetected"/> class.
        /// </summary>
        /// <param name="hero">
        /// The hero.
        /// </param>
        public HeroDetected(string hero)
            : base(string.Format("Detected your hero: " + hero))
        {
            this.Hero = hero;
        }

        /// <summary>
        /// Gets or sets the hero.
        /// </summary>
        public string Hero { get; protected set; }
    }
}