// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeckDetected.cs" company="">
//   
// </copyright>
// <summary>
//   The deck detected.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Core.GameCapture.HS.Events
{
    /// <summary>
    /// The deck detected.
    /// </summary>
    public class DeckDetected : GameEvent
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public string Key { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeckDetected"/> class.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        public DeckDetected(string key)
            :base("Deck detected: " + key)
        {
            this.Key = key;
        }
    }
}