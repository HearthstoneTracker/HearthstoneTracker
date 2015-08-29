using HearthCap.Shell.UserPreferences;

namespace HearthCap.Logging
{
    public class DataDirectorySettings : RegistrySettings
    {
        public DataDirectorySettings()
            : base(@"Software\HearthstoneTracker\")
        {
        }

        public string DataDirectory
        {
            get { return GetOrCreate("DataDirectory", string.Empty); }
            set { SetValue("DataDirectory", value); }
        }
    }
}
