namespace HearthCap.Features.WebApi.Twitter
{
    public class TwitterApiEventsHandler : IWebApiEventsHandler
    {
        private WebApiProviderDescriptor providerDescriptor;

        public void Initialize(WebApiProviderDescriptor providerDescriptor)
        {
            this.providerDescriptor = providerDescriptor;
        }
    }
}
