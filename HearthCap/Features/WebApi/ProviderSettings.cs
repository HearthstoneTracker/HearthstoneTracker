namespace HearthCap.Features.WebApi
{
    using System;

    using HearthCap.Shell.UserPreferences;

    public class ProviderSettings : RegistrySettings
    {
        public ProviderSettings(string providerKey)
            : base(String.Format(@"Software\HearthstoneTracker\{0}", providerKey))
        {
        }

        public bool Enabled
        {
            get
            {
                return this.GetOrCreate("Enabled", false);
            }
            set
            {
                this.SetValue("Enabled", value);
            }
        }
    }
}