namespace HearthCap.Features.WebApi.Generic
{
    public class WebApiProviderSettings : ProviderSettings
    {
        public WebApiProviderSettings(string providerKey)
            : base(providerKey)
        {
        }

        public string Url
        {
            get { return GetOrCreate("Url", ""); }
            set { SetValue("Url", value); }
        }

        public string ApiKey
        {
            get { return GetOrCreate("ApiKey", ""); }
            set { SetValue("ApiKey", value); }
        }

        public string SecretKey
        {
            get { return GetOrCreate("SecretKey", ""); }
            set { SetValue("SecretKey", value); }
        }
    }
}
