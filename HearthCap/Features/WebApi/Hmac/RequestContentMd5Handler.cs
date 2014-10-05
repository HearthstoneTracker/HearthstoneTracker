// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequestContentMd5Handler.cs" company="">
//   
// </copyright>
// <summary>
//   The request content md 5 handler.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.WebApi.Hmac
{
    using System.Net.Http;
    using System.Security.Cryptography;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// The request content md 5 handler.
    /// </summary>
    public class RequestContentMd5Handler : DelegatingHandler
    {
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
        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Content == null)
            {
                return await base.SendAsync(request, cancellationToken);
            }

            byte[] content = await request.Content.ReadAsByteArrayAsync();
            MD5 md5 = MD5.Create();
            byte[] hash = md5.ComputeHash(content);
            request.Content.Headers.ContentMD5 = hash;
            var response = await base.SendAsync(request, cancellationToken);
            return response;
        }
    }
}