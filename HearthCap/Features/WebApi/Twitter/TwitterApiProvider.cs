// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TwitterApiProvider.cs" company="">
//   
// </copyright>
// <summary>
//   The twitter api provider.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.WebApi.Twitter
{
    using System.ComponentModel.Composition;

    /// <summary>
    /// The twitter api provider.
    /// </summary>
    [Export(typeof(IWebApiProviderDescriptor))]
    public class TwitterApiProvider : WebApiProviderDescriptor
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
        /// Initializes a new instance of the <see cref="TwitterApiProvider"/> class.
        /// </summary>
        [ImportingConstructor]
        public TwitterApiProvider()
            : base("Twitter")
        {
            this.ProviderName = "Twitter";
            this.settings = new TwitterSettingsViewModel(this);
            this.eventsHandler = new TwitterApiEventsHandler();
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