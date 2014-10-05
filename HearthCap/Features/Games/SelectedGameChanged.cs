// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectedGameChanged.cs" company="">
//   
// </copyright>
// <summary>
//   The selected game changed.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Games
{
    using HearthCap.Features.Games.Models;

    /// <summary>
    /// The selected game changed.
    /// </summary>
    public class SelectedGameChanged
    {
        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        public object Source { get; protected set; }

        /// <summary>
        /// Gets or sets the game.
        /// </summary>
        public GameResultModel Game { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectedGameChanged"/> class.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="game">
        /// The game.
        /// </param>
        public SelectedGameChanged(object source, GameResultModel game)
        {
            this.Source = source;
            this.Game = game;
        }
    }
}