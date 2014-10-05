// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CurrentGameFlyoutViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The current game flyout view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Games.CurrentGame
{
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
    using HearthCap.Features.GameManager;
    using HearthCap.Features.Games;
    using HearthCap.Features.Games.Balloons;
    using HearthCap.Features.Games.Models;
    using HearthCap.Features.Games.Statistics;
    using HearthCap.Framework;
    using HearthCap.Shell.Flyouts;
    using HearthCap.Shell.TrayIcon;
    using HearthCap.Util;

    using MahApps.Metro.Controls;

    using NLog;

    using LogManager = NLog.LogManager;

    /// <summary>The current game flyout view model.</summary>
    [Export(typeof(IFlyout))]
    public class CurrentGameFlyoutViewModel : FlyoutViewModel, 
                                              IHandleWithTask<GameEnded>, 
                                              IHandle<GameStarted>, 
                                              IHandle<NewRound>, 
                                              IPartImportsSatisfiedNotification
    {
        #region Fields

        /// <summary>
        /// The log.
        /// </summary>
        private Logger Log = LogManager.GetCurrentClassLogger();

        /// <summary>The events.</summary>
        private readonly IEventAggregator events;

        /// <summary>
        /// The game repository.
        /// </summary>
        private readonly IRepository<GameResult> gameRepository;

        /// <summary>
        /// The arena repository.
        /// </summary>
        private readonly IRepository<ArenaSession> arenaRepository;

        /// <summary>The game mode.</summary>
        private GameMode gameMode;

        /// <summary>The game modes.</summary>
        private BindableCollection<GameMode> gameModes = new GameModesCollection();

        /// <summary>The go first.</summary>
        private bool goFirst;

        /// <summary>The hero.</summary>
        private Hero hero;

        /// <summary>The heroes.</summary>
        private BindableCollection<Hero> heroes;

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

        /// <summary>
        /// The selected deck.
        /// </summary>
        private DeckModel selectedDeck;

        /// <summary>
        /// The deck manager.
        /// </summary>
        private IDeckManager deckManager;

        /// <summary>
        /// The game manager.
        /// </summary>
        private readonly GameManager gameManager;

        /// <summary>
        /// The initialized.
        /// </summary>
        private bool initialized;

        /// <summary>
        /// The win loss ratio.
        /// </summary>
        private BindableCollection<StatModel> winLossRatio = new BindableCollection<StatModel>();

        /// <summary>
        /// The win loss ratio hero.
        /// </summary>
        private BindableCollection<StatModel> winLossRatioHero = new BindableCollection<StatModel>();

        /// <summary>
        /// The win loss ratio opponent hero.
        /// </summary>
        private BindableCollection<StatModel> winLossRatioOpponentHero = new BindableCollection<StatModel>();

        /// <summary>
        /// The date filter.
        /// </summary>
        private readonly DateFilter dateFilter = new DateFilter();

        /// <summary>
        /// The deck.
        /// </summary>
        private Deck deck;

        /// <summary>
        /// The victory.
        /// </summary>
        private bool victory;

        /// <summary>
        /// The stopped.
        /// </summary>
        private DateTime stopped;

        /// <summary>
        /// The started.
        /// </summary>
        private DateTime started;

        /// <summary>
        /// The conceded.
        /// </summary>
        private bool conceded;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CurrentGameFlyoutViewModel"/> class.
        /// </summary>
        public CurrentGameFlyoutViewModel()
        {
            if (Execute.InDesignMode)
            {
                this.IsRunning = true;
                this.WinLossRatio.Add(new StatModel("Wins", 60));
                this.WinLossRatio.Add(new StatModel("Losses", 40));
                this.WinLossRatioHero.Add(new StatModel("Wins", 0));
                this.WinLossRatioHero.Add(new StatModel("Losses", 0));
                this.WinLossRatioOpponentHero.Add(new StatModel("Wins", 50));
                this.WinLossRatioOpponentHero.Add(new StatModel("Losses", 50));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CurrentGameFlyoutViewModel"/> class.
        /// </summary>
        /// <param name="events">
        /// The events.
        /// </param>
        /// <param name="gameRepository">
        /// The game Repository.
        /// </param>
        /// <param name="arenaRepository">
        /// The arena Repository.
        /// </param>
        /// <param name="deckManager">
        /// The deck manager
        /// </param>
        /// <param name="gameManager">
        /// The game Manager.
        /// </param>
        [ImportingConstructor]
        public CurrentGameFlyoutViewModel(
            IEventAggregator events, 
            IRepository<GameResult> gameRepository, 
            IRepository<ArenaSession> arenaRepository, 
            IDeckManager deckManager, 
            GameManager gameManager)
        {
            this.events = events;
            this.gameRepository = gameRepository;
            this.arenaRepository = arenaRepository;
            this.deckManager = deckManager;
            this.gameManager = gameManager;
            this.Name = Flyouts.CurrentGame;
            this.Header = "Current Game:";
            this.SetPosition(Position.Right);
            this.heroes = new BindableCollection<Hero>();
            events.Subscribe(this);

            this.GameMode = GameMode.Practice;
            this.StartTime = DateTime.Now;
            this.GoFirst = true;
            this.dateFilter.From = DateTime.Now.AddMonths(-1).SetToBeginOfDay();
            this.dateFilter.DateChanged += (sender, args) => this.RefreshStats();
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the busy.</summary>
        [Import]
        public IBusyWatcher Busy { get; set; }

        /// <summary>
        /// Gets the date filter.
        /// </summary>
        public DateFilter DateFilter
        {
            get
            {
                return this.dateFilter;
            }
        }

        /// <summary>
        /// Gets the win loss ratio.
        /// </summary>
        public BindableCollection<StatModel> WinLossRatio
        {
            get
            {
                return this.winLossRatio;
            }
        }

        /// <summary>
        /// Gets the win loss ratio hero.
        /// </summary>
        public BindableCollection<StatModel> WinLossRatioHero
        {
            get
            {
                return this.winLossRatioHero;
            }
        }

        /// <summary>
        /// Gets the win loss ratio opponent hero.
        /// </summary>
        public BindableCollection<StatModel> WinLossRatioOpponentHero
        {
            get
            {
                return this.winLossRatioOpponentHero;
            }
        }

        /// <summary>Gets or sets the game mode.</summary>
        public GameMode GameMode
        {
            get
            {
                return this.gameMode;
            }

            set
            {
                if (value == this.gameMode)
                {
                    return;
                }

                this.gameMode = value;
                this.NotifyOfPropertyChange(() => this.GameMode);
                this.NotifyOfPropertyChange(() => this.IsArena);
            }
        }

        /// <summary>Gets the game modes.</summary>
        public BindableCollection<GameMode> GameModes
        {
            get
            {
                return this.gameModes;
            }
        }

        /// <summary>Gets or sets the global data.</summary>
        [Import]
        public GlobalData GlobalData { get; set; }

        /// <summary>Gets or sets a value indicating whether go first.</summary>
        public bool GoFirst
        {
            get
            {
                return this.goFirst;
            }

            set
            {
                if (value.Equals(this.goFirst))
                {
                    return;
                }

                this.goFirst = value;
                this.NotifyOfPropertyChange(() => this.GoFirst);
            }
        }

        /// <summary>Gets or sets the hero.</summary>
        public Hero Hero
        {
            get
            {
                return this.hero;
            }

            set
            {
                if (value == this.hero)
                {
                    return;
                }

                this.hero = value;
                this.NotifyOfPropertyChange(() => this.Hero);
            }
        }

        /// <summary>Gets the heroes.</summary>
        public IObservableCollection<Hero> Heroes
        {
            get
            {
                return this.heroes;
            }
        }

        /// <summary>Gets or sets a value indicating whether is running.</summary>
        public bool IsRunning
        {
            get
            {
                return this.isRunning;
            }

            set
            {
                if (value.Equals(this.isRunning))
                {
                    return;
                }

                this.isRunning = value;
                this.NotifyOfPropertyChange(() => this.IsRunning);
            }
        }

        /// <summary>Gets or sets the notes.</summary>
        public string Notes
        {
            get
            {
                return this.notes;
            }

            set
            {
                if (value == this.notes)
                {
                    return;
                }

                this.notes = value;
                this.NotifyOfPropertyChange(() => this.Notes);
            }
        }

        /// <summary>Gets or sets the opponent hero.</summary>
        public Hero OpponentHero
        {
            get
            {
                return this.opponentHero;
            }

            set
            {
                if (value == this.opponentHero)
                {
                    return;
                }

                this.opponentHero = value;
                this.NotifyOfPropertyChange(() => this.OpponentHero);
            }
        }

        /// <summary>Gets or sets the start time.</summary>
        public DateTime StartTime
        {
            get
            {
                return this.startTime;
            }

            set
            {
                if (value.Equals(this.startTime))
                {
                    return;
                }

                this.startTime = value;
                this.NotifyOfPropertyChange(() => this.StartTime);
            }
        }

        /// <summary>Gets or sets the turns.</summary>
        public int Turns
        {
            get
            {
                return this.turns;
            }

            set
            {
                if (value == this.turns || value < 1)
                {
                    return;
                }

                this.turns = value;
                this.NotifyOfPropertyChange(() => this.Turns);
            }
        }

        /// <summary>
        /// Gets a value indicating whether is arena.
        /// </summary>
        public bool IsArena
        {
            get
            {
                return this.GameMode == GameMode.Arena;
            }
        }

        /// <summary>
        /// Gets or sets the deck.
        /// </summary>
        public Deck Deck
        {
            get
            {
                return this.deck;
            }

            set
            {
                if (Equals(value, this.deck))
                {
                    return;
                }

                this.deck = value;
                this.NotifyOfPropertyChange(() => this.Deck);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether victory.
        /// </summary>
        public bool Victory
        {
            get
            {
                return this.victory;
            }

            set
            {
                if (value.Equals(this.victory))
                {
                    return;
                }

                this.victory = value;
                this.NotifyOfPropertyChange(() => this.Victory);
            }
        }

        /// <summary>
        /// Gets or sets the stopped.
        /// </summary>
        public DateTime Stopped
        {
            get
            {
                return this.stopped;
            }

            set
            {
                if (value.Equals(this.stopped))
                {
                    return;
                }

                this.stopped = value;
                this.NotifyOfPropertyChange(() => this.Stopped);
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
        /// Gets or sets a value indicating whether conceded.
        /// </summary>
        public bool Conceded
        {
            get
            {
                return this.conceded;
            }

            set
            {
                if (value.Equals(this.conceded))
                {
                    return;
                }

                this.conceded = value;
                this.NotifyOfPropertyChange(() => this.Conceded);
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task Handle(GameEnded message)
        {
            this.events.PublishOnBackgroundThread(new ResetCurrentGame());

            // TODO: do not override if manually set.
            this.GameMode = message.GameMode;
            this.StartTime = message.StartTime;

            if (!string.IsNullOrWhiteSpace(message.Hero))
            {
                this.Hero = this.Heroes.FirstOrDefault(x => x.Key == message.Hero);
            }

            if (!string.IsNullOrWhiteSpace(message.OpponentHero))
            {
                this.OpponentHero = this.Heroes.FirstOrDefault(x => x.Key == message.OpponentHero);
            }

            if (message.GoFirst.HasValue)
            {
                this.GoFirst = message.GoFirst.Value;
            }

            this.IsRunning = false;
            this.Turns = message.Turns;

            this.GameMode = message.GameMode;
            this.Conceded = message.Conceded;

            this.GoFirst = !message.GoFirst.HasValue || message.GoFirst.Value;
            this.Hero = this.Heroes.FirstOrDefault(x => x.Key == message.Hero);
            this.OpponentHero = this.Heroes.FirstOrDefault(x => x.Key == message.OpponentHero);
            this.Started = message.StartTime;
            this.Stopped = message.EndTime;
            this.Victory = message.Victory.HasValue && message.Victory.Value;
            this.Deck = this.deckManager.GetOrCreateDeckBySlot(BindableServerCollection.Instance.DefaultName, message.Deck);

            await this.Save();
        }

        /// <summary>
        /// The save.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task Save()
        {
            using (var bsy = this.Busy.GetTicket())
            {
                var gameResult = new GameResultModel { };
                ArenaSessionModel arenasession = null;
                bool newArena = false;
                gameResult.GameMode = this.GameMode;
                gameResult.Conceded = this.Conceded;

                if (this.Deck != null)
                {
                    gameResult.Deck = this.Deck;
                }

                gameResult.GoFirst = this.GoFirst;
                gameResult.Hero = this.Hero;
                gameResult.OpponentHero = this.OpponentHero;
                gameResult.Started = this.StartTime;
                gameResult.Stopped = this.Stopped;
                gameResult.Turns = this.Turns;
                gameResult.Victory = this.Victory;
                gameResult.Notes = this.Notes;
                gameResult.Server = BindableServerCollection.Instance.DefaultName;

                if (gameResult.GameMode == GameMode.Arena)
                {
                    var serverName = gameResult.Server;
                    var latestArena =
                        this.arenaRepository.Query(a => a.Where(x => x.Server == serverName).OrderByDescending(x => x.StartDate).FirstOrDefault());
                    if (latestArena == null || latestArena.IsEnded || (gameResult.Hero != null && latestArena.Hero.Key != gameResult.Hero.Key))
                    {
                        this.Log.Debug("Creating new arena.");
                        newArena = true;
                        arenasession = new ArenaSessionModel { Hero = gameResult.Hero, StartDate = gameResult.Started };
                        try
                        {
                            GlobalLocks.NewArenaLock.Reset();
                            await this.gameManager.AddArenaSession(arenasession);
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

                await this.gameManager.AddGame(gameResult);

                // for webapi
                if (arenasession != null)
                {
                    if (newArena)
                    {
                        this.events.PublishOnBackgroundThread(
                            new ArenaSessionStarted(arenasession.StartDate, arenasession.Hero.Key, arenasession.Wins, arenasession.Losses));
                    }
                    else
                    {
                        if (arenasession.IsEnded && arenasession.EndDate.HasValue)
                        {
                            this.events.PublishOnBackgroundThread(
                                new ArenaSessionEnded(
                                    arenasession.StartDate, 
                                    arenasession.EndDate.Value, 
                                    arenasession.Hero.Key, 
                                    arenasession.Wins, 
                                    arenasession.Losses));
                        }
                    }
                }

                // notifications
                // var wonString = gameResult.Victory ? "You won!" : "You lost!";
                var title = "New game tracked.";

                // var hero = gameResult.Hero != null ? gameResult.Hero.ClassName : String.Empty;
                // var oppHero = gameResult.OpponentHero != null ? gameResult.OpponentHero.ClassName : String.Empty;
                // var msg = string.Format("Hero: {0}, Opponent: {1}, {2}", hero, oppHero, wonString);
                // events.PublishOnBackgroundThread(new SendNotification(String.Format("{0} {1}", title, msg)));
                var vm = IoC.Get<GameResultBalloonViewModel>();
                vm.SetGameResult(gameResult);
                this.events.PublishOnBackgroundThread(new TrayNotification(title, vm, 10000) { BalloonType = BalloonTypes.GameStartEnd });

                // reset
                this.Clear();
                this.IsOpen = false;
            }
        }

        /// <summary>The save new game.</summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Dependencies("Hero", "OpponentHero", "GameMode", "IsRunning")]
        public async Task SaveNewGame()
        {
            this.events.PublishOnBackgroundThread(new ResetCurrentGame());
            this.IsRunning = false;
            this.Stopped = DateTime.Now;
            await this.Save();
        }

        /// <summary>
        /// The can save new game.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool CanSaveNewGame()
        {
            return this.Hero != null && this.OpponentHero != null && this.IsRunning;
        }

        /// <summary>
        /// The stop game.
        /// </summary>
        [Dependencies("IsRunning")]
        public void StopGame()
        {
            this.events.PublishOnBackgroundThread(new ResetCurrentGame());
            this.IsOpen = false;
            this.Clear();
        }

        /// <summary>
        /// The clear.
        /// </summary>
        private void Clear()
        {
            this.IsRunning = false;
            this.StartTime = DateTime.Now;
            this.Hero = null;
            this.OpponentHero = null;
            this.GoFirst = true;
            this.Turns = 1;
            this.Notes = string.Empty;
            this.WinLossRatio.Clear();
            this.WinLossRatioHero.Clear();
            this.WinLossRatioOpponentHero.Clear();

            // don't reset (e.g. just remember last deck)
            // if (this.decks.Count > 0)
            // {
            // this.SelectedDeck = this.decks.First();
            // }
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Handle(GameStarted message)
        {
            this.EnsureInitialized();
            this.GameMode = message.GameMode;
            this.Hero = this.Heroes.FirstOrDefault(x => x.Key == message.Hero);
            this.OpponentHero = this.Heroes.FirstOrDefault(x => x.Key == message.OpponentHero);
            this.GoFirst = message.GoFirst;
            this.StartTime = message.StartTime;
            this.IsRunning = true;
            this.Deck = this.deckManager.GetOrCreateDeckBySlot(BindableServerCollection.Instance.DefaultName, message.Deck);
            this.events.PublishOnBackgroundThread(new ToggleFlyoutCommand(Flyouts.CurrentGame) { Show = true });
            this.events.PublishOnBackgroundThread(new ToggleFlyoutCommand(Flyouts.EditGame) { Show = false });
            var vm = IoC.Get<GameStartedBalloonViewModel>();
            vm.SetGameResult(message);
            this.events.PublishOnBackgroundThread(new TrayNotification("A new game has started", vm, 10000)
                                                 {
                                                     BalloonType = BalloonTypes.GameStartEnd
                                                 });
            this.RefreshStats();
        }

        /// <summary>
        /// The refresh stats.
        /// </summary>
        private void RefreshStats()
        {
            var from = this.dateFilter.From;
            float wins;
            float losses;
            float total;

            this.WinLossRatio.Clear();
            if (this.Hero != null && this.OpponentHero != null)
            {
                wins =
                    this.gameRepository.Query(
                        x => x.Count(g => g.Started > @from && g.Victory && g.Hero.Id == this.Hero.Id && g.OpponentHero.Id == this.OpponentHero.Id));
                losses =
                    this.gameRepository.Query(
                        x => x.Count(g => g.Started > @from && !g.Victory && g.Hero.Id == this.Hero.Id && g.OpponentHero.Id == this.OpponentHero.Id));
                total = wins + losses;
                if (total > 0)
                {
                    this.WinLossRatio.Add(new StatModel(string.Format("Wins: {0}", wins), wins / total * 100));
                    this.WinLossRatio.Add(new StatModel(string.Format("Losses: {0}", losses), losses / total * 100));
                }
                else
                {
                    this.WinLossRatio.Add(new StatModel("Wins", 0));
                    this.WinLossRatio.Add(new StatModel("Losses", 0));
                }
            }
            else
            {
                this.WinLossRatio.Add(new StatModel("Wins", 0));
                this.WinLossRatio.Add(new StatModel("Losses", 0));
            }

            this.WinLossRatioHero.Clear();
            if (this.Hero != null)
            {
                wins = this.gameRepository.Query(x => x.Count(g => g.Started > @from && g.Victory && g.Hero.Id == this.Hero.Id));
                losses = this.gameRepository.Query(x => x.Count(g => g.Started > @from && !g.Victory && g.Hero.Id == this.Hero.Id));
                total = wins + losses;
                if (total > 0)
                {
                    this.WinLossRatioHero.Add(new StatModel(string.Format("Wins: {0}", wins), wins / total * 100));
                    this.WinLossRatioHero.Add(new StatModel(string.Format("Losses: {0}", losses), losses / total * 100));
                }
                else
                {
                    this.WinLossRatioHero.Add(new StatModel("Wins", 0));
                    this.WinLossRatioHero.Add(new StatModel("Losses", 0));
                }
            }
            else
            {
                this.WinLossRatioHero.Add(new StatModel("Wins", 0));
                this.WinLossRatioHero.Add(new StatModel("Losses", 0));
            }

            this.WinLossRatioOpponentHero.Clear();
            if (this.OpponentHero != null)
            {
                wins = this.gameRepository.Query(x => x.Count(g => g.Started > @from && g.Victory && g.OpponentHero.Id == this.OpponentHero.Id));
                losses = this.gameRepository.Query(x => x.Count(g => g.Started > @from && !g.Victory && g.OpponentHero.Id == this.OpponentHero.Id));
                total = wins + losses;
                if (total > 0)
                {
                    this.WinLossRatioOpponentHero.Add(new StatModel(string.Format("Wins: {0}", wins), wins / total * 100));
                    this.WinLossRatioOpponentHero.Add(new StatModel(string.Format("Losses: {0}", losses), losses / total * 100));
                }
                else
                {
                    this.WinLossRatioOpponentHero.Add(new StatModel("Wins", 0));
                    this.WinLossRatioOpponentHero.Add(new StatModel("Losses", 0));
                }
            }
            else
            {
                this.WinLossRatioOpponentHero.Add(new StatModel("Wins", 0));
                this.WinLossRatioOpponentHero.Add(new StatModel("Losses", 0));
            }
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Handle(NewRound message)
        {
            this.Turns = message.Current;
        }

        #endregion

        #region Methods

        /// <summary>The on initialize.</summary>
        protected override async void OnInitialize()
        {
        }

        /// <summary>
        /// The load data.
        /// </summary>
        private void EnsureInitialized()
        {
            if (this.initialized) return;
            this.initialized = true;

            var data = this.GlobalData.Get();
            this.heroes.Clear();
            this.heroes.AddRange(data.Heroes);
        }



        #endregion

        /// <summary>
        /// Called when a part's imports have been satisfied and it is safe to use.
        /// </summary>
        public void OnImportsSatisfied()
        {
            this.EnsureInitialized();
        }
    }
}