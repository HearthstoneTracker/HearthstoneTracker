using HearthCap.Data;

namespace HearthCap.Features.Decks
{
    public class DeckUpdated
    {
        public Deck Deck { get; set; }

        public DeckUpdated(Deck deck)
        {
            Deck = deck;
        }
    }

    //public class DecksUpdated
    //{
    //    public string Server { get; set; }

    //    public DecksUpdated(string server)
    //    {
    //        this.Server = server;
    //    }
    //}
}
