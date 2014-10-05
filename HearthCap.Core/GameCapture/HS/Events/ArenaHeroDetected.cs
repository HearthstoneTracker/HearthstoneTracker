// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArenaHeroDetected.cs" company="">
//   
// </copyright>
// <summary>
//   The arena hero detected.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Core.GameCapture.HS.Events
{
    /// <summary>
    /// The arena hero detected.
    /// </summary>
    public class ArenaHeroDetected : GameEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArenaHeroDetected"/> class.
        /// </summary>
        /// <param name="hero">
        /// The hero.
        /// </param>
        public ArenaHeroDetected(string hero)
            : base("hero detected: " + hero)
        {
            this.Hero = hero;
        }

        /// <summary>
        /// Gets or sets the hero.
        /// </summary>
        public string Hero { get; protected set; }
    }
}