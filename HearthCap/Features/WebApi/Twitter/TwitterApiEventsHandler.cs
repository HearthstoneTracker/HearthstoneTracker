namespace HearthCap.Features.WebApi.Twitter
{
    public class TwitterApiEventsHandler : IWebApiEventsHandler
    {
        private WebApiProviderDescriptor providerDescriptor;

        public TwitterApiEventsHandler()
        {
            
        }

        public void Initialize(WebApiProviderDescriptor providerDescriptor)
        {
            this.providerDescriptor = providerDescriptor;
        }
    }
}