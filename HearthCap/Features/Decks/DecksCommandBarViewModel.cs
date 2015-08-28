namespace HearthCap.Features.Decks
{
    using System.ComponentModel.Composition;

    using Caliburn.Micro;

    using HearthCap.Shell.CommandBar;
    using HearthCap.Shell.Dialogs;
    using HearthCap.Shell.Flyouts;

    [Export(typeof(ICommandBarItem))]
    public class DecksCommandBarViewModel : CommandBarItemViewModel        
    {
        private readonly IDialogManager dialogManager;

        private readonly IEventAggregator eventAggregator;

        [ImportingConstructor]
        public DecksCommandBarViewModel(IDialogManager dialogManager, IEventAggregator eventAggregator)
        {
            this.Order = -2;
            this.dialogManager = dialogManager;
            this.eventAggregator = eventAggregator;
            this.eventAggregator.Subscribe(this);
        }

        public void ShowDecks()
        {
            this.eventAggregator.PublishOnCurrentThread(new ToggleFlyoutCommand(Flyouts.Decks));
        }
    }
}