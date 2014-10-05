// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeckModelMapper.cs" company="">
//   
// </copyright>
// <summary>
//   The deck model mapper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Decks.ModelMappers
{
    using System.Collections.Generic;
    using System.Linq;

    using HearthCap.Data;

    using Omu.ValueInjecter;

    /// <summary>
    /// The deck model mapper.
    /// </summary>
    public static class DeckModelMapper
    {
        /// <summary>
        /// The to model.
        /// </summary>
        /// <param name="deck">
        /// The deck.
        /// </param>
        /// <returns>
        /// The <see cref="DeckModel"/>.
        /// </returns>
        public static DeckModel ToModel(this Deck deck)
        {
            if (deck == null) return null;
            var model = new DeckModel();
            model.InjectFrom(deck);
            return model;
        }

        /// <summary>
        /// The to model.
        /// </summary>
        /// <param name="decks">
        /// The decks.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public static IEnumerable<DeckModel> ToModel(this IEnumerable<Deck> decks)
        {
            return decks.Where(x=>x != null).Select(deck => deck.ToModel()).ToList();
        }
    }
}