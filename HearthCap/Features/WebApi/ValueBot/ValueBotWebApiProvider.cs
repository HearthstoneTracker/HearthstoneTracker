using System;
using System.ComponentModel.Composition;
using HearthCap.Features.WebApi.Generic;

namespace HearthCap.Features.WebApi.ValueBot
{
    [Export(typeof(IWebApiProviderDescriptor))]
    public sealed class ValueBotWebApiProvider : WebApiProviderDescriptor, IDisposable
    {
        private readonly GenericWebApiEventsHandler eventsHandler;
        private readonly IWebApiProviderScreen settings;

        [ImportingConstructor]
        public ValueBotWebApiProvider()
            : base("ValueBot")
        {
            ProviderName = "Twitch.TV - ValueBot";
            settings = new ProviderSettingsViewModel(this);
            eventsHandler = new GenericWebApiEventsHandler();
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
