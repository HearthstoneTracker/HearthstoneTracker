// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GameResultUpdated.cs" company="">
//   
// </copyright>
// <summary>
//   The game result updated.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.GameManager.Events
{
    using System;

    /// <summary>
    /// The game result updated.
    /// </summary>
    public class GameResultUpdated
    {
        /// <summary>
        /// Gets or sets the game result id.
        /// </summary>
        public Guid GameResultId { get; set; }

        /// <summary>
        /// Gets or sets the arena session id.
        /// </summary>
        public Guid? ArenaSessionId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameResultUpdated"/> class.
        /// </summary>
        /// <param name="gameResultId">
        /// The game result id.
        /// </param>
        /// <param name="arenaSessionId">
        /// The arena session id.
        /// </param>
        public GameResultUpdated(Guid gameResultId, Guid? arenaSessionId)
        {
            this.GameResultId = gameResultId;
            this.ArenaSessionId = arenaSessionId;
        }
    }
}