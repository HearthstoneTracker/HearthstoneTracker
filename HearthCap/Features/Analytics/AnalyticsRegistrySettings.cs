namespace HearthCap.Features.Analytics
{
    using HearthCap.Shell.UserPreferences;

    public class AnalyticsRegistrySettings : RegistrySettings
    {
        public AnalyticsRegistrySettings()
            : base(@"Software\HearthstoneTracker\")
        {
        }

        public string Cookie
        {
            get
            {
                return this.GetOrCreate("pstore", "");
            }
            set
            {
                this.SetValue("pstore", value);
            }
        }

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