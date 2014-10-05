// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueBotWebApiProvider.cs" company="">
//   
// </copyright>
// <summary>
//   The value bot web api provider.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.WebApi.ValueBot
{
    using System.ComponentModel.Composition;

    using HearthCap.Features.WebApi.Generic;

    /// <summary>
    /// The value bot web api provider.
    /// </summary>
    [Export(typeof(IWebApiProviderDescriptor))]
    public class ValueBotWebApiProvider : WebApiProviderDescriptor
    {
        /// <summary>
        /// The events handler.
        /// </summary>
        private IWebApiEventsHandler eventsHandler;

        /// <summary>
        /// The settings.
        /// </summary>
        private IWebApiProviderScreen settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueBotWebApiProvider"/> class.
        /// </summary>
        [ImportingConstructor]
        public ValueBotWebApiProvider()
            : base("ValueBot")
        {
            this.ProviderName = "Twitch.TV - ValueBot";
            this.settings = new ProviderSettingsViewModel(this);
            this.eventsHandler = new GenericWebApiEventsHandler();
        }

        /// <summary>
        /// Gets the settings.
        /// </summary>
        public override IWebApiProviderScreen Settings
        {
            get
            {
                return this.settings;
            }
        }

        /// <summary>
        /// Gets the events handler.
        /// </summary>
        public override IWebApiEventsHandler EventsHandler
        {
            get
            {
                return this.eventsHandler;
            }
        }
    }
}