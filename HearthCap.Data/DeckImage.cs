using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HearthCap.Data
{
    public class DeckImage
    {
        protected DeckImage()
        {
        }

        public DeckImage(Deck deck)
        {
            DeckId = deck.Id;
            Deck = deck;
            Created = DateTime.Now;
            Modified = DateTime.Now;
        }

        [Key]
        [ForeignKey("Deck")]
        public Guid DeckId { get; protected set; }

        public Deck Deck { get; protected set; }

        [MaxLength]
        public byte[] Image { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        [Timestamp]
        public byte[] Version { get; protected set; }
    }
}
