// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArenaLossesDetected.cs" company="">
//   
// </copyright>
// <summary>
//   The arena losses detected.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Core.GameCapture.HS.Events
{
    /// <summary>
    /// The arena losses detected.
    /// </summary>
    public class ArenaLossesDetected : GameEvent
    {
        /// <summary>
        /// Gets or sets the losses.
        /// </summary>
        public int Losses { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArenaLossesDetected"/> class.
        /// </summary>
        /// <param name="losses">
        /// The losses.
        /// </param>
        public ArenaLossesDetected(int losses)
            : base("Losses: " + losses)
        {
            this.Losses = losses;
        }
    }
}