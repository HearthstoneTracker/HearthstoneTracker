// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataDirectorySettings.cs" company="">
//   
// </copyright>
// <summary>
//   The data directory settings.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Logging
{
    using HearthCap.Shell.UserPreferences;

    /// <summary>
    /// The data directory settings.
    /// </summary>
    public class DataDirectorySettings : RegistrySettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataDirectorySettings"/> class.
        /// </summary>
        public DataDirectorySettings()
            : base(@"Software\HearthstoneTracker\")
        {
        }

        /// <summary>
        /// Gets or sets the data directory.
        /// </summary>
        public string DataDirectory
        {
            get
            {
                return this.GetOrCreate("DataDirectory", string.Empty);
            }

            set
            {
                this.SetValue("DataDirectory", value);
            }
        }
    }
}