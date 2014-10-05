// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddGameCommandBarViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The add game command bar view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Games.AddGame
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.Composition;

    using Caliburn.Micro;

    using HearthCap.Features.ArenaSessions;
    using HearthCap.Shell.CommandBar;
    using HearthCap.Shell.Flyouts;

    /// <summary>
    /// The add game command bar view model.
    /// </summary>
    [Export(typeof(ICommandBarItem))]
    public class AddGameCommandBarViewModel : CommandBarItemViewModel, IHandle<SelectedArenaSessionChanged>
    {
        /// <summary>
        /// The events.
        /// </summary>
        private readonly IEventAggregator events;

        /// <summary>
        /// The arena sessions view model.
        /// </summary>
        private ArenaSessionsViewModel arenaSessionsViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddGameCommandBarViewModel"/> class.
        /// </summary>
        /// <param name="events">
        /// The events.
        /// </param>
        /// <param name="arenaSessionsViewModel">
        /// The arena sessions view model.
        /// </param>
        /// <param name="arenaSessionViewModel">
        /// The arena session view model.
        /// </param>
        [ImportingConstructor]
        public AddGameCommandBarViewModel(
            IEventAggregator events, 
            ArenaSessionsViewModel arenaSessionsViewModel, 
            CurrentSessionFlyoutViewModel arenaSessionViewModel)
        {
            this.events = events;
            this.arenaSessionsViewModel = arenaSessionsViewModel;
            this.Order = -1;
            this.arenaSessionsViewModel.Activated += this.UpdateArenaStatus;
            this.arenaSessionsViewModel.Deactivated += this.UpdateArenaStatus;
            this.arenaSessionsViewModel.PropertyChanged += this.ArenaSessionsViewModelOnPropertyChanged;
            arenaSessionViewModel.PropertyChanged += this.ArenaSessionViewModelOnPropertyChanged;
            events.Subscribe(this);
        }

        /// <summary>
        /// The arena sessions view model on property changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        private void ArenaSessionsViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "SelectedArenaSession")
            {
                // NotifyOfPropertyChange(() => CanAddArenaGame);
                this.NotifyOfPropertyChange(() => this.IsArenasActiveAndSelected);
            }
        }

        /// <summary>
        /// The arena session view model on property changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        private void ArenaSessionViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "IsEnded")
            {
                // NotifyOfPropertyChange(() => CanAddArenaGame);
                this.NotifyOfPropertyChange(() => this.IsArenasActiveAndSelected);
            }
        }

        /// <summary>
        /// The update arena status.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="activationEventArgs">
        /// The activation event args.
        /// </param>
        private void UpdateArenaStatus(object sender, EventArgs activationEventArgs)
        {
            this.NotifyOfPropertyChange(() => this.IsArenasActiveAndSelected);

            // NotifyOfPropertyChange(() => CanAddArenaGame);
        }

        /// <summary>
        /// Gets a value indicating whether is arenas active and selected.
        /// </summary>
        public bool IsArenasActiveAndSelected
        {
            get
            {
                return this.arenaSessionsViewModel.IsActive && this.arenaSessionsViewModel.SelectedArenaSession != null;
            }
        }

        // public bool CanAddArenaGame
        // {
        // get
        // {
        // if (!IsArenasActiveAndSelected) return false;
        // if (arenaSessionsViewModel.SelectedArenaSession.IsEnded) return false;
        // return true;
        // }
        // }

        /// <summary>
        /// The add game.
        /// </summary>
        public void AddGame()
        {
            this.events.PublishOnCurrentThread(new CreateNewGame());
        }

        /// <summary>
        /// The add arena game.
        /// </summary>
        public void AddArenaGame()
        {
            if (this.IsArenasActiveAndSelected)
            {
                this.events.PublishOnCurrentThread(new CreateNewGame { ArenaSession = this.arenaSessionsViewModel.SelectedArenaSession });
            }
        }

        /// <summary>
        /// The show current game.
        /// </summary>
        public void ShowCurrentGame()
        {
            this.events.PublishOnCurrentThread(new ToggleFlyoutCommand(Flyouts.CurrentGame));
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Handle(SelectedArenaSessionChanged message)
        {
            this.NotifyOfPropertyChange(() => this.IsArenasActiveAndSelected);
        }
    }
}