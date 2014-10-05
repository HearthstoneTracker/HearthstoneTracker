// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddArenaViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The add arena view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.ArenaSessions
{
    using System;
    using System.ComponentModel.Composition;
    using System.Threading.Tasks;

    using Caliburn.Micro;
    using Caliburn.Micro.Recipes.Filters;

    using HearthCap.Data;
    using HearthCap.Features.Core;
    using HearthCap.Features.GameManager;
    using HearthCap.Features.Games.Models;
    using HearthCap.Shell.Flyouts;
    using HearthCap.Shell.Notifications;

    using MahApps.Metro.Controls;

    /// <summary>
    /// The add arena view model.
    /// </summary>
    [Export(typeof(IFlyout))]
    [Export(typeof(AddArenaViewModel))]
    public class AddArenaViewModel : FlyoutViewModel, IPartImportsSatisfiedNotification
    {
        /// <summary>
        /// The events.
        /// </summary>
        private readonly IEventAggregator events;

        /// <summary>
        /// The game manager.
        /// </summary>
        private readonly GameManager gameManager;

        /// <summary>
        /// The hero.
        /// </summary>
        private Hero hero;

        /// <summary>
        /// The heroes.
        /// </summary>
        private readonly BindableCollection<Hero> heroes = new BindableCollection<Hero>();

        /// <summary>
        /// The losses.
        /// </summary>
        private int losses;

        /// <summary>
        /// The started.
        /// </summary>
        private DateTime started;

        /// <summary>
        /// The wins.
        /// </summary>
        private int wins;

        /// <summary>
        /// The initialized.
        /// </summary>
        private bool initialized;

        /// <summary>
        /// The selected server.
        /// </summary>
        private ServerItemModel selectedServer;

        /// <summary>
        /// The servers.
        /// </summary>
        private readonly BindableServerCollection servers = BindableServerCollection.Instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddArenaViewModel"/> class.
        /// </summary>
        /// <param name="events">
        /// The events.
        /// </param>
        /// <param name="gameManager">
        /// The game manager.
        /// </param>
        [ImportingConstructor]
        public AddArenaViewModel(IEventAggregator events, GameManager gameManager)
        {
            this.DisplayName = this.Header = "Add new arena:";
            this.SetPosition(Position.Right);
            this.events = events;
            this.gameManager = gameManager;
            events.Subscribe(this);
            this.SelectedServer = this.servers.Default;
        }

        /// <summary>
        /// Gets or sets the global data.
        /// </summary>
        [Import]
        public GlobalData GlobalData { get; set; }

        /// <summary>
        /// Gets or sets the arena flyout.
        /// </summary>
        [Import]
        public CurrentSessionFlyoutViewModel ArenaFlyout { get; set; }

        /// <summary>
        /// Gets or sets the selected server.
        /// </summary>
        public ServerItemModel SelectedServer
        {
            get
            {
                return this.selectedServer;
            }

            set
            {
                if (Equals(value, this.selectedServer))
                {
                    return;
                }

                this.selectedServer = value;
                this.NotifyOfPropertyChange(() => this.SelectedServer);
            }
        }

        /// <summary>
        /// Gets or sets the hero.
        /// </summary>
        public Hero Hero
        {
            get
            {
                return this.hero;
            }

            set
            {
                if (Equals(value, this.hero))
                {
                    return;
                }

                this.hero = value;
                this.NotifyOfPropertyChange(() => this.Hero);
            }
        }

        /// <summary>
        /// Gets the heroes.
        /// </summary>
        public IObservableCollection<Hero> Heroes
        {
            get
            {
                return this.heroes;
            }
        }

        /// <summary>
        /// Gets or sets the losses.
        /// </summary>
        public int Losses
        {
            get
            {
                return this.losses;
            }

            set
            {
                if (value == this.losses)
                {
                    return;
                }

                this.losses = value;
                this.NotifyOfPropertyChange(() => this.Losses);
            }
        }

        /// <summary>
        /// Gets or sets the started.
        /// </summary>
        public DateTime Started
        {
            get
            {
                return this.started;
            }

            set
            {
                if (value.Equals(this.started))
                {
                    return;
                }

                this.started = value;
                this.NotifyOfPropertyChange(() => this.Started);
            }
        }

        /// <summary>
        /// Gets or sets the wins.
        /// </summary>
        public int Wins
        {
            get
            {
                return this.wins;
            }

            set
            {
                if (value == this.wins)
                {
                    return;
                }

                this.wins = value;
                this.NotifyOfPropertyChange(() => this.Wins);
            }
        }

        /// <summary>
        /// Gets the servers.
        /// </summary>
        public BindableServerCollection Servers
        {
            get
            {
                return this.servers;
            }
        }

        /// <summary>
        /// The save.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Dependencies("Hero", "SelectedServer")]
        public async Task Save()
        {
            var arena = new ArenaSessionModel();
            arena.Hero = this.Hero;
            arena.StartDate = this.Started;
            arena.Losses = this.Losses;
            arena.Wins = this.Wins;
            arena.Server = this.SelectedServer.Name;

            await this.gameManager.AddArenaSession(arena);
            this.events.PublishOnBackgroundThread(new SendNotification("Arena successfully saved."));
            this.Reset();
            this.IsOpen = false;
            this.ArenaFlyout.Load(arena);
        }

        /// <summary>
        /// The can save.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool CanSave()
        {
            return this.Hero != null && this.SelectedServer != null;
        }

        /// <summary>
        /// The add new arena.
        /// </summary>
        public void AddNewArena()
        {
            this.EnsureInitialized();
            this.Reset();
            this.IsOpen = true;
        }

        /// <summary>
        /// The reset.
        /// </summary>
        private void Reset()
        {
            this.Hero = null;
            this.Started = DateTime.Now;
            this.Wins = 0;
            this.Losses = 0;
            this.SelectedServer = this.servers.Default;
        }

        /// <summary>
        /// The ensure initialized.
        /// </summary>
        private void EnsureInitialized()
        {
            if (this.initialized) return;
            this.initialized = true;

            var data = this.GlobalData.Get();
            this.heroes.Clear();
            this.heroes.AddRange(data.Heroes);
            this.Reset();
        }

        /// <summary>
        /// Called when a part's imports have been satisfied and it is safe to use.
        /// </summary>
        public void OnImportsSatisfied()
        {
            this.EnsureInitialized();
        }
    }
}