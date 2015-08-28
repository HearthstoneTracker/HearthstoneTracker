namespace HearthCap.Data
{
    using System;

    public class Deck : IEntityWithId<Guid>
    {
        public Deck()
        {
            Id = Guid.NewGuid();
            Created = DateTime.Now;
            Modified = Created;
        }

        public Guid Id { get; protected set; }

        public string Key { get; set; }

        public string Name { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        public bool Deleted { get; set; }

        public string Server { get; set; }

        public DeckImage Image { get; set; }

        public string Notes { get; set; }

        // TODO: more deck features :)
        // like associate a hero
    }
}