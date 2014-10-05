// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebApiSettingsViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The web api settings view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

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

    /// <summary>
    /// The web api settings view model.
    /// </summary>
    [Export(typeof(IFlyout))]
    public class WebApiSettingsViewModel : FlyoutViewModel<IWebApiProviderScreen>.Collection.OneActive
    {
        /// <summary>
        /// The events.
        /// </summary>
        private readonly IEventAggregator events;

        /// <summary>
        /// The web api providers.
        /// </summary>
        private readonly IList<IWebApiProviderDescriptor> webApiProviders = new List<IWebApiProviderDescriptor>();

        /// <summary>
        /// The selected provider.
        /// </summary>
        private WebApiProviderDescriptor selectedProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebApiSettingsViewModel"/> class.
        /// </summary>
        /// <param name="events">
        /// The events.
        /// </param>
        /// <param name="webApiProviders">
        /// The web api providers.
        /// </param>
        [ImportingConstructor]
        public WebApiSettingsViewModel(
            IEventAggregator events, 
            [ImportMany]IEnumerable<IWebApiProviderDescriptor> webApiProviders)
        {
            this.SetPosition(Position.Left);
            this.Name = Flyouts.WebApi;
            this.events = events;
            this.Header = this.DisplayName = "Web API Settings:";

            // filter out providers
            var enabledProviders = ConfigurationManager.AppSettings["webapi_providers"];
            if (!string.IsNullOrEmpty(enabledProviders))
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

            this.PropertyChanged += this.OnPropertyChanged;
        }

        /// <summary>
        /// Gets the providers.
        /// </summary>
        public IEnumerable<IWebApiProviderDescriptor> Providers
        {
            get
            {
                return this.webApiProviders;
            }
        }

        /// <summary>
        /// Gets or sets the selected provider.
        /// </summary>
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

        /// <summary>
        /// Gets a value indicating whether is debug.
        /// </summary>
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

        /// <summary>
        /// The game started.
        /// </summary>
        public void GameStarted()
        {
            var msg = new GameStarted(GameMode.Practice, DateTime.Now, "mage", "warrior", true, "1");
            this.events.PublishOnBackgroundThread(msg);
        }

        /// <summary>
        /// The game ended.
        /// </summary>
        public void GameEnded()
        {
            var msg = new GameEnded {
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
            this.events.PublishOnBackgroundThread(msg);
        }

        /// <summary>
        /// The arena started.
        /// </summary>
        public void ArenaStarted()
        {
            var msg = new ArenaSessionStarted(DateTime.Now, "mage", 0, 0);
            this.events.PublishOnBackgroundThread(msg);
        }

        /// <summary>
        /// The arena ended.
        /// </summary>
        public void ArenaEnded()
        {
            var msg = new ArenaSessionEnded(DateTime.Now, DateTime.Now.AddMinutes(-5), "mage", 7, 3);
            this.events.PublishOnBackgroundThread(msg);
        }

        #endregion

        /// <summary>
        /// The on property changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
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