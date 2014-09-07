namespace HearthCap.Features.WebApi.HearthstoneTracker
{
    using System.ComponentModel.Composition;

    using HearthCap.Features.WebApi.Generic;

    [Export(typeof(IWebApiProviderDescriptor))]
    public class HearthstoneTrackerWebApiProvider : WebApiProviderDescriptor
    {
        private IWebApiEventsHandler eventsHandler;
        private IWebApiProviderScreen settings;

        [ImportingConstructor]
        public HearthstoneTrackerWebApiProvider()
            : base("HearthstoneTracker")
        {
            this.ProviderName = "HearthstoneTracker.com";
            this.settings = new ProviderSettingsViewModel(this);
            this.eventsHandler = new HearthstoneTrackerWebApiEventsHandler();
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