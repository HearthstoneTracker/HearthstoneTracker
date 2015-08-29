using System.ComponentModel.Composition;
using Caliburn.Micro;
using HearthCap.Features.Analytics;
using HearthCap.Properties;
using HearthCap.Shell.Commands;
using HearthCap.Shell.Dialogs;
using HearthCap.Shell.WindowCommands;

namespace HearthCap.Features.About
{
    [Export(typeof(IWindowCommand))]
    public class DonateCommandBarViewModel : WindowCommandViewModel
    {
        private readonly IDialogManager dialogManager;

        private readonly IEventAggregator events;

        [ImportingConstructor]
        public DonateCommandBarViewModel(IDialogManager dialogManager, IEventAggregator events)
        {
            Order = 100;
            this.dialogManager = dialogManager;
            this.events = events;
            this.events.Subscribe(this);
        }

        public void Donate()
        {
            Tracker.TrackEventAsync(Tracker.CommonCategory, "Donate", Tracker.Version.ToString(), 1);

            events.PublishOnBackgroundThread(new VisitWebsiteCommand(Resources.DonationLink));
        }
    }
}
