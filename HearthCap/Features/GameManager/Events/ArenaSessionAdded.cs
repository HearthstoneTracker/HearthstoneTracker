// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArenaSessionAdded.cs" company="">
//   
// </copyright>
// <summary>
//   The arena session added.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.GameManager.Events
{
    using HearthCap.Features.Games.Models;

    /// <summary>
    /// The arena session added.
    /// </summary>
    public class ArenaSessionAdded
    {
        /// <summary>
        /// Gets or sets the arena session.
        /// </summary>
        public ArenaSessionModel ArenaSession { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArenaSessionAdded"/> class.
        /// </summary>
        /// <param name="arenaSession">
        /// The arena session.
        /// </param>
        public ArenaSessionAdded(ArenaSessionModel arenaSession)
        {
            this.ArenaSession = arenaSession;
        }
    }
}