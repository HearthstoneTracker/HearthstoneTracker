namespace HearthCap.Shell.UserPreferences
{
    using System;

    public class ApplicationRegistrySettings : RegistrySettings
    {
        public ApplicationRegistrySettings()
            : base(@"Software\HearthstoneTracker")
        {
        }

        public string DefaultServer
        {
            get
            {
                return GetOrCreate("DefaultServer", String.Empty);
            }
            set
            {
                SetValue("DefaultServer", value);
            }
        }

        public string Servers
        {
            get
            {
                return GetOrCreate("Servers", "EU|NA|Asia|CN");
            }
            set
            {
                SetValue("Servers", value);
            }
        }
    }
}