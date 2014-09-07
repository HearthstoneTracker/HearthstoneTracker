namespace HearthCap.Features.Settings
{
    using System.ComponentModel.Composition;

    using Caliburn.Micro;

    using HearthCap.Shell.Dialogs;
    using HearthCap.Shell.Flyouts;
    using HearthCap.Shell.WindowCommands;

    using MahApps.Metro.Controls;

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
            this.eventAggregator.PublishOnCurrentThread(new ToggleFlyoutCommand("settings"));
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(ToggleSettingsCommand message)
        {
            this.ToggleSettings();
        }
    }
}