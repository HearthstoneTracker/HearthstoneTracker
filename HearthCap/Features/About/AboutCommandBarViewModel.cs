namespace HearthCap.Features.About
{
    using System.ComponentModel.Composition;

    using Caliburn.Micro;

    using HearthCap.Shell.Commands;
    using HearthCap.Shell.Dialogs;
    using HearthCap.Shell.Flyouts;
    using HearthCap.Shell.WindowCommands;

    [Export(typeof(IWindowCommand))]
    public class AboutCommandBarViewModel : WindowCommandViewModel
    {
        private readonly IDialogManager dialogManager;

        private readonly IEventAggregator eventAggregator;

        [ImportingConstructor]
        public AboutCommandBarViewModel(IDialogManager dialogManager, IEventAggregator eventAggregator)
        {
            Order = -10;
            this.dialogManager = dialogManager;
            this.eventAggregator = eventAggregator;
            this.eventAggregator.Subscribe(this);
        }

        public void ToggleAbout()
        {
            eventAggregator.PublishOnBackgroundThread(new ToggleFlyoutCommand("about"));
        }
    }
}