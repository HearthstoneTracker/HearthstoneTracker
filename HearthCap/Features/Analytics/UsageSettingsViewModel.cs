namespace HearthCap.Features.Analytics
{
    using System.ComponentModel.Composition;

    using Caliburn.Micro;

    using HearthCap.Features.Settings;
    using HearthCap.Util;

    [Export(typeof(ISettingsScreen))]
    public class UsageSettingsViewModel : SettingsScreen
    {
        private IEventAggregator events;

        private bool shareUsageStatistics;

        [ImportingConstructor]
        public UsageSettingsViewModel(IEventAggregator events)
        {
            this.events = events;
            this.DisplayName = "Improve Hearthstone Tracker:";
            this.Order = 11;
            events.Subscribe(this);
        }

        private void UpdateSettings()
        {
            Tracker.IsEnabled = this.ShareUsageStatistics;
        }

        private void LoadSettings()
        {
            using (var reg = new AnalyticsRegistrySettings())
            {
                this.ShareUsageStatistics = reg.ShareUsageStatistics;
            }
        }

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