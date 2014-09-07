namespace HearthCap.Features.WebApi.Hmac
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    public class HmacSigningHandler : HttpClientHandler
    {
        private readonly string apiKey;

        private readonly string secretKey;

        private readonly IBuildMessageRepresentation representationBuilder;
        private readonly ICalculteSignature signatureCalculator;

        public HmacSigningHandler(
            string apiKey, 
            string secretKey,
            IBuildMessageRepresentation representationBuilder,
            ICalculteSignature signatureCalculator)
        {
            this.apiKey = apiKey;
            this.secretKey = secretKey;
            this.representationBuilder = representationBuilder;
            this.signatureCalculator = signatureCalculator;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                                                               System.Threading.CancellationToken cancellationToken)
        {
            if (!request.Headers.Contains(Configuration.ApiKeyHeader))
            {
                request.Headers.Add(Configuration.ApiKeyHeader, this.apiKey);
            }

            request.Headers.Date = new DateTimeOffset(DateTime.Now, DateTime.Now - DateTime.UtcNow);
            var representation = this.representationBuilder.BuildRequestRepresentation(request);
            string signature = this.signatureCalculator.Signature(this.secretKey, representation);
            var header = new AuthenticationHeaderValue(Configuration.AuthenticationScheme, signature);
            request.Headers.Authorization = header;
            return base.SendAsync(request, cancellationToken);
        }
    }
}