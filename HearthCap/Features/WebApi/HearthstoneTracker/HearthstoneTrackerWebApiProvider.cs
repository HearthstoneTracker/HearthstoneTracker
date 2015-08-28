namespace HearthCap.Features.WebApi.HearthstoneTracker
{
    using System;
    using System.ComponentModel.Composition;

    using HearthCap.Features.WebApi.Generic;

    [Export(typeof(IWebApiProviderDescriptor))]
    public sealed class HearthstoneTrackerWebApiProvider : WebApiProviderDescriptor, IDisposable
    {
        private HearthstoneTrackerWebApiEventsHandler eventsHandler;
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

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
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