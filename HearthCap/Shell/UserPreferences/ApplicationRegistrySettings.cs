// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationRegistrySettings.cs" company="">
//   
// </copyright>
// <summary>
//   The application registry settings.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Shell.UserPreferences
{
    /// <summary>
    /// The application registry settings.
    /// </summary>
    public class ApplicationRegistrySettings : RegistrySettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationRegistrySettings"/> class.
        /// </summary>
        public ApplicationRegistrySettings()
            : base(@"Software\HearthstoneTracker")
        {
        }

        /// <summary>
        /// Gets or sets the default server.
        /// </summary>
        public string DefaultServer
        {
            get
            {
                return this.GetOrCreate("DefaultServer", string.Empty);
            }

            set
            {
                this.SetValue("DefaultServer", value);
            }
        }

        /// <summary>
        /// Gets or sets the servers.
        /// </summary>
        public string Servers
        {
            get
            {
                return this.GetOrCreate("Servers", "EU|NA|Asia|CN");
            }

            set
            {
                this.SetValue("Servers", value);
            }
        }
    }
}