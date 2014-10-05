// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GameEnded.cs" company="">
//   
// </copyright>
// <summary>
//   The game ended.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Core.GameCapture.HS.Events
{
    using System;

    using HearthCap.Data;

    /// <summary>The game ended.</summary>
    public class GameEnded : GameEvent
    {
        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Gets or sets the end time.
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Gets or sets the victory.
        /// </summary>
        public bool? Victory { get; set; }

        /// <summary>
        /// Gets or sets the go first.
        /// </summary>
        public bool? GoFirst { get; set; }

        /// <summary>
        /// Gets or sets the hero.
        /// </summary>
        public string Hero { get; set; }

        /// <summary>
        /// Gets or sets the opponent hero.
        /// </summary>
        public string OpponentHero { get; set; }

        /// <summary>
        /// Gets or sets the game mode.
        /// </summary>
        public GameMode GameMode { get; set; }

        /// <summary>
        /// Gets or sets the turns.
        /// </summary>
        public int Turns { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether conceded.
        /// </summary>
        public bool Conceded { get; set; }

        /// <summary>
        /// Gets or sets the deck.
        /// </summary>
        public string Deck { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameEnded"/> class.
        /// </summary>
        public GameEnded()
            : base("Detected end of game")
        {
        }
    }
}