namespace HearthCap.Features.Settings
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;

    using Caliburn.Micro;

    using HearthCap.Features.Analytics;
    using HearthCap.Shell.Flyouts;

    using MahApps.Metro.Controls;

    [Export(typeof(IFlyout))]
    public class SettingsViewModel : FlyoutViewModel<ISettingsScreen>.Collection.AllActive
    {
        private readonly BindableCollection<ISettingsScreen> settingsScreens;

        private bool firstTime = true;

        [ImportingConstructor]
        public SettingsViewModel([ImportMany]IEnumerable<ISettingsScreen> settingsScreens)
        {
            // this.settingsScreens = new BindableCollection<ISettingsScreen>(settingsScreens.OrderBy(x=>x.Order));
            this.Name = "settings";
            this.Header = "Settings";
            SetPosition(Position.Left);
            this.Items.AddRange(settingsScreens.OrderBy(x => x.Order));
            this.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "IsOpen" && IsOpen)
                {
                    Tracker.TrackEventAsync(Tracker.FlyoutsCategory, "Open", Name, 1);
                }
            };
        }

        //public IObservableCollection<ISettingsScreen> SettingsScreens
        //{
        //    get
        //    {
        //        return this.settingsScreens;
        //    }
        //}

        /// <summary>
        /// Called when activating.
        /// </summary>
        protected override void OnActivate()
        {
            if (firstTime)
            {
                firstTime = false;

                foreach (var settingsScreen in Items)
                {
                    ActivateItem(settingsScreen);
                }
            }
        }
    }
}