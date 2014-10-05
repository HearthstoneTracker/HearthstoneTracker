// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArenaDeckImage.cs" company="">
//   
// </copyright>
// <summary>
//   The arena deck image.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Data
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// The arena deck image.
    /// </summary>
    public class ArenaDeckImage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArenaDeckImage"/> class.
        /// </summary>
        public ArenaDeckImage()
        {
            this.Id = Guid.NewGuid();
            this.Created = DateTime.Now;
            this.Modified = DateTime.Now;
        }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public Guid Id { get; set; }

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