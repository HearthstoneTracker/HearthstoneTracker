namespace HearthCap.Features.Support
{
    using System.ComponentModel.Composition;

    using Caliburn.Micro;

    using HearthCap.Shell.Dialogs;
    using HearthCap.Shell.WindowCommands;

    [Export(typeof(IWindowCommand))]
    public class SupportCommandBarViewModel : WindowCommandViewModel
    {
        private readonly IDialogManager dialogManager;

        private readonly IEventAggregator events;

        private readonly SupportViewModel supportViewModel;

        [ImportingConstructor]
        public SupportCommandBarViewModel(
            IDialogManager dialogManager,
            IEventAggregator events,
            SupportViewModel supportViewModel)
        {
            this.Order = 95;
            this.dialogManager = dialogManager;
            this.events = events;
            this.supportViewModel = supportViewModel;
            this.events.Subscribe(this);
        }

        public void SupportRequest()
        {
            dialogManager.ShowDialog(supportViewModel);
        }
    }
}