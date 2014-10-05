// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UsageSettingsViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The usage settings view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Analytics
{
    using System.ComponentModel.Composition;

    using Caliburn.Micro;

    using HearthCap.Features.Settings;

    /// <summary>
    /// The usage settings view model.
    /// </summary>
    [Export(typeof(ISettingsScreen))]
    public class UsageSettingsViewModel : SettingsScreen
    {
        /// <summary>
        /// The events.
        /// </summary>
        private IEventAggregator events;

        /// <summary>
        /// The share usage statistics.
        /// </summary>
        private bool shareUsageStatistics;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsageSettingsViewModel"/> class.
        /// </summary>
        /// <param name="events">
        /// The events.
        /// </param>
        [ImportingConstructor]
        public UsageSettingsViewModel(IEventAggregator events)
        {
            this.events = events;
            this.DisplayName = "Improve Hearthstone Tracker:";
            this.Order = 11;
            events.Subscribe(this);
        }

        /// <summary>
        /// The update settings.
        /// </summary>
        private void UpdateSettings()
        {
            Tracker.IsEnabled = this.ShareUsageStatistics;
        }

        /// <summary>
        /// The load settings.
        /// </summary>
        private void LoadSettings()
        {
            using (var reg = new AnalyticsRegistrySettings())
            {
                this.ShareUsageStatistics = reg.ShareUsageStatistics;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether share usage statistics.
        /// </summary>
        public bool ShareUsageStatistics
        {
            get
            {
                return this.shareUsageStatistics;
            }

            set
            {
                if (value.Equals(this.shareUsageStatistics))
                {
                    return;
                }

                this.shareUsageStatistics = value;
                this.NotifyOfPropertyChange(() => this.ShareUsageStatistics);
                this.UpdateSettings();
            }
        }

        /// <summary>
        /// Called when initializing.
        /// </summary>
        protected override void OnInitialize()
        {
            this.LoadSettings();
        }

    }
}