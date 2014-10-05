// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProviderSettings.cs" company="">
//   
// </copyright>
// <summary>
//   The provider settings.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.WebApi
{
    using HearthCap.Shell.UserPreferences;

    /// <summary>
    /// The provider settings.
    /// </summary>
    public class ProviderSettings : RegistrySettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProviderSettings"/> class.
        /// </summary>
        /// <param name="providerKey">
        /// The provider key.
        /// </param>
        public ProviderSettings(string providerKey)
            : base(string.Format(@"Software\HearthstoneTracker\{0}", providerKey))
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether enabled.
        /// </summary>
        public bool Enabled
        {
            get
            {
                return this.GetOrCreate("Enabled", false);
            }

            set
            {
                this.SetValue("Enabled", value);
            }
        }
    }
}