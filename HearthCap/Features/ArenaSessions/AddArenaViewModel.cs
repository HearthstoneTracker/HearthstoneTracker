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

    [Export(typeof(IFlyout))]
    [Export(typeof(AddArenaViewModel))]
    public class AddArenaViewModel : FlyoutViewModel, IPartImportsSatisfiedNotification
    {
        private readonly IEventAggregator events;

        private readonly GameManager gameManager;

        private Hero hero;

        private readonly BindableCollection<Hero> heroes = new BindableCollection<Hero>();

        private int losses;

        private DateTime started;

        private int wins;

        private bool initialized;

        private ServerItemModel selectedServer;

        private readonly BindableServerCollection servers = BindableServerCollection.Instance;

        [ImportingConstructor]
        public AddArenaViewModel(IEventAggregator events, GameManager gameManager)
        {
            this.DisplayName = this.Header = "Add new arena:";
            SetPosition(Position.Right);
            this.events = events;
            this.gameManager = gameManager;
            events.Subscribe(this);
            SelectedServer = servers.Default;
        }

        [Import]
        public GlobalData GlobalData { get; set; }

        [Import]
        public CurrentSessionFlyoutViewModel ArenaFlyout { get; set; }

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

        public IObservableCollection<Hero> Heroes
        {
            get
            {
                return heroes;
            }
        }

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

        public BindableServerCollection Servers
        {
            get
            {
                return this.servers;
            }
        }

        [Dependencies("Hero", "SelectedServer")]
        public async Task Save()
        {
            var arena = new ArenaSessionModel();
            arena.Hero = this.Hero;
            arena.StartDate = this.Started;
            arena.Losses = this.Losses;
            arena.Wins = this.Wins;
            arena.Server = SelectedServer.Name;

            await gameManager.AddArenaSession(arena);
            events.PublishOnBackgroundThread(new SendNotification("Arena successfully saved."));
            Reset();
            IsOpen = false;
            ArenaFlyout.Load(arena);
        }

        public bool CanSave()
        {
            return Hero != null && SelectedServer != null;
        }

        public void AddNewArena()
        {
            EnsureInitialized();
            Reset();
            IsOpen = true;
        }

        private void Reset()
        {
            Hero = null;
            Started = DateTime.Now;
            Wins = 0;
            Losses = 0;
            SelectedServer = servers.Default;
        }

        private void EnsureInitialized()
        {
            if (initialized) return;
            initialized = true;

            var data = this.GlobalData.Get();
            this.heroes.Clear();
            this.heroes.AddRange(data.Heroes);
            Reset();
        }

        /// <summary>
        /// Called when a part's imports have been satisfied and it is safe to use.
        /// </summary>
        public void OnImportsSatisfied()
        {
            EnsureInitialized();
        }
    }
}