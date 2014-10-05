// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArenaSessionEnded.cs" company="">
//   
// </copyright>
// <summary>
//   The arena session ended.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Core.GameCapture.HS.Events
{
    using System;

    /// <summary>
    /// The arena session ended.
    /// </summary>
    public class ArenaSessionEnded : GameEvent
    {
        /// <summary>
        /// Gets or sets the started.
        /// </summary>
        public DateTime Started { get; set; }

        /// <summary>
        /// Gets or sets the ended.
        /// </summary>
        public DateTime Ended { get; set; }

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
        /// Initializes a new instance of the <see cref="ArenaSessionEnded"/> class.
        /// </summary>
        /// <param name="started">
        /// The started.
        /// </param>
        /// <param name="ended">
        /// The ended.
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
        public ArenaSessionEnded(DateTime started, DateTime ended, string heroKey, int wins, int losses)
            : base("Arena ended")
        {
            this.Started = started;
            this.Ended = ended;
            this.HeroKey = heroKey;
            this.Wins = wins;
            this.Losses = losses;
        }
    }
}