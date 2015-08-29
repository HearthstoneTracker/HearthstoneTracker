using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Caliburn.Micro;
using Caliburn.Micro.Recipes.Filters;
using HearthCap.Data;
using HearthCap.Features.Core;
using HearthCap.Features.Games.Models;
using HearthCap.Shell.Flyouts;
using HearthCap.Shell.Notifications;
using MahApps.Metro.Controls;

namespace HearthCap.Features.ArenaSessions
{
    [Export(typeof(IFlyout))]
    [Export(typeof(AddArenaViewModel))]
    public class AddArenaViewModel : FlyoutViewModel, IPartImportsSatisfiedNotification
    {
        private readonly IEventAggregator events;

        private readonly GameManager.GameManager gameManager;

        private Hero hero;

        private readonly BindableCollection<Hero> heroes = new BindableCollection<Hero>();

        private int losses;

        private DateTime started;

        private int wins;

        private bool initialized;

        private ServerItemModel selectedServer;

        private readonly BindableServerCollection servers = BindableServerCollection.Instance;

        [ImportingConstructor]
        public AddArenaViewModel(IEventAggregator events, GameManager.GameManager gameManager)
        {
            DisplayName = Header = "Add new arena:";
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
            get { return selectedServer; }
            set
            {
                if (Equals(value, selectedServer))
                {
                    return;
                }
                selectedServer = value;
                NotifyOfPropertyChange(() => SelectedServer);
            }
        }

        public Hero Hero
        {
            get { return hero; }
            set
            {
                if (Equals(value, hero))
                {
                    return;
                }
                hero = value;
                NotifyOfPropertyChange(() => Hero);
            }
        }

        public IObservableCollection<Hero> Heroes
        {
            get { return heroes; }
        }

        public int Losses
        {
            get { return losses; }
            set
            {
                if (value == losses)
                {
                    return;
                }
                losses = value;
                NotifyOfPropertyChange(() => Losses);
            }
        }

        public DateTime Started
        {
            get { return started; }
            set
            {
                if (value.Equals(started))
                {
                    return;
                }
                started = value;
                NotifyOfPropertyChange(() => Started);
            }
        }

        public int Wins
        {
            get { return wins; }
            set
            {
                if (value == wins)
                {
                    return;
                }
                wins = value;
                NotifyOfPropertyChange(() => Wins);
            }
        }

        public BindableServerCollection Servers
        {
            get { return servers; }
        }

        [Dependencies("Hero", "SelectedServer")]
        public async Task Save()
        {
            var arena = new ArenaSessionModel();
            arena.Hero = Hero;
            arena.StartDate = Started;
            arena.Losses = Losses;
            arena.Wins = Wins;
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
            if (initialized)
            {
                return;
            }
            initialized = true;

            var data = GlobalData.Get();
            heroes.Clear();
            heroes.AddRange(data.Heroes);
            Reset();
        }

        /// <summary>
        ///     Called when a part's imports have been satisfied and it is safe to use.
        /// </summary>
        public void OnImportsSatisfied()
        {
            EnsureInitialized();
        }
    }
}
