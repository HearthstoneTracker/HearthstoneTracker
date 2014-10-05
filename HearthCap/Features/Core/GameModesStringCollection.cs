// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GameModesStringCollection.cs" company="">
//   
// </copyright>
// <summary>
//   The game modes string collection.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Core
{
    using System;
    using System.Linq;

    using Caliburn.Micro;

    using HearthCap.Data;

    /// <summary>
    /// The game modes string collection.
    /// </summary>
    public class GameModesStringCollection : BindableCollection<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GameModesStringCollection"/> class.
        /// </summary>
        /// <param name="emptyItem">
        /// The empty item.
        /// </param>
        public GameModesStringCollection(bool emptyItem = false)
        {
            if (emptyItem)
            {
                this.Add(string.Empty);
            }

            var values = Enum.GetValues(typeof(GameMode)).Cast<GameMode>().Select(x => x.ToString());
            this.AddRange(values);
        }
    }
}