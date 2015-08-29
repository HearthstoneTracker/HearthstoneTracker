using System;
using HearthCap.Shell.UserPreferences;

namespace HearthCap.Features.WebApi
{
    public class ProviderSettings : RegistrySettings
    {
        public ProviderSettings(string providerKey)
            : base(String.Format(@"Software\HearthstoneTracker\{0}", providerKey))
        {
        }

        public bool Enabled
        {
            get { return GetOrCreate("Enabled", false); }
            set { SetValue("Enabled", value); }
        }
    }
}
