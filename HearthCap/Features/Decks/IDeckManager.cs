namespace HearthCap.Features.Decks
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    using HearthCap.Data;

    public interface IDeckManager
    {
        IEnumerable<Deck> GetDecks(string server, bool includeDeleted = false);

        void ClearCache();

        void AddDeck(DeckModel deck);

        void UpdateDeck(DeckModel deck, bool suppressEvent = false);

        void DeleteDeck(Guid id);

        Deck GetOrCreateDeckBySlot(string server, string slot);

        Deck GetDeckById(Guid id);

        IEnumerable<Deck> GetAllDecks(bool includeDeleted = false);

        void UpdateDeckImage(Guid deckModel, byte[] image);

        void UndeleteDeck(Guid id);
    }
}