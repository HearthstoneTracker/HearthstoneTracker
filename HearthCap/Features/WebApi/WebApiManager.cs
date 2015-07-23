namespace HearthCap.Features.WebApi
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.Composition;
    using System.Linq;

    using Caliburn.Micro;

    using HearthCap.StartUp;

    [Export(typeof(IStartupTask))]
    public class WebApiManager : IStartupTask
    {
        private readonly IEventAggregator events;

        private readonly IList<IWebApiProviderDescriptor> webApiProviders;

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