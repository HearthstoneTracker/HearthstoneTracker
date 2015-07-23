namespace HearthCap.Features.Decks
{
    public class SelectDeck
    {
        public DeckModel Deck { get; set; }

        public SelectDeck(DeckModel deck)
        {
            this.Deck = deck;
        }
    }
}