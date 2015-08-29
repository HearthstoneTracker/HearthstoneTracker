using HearthCap.Shell.UserPreferences;

namespace HearthCap.Features.Analytics
{
    public class AnalyticsRegistrySettings : RegistrySettings
    {
        public AnalyticsRegistrySettings()
            : base(@"Software\HearthstoneTracker\")
        {
        }

        public string Cookie
        {
            get { return GetOrCreate("pstore", ""); }
            set { SetValue("pstore", value); }
        }

        public bool ShareUsageStatistics
        {
            get { return GetOrCreate("ShareUsageStatistics", true); }
            set { SetValue("ShareUsageStatistics", value); }
        }
    }
}
