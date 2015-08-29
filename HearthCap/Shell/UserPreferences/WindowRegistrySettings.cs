using System.Windows;

namespace HearthCap.Shell.UserPreferences
{
    public class WindowRegistrySettings : RegistrySettings
    {
        public WindowRegistrySettings()
            : base(@"Software\HearthstoneTracker")
        {
        }

        public double WindowTop
        {
            get { return GetOrCreate("WindowTop", (double)-1); }
            set { SetValue("WindowTop", value); }
        }

        public double WindowLeft
        {
            get { return GetOrCreate("WindowLeft", (double)-1); }
            set { SetValue("WindowLeft", value); }
        }

        public double WindowHeight
        {
            get { return GetOrCreate("WindowHeight", (double)600); }
            set { SetValue("WindowHeight", value); }
        }

        public double WindowWidth
        {
            get { return GetOrCreate("WindowWidth", (double)800); }
            set { SetValue("WindowWidth", value); }
        }

        public WindowState WindowState
        {
            get { return GetOrCreate("WindowState", WindowState.Normal); }
            set { SetValue("WindowState", value.ToString()); }
        }

        public bool StartMinimized
        {
            get { return GetOrCreate("StartMinimized", 0) == 1; }
            set { SetValue("StartMinimized", value ? 1 : 0); }
        }

        public bool MinimizeToTray
        {
            get { return GetOrCreate("MinimizeToTray", 0) == 1; }
            set { SetValue("MinimizeToTray", value ? 1 : 0); }
        }
    }
}
