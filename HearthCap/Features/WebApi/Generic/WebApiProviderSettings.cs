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
            get
            {
                return this.GetOrCreate("Url", "");
            }
            set
            {
                this.SetValue("Url", value);
            }
        }

        public string ApiKey
        {
            get
            {
                return this.GetOrCreate("ApiKey", "");
            }
            set
            {
                this.SetValue("ApiKey", value);
            }
        }

        public string SecretKey
        {
            get
            {
                return this.GetOrCreate("SecretKey", "");
            }
            set
            {
                this.SetValue("SecretKey", value);
            }
        }
    }
}