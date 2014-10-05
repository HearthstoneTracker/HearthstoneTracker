// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GameResultAdded.cs" company="">
//   
// </copyright>
// <summary>
//   The game result added.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.GameManager.Events
{
    using HearthCap.Features.Games.Models;

    /// <summary>
    /// The game result added.
    /// </summary>
    public class GameResultAdded
    {
        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        public object Source { get; set; }

        /// <summary>
        /// Gets or sets the game result.
        /// </summary>
        public GameResultModel GameResult { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameResultAdded"/> class.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="gameResult">
        /// The game result.
        /// </param>
        public GameResultAdded(object source, GameResultModel gameResult)
        {
            this.Source = source;
            this.GameResult = gameResult;
        }
    }
}