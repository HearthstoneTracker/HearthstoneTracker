namespace HearthCap.Features.WebApi.Twitter
{
    using System.ComponentModel.Composition;

    using HearthCap.Features.WebApi.Generic;

    [Export(typeof(IWebApiProviderDescriptor))]
    public class TwitterApiProvider : WebApiProviderDescriptor
    {
        private IWebApiEventsHandler eventsHandler;
        private IWebApiProviderScreen settings;

        [ImportingConstructor]
        public TwitterApiProvider()
            : base("Twitter")
        {
            this.ProviderName = "Twitter";
            this.settings = new TwitterSettingsViewModel(this);
            this.eventsHandler = new TwitterApiEventsHandler();
        }

        public override IWebApiProviderScreen Settings
        {
            get
            {
                return this.settings;
            }
        }

        public override IWebApiEventsHandler EventsHandler
        {
            get
            {
                return this.eventsHandler;
            }
        }
    }
}