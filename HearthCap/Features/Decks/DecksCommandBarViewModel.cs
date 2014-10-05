// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DecksCommandBarViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The decks command bar view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Decks
{
    using System.ComponentModel.Composition;

    using Caliburn.Micro;

    using HearthCap.Shell.CommandBar;
    using HearthCap.Shell.Dialogs;
    using HearthCap.Shell.Flyouts;

    /// <summary>
    /// The decks command bar view model.
    /// </summary>
    [Export(typeof(ICommandBarItem))]
    public class DecksCommandBarViewModel : CommandBarItemViewModel        
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
        /// Initializes a new instance of the <see cref="DecksCommandBarViewModel"/> class.
        /// </summary>
        /// <param name="dialogManager">
        /// The dialog manager.
        /// </param>
        /// <param name="eventAggregator">
        /// The event aggregator.
        /// </param>
        [ImportingConstructor]
        public DecksCommandBarViewModel(IDialogManager dialogManager, IEventAggregator eventAggregator)
        {
            this.Order = -2;
            this.dialogManager = dialogManager;
            this.eventAggregator = eventAggregator;
            this.eventAggregator.Subscribe(this);
        }

        /// <summary>
        /// The show decks.
        /// </summary>
        public void ShowDecks()
        {
            this.eventAggregator.PublishOnCurrentThread(new ToggleFlyoutCommand(Flyouts.Decks));
        }
    }
}