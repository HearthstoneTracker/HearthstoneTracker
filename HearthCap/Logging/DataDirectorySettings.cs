namespace HearthCap.Logging
{
    using HearthCap.Shell.UserPreferences;

    public class DataDirectorySettings : RegistrySettings
    {
        public DataDirectorySettings()
            : base(@"Software\HearthstoneTracker\")
        {
        }

        public string DataDirectory
        {
            get
            {
                return this.GetOrCreate("DataDirectory", string.Empty);
            }
            set
            {
                this.SetValue("DataDirectory", value);
            }
        }
    }
}