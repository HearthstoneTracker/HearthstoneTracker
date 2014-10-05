// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SettingsItem.cs" company="">
//   
// </copyright>
// <summary>
//   The settings item.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Data
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// The settings item.
    /// </summary>
    public class SettingsItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsItem"/> class.
        /// </summary>
        protected SettingsItem()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsItem"/> class.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public SettingsItem(string key, Settings settings)
        {
            this.Key = key;
            this.Settings = settings;
        }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        [Key]
        public string Key { get; protected set; }

        /// <summary>
        /// Gets or sets the settings.
        /// </summary>
        public Settings Settings { get; protected set; }

        /// <summary>
        /// Gets or sets the string value.
        /// </summary>
        public string StringValue { get; set; }

        /// <summary>
        /// Gets or sets the int value.
        /// </summary>
        public int IntValue { get; set; }
    }
}