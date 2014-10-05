// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SettingsViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The settings view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Settings
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;

    using Caliburn.Micro;

    using HearthCap.Features.Analytics;
    using HearthCap.Shell.Flyouts;

    using MahApps.Metro.Controls;

    /// <summary>
    /// The settings view model.
    /// </summary>
    [Export(typeof(IFlyout))]
    public class SettingsViewModel : FlyoutViewModel<ISettingsScreen>.Collection.AllActive
    {
        /// <summary>
        /// The settings screens.
        /// </summary>
        private readonly BindableCollection<ISettingsScreen> settingsScreens;

        /// <summary>
        /// The first time.
        /// </summary>
        private bool firstTime = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsViewModel"/> class.
        /// </summary>
        /// <param name="settingsScreens">
        /// The settings screens.
        /// </param>
        [ImportingConstructor]
        public SettingsViewModel([ImportMany]IEnumerable<ISettingsScreen> settingsScreens)
        {
            // this.settingsScreens = new BindableCollection<ISettingsScreen>(settingsScreens.OrderBy(x=>x.Order));
            this.Name = "settings";
            this.Header = "Settings";
            this.SetPosition(Position.Left);
            this.Items.AddRange(settingsScreens.OrderBy(x => x.Order));
            this.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "IsOpen" && this.IsOpen)
                {
                    Tracker.TrackEventAsync(Tracker.FlyoutsCategory, "Open", this.Name, 1);
                }
            };
        }

        // public IObservableCollection<ISettingsScreen> SettingsScreens
        // {
        // get
        // {
        // return this.settingsScreens;
        // }
        // }

        /// <summary>
        /// Called when activating.
        /// </summary>
        protected override void OnActivate()
        {
            if (this.firstTime)
            {
                this.firstTime = false;

                foreach (var settingsScreen in this.Items)
                {
                    this.ActivateItem(settingsScreen);
                }
            }
        }
    }
}