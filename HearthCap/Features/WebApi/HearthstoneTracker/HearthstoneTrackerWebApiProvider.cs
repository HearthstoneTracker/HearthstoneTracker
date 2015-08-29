using System;
using System.ComponentModel.Composition;

namespace HearthCap.Features.WebApi.HearthstoneTracker
{
    [Export(typeof(IWebApiProviderDescriptor))]
    public sealed class HearthstoneTrackerWebApiProvider : WebApiProviderDescriptor, IDisposable
    {
        private readonly HearthstoneTrackerWebApiEventsHandler eventsHandler;
        private readonly IWebApiProviderScreen settings;

        [ImportingConstructor]
        public HearthstoneTrackerWebApiProvider()
            : base("HearthstoneTracker")
        {
            ProviderName = "HearthstoneTracker.com";
            settings = new ProviderSettingsViewModel(this);
            eventsHandler = new HearthstoneTrackerWebApiEventsHandler();
        }

        public override IWebApiProviderScreen Settings
        {
            get { return settings; }
        }

        public override IWebApiEventsHandler EventsHandler
        {
            get { return eventsHandler; }
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (eventsHandler != null)
            {
                eventsHandler.Dispose();
            }
        }
    }
}
