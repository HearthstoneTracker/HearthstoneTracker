// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CoinDetected.cs" company="">
//   
// </copyright>
// <summary>
//   The game started.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Core.GameCapture.HS.Events
{
    /// <summary>The game started.</summary>
    public class CoinDetected : GameEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CoinDetected"/> class.
        /// </summary>
        /// <param name="goFirst">
        /// The go first.
        /// </param>
        public CoinDetected(bool goFirst)
            : base("Coin detected: " + (goFirst ? "gofirst" : "gosecond"))
        {
            this.GoFirst = goFirst;
        }

        /// <summary>
        /// Gets or sets a value indicating whether go first.
        /// </summary>
        public bool GoFirst { get; protected set; }
    }
}