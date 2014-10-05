// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SettingsWindowCommandViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The settings window command view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Settings
{
    using System.ComponentModel.Composition;

    using Caliburn.Micro;

    using HearthCap.Shell.Dialogs;
    using HearthCap.Shell.Flyouts;
    using HearthCap.Shell.WindowCommands;

    /// <summary>
    /// The settings window command view model.
    /// </summary>
    [Export(typeof(IWindowCommand))]
    public class SettingsWindowCommandViewModel : WindowCommandViewModel, IHandle<ToggleSettingsCommand>
    {
        /// <summary>
        /// The dialog manager.
        /// </summary>
        private readonly IDialogManager dialogManager;

        /// <summary>
        /// The event aggregator.
        /// </summary>
        private readonly IEventAggregator eventAggregator;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsWindowCommandViewModel"/> class.
        /// </summary>
        /// <param name="dialogManager">
        /// The dialog manager.
        /// </param>
        /// <param name="eventAggregator">
        /// The event aggregator.
        /// </param>
        [ImportingConstructor]
        public SettingsWindowCommandViewModel(IDialogManager dialogManager, IEventAggregator eventAggregator)
        {
            this.Order = 0;
            this.dialogManager = dialogManager;
            this.eventAggregator = eventAggregator;
            this.eventAggregator.Subscribe(this);
        }

        /// <summary>
        /// The toggle settings.
        /// </summary>
        public void ToggleSettings()
        {
            this.eventAggregator.PublishOnCurrentThread(new ToggleFlyoutCommand("settings"));
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Handle(ToggleSettingsCommand message)
        {
            this.ToggleSettings();
        }
    }
}