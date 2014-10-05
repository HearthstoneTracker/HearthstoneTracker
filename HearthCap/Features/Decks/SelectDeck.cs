// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectDeck.cs" company="">
//   
// </copyright>
// <summary>
//   The select deck.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Decks
{
    /// <summary>
    /// The select deck.
    /// </summary>
    public class SelectDeck
    {
        /// <summary>
        /// Gets or sets the deck.
        /// </summary>
        public DeckModel Deck { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectDeck"/> class.
        /// </summary>
        /// <param name="deck">
        /// The deck.
        /// </param>
        public SelectDeck(DeckModel deck)
        {
            this.Deck = deck;
        }
    }
}