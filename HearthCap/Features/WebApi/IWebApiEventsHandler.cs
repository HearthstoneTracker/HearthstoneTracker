namespace HearthCap.Features.WebApi
{
    public interface IWebApiEventsHandler
    {
        void Initialize(WebApiProviderDescriptor providerDescriptor);
    }
}
