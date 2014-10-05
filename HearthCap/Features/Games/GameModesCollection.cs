// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GameModesCollection.cs" company="">
//   
// </copyright>
// <summary>
//   The game modes collection.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Games
{
    using Caliburn.Micro;

    using HearthCap.Data;

    /// <summary>
    /// The game modes collection.
    /// </summary>
    public class GameModesCollection : BindableCollection<GameMode>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GameModesCollection"/> class.
        /// </summary>
        public GameModesCollection()
            : base(new[]
                       {
                           GameMode.Unknown, 
                           GameMode.Arena, 
                           GameMode.Casual, 
                           GameMode.Challenge, 
                           GameMode.Practice, 
                           GameMode.Ranked, 
                           GameMode.Mission
                       })
        {
        }
    }
}