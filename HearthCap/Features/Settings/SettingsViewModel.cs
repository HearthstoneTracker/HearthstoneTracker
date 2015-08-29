using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using HearthCap.Features.Analytics;
using HearthCap.Shell.Flyouts;
using MahApps.Metro.Controls;

namespace HearthCap.Features.Settings
{
    [Export(typeof(IFlyout))]
    public class SettingsViewModel : FlyoutViewModel<ISettingsScreen>.Collection.AllActive
    {
        private bool firstTime = true;

        [ImportingConstructor]
        public SettingsViewModel([ImportMany] IEnumerable<ISettingsScreen> settingsScreens)
        {
            // this.settingsScreens = new BindableCollection<ISettingsScreen>(settingsScreens.OrderBy(x=>x.Order));
            Name = "settings";
            Header = "Settings";
            SetPosition(Position.Left);
            Items.AddRange(settingsScreens.OrderBy(x => x.Order));
            PropertyChanged += (sender, args) =>
                {
                    if (args.PropertyName == "IsOpen" && IsOpen)
                    {
                        Tracker.TrackEventAsync(Tracker.FlyoutsCategory, "Open", Name, 1);
                    }
                };
        }

        /// <summary>
        ///     Called when activating.
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
