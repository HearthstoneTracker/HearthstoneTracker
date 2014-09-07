namespace HearthCap.Features.WebApi
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.Composition;
    using System.Configuration;
    using System.Linq;

    using Caliburn.Micro;

    using HearthCap.Core.GameCapture.HS.Events;
    using HearthCap.Data;
    using HearthCap.Shell.Flyouts;

    using MahApps.Metro.Controls;

    [Export(typeof(IFlyout))]
    public class WebApiSettingsViewModel : FlyoutViewModel<IWebApiProviderScreen>.Collection.OneActive
    {
        private readonly IEventAggregator events;

        private readonly IList<IWebApiProviderDescriptor> webApiProviders = new List<IWebApiProviderDescriptor>();

        private WebApiProviderDescriptor selectedProvider;

        [ImportingConstructor]
        public WebApiSettingsViewModel(
            IEventAggregator events,
            [ImportMany]IEnumerable<IWebApiProviderDescriptor> webApiProviders)
        {
            SetPosition(Position.Left);
            this.Name = Flyouts.WebApi;
            this.events = events;
            this.Header = this.DisplayName = "Web API Settings:";

            // filter out providers
            var enabledProviders = ConfigurationManager.AppSettings["webapi_providers"];
            if (!String.IsNullOrEmpty(enabledProviders))
            {
                var providerKeys = enabledProviders.ToLower().Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var provider in webApiProviders)
                {
                    if (providerKeys.Any(x => x.Trim() == provider.ProviderKey.ToLower()))
                    {
                        this.webApiProviders.Add(provider);
                    }
                }
            }
            this.PropertyChanged += OnPropertyChanged;
        }

        public IEnumerable<IWebApiProviderDescriptor> Providers
        {
            get
            {
                return this.webApiProviders;
            }
        }

        public WebApiProviderDescriptor SelectedProvider
        {
            get
            {
                return this.selectedProvider;
            }
            set
            {
                if (Equals(value, this.selectedProvider))
                {
                    return;
                }
                this.selectedProvider = value;
                this.NotifyOfPropertyChange(() => this.SelectedProvider);
            }
        }

        public bool IsDebug
        {
            get
            {
#if DEBUG
                return true;
#else
                if (ConfigurationManager.AppSettings["supersecrettestsetting"] == "1")
                {
                    return true;
                }
                else
                {
                    return false;
                }
#endif
            }
        }

        #region TestMethod

        public void GameStarted()
        {
            var msg = new GameStarted(GameMode.Practice, DateTime.Now, "mage", "warrior", true, "1");
            events.PublishOnBackgroundThread(msg);
        }

        public void GameEnded()
        {
            var msg = new GameEnded()
                          {
                              GameMode = GameMode.Practice,
                              Date = DateTime.Now,
                              Hero = "mage",
                              OpponentHero = "warrior",
                              Victory = true,
                              Turns = 17,
                              GoFirst = true,
                              StartTime = DateTime.Now.Subtract(TimeSpan.FromMinutes(2)),
                              EndTime = DateTime.Now,
                              Deck = "1"
                          };
            events.PublishOnBackgroundThread(msg);
        }

        public void ArenaStarted()
        {
            var msg = new ArenaSessionStarted(DateTime.Now, "mage", 0, 0);
            events.PublishOnBackgroundThread(msg);
        }

        public void ArenaEnded()
        {
            var msg = new ArenaSessionEnded(DateTime.Now, DateTime.Now.AddMinutes(-5), "mage", 7, 3);
            events.PublishOnBackgroundThread(msg);
        }
        #endregion

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedProvider")
            {
                if (this.SelectedProvider != null && this.SelectedProvider.Settings != null)
                {
                    this.ActivateItem(this.SelectedProvider.Settings);
                }
            }
        }
    }
}