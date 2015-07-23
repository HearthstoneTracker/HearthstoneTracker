namespace HearthCap.Features.WebApi.Hmac
{
    public interface ICalculteSignature
    {
        string Signature(string secret, string value);
    }
}