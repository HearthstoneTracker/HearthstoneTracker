// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnalyticsRegistrySettings.cs" company="">
//   
// </copyright>
// <summary>
//   The analytics registry settings.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Analytics
{
    using HearthCap.Shell.UserPreferences;

    /// <summary>
    /// The analytics registry settings.
    /// </summary>
    public class AnalyticsRegistrySettings : RegistrySettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnalyticsRegistrySettings"/> class.
        /// </summary>
        public AnalyticsRegistrySettings()
            : base(@"Software\HearthstoneTracker\")
        {
        }

        /// <summary>
        /// Gets or sets the cookie.
        /// </summary>
        public string Cookie
        {
            get
            {
                return this.GetOrCreate("pstore", string.Empty);
            }

            set
            {
                this.SetValue("pstore", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether share usage statistics.
        /// </summary>
        public bool ShareUsageStatistics
        {
            get
            {
                return this.GetOrCreate("ShareUsageStatistics", true);
            }

            set
            {
                this.SetValue("ShareUsageStatistics", value);
            }
        }
    }
}