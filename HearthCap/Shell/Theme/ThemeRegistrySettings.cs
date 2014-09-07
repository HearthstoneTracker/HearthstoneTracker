namespace HearthCap.Shell.Theme
{
    using HearthCap.Shell.UserPreferences;

    using MahApps.Metro.Controls;

    public class ThemeRegistrySettings : RegistrySettings
    {
        public ThemeRegistrySettings()
            : base(@"Software\HearthstoneTracker")
        {
        }

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