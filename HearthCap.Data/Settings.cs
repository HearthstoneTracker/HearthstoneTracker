// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Settings.cs" company="">
//   
// </copyright>
// <summary>
//   The settings.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Data
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// The settings.
    /// </summary>
    public class Settings : IEntityWithId<int>
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [Key]
        public int Id { get; protected set; }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        public virtual IList<SettingsItem> Items { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Settings"/> class.
        /// </summary>
        public Settings()
        {
            this.Items = new List<SettingsItem>();
        }
    }
}