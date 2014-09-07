namespace HearthCap.Features.WebApi.Hmac
{
    using System.Net.Http;

    public interface IBuildMessageRepresentation
    {
        string BuildRequestRepresentation(HttpRequestMessage requestMessage);
    }
}