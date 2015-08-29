using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace HearthCap.Features.WebApi.Hmac
{
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
            CancellationToken cancellationToken)
        {
            if (!request.Headers.Contains(Configuration.ApiKeyHeader))
            {
                request.Headers.Add(Configuration.ApiKeyHeader, apiKey);
            }

            request.Headers.Date = new DateTimeOffset(DateTime.Now, DateTime.Now - DateTime.UtcNow);
            var representation = representationBuilder.BuildRequestRepresentation(request);
            var signature = signatureCalculator.Signature(secretKey, representation);
            var header = new AuthenticationHeaderValue(Configuration.AuthenticationScheme, signature);
            request.Headers.Authorization = header;
            return base.SendAsync(request, cancellationToken);
        }
    }
}
