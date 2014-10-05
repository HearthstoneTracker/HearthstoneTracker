// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeckUpdated.cs" company="">
//   
// </copyright>
// <summary>
//   The deck updated.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Decks
{
    using HearthCap.Data;

    /// <summary>
    /// The deck updated.
    /// </summary>
    public class DeckUpdated
    {
        /// <summary>
        /// Gets or sets the deck.
        /// </summary>
        public Deck Deck { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeckUpdated"/> class.
        /// </summary>
        /// <param name="deck">
        /// The deck.
        /// </param>
        public DeckUpdated(Deck deck)
        {
            this.Deck = deck;
        }
    }

    // public class DecksUpdated
    // {
    // public string Server { get; set; }

    // public DecksUpdated(string server)
    // {
    // this.Server = server;
    // }
    // }
}