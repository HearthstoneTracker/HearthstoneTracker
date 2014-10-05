// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GameResultDeleted.cs" company="">
//   
// </copyright>
// <summary>
//   The game result deleted.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.GameManager.Events
{
    using System;

    /// <summary>
    /// The game result deleted.
    /// </summary>
    public class GameResultDeleted
    {
        /// <summary>
        /// Gets or sets the game id.
        /// </summary>
        public Guid GameId { get; set; }

        /// <summary>
        /// Gets or sets the arena id.
        /// </summary>
        public Guid? ArenaId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameResultDeleted"/> class.
        /// </summary>
        /// <param name="gameId">
        /// The game id.
        /// </param>
        /// <param name="arenaId">
        /// The arena id.
        /// </param>
        public GameResultDeleted(Guid gameId, Guid? arenaId = null)
        {
            this.GameId = gameId;
            this.ArenaId = arenaId;
        }
    }
}