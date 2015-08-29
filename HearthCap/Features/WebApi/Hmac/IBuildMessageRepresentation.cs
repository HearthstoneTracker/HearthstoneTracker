using System.Net.Http;

namespace HearthCap.Features.WebApi.Hmac
{
    public interface IBuildMessageRepresentation
    {
        string BuildRequestRepresentation(HttpRequestMessage requestMessage);
    }
}
