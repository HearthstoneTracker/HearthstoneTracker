namespace HearthCap.Shell.UserPreferences
{
    using System.Windows;

    public class WindowRegistrySettings : RegistrySettings
    {
        public WindowRegistrySettings()
            : base(@"Software\HearthstoneTracker")
        {
        }

        public double WindowTop
        {
            get
            {
                return this.GetOrCreate("WindowTop", (double)-1);
            }
            set
            {
                this.SetValue("WindowTop", value);
            }
        }

        public double WindowLeft
        {
            get
            {
                return this.GetOrCreate("WindowLeft", (double)-1);
            }
            set
            {
                this.SetValue("WindowLeft", value);
            }
        }

        public double WindowHeight
        {
            get
            {
                return this.GetOrCreate("WindowHeight", (double)600);
            }
            set
            {
                this.SetValue("WindowHeight", value);
            }
        }

        public double WindowWidth
        {
            get
            {
                return this.GetOrCreate("WindowWidth", (double)800);
            }
            set
            {
                this.SetValue("WindowWidth", value);
            }
        }

        public WindowState WindowState
        {
            get
            {
                return this.GetOrCreate("WindowState", WindowState.Normal);
            }
            set
            {
                this.SetValue("WindowState", value.ToString());
            }
        }

        public bool StartMinimized
        {
            get
            {
                return this.GetOrCreate("StartMinimized", 0) == 1;
            }
            set
            {
                this.SetValue("StartMinimized", value ? 1 : 0);
            }
        }

        public bool MinimizeToTray
        {
            get
            {
                return this.GetOrCreate("MinimizeToTray", 0) == 1;
            }
            set
            {
                this.SetValue("MinimizeToTray", value ? 1 : 0);
            }
        }
    }
}