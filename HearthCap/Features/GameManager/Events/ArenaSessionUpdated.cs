// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArenaSessionUpdated.cs" company="">
//   
// </copyright>
// <summary>
//   The arena session updated.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.GameManager.Events
{
    using System;

    /// <summary>
    /// The arena session updated.
    /// </summary>
    public class ArenaSessionUpdated
    {
        /// <summary>
        /// Gets or sets the arena session id.
        /// </summary>
        public Guid ArenaSessionId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is latest.
        /// </summary>
        public bool IsLatest { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArenaSessionUpdated"/> class.
        /// </summary>
        /// <param name="arenaSessionId">
        /// The arena session id.
        /// </param>
        /// <param name="isLatest">
        /// The is latest.
        /// </param>
        public ArenaSessionUpdated(Guid arenaSessionId, bool isLatest = false)
        {
            this.ArenaSessionId = arenaSessionId;
            this.IsLatest = isLatest;
        }
    }
}