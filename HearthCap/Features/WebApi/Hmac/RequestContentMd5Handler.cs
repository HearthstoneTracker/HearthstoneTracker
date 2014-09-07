namespace HearthCap.Features.WebApi.Hmac
{
    using System.Net.Http;
    using System.Security.Cryptography;
    using System.Threading.Tasks;

    public class RequestContentMd5Handler : DelegatingHandler
    {
        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
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