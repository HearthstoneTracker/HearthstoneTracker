// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DonateCommandBarViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The donate command bar view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.About
{
    using System.ComponentModel.Composition;

    using Caliburn.Micro;

    using HearthCap.Features.Analytics;
    using HearthCap.Properties;
    using HearthCap.Shell.Commands;
    using HearthCap.Shell.Dialogs;
    using HearthCap.Shell.WindowCommands;

    /// <summary>
    /// The donate command bar view model.
    /// </summary>
    [Export(typeof(IWindowCommand))]
    public class DonateCommandBarViewModel : WindowCommandViewModel
    {
        /// <summary>
        /// The dialog manager.
        /// </summary>
        private readonly IDialogManager dialogManager;

        /// <summary>
        /// The events.
        /// </summary>
        private readonly IEventAggregator events;

        /// <summary>
        /// Initializes a new instance of the <see cref="DonateCommandBarViewModel"/> class.
        /// </summary>
        /// <param name="dialogManager">
        /// The dialog manager.
        /// </param>
        /// <param name="events">
        /// The events.
        /// </param>
        [ImportingConstructor]
        public DonateCommandBarViewModel(IDialogManager dialogManager, IEventAggregator events)
        {
            this.Order = 100;
            this.dialogManager = dialogManager;
            this.events = events;
            this.events.Subscribe(this);
        }

        /// <summary>
        /// The donate.
        /// </summary>
        public void Donate()
        {
            Tracker.TrackEventAsync(Tracker.CommonCategory, "Donate", Tracker.Version.ToString(), 1);
           
            this.events.PublishOnBackgroundThread(new VisitWebsiteCommand(Resources.DonationLink));
        }
    }
}