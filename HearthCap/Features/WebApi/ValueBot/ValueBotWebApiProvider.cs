namespace HearthCap.Features.WebApi.ValueBot
{
    using System.ComponentModel.Composition;

    using HearthCap.Features.WebApi.Generic;

    [Export(typeof(IWebApiProviderDescriptor))]
    public class ValueBotWebApiProvider : WebApiProviderDescriptor
    {
        private IWebApiEventsHandler eventsHandler;
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
    }
}