namespace HearthCap.Features.ArenaSessions
{
    using System.ComponentModel.Composition;

    using Caliburn.Micro;

    using HearthCap.Shell.CommandBar;
    using HearthCap.Shell.Dialogs;

    [Export(typeof(ICommandBarItem))]
    public class CurrentSessionCommandBarViewModel : CommandBarItemViewModel
    {
        private readonly IDialogManager dialogManager;

        private readonly IEventAggregator events;

        [ImportingConstructor]
        public CurrentSessionCommandBarViewModel(IDialogManager dialogManager, IEventAggregator events)
        {
            this.Order = 10;
            this.dialogManager = dialogManager;
            this.events = events;
            this.events.Subscribe(this);
        }

        [Import]
        public CurrentSessionFlyoutViewModel ArenaViewModel { get; set; }

        [Import]
        public AddArenaViewModel AddArenaViewModel { get; set; }

        public void ShowLatest()
        {
            ArenaViewModel.LoadLatest();
        }

        public void AddArena()
        {
            AddArenaViewModel.AddNewArena();
        }
    }
}