using System.ComponentModel.Composition;
using Caliburn.Micro;
using HearthCap.Shell.CommandBar;
using HearthCap.Shell.Dialogs;
using HearthCap.Shell.Flyouts;

namespace HearthCap.Features.Decks
{
    [Export(typeof(ICommandBarItem))]
    public class DecksCommandBarViewModel : CommandBarItemViewModel
    {
        private readonly IDialogManager dialogManager;

        private readonly IEventAggregator eventAggregator;

        [ImportingConstructor]
        public DecksCommandBarViewModel(IDialogManager dialogManager, IEventAggregator eventAggregator)
        {
            Order = -2;
            this.dialogManager = dialogManager;
            this.eventAggregator = eventAggregator;
            this.eventAggregator.Subscribe(this);
        }

        public void ShowDecks()
        {
            eventAggregator.PublishOnCurrentThread(new ToggleFlyoutCommand(Flyouts.Decks));
        }
    }
}
