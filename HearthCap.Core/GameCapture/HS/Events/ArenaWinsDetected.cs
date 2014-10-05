// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArenaWinsDetected.cs" company="">
//   
// </copyright>
// <summary>
//   The arena wins detected.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Core.GameCapture.HS.Events
{
    /// <summary>
    /// The arena wins detected.
    /// </summary>
    public class ArenaWinsDetected : GameEvent
    {
        /// <summary>
        /// Gets or sets the wins.
        /// </summary>
        public int Wins { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArenaWinsDetected"/> class.
        /// </summary>
        /// <param name="arenaWins">
        /// The arena wins.
        /// </param>
        public ArenaWinsDetected(int arenaWins)
            : base("Wins: " + arenaWins)
        {
            this.Wins = arenaWins;
        }
    }
}