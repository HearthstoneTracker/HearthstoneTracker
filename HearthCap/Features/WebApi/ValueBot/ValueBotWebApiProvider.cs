namespace HearthCap.Features.WebApi.ValueBot
{
    using System;
    using System.ComponentModel.Composition;

    using HearthCap.Features.WebApi.Generic;

    [Export(typeof(IWebApiProviderDescriptor))]
    public sealed class ValueBotWebApiProvider : WebApiProviderDescriptor, IDisposable
    {
        private GenericWebApiEventsHandler eventsHandler;
        private IWebApiProviderScreen settings;

        [ImportingConstructor]
        public ValueBotWebApiProvider()
            : base("ValueBot")
        {
            this.ProviderName = "Twitch.TV - ValueBot";
            this.settings = new ProviderSettingsViewModel(this);
            this.eventsHandler = new GenericWebApiEventsHandler();
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