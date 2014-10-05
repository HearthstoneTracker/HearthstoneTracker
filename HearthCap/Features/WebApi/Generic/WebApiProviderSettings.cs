// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebApiProviderSettings.cs" company="">
//   
// </copyright>
// <summary>
//   The web api provider settings.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.WebApi.Generic
{
    /// <summary>
    /// The web api provider settings.
    /// </summary>
    public class WebApiProviderSettings : ProviderSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebApiProviderSettings"/> class.
        /// </summary>
        /// <param name="providerKey">
        /// The provider key.
        /// </param>
        public WebApiProviderSettings(string providerKey)
            : base(providerKey)
        {
        }

        /// <summary>
        /// Gets or sets the url.
        /// </summary>
        public string Url
        {
            get
            {
                return this.GetOrCreate("Url", string.Empty);
            }

            set
            {
                this.SetValue("Url", value);
            }
        }

        /// <summary>
        /// Gets or sets the api key.
        /// </summary>
        public string ApiKey
        {
            get
            {
                return this.GetOrCreate("ApiKey", string.Empty);
            }

            set
            {
                this.SetValue("ApiKey", value);
            }
        }

        /// <summary>
        /// Gets or sets the secret key.
        /// </summary>
        public string SecretKey
        {
            get
            {
                return this.GetOrCreate("SecretKey", string.Empty);
            }

            set
            {
                this.SetValue("SecretKey", value);
            }
        }
    }
}