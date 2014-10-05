// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArenaSessionDeleted.cs" company="">
//   
// </copyright>
// <summary>
//   The arena session deleted.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.GameManager.Events
{
    using HearthCap.Features.Games.Models;

    /// <summary>
    /// The arena session deleted.
    /// </summary>
    public class ArenaSessionDeleted
    {
        /// <summary>
        /// Gets or sets the arena session.
        /// </summary>
        public ArenaSessionModel ArenaSession { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArenaSessionDeleted"/> class.
        /// </summary>
        /// <param name="arenaSession">
        /// The arena session.
        /// </param>
        public ArenaSessionDeleted(ArenaSessionModel arenaSession)
        {
            this.ArenaSession = arenaSession;
        }
    }
}