// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HearthstoneTrackerWebApiProvider.cs" company="">
//   
// </copyright>
// <summary>
//   The hearthstone tracker web api provider.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.WebApi.HearthstoneTracker
{
    using System.ComponentModel.Composition;

    /// <summary>
    /// The hearthstone tracker web api provider.
    /// </summary>
    [Export(typeof(IWebApiProviderDescriptor))]
    public class HearthstoneTrackerWebApiProvider : WebApiProviderDescriptor
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
        /// Initializes a new instance of the <see cref="HearthstoneTrackerWebApiProvider"/> class.
        /// </summary>
        [ImportingConstructor]
        public HearthstoneTrackerWebApiProvider()
            : base("HearthstoneTracker")
        {
            this.ProviderName = "HearthstoneTracker.com";
            this.settings = new ProviderSettingsViewModel(this);
            this.eventsHandler = new HearthstoneTrackerWebApiEventsHandler();
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