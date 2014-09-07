namespace HearthCap.Features.Games.AddGame
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.Composition;

    using Caliburn.Micro;

    using HearthCap.Features.ArenaSessions;
    using HearthCap.Shell.CommandBar;
    using HearthCap.Shell.Dialogs;
    using HearthCap.Shell.Flyouts;

    [Export(typeof(ICommandBarItem))]
    public class AddGameCommandBarViewModel : CommandBarItemViewModel, IHandle<SelectedArenaSessionChanged>
    {
        private readonly IEventAggregator events;


        private ArenaSessionsViewModel arenaSessionsViewModel;

        [ImportingConstructor]
        public AddGameCommandBarViewModel(
            IEventAggregator events, 
            ArenaSessionsViewModel arenaSessionsViewModel,
            CurrentSessionFlyoutViewModel arenaSessionViewModel)
        {
            this.events = events;
            this.arenaSessionsViewModel = arenaSessionsViewModel;
            this.Order = -1;
            this.arenaSessionsViewModel.Activated += UpdateArenaStatus;
            this.arenaSessionsViewModel.Deactivated += UpdateArenaStatus;
            this.arenaSessionsViewModel.PropertyChanged += ArenaSessionsViewModelOnPropertyChanged;
            arenaSessionViewModel.PropertyChanged += ArenaSessionViewModelOnPropertyChanged;
            events.Subscribe(this);
        }

        private void ArenaSessionsViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "SelectedArenaSession")
            {
                //NotifyOfPropertyChange(() => CanAddArenaGame);
                NotifyOfPropertyChange(() => IsArenasActiveAndSelected);
            }
        }

        private void ArenaSessionViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "IsEnded")
            {
                //NotifyOfPropertyChange(() => CanAddArenaGame);
                NotifyOfPropertyChange(() => IsArenasActiveAndSelected);
            }
        }

        private void UpdateArenaStatus(object sender, EventArgs activationEventArgs)
        {
            NotifyOfPropertyChange(() => IsArenasActiveAndSelected);
            //NotifyOfPropertyChange(() => CanAddArenaGame);
        }

        public bool IsArenasActiveAndSelected
        {
            get
            {
                return arenaSessionsViewModel.IsActive && arenaSessionsViewModel.SelectedArenaSession != null;
            }
        }

        //public bool CanAddArenaGame
        //{
        //    get
        //    {
        //        if (!IsArenasActiveAndSelected) return false;
        //        if (arenaSessionsViewModel.SelectedArenaSession.IsEnded) return false;
        //        return true;
        //    }
        //}

        public void AddGame()
        {
            events.PublishOnCurrentThread(new CreateNewGame());
        }

        public void AddArenaGame()
        {
            if (IsArenasActiveAndSelected)
            {
                events.PublishOnCurrentThread(new CreateNewGame() { ArenaSession = arenaSessionsViewModel.SelectedArenaSession });
            }
        }

        public void ShowCurrentGame()
        {
            events.PublishOnCurrentThread(new ToggleFlyoutCommand(Flyouts.CurrentGame));
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(SelectedArenaSessionChanged message)
        {
            NotifyOfPropertyChange(() => IsArenasActiveAndSelected);
        }
    }
}