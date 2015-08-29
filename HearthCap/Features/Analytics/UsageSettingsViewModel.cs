using System.ComponentModel.Composition;
using Caliburn.Micro;
using HearthCap.Features.Settings;

namespace HearthCap.Features.Analytics
{
    [Export(typeof(ISettingsScreen))]
    public class UsageSettingsViewModel : SettingsScreen
    {
        private IEventAggregator events;

        private bool shareUsageStatistics;

        [ImportingConstructor]
        public UsageSettingsViewModel(IEventAggregator events)
        {
            this.events = events;
            DisplayName = "Improve Hearthstone Tracker:";
            Order = 11;
            events.Subscribe(this);
        }

        private void UpdateSettings()
        {
            Tracker.IsEnabled = ShareUsageStatistics;
        }

        private void LoadSettings()
        {
            using (var reg = new AnalyticsRegistrySettings())
            {
                ShareUsageStatistics = reg.ShareUsageStatistics;
            }
        }

        public bool ShareUsageStatistics
        {
            get { return shareUsageStatistics; }
            set
            {
                if (value.Equals(shareUsageStatistics))
                {
                    return;
                }
                shareUsageStatistics = value;
                NotifyOfPropertyChange(() => ShareUsageStatistics);
                UpdateSettings();
            }
        }

        /// <summary>
        ///     Called when initializing.
        /// </summary>
        protected override void OnInitialize()
        {
            LoadSettings();
        }
    }
}
