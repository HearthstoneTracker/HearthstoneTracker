// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HmacSigningHandler.cs" company="">
//   
// </copyright>
// <summary>
//   The hmac signing handler.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.WebApi.Hmac
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// The hmac signing handler.
    /// </summary>
    public class HmacSigningHandler : HttpClientHandler
    {
        /// <summary>
        /// The api key.
        /// </summary>
        private readonly string apiKey;

        /// <summary>
        /// The secret key.
        /// </summary>
        private readonly string secretKey;

        /// <summary>
        /// The representation builder.
        /// </summary>
        private readonly IBuildMessageRepresentation representationBuilder;

        /// <summary>
        /// The signature calculator.
        /// </summary>
        private readonly ICalculteSignature signatureCalculator;

        /// <summary>
        /// Initializes a new instance of the <see cref="HmacSigningHandler"/> class.
        /// </summary>
        /// <param name="apiKey">
        /// The api key.
        /// </param>
        /// <param name="secretKey">
        /// The secret key.
        /// </param>
        /// <param name="representationBuilder">
        /// The representation builder.
        /// </param>
        /// <param name="signatureCalculator">
        /// The signature calculator.
        /// </param>
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

        /// <summary>
        /// The send async.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation token.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, 
                                                               CancellationToken cancellationToken)
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