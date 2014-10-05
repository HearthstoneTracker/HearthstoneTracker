// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Configuration.cs" company="">
//   
// </copyright>
// <summary>
//   The configuration.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.WebApi.Hmac
{
    /// <summary>
    /// The configuration.
    /// </summary>
    public class Configuration
    {
        /// <summary>
        /// The api key header.
        /// </summary>
        public const string ApiKeyHeader = "X-ApiAuth-ApiKey";

        /// <summary>
        /// The authentication scheme.
        /// </summary>
        public const string AuthenticationScheme = "ApiAuth";

        /// <summary>
        /// The validity period in minutes.
        /// </summary>
        public const int ValidityPeriodInMinutes = 5;
    }
}