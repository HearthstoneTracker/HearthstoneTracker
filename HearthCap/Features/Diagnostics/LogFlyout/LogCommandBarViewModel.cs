using System.ComponentModel.Composition;
using Caliburn.Micro;
using HearthCap.Shell.Dialogs;
using HearthCap.Shell.Flyouts;
using HearthCap.Shell.WindowCommands;

namespace HearthCap.Features.Diagnostics.LogFlyout
{
    // [Export(typeof(IWindowCommand))]
    public class LogCommandBarViewModel : WindowCommandViewModel, IHandle<ToggleLogFlyoutCommand>
    {
        private readonly IDialogManager dialogManager;

        private readonly IEventAggregator eventAggregator;

        [ImportingConstructor]
        public LogCommandBarViewModel(IDialogManager dialogManager, IEventAggregator eventAggregator)
        {
            Order = -5;
            this.dialogManager = dialogManager;
            this.eventAggregator = eventAggregator;
            this.eventAggregator.Subscribe(this);
        }

        public void ToggleLogFlyout()
        {
            eventAggregator.PublishOnCurrentThread(new ToggleFlyoutCommand("log"));
        }

        /// <summary>
        ///     Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(ToggleLogFlyoutCommand message)
        {
            ToggleLogFlyout();
        }
    }
}
