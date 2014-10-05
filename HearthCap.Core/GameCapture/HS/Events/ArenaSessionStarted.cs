// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArenaSessionStarted.cs" company="">
//   
// </copyright>
// <summary>
//   The arena session started.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Core.GameCapture.HS.Events
{
    using System;

    /// <summary>
    /// The arena session started.
    /// </summary>
    public class ArenaSessionStarted : GameEvent
    {
        /// <summary>
        /// Gets or sets the started.
        /// </summary>
        public DateTime Started { get; set; }

        /// <summary>
        /// Gets or sets the hero key.
        /// </summary>
        public string HeroKey { get; set; }

        /// <summary>
        /// Gets or sets the wins.
        /// </summary>
        public int Wins { get; set; }

        /// <summary>
        /// Gets or sets the losses.
        /// </summary>
        public int Losses { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArenaSessionStarted"/> class.
        /// </summary>
        /// <param name="started">
        /// The started.
        /// </param>
        /// <param name="heroKey">
        /// The hero key.
        /// </param>
        /// <param name="wins">
        /// The wins.
        /// </param>
        /// <param name="losses">
        /// The losses.
        /// </param>
        public ArenaSessionStarted(DateTime started, string heroKey, int wins, int losses)
            : base("Arena started")
        {
            this.Started = started;
            this.HeroKey = heroKey;
            this.Wins = wins;
            this.Losses = losses;
        }
    }
}