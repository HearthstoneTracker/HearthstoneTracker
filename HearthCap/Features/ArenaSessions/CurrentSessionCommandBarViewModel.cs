// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CurrentSessionCommandBarViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The current session command bar view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.ArenaSessions
{
    using System.ComponentModel.Composition;

    using Caliburn.Micro;

    using HearthCap.Shell.CommandBar;
    using HearthCap.Shell.Dialogs;

    /// <summary>
    /// The current session command bar view model.
    /// </summary>
    [Export(typeof(ICommandBarItem))]
    public class CurrentSessionCommandBarViewModel : CommandBarItemViewModel
    {
        /// <summary>
        /// The dialog manager.
        /// </summary>
        private readonly IDialogManager dialogManager;

        /// <summary>
        /// The events.
        /// </summary>
        private readonly IEventAggregator events;

        /// <summary>
        /// Initializes a new instance of the <see cref="CurrentSessionCommandBarViewModel"/> class.
        /// </summary>
        /// <param name="dialogManager">
        /// The dialog manager.
        /// </param>
        /// <param name="events">
        /// The events.
        /// </param>
        [ImportingConstructor]
        public CurrentSessionCommandBarViewModel(IDialogManager dialogManager, IEventAggregator events)
        {
            this.Order = 10;
            this.dialogManager = dialogManager;
            this.events = events;
            this.events.Subscribe(this);
        }

        /// <summary>
        /// Gets or sets the arena view model.
        /// </summary>
        [Import]
        public CurrentSessionFlyoutViewModel ArenaViewModel { get; set; }

        /// <summary>
        /// Gets or sets the add arena view model.
        /// </summary>
        [Import]
        public AddArenaViewModel AddArenaViewModel { get; set; }

        /// <summary>
        /// The show latest.
        /// </summary>
        public void ShowLatest()
        {
            this.ArenaViewModel.LoadLatest();
        }

        /// <summary>
        /// The add arena.
        /// </summary>
        public void AddArena()
        {
            this.AddArenaViewModel.AddNewArena();
        }
    }
}