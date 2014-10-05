// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VictoryDetected.cs" company="">
//   
// </copyright>
// <summary>
//   The victory detected.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Core.GameCapture.HS.Events
{
    /// <summary>
    /// The victory detected.
    /// </summary>
    public class VictoryDetected : GameEvent
    {
        /// <summary>
        /// Gets or sets a value indicating whether is victory.
        /// </summary>
        public bool IsVictory { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether conceded.
        /// </summary>
        public bool Conceded { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="VictoryDetected"/> class.
        /// </summary>
        /// <param name="victory">
        /// The victory.
        /// </param>
        /// <param name="conceded">
        /// The conceded.
        /// </param>
        public VictoryDetected(bool victory, bool conceded = false)
            : base(string.Format("Victory detected: {0} {1}", victory ? "won" : "lost", conceded ? " (conceded)" : string.Empty))
        {
            this.IsVictory = victory;
            this.Conceded = conceded;
        }
    }
}