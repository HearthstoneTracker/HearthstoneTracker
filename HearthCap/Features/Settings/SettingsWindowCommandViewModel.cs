using System.ComponentModel.Composition;
using Caliburn.Micro;
using HearthCap.Shell.Dialogs;
using HearthCap.Shell.Flyouts;
using HearthCap.Shell.WindowCommands;

namespace HearthCap.Features.Settings
{
    [Export(typeof(IWindowCommand))]
    public class SettingsWindowCommandViewModel : WindowCommandViewModel, IHandle<ToggleSettingsCommand>
    {
        private readonly IDialogManager dialogManager;

        private readonly IEventAggregator eventAggregator;

        [ImportingConstructor]
        public SettingsWindowCommandViewModel(IDialogManager dialogManager, IEventAggregator eventAggregator)
        {
            Order = 0;
            this.dialogManager = dialogManager;
            this.eventAggregator = eventAggregator;
            this.eventAggregator.Subscribe(this);
        }

        public void ToggleSettings()
        {
            eventAggregator.PublishOnCurrentThread(new ToggleFlyoutCommand("settings"));
        }

        /// <summary>
        ///     Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(ToggleSettingsCommand message)
        {
            ToggleSettings();
        }
    }
}
