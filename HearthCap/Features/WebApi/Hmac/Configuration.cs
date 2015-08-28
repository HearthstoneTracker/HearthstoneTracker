namespace HearthCap.Features.WebApi.Hmac
{
    public class Configuration
    {
        public const string ApiKeyHeader = "X-ApiAuth-ApiKey";
        public const string AuthenticationScheme = "ApiAuth";
        public const int ValidityPeriodInMinutes = 5;
    }
}