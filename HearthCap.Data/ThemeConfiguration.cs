// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ThemeConfiguration.cs" company="">
//   
// </copyright>
// <summary>
//   The theme configuration.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Data
{
    using System;

    /// <summary>
    /// The theme configuration.
    /// </summary>
    public class ThemeConfiguration : IEntityWithId<Guid>
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public Guid Id { get; protected set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the accent.
        /// </summary>
        public string Accent { get; set; }

        /// <summary>
        /// Gets or sets the theme.
        /// </summary>
        public string Theme { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThemeConfiguration"/> class.
        /// </summary>
        public ThemeConfiguration()
        {
            this.Id = Guid.NewGuid();
        }
    }
}