// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeckImage.cs" company="">
//   
// </copyright>
// <summary>
//   The deck image.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Data
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// The deck image.
    /// </summary>
    public class DeckImage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeckImage"/> class.
        /// </summary>
        protected DeckImage()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeckImage"/> class.
        /// </summary>
        /// <param name="deck">
        /// The deck.
        /// </param>
        public DeckImage(Deck deck)
        {
            this.DeckId = deck.Id;
            this.Deck = deck;
            this.Created = DateTime.Now;
            this.Modified = DateTime.Now;
        }

        /// <summary>
        /// Gets or sets the deck id.
        /// </summary>
        [Key, ForeignKey("Deck")]
        public Guid DeckId { get; protected set; }

        /// <summary>
        /// Gets or sets the deck.
        /// </summary>
        public Deck Deck { get; protected set; }

        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        [MaxLength]
        public byte[] Image { get; set; }

        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the modified.
        /// </summary>
        public DateTime Modified { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        [Timestamp]
        public byte[] Version { get; protected set; }
    }
}