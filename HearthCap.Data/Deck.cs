// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Deck.cs" company="">
//   
// </copyright>
// <summary>
//   The deck.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Data
{
    using System;

    /// <summary>
    /// The deck.
    /// </summary>
    public class Deck : IEntityWithId<Guid>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Deck"/> class.
        /// </summary>
        public Deck()
        {
            this.Id = Guid.NewGuid();
            this.Created = DateTime.Now;
            this.Modified = this.Created;
        }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public Guid Id { get; protected set; }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the modified.
        /// </summary>
        public DateTime Modified { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether deleted.
        /// </summary>
        public bool Deleted { get; set; }

        /// <summary>
        /// Gets or sets the server.
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        public DeckImage Image { get; set; }

        /// <summary>
        /// Gets or sets the notes.
        /// </summary>
        public string Notes { get; set; }

        // TODO: more deck features :)
        // like associate a hero
    }
}