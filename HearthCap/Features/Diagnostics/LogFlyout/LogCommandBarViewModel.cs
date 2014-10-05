// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogCommandBarViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The log command bar view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Diagnostics.LogFlyout
{
    using System.ComponentModel.Composition;

    using Caliburn.Micro;

    using HearthCap.Shell.Dialogs;
    using HearthCap.Shell.Flyouts;
    using HearthCap.Shell.WindowCommands;

    // [Export(typeof(IWindowCommand))]
    /// <summary>
    /// The log command bar view model.
    /// </summary>
    public class LogCommandBarViewModel : WindowCommandViewModel, IHandle<ToggleLogFlyoutCommand>
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
        /// Initializes a new instance of the <see cref="LogCommandBarViewModel"/> class.
        /// </summary>
        /// <param name="dialogManager">
        /// The dialog manager.
        /// </param>
        /// <param name="eventAggregator">
        /// The event aggregator.
        /// </param>
        [ImportingConstructor]
        public LogCommandBarViewModel(IDialogManager dialogManager, IEventAggregator eventAggregator)
        {
            this.Order = -5;
            this.dialogManager = dialogManager;
            this.eventAggregator = eventAggregator;
            this.eventAggregator.Subscribe(this);
        }

        /// <summary>
        /// The toggle log flyout.
        /// </summary>
        public void ToggleLogFlyout()
        {
            this.eventAggregator.PublishOnCurrentThread(new ToggleFlyoutCommand("log"));
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Handle(ToggleLogFlyoutCommand message)
        {
            this.ToggleLogFlyout();
        }
    }
}