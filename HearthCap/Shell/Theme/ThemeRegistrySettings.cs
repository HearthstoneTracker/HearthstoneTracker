using HearthCap.Shell.UserPreferences;
using MahApps.Metro.Controls;

namespace HearthCap.Shell.Theme
{
    public class ThemeRegistrySettings : RegistrySettings
    {
        public ThemeRegistrySettings()
            : base(@"Software\HearthstoneTracker")
        {
        }

        public FlyoutTheme FlyoutTheme
        {
            get { return GetOrCreate("FlyoutTheme", FlyoutTheme.Dark); }
            set { SetValue("FlyoutTheme", value); }
        }
    }
}
