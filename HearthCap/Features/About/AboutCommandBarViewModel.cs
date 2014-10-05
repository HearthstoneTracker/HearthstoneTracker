// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AboutCommandBarViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The about command bar view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.About
{
    using System.ComponentModel.Composition;

    using Caliburn.Micro;

    using HearthCap.Shell.Dialogs;
    using HearthCap.Shell.Flyouts;
    using HearthCap.Shell.WindowCommands;

    /// <summary>
    /// The about command bar view model.
    /// </summary>
    [Export(typeof(IWindowCommand))]
    public class AboutCommandBarViewModel : WindowCommandViewModel
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
        /// Initializes a new instance of the <see cref="AboutCommandBarViewModel"/> class.
        /// </summary>
        /// <param name="dialogManager">
        /// The dialog manager.
        /// </param>
        /// <param name="eventAggregator">
        /// The event aggregator.
        /// </param>
        [ImportingConstructor]
        public AboutCommandBarViewModel(IDialogManager dialogManager, IEventAggregator eventAggregator)
        {
            this.Order = -10;
            this.dialogManager = dialogManager;
            this.eventAggregator = eventAggregator;
            this.eventAggregator.Subscribe(this);
        }

        /// <summary>
        /// The toggle about.
        /// </summary>
        public void ToggleAbout()
        {
            this.eventAggregator.PublishOnBackgroundThread(new ToggleFlyoutCommand("about"));
        }
    }
}