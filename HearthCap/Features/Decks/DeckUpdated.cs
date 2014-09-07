namespace HearthCap.Features.Decks
{
    using HearthCap.Data;

    public class DeckUpdated
    {
        public Deck Deck { get; set; }

        public DeckUpdated(Deck deck)
        {
            this.Deck = deck;
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