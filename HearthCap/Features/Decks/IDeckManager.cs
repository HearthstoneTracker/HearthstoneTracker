// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDeckManager.cs" company="">
//   
// </copyright>
// <summary>
//   The DeckManager interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Decks
{
    using System;
    using System.Collections.Generic;

    using HearthCap.Data;

    /// <summary>
    /// The DeckManager interface.
    /// </summary>
    public interface IDeckManager
    {
        /// <summary>
        /// The get decks.
        /// </summary>
        /// <param name="server">
        /// The server.
        /// </param>
        /// <param name="includeDeleted">
        /// The include deleted.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        IEnumerable<Deck> GetDecks(string server, bool includeDeleted = false);

        /// <summary>
        /// The clear cache.
        /// </summary>
        void ClearCache();

        /// <summary>
        /// The add deck.
        /// </summary>
        /// <param name="deck">
        /// The deck.
        /// </param>
        void AddDeck(DeckModel deck);

        /// <summary>
        /// The update deck.
        /// </summary>
        /// <param name="deck">
        /// The deck.
        /// </param>
        /// <param name="suppressEvent">
        /// The suppress event.
        /// </param>
        void UpdateDeck(DeckModel deck, bool suppressEvent = false);

        /// <summary>
        /// The delete deck.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        void DeleteDeck(Guid id);

        /// <summary>
        /// The get or create deck by slot.
        /// </summary>
        /// <param name="server">
        /// The server.
        /// </param>
        /// <param name="slot">
        /// The slot.
        /// </param>
        /// <returns>
        /// The <see cref="Deck"/>.
        /// </returns>
        Deck GetOrCreateDeckBySlot(string server, string slot);

        /// <summary>
        /// The get deck by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="Deck"/>.
        /// </returns>
        Deck GetDeckById(Guid id);

        /// <summary>
        /// The get all decks.
        /// </summary>
        /// <param name="includeDeleted">
        /// The include deleted.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        IEnumerable<Deck> GetAllDecks(bool includeDeleted = false);

        /// <summary>
        /// The update deck image.
        /// </summary>
        /// <param name="deckModel">
        /// The deck model.
        /// </param>
        /// <param name="image">
        /// The image.
        /// </param>
        void UpdateDeckImage(Guid deckModel, byte[] image);

        /// <summary>
        /// The undelete deck.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        void UndeleteDeck(Guid id);
    }
}