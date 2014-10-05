// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ThemeRegistrySettings.cs" company="">
//   
// </copyright>
// <summary>
//   The theme registry settings.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Shell.Theme
{
    using HearthCap.Shell.UserPreferences;

    using MahApps.Metro.Controls;

    /// <summary>
    /// The theme registry settings.
    /// </summary>
    public class ThemeRegistrySettings : RegistrySettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThemeRegistrySettings"/> class.
        /// </summary>
        public ThemeRegistrySettings()
            : base(@"Software\HearthstoneTracker")
        {
        }

        /// <summary>
        /// Gets or sets the flyout theme.
        /// </summary>
        public FlyoutTheme FlyoutTheme
        {
            get
            {
                return this.GetOrCreate("FlyoutTheme", FlyoutTheme.Dark);
            }

            set
            {
                this.SetValue("FlyoutTheme", value);
            }
        }
    }
}