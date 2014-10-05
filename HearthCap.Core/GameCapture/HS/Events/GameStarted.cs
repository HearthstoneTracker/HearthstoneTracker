// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GameStarted.cs" company="">
//   
// </copyright>
// <summary>
//   The game started.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Core.GameCapture.HS.Events
{
    using System;

    using HearthCap.Data;

    /// <summary>
    /// The game started.
    /// </summary>
    public class GameStarted : GameEvent
    {
        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        public DateTime StartTime { get; protected set; }

        /// <summary>
        /// Gets or sets the hero.
        /// </summary>
        public string Hero { get; protected set; }

        /// <summary>
        /// Gets or sets the opponent hero.
        /// </summary>
        public string OpponentHero { get; protected set; }

        /// <summary>
        /// Gets or sets the game mode.
        /// </summary>
        public GameMode GameMode { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether go first.
        /// </summary>
        public bool GoFirst { get; protected set; }

        /// <summary>
        /// Gets or sets the deck.
        /// </summary>
        public string Deck { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameStarted"/> class.
        /// </summary>
        /// <param name="gameMode">
        /// The game mode.
        /// </param>
        /// <param name="startTime">
        /// The start time.
        /// </param>
        /// <param name="hero">
        /// The hero.
        /// </param>
        /// <param name="opponentHero">
        /// The opponent hero.
        /// </param>
        /// <param name="goFirst">
        /// The go first.
        /// </param>
        /// <param name="lastDeck">
        /// The last deck.
        /// </param>
        public GameStarted(GameMode gameMode, DateTime startTime, string hero, string opponentHero, bool goFirst, string lastDeck)
            : base("Detected started game")
        {
            this.GameMode = gameMode;
            this.StartTime = startTime;
            this.Hero = hero;
            this.OpponentHero = opponentHero;
            this.GoFirst = goFirst;
            this.Deck = lastDeck;
        }
    }
}