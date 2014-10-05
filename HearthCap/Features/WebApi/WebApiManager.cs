// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebApiManager.cs" company="">
//   
// </copyright>
// <summary>
//   The web api manager.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.WebApi
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.Composition;
    using System.Linq;

    using Caliburn.Micro;

    using HearthCap.StartUp;

    /// <summary>
    /// The web api manager.
    /// </summary>
    [Export(typeof(IStartupTask))]
    public class WebApiManager : IStartupTask
    {
        /// <summary>
        /// The events.
        /// </summary>
        private readonly IEventAggregator events;

        /// <summary>
        /// The web api providers.
        /// </summary>
        private readonly IList<IWebApiProviderDescriptor> webApiProviders;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebApiManager"/> class.
        /// </summary>
        /// <param name="events">
        /// The events.
        /// </param>
        /// <param name="webApiProviders">
        /// The web api providers.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        [ImportingConstructor]
        public WebApiManager(
            IEventAggregator events, 
            [ImportMany] IEnumerable<IWebApiProviderDescriptor> webApiProviders)
        {
            if (webApiProviders == null)
            {
                throw new ArgumentNullException("webApiProviders");
            }

            this.events = events;
            this.webApiProviders = webApiProviders.ToList();
            foreach (var provider in this.webApiProviders)
            {
                provider.PropertyChanged += this.ProviderOnPropertyChanged;
            }
        }

        /// <summary>
        /// The provider on property changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ProviderOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var provider = (IWebApiProviderDescriptor)sender;
            if (e.PropertyName == "IsEnabled")
            {
                if (provider.IsEnabled)
                {
                    this.events.Subscribe(provider.EventsHandler);
                }
                else
                {
                    this.events.Unsubscribe(provider.EventsHandler);
                }
            }
        }

        /// <summary>
        /// The run.
        /// </summary>
        public void Run()
        {
            foreach (var provider in this.webApiProviders)
            {
                provider.Initialize();

                if (provider.IsEnabled && provider.EventsHandler != null)
                {
                    this.events.Subscribe(provider.EventsHandler);
                }
            }            
        }
    }
}