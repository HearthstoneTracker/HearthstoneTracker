using System.ComponentModel.Composition;

namespace HearthCap.Features.WebApi.Twitter
{
    [Export(typeof(IWebApiProviderDescriptor))]
    public class TwitterApiProvider : WebApiProviderDescriptor
    {
        private readonly IWebApiEventsHandler eventsHandler;
        private readonly IWebApiProviderScreen settings;

        [ImportingConstructor]
        public TwitterApiProvider()
            : base("Twitter")
        {
            ProviderName = "Twitter";
            settings = new TwitterSettingsViewModel(this);
            eventsHandler = new TwitterApiEventsHandler();
        }

        public override IWebApiProviderScreen Settings
        {
            get { return settings; }
        }

        public override IWebApiEventsHandler EventsHandler
        {
            get { return eventsHandler; }
        }
    }
}
