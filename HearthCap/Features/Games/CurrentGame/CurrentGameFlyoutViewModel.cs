using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using Caliburn.Micro.Recipes.Filters;
using HearthCap.Core.GameCapture.HS.Commands;
using HearthCap.Core.GameCapture.HS.Events;
using HearthCap.Data;
using HearthCap.Features.BalloonSettings;
using HearthCap.Features.Core;
using HearthCap.Features.Decks;
using HearthCap.Features.Games.Balloons;
using HearthCap.Features.Games.Models;
using HearthCap.Features.Games.Statistics;
using HearthCap.Framework;
using HearthCap.Shell.Flyouts;
using HearthCap.Shell.TrayIcon;
using HearthCap.Util;
using MahApps.Metro.Controls;
using LogManager = NLog.LogManager;

namespace HearthCap.Features.Games.CurrentGame
{
    /// <summary>The current game flyout view model.</summary>
    [Export(typeof(IFlyout))]
    public class CurrentGameFlyoutViewModel : FlyoutViewModel,
        IHandleWithTask<GameEnded>,
        IHandle<GameStarted>,
        IHandle<NewRound>,
        IPartImportsSatisfiedNotification
    {
        #region Fields

        private readonly NLog.Logger Log = LogManager.GetCurrentClassLogger();

        /// <summary>The events.</summary>
        private readonly IEventAggregator events;

        private readonly IRepository<GameResult> gameRepository;

        private readonly IRepository<ArenaSession> arenaRepository;

        /// <summary>The game mode.</summary>
        private GameMode gameMode;

        /// <summary>The game modes.</summary>
        private readonly BindableCollection<GameMode> gameModes = new GameModesCollection();

        /// <summary>The go first.</summary>
        private bool goFirst;

        /// <summary>The hero.</summary>
        private Hero hero;

        /// <summary>The heroes.</summary>
        private readonly BindableCollection<Hero> heroes;

        /// <summary>The is running.</summary>
        private bool isRunning;

        /// <summary>The notes.</summary>
        private string notes;

        /// <summary>The opponent hero.</summary>
        private Hero opponentHero;

        /// <summary>The start time.</summary>
        private DateTime startTime;

        /// <summary>The turns.</summary>
        private int turns;

        private readonly IDeckManager deckManager;

        private readonly GameManager.GameManager gameManager;

        private bool initialized;

        private readonly BindableCollection<StatModel> winLossRatio = new BindableCollection<StatModel>();
        private readonly BindableCollection<StatModel> winLossRatioHero = new BindableCollection<StatModel>();
        private readonly BindableCollection<StatModel> winLossRatioOpponentHero = new BindableCollection<StatModel>();

        private readonly DateFilter dateFilter = new DateFilter();

        private Deck deck;

        private bool victory;

        private DateTime stopped;

        private DateTime started;

        private bool conceded;

        #endregion

        #region Constructors and Destructors

        public CurrentGameFlyoutViewModel()
        {
            if (Execute.InDesignMode)
            {
                IsRunning = true;
                WinLossRatio.Add(new StatModel("Wins", 60));
                WinLossRatio.Add(new StatModel("Losses", 40));
                WinLossRatioHero.Add(new StatModel("Wins", 0));
                WinLossRatioHero.Add(new StatModel("Losses", 0));
                WinLossRatioOpponentHero.Add(new StatModel("Wins", 50));
                WinLossRatioOpponentHero.Add(new StatModel("Losses", 50));
            }
        }

        /// <summary>Initializes a new instance of the <see cref="CurrentGameFlyoutViewModel" /> class.</summary>
        /// <param name="dialogManager">The dialog manager.</param>
        /// <param name="events">The events.</param>
        /// <param name="captureEngine">The capture engine.</param>
        /// <param name="deckManager">The deck manager</param>
        [ImportingConstructor]
        public CurrentGameFlyoutViewModel(
            IEventAggregator events,
            IRepository<GameResult> gameRepository,
            IRepository<ArenaSession> arenaRepository,
            IDeckManager deckManager,
            GameManager.GameManager gameManager)
        {
            this.events = events;
            this.gameRepository = gameRepository;
            this.arenaRepository = arenaRepository;
            this.deckManager = deckManager;
            this.gameManager = gameManager;
            Name = Flyouts.CurrentGame;
            Header = "Current Game:";
            SetPosition(Position.Right);
            heroes = new BindableCollection<Hero>();
            events.Subscribe(this);

            GameMode = GameMode.Practice;
            StartTime = DateTime.Now;
            GoFirst = true;
            dateFilter.From = DateTime.Now.AddMonths(-1).SetToBeginOfDay();
            dateFilter.DateChanged += (sender, args) => RefreshStats();
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the busy.</summary>
        [Import]
        public IBusyWatcher Busy { get; set; }

        public DateFilter DateFilter
        {
            get { return dateFilter; }
        }

        public BindableCollection<StatModel> WinLossRatio
        {
            get { return winLossRatio; }
        }

        public BindableCollection<StatModel> WinLossRatioHero
        {
            get { return winLossRatioHero; }
        }

        public BindableCollection<StatModel> WinLossRatioOpponentHero
        {
            get { return winLossRatioOpponentHero; }
        }

        /// <summary>Gets or sets the game mode.</summary>
        public GameMode GameMode
        {
            get { return gameMode; }

            set
            {
                if (value == gameMode)
                {
                    return;
                }

                gameMode = value;
                NotifyOfPropertyChange(() => GameMode);
                NotifyOfPropertyChange(() => IsArena);
            }
        }

        /// <summary>Gets the game modes.</summary>
        public BindableCollection<GameMode> GameModes
        {
            get { return gameModes; }
        }

        /// <summary>Gets or sets the global data.</summary>
        [Import]
        public GlobalData GlobalData { get; set; }

        /// <summary>Gets or sets a value indicating whether go first.</summary>
        public bool GoFirst
        {
            get { return goFirst; }

            set
            {
                if (value.Equals(goFirst))
                {
                    return;
                }

                goFirst = value;
                NotifyOfPropertyChange(() => GoFirst);
            }
        }

        /// <summary>Gets or sets the hero.</summary>
        public Hero Hero
        {
            get { return hero; }

            set
            {
                if (value == hero)
                {
                    return;
                }

                hero = value;
                NotifyOfPropertyChange(() => Hero);
            }
        }

        /// <summary>Gets the heroes.</summary>
        public IObservableCollection<Hero> Heroes
        {
            get { return heroes; }
        }

        /// <summary>Gets or sets a value indicating whether is running.</summary>
        public bool IsRunning
        {
            get { return isRunning; }

            set
            {
                if (value.Equals(isRunning))
                {
                    return;
                }

                isRunning = value;
                NotifyOfPropertyChange(() => IsRunning);
            }
        }

        /// <summary>Gets or sets the notes.</summary>
        public string Notes
        {
            get { return notes; }

            set
            {
                if (value == notes)
                {
                    return;
                }

                notes = value;
                NotifyOfPropertyChange(() => Notes);
            }
        }

        /// <summary>Gets or sets the opponent hero.</summary>
        public Hero OpponentHero
        {
            get { return opponentHero; }

            set
            {
                if (value == opponentHero)
                {
                    return;
                }

                opponentHero = value;
                NotifyOfPropertyChange(() => OpponentHero);
            }
        }

        /// <summary>Gets or sets the start time.</summary>
        public DateTime StartTime
        {
            get { return startTime; }

            set
            {
                if (value.Equals(startTime))
                {
                    return;
                }

                startTime = value;
                NotifyOfPropertyChange(() => StartTime);
            }
        }

        /// <summary>Gets or sets the turns.</summary>
        public int Turns
        {
            get { return turns; }

            set
            {
                if (value == turns
                    || value < 1)
                {
                    return;
                }

                turns = value;
                NotifyOfPropertyChange(() => Turns);
            }
        }

        public bool IsArena
        {
            get { return GameMode == GameMode.Arena; }
        }

        public Deck Deck
        {
            get { return deck; }
            set
            {
                if (Equals(value, deck))
                {
                    return;
                }
                deck = value;
                NotifyOfPropertyChange(() => Deck);
            }
        }

        public bool Victory
        {
            get { return victory; }
            set
            {
                if (value.Equals(victory))
                {
                    return;
                }
                victory = value;
                NotifyOfPropertyChange(() => Victory);
            }
        }

        public DateTime Stopped
        {
            get { return stopped; }
            set
            {
                if (value.Equals(stopped))
                {
                    return;
                }
                stopped = value;
                NotifyOfPropertyChange(() => Stopped);
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

        public bool Conceded
        {
            get { return conceded; }
            set
            {
                if (value.Equals(conceded))
                {
                    return;
                }
                conceded = value;
                NotifyOfPropertyChange(() => Conceded);
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>Handles the message.</summary>
        /// <param name="message">The message.</param>
        public async Task Handle(GameEnded message)
        {
            events.PublishOnBackgroundThread(new ResetCurrentGame());

            // TODO: do not override if manually set.
            GameMode = message.GameMode;
            StartTime = message.StartTime;

            if (!string.IsNullOrWhiteSpace(message.Hero))
            {
                Hero = Heroes.FirstOrDefault(x => x.Key == message.Hero);
            }

            if (!string.IsNullOrWhiteSpace(message.OpponentHero))
            {
                OpponentHero = Heroes.FirstOrDefault(x => x.Key == message.OpponentHero);
            }

            if (message.GoFirst.HasValue)
            {
                GoFirst = message.GoFirst.Value;
            }

            IsRunning = false;
            Turns = message.Turns;

            GameMode = message.GameMode;
            Conceded = message.Conceded;

            GoFirst = !message.GoFirst.HasValue || message.GoFirst.Value;
            Hero = Heroes.FirstOrDefault(x => x.Key == message.Hero);
            OpponentHero = Heroes.FirstOrDefault(x => x.Key == message.OpponentHero);
            Started = message.StartTime;
            Stopped = message.EndTime;
            Victory = message.Victory.HasValue && message.Victory.Value;
            Deck = deckManager.GetOrCreateDeckBySlot(BindableServerCollection.Instance.DefaultName, message.Deck);

            await Save();
        }

        private async Task Save()
        {
            using (var bsy = Busy.GetTicket())
            {
                var gameResult = new GameResultModel();
                ArenaSessionModel arenasession = null;
                var newArena = false;
                gameResult.GameMode = GameMode;
                gameResult.Conceded = Conceded;

                if (Deck != null)
                {
                    gameResult.Deck = Deck;
                }

                gameResult.GoFirst = GoFirst;
                gameResult.Hero = Hero;
                gameResult.OpponentHero = OpponentHero;
                gameResult.Started = StartTime;
                gameResult.Stopped = Stopped;
                gameResult.Turns = Turns;
                gameResult.Victory = Victory;
                gameResult.Notes = Notes;
                gameResult.Server = BindableServerCollection.Instance.DefaultName;

                if (gameResult.GameMode == GameMode.Arena)
                {
                    var serverName = gameResult.Server;
                    var latestArena = arenaRepository.Query(a => a.Where(x => x.Server == serverName).OrderByDescending(x => x.StartDate).FirstOrDefault());
                    if (latestArena == null
                        ||
                        latestArena.IsEnded
                        ||
                        (gameResult.Hero != null && latestArena.Hero.Key != gameResult.Hero.Key))
                    {
                        Log.Debug("Creating new arena.");
                        newArena = true;
                        arenasession = new ArenaSessionModel
                            {
                                Hero = gameResult.Hero,
                                StartDate = gameResult.Started
                            };
                        try
                        {
                            GlobalLocks.NewArenaLock.Reset();
                            await gameManager.AddArenaSession(arenasession);
                        }
                        finally
                        {
                            GlobalLocks.NewArenaLock.Set();
                        }
                    }
                    else
                    {
                        arenasession = latestArena.ToModel();
                    }
                    gameResult.ArenaSession = arenasession;
                }
                await gameManager.AddGame(gameResult);

                // for webapi
                if (arenasession != null)
                {
                    if (newArena)
                    {
                        events.PublishOnBackgroundThread(new ArenaSessionStarted(arenasession.StartDate, arenasession.Hero.Key, arenasession.Wins, arenasession.Losses));
                    }
                    else
                    {
                        if (arenasession.IsEnded
                            && arenasession.EndDate.HasValue)
                        {
                            events.PublishOnBackgroundThread(new ArenaSessionEnded(arenasession.StartDate, arenasession.EndDate.Value, arenasession.Hero.Key, arenasession.Wins, arenasession.Losses));
                        }
                    }
                }

                // notifications
                //var wonString = gameResult.Victory ? "You won!" : "You lost!";

                var title = "New game tracked.";
                //var hero = gameResult.Hero != null ? gameResult.Hero.ClassName : String.Empty;
                //var oppHero = gameResult.OpponentHero != null ? gameResult.OpponentHero.ClassName : String.Empty;
                //var msg = string.Format("Hero: {0}, Opponent: {1}, {2}", hero, oppHero, wonString);
                //events.PublishOnBackgroundThread(new SendNotification(String.Format("{0} {1}", title, msg)));

                var vm = IoC.Get<GameResultBalloonViewModel>();
                vm.SetGameResult(gameResult);
                events.PublishOnBackgroundThread(new TrayNotification(title, vm, 10000)
                    {
                        BalloonType = BalloonTypes.GameStartEnd
                    });

                // reset
                Clear();
                IsOpen = false;
            }
        }

        /// <summary>The save new game.</summary>
        /// <returns>The <see cref="Task" />.</returns>
        [Dependencies("Hero", "OpponentHero", "GameMode", "IsRunning")]
        public async Task SaveNewGame()
        {
            events.PublishOnBackgroundThread(new ResetCurrentGame());
            IsRunning = false;
            Stopped = DateTime.Now;
            await Save();
        }

        public bool CanSaveNewGame()
        {
            return Hero != null && OpponentHero != null && IsRunning;
        }

        [Dependencies("IsRunning")]
        public void StopGame()
        {
            events.PublishOnBackgroundThread(new ResetCurrentGame());
            IsOpen = false;
            Clear();
        }

        private void Clear()
        {
            IsRunning = false;
            StartTime = DateTime.Now;
            Hero = null;
            OpponentHero = null;
            GoFirst = true;
            Turns = 1;
            Notes = String.Empty;
            WinLossRatio.Clear();
            WinLossRatioHero.Clear();
            WinLossRatioOpponentHero.Clear();

            // don't reset (e.g. just remember last deck)
            //if (this.decks.Count > 0)
            //{
            //    this.SelectedDeck = this.decks.First();
            //}
        }

        /// <summary>Handles the message.</summary>
        /// <param name="message">The message.</param>
        public void Handle(GameStarted message)
        {
            EnsureInitialized();
            GameMode = message.GameMode;
            Hero = Heroes.FirstOrDefault(x => x.Key == message.Hero);
            OpponentHero = Heroes.FirstOrDefault(x => x.Key == message.OpponentHero);
            GoFirst = message.GoFirst;
            StartTime = message.StartTime;
            IsRunning = true;
            Deck = deckManager.GetOrCreateDeckBySlot(BindableServerCollection.Instance.DefaultName, message.Deck);
            events.PublishOnBackgroundThread(new ToggleFlyoutCommand(Flyouts.CurrentGame) { Show = true });
            events.PublishOnBackgroundThread(new ToggleFlyoutCommand(Flyouts.EditGame) { Show = false });
            var vm = IoC.Get<GameStartedBalloonViewModel>();
            vm.SetGameResult(message);
            events.PublishOnBackgroundThread(new TrayNotification("A new game has started", vm, 10000)
                {
                    BalloonType = BalloonTypes.GameStartEnd
                });
            RefreshStats();
        }

        private void RefreshStats()
        {
            var from = dateFilter.From;
            float wins;
            float losses;
            float total;

            WinLossRatio.Clear();
            if (Hero != null
                && OpponentHero != null)
            {
                wins =
                    gameRepository.Query(
                        x => x.Count(g => g.Started > @from && g.Victory && g.Hero.Id == Hero.Id && g.OpponentHero.Id == OpponentHero.Id));
                losses =
                    gameRepository.Query(
                        x => x.Count(g => g.Started > @from && !g.Victory && g.Hero.Id == Hero.Id && g.OpponentHero.Id == OpponentHero.Id));
                total = wins + losses;
                if (total > 0)
                {
                    WinLossRatio.Add(new StatModel(string.Format("Wins: {0}", wins), wins / total * 100));
                    WinLossRatio.Add(new StatModel(string.Format("Losses: {0}", losses), losses / total * 100));
                }
                else
                {
                    WinLossRatio.Add(new StatModel("Wins", 0));
                    WinLossRatio.Add(new StatModel("Losses", 0));
                }
            }
            else
            {
                WinLossRatio.Add(new StatModel("Wins", 0));
                WinLossRatio.Add(new StatModel("Losses", 0));
            }

            WinLossRatioHero.Clear();
            if (Hero != null)
            {
                wins = gameRepository.Query(x => x.Count(g => g.Started > @from && g.Victory && g.Hero.Id == Hero.Id));
                losses = gameRepository.Query(x => x.Count(g => g.Started > @from && !g.Victory && g.Hero.Id == Hero.Id));
                total = wins + losses;
                if (total > 0)
                {
                    WinLossRatioHero.Add(new StatModel(string.Format("Wins: {0}", wins), wins / total * 100));
                    WinLossRatioHero.Add(new StatModel(string.Format("Losses: {0}", losses), losses / total * 100));
                }
                else
                {
                    WinLossRatioHero.Add(new StatModel("Wins", 0));
                    WinLossRatioHero.Add(new StatModel("Losses", 0));
                }
            }
            else
            {
                WinLossRatioHero.Add(new StatModel("Wins", 0));
                WinLossRatioHero.Add(new StatModel("Losses", 0));
            }

            WinLossRatioOpponentHero.Clear();
            if (OpponentHero != null)
            {
                wins = gameRepository.Query(x => x.Count(g => g.Started > @from && g.Victory && g.OpponentHero.Id == OpponentHero.Id));
                losses = gameRepository.Query(x => x.Count(g => g.Started > @from && !g.Victory && g.OpponentHero.Id == OpponentHero.Id));
                total = wins + losses;
                if (total > 0)
                {
                    WinLossRatioOpponentHero.Add(new StatModel(string.Format("Wins: {0}", wins), wins / total * 100));
                    WinLossRatioOpponentHero.Add(new StatModel(string.Format("Losses: {0}", losses), losses / total * 100));
                }
                else
                {
                    WinLossRatioOpponentHero.Add(new StatModel("Wins", 0));
                    WinLossRatioOpponentHero.Add(new StatModel("Losses", 0));
                }
            }
            else
            {
                WinLossRatioOpponentHero.Add(new StatModel("Wins", 0));
                WinLossRatioOpponentHero.Add(new StatModel("Losses", 0));
            }
        }

        /// <summary>Handles the message.</summary>
        /// <param name="message">The message.</param>
        public void Handle(NewRound message)
        {
            Turns = message.Current;
        }

        #endregion

        #region Methods

        /// <summary>The load data.</summary>
        /// <returns>The <see cref="Task" />.</returns>
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
        }

        #endregion

        /// <summary>
        ///     Called when a part's imports have been satisfied and it is safe to use.
        /// </summary>
        public void OnImportsSatisfied()
        {
            EnsureInitialized();
        }
    }
}
