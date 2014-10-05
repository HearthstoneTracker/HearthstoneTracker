// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GameModeChanged.cs" company="">
//   
// </copyright>
// <summary>
//   The game mode changed.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Core.GameCapture.HS.Events
{
    using HearthCap.Data;

    /// <summary>
    /// The game mode changed.
    /// </summary>
    public class GameModeChanged : GameEvent
    {
        /// <summary>
        /// Gets or sets the old game mode.
        /// </summary>
        public GameMode OldGameMode { get; protected set; }

        /// <summary>
        /// Gets or sets the game mode.
        /// </summary>
        public GameMode GameMode { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameModeChanged"/> class.
        /// </summary>
        /// <param name="oldGameMode">
        /// The old game mode.
        /// </param>
        /// <param name="gameMode">
        /// The game mode.
        /// </param>
        public GameModeChanged(GameMode oldGameMode, GameMode gameMode)
            : base(string.Format("Mode changed from {0} to {1}.", oldGameMode, gameMode))
        {
            this.OldGameMode = oldGameMode;
            this.GameMode = gameMode;
        }
    }
}