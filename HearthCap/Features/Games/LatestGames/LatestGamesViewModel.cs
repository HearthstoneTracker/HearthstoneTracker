// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LatestGamesViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The latest games view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Games.LatestGames
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Linq.Dynamic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Threading;

    using Caliburn.Micro;

    using HearthCap.Data;
    using HearthCap.Features.Analytics;
    using HearthCap.Features.Core;
    using HearthCap.Features.Decks;
    using HearthCap.Features.Decks.ModelMappers;
    using HearthCap.Features.GameManager;
    using HearthCap.Features.GameManager.Events;
    using HearthCap.Features.Games.Models;
    using HearthCap.Features.Games.Statistics;
    using HearthCap.Framework;
    using HearthCap.Shell;
    using HearthCap.Shell.Events;
    using HearthCap.Shell.Tabs;
    using HearthCap.Util;

    using NLog;

    using Action = System.Action;
    using LogManager = NLog.LogManager;

    /// <summary>
    /// The latest games view model.
    /// </summary>
    [Export(typeof(ITab))]
    [Export(typeof(LatestGamesViewModel))]
    public class LatestGamesViewModel : TabViewModel, 
                                        IHandle<GameResultAdded>, 
                                        IHandle<GameResultUpdated>, 
                                        IHandle<GameResultDeleted>, 
                                        IHandle<SelectedGameChanged>, 
                                        IHandle<RefreshAll>, 
                                        IHandle<DeckUpdated>
    {
        #region Static Fields

        /// <summary>
        /// The log.
        /// </summary>
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        #endregion

        #region Fields

        /// <summary>
        /// The deck manager.
        /// </summary>
        private readonly IDeckManager deckManager;

        /// <summary>
        /// The db context.
        /// </summary>
        private readonly Func<HearthStatsDbContext> dbContext;

        /// <summary>
        /// The game manager.
        /// </summary>
        private readonly GameManager gameManager;

        /// <summary>
        /// The events.
        /// </summary>
        private readonly IEventAggregator events;

        /// <summary>
        /// The game modes.
        /// </summary>
        private readonly BindableCollection<string> gameModes =
            new BindableCollection<string>(
                new[]
                    {
                        string.Empty, GameMode.Arena.ToString(), GameMode.Casual.ToString(), GameMode.Challenge.ToString(), GameMode.Practice.ToString(), 
                        GameMode.Ranked.ToString()
                    });

        /// <summary>
        /// The game repository.
        /// </summary>
        private readonly IRepository<GameResult> gameRepository;

        /// <summary>
        /// The game results.
        /// </summary>
        private readonly BindableCollection<GameResultModel> gameResults = new BindableCollection<GameResultModel>();

        /// <summary>
        /// The filter deck.
        /// </summary>
        private DeckModel filterDeck;

        /// <summary>
        /// The filter game mode.
        /// </summary>
        private string filterGameMode;

        /// <summary>
        /// The init data loaded.
        /// </summary>
        private bool initDataLoaded;

        /// <summary>
        /// The selected game.
        /// </summary>
        private GameResultModel selectedGame;

        /// <summary>
        /// The first time loaded.
        /// </summary>
        private bool firstTimeLoaded;

        /// <summary>
        /// The filter hero.
        /// </summary>
        private Hero filterHero;

        /// <summary>
        /// The heroes.
        /// </summary>
        private readonly BindableCollection<Hero> heroes = new BindableCollection<Hero>();

        /// <summary>
        /// The totals.
        /// </summary>
        private GameResultTotalsModel totals;

        /// <summary>
        /// The filter opponent hero.
        /// </summary>
        private Hero filterOpponentHero;

        /// <summary>
        /// The need refresh.
        /// </summary>
        private bool needRefresh = true;

        /// <summary>
        /// The game results cv.
        /// </summary>
        private ICollectionView gameResultsCV;

        /// <summary>
        /// The servers.
        /// </summary>
        private IObservableCollection<ServerItemModel> servers = new BindableCollection<ServerItemModel>(BindableServerCollection.Instance);

        /// <summary>
        /// The decks.
        /// </summary>
        private BindableCollection<DeckModel> decks = new BindableCollection<DeckModel>();

        /// <summary>
        /// The filter server.
        /// </summary>
        private ServerItemModel filterServer;

        /// <summary>
        /// The date filter.
        /// </summary>
        private readonly DateFilter dateFilter = new DateFilter { ShowAllTime = true };

        /// <summary>
        /// The current max.
        /// </summary>
        private int currentMax;

        /// <summary>
        /// The total count.
        /// </summary>
        private int totalCount;

        /// <summary>
        /// The search.
        /// </summary>
        private string search;

        /// <summary>
        /// The selected games.
        /// </summary>
        private BindableCollection<GameResultModel> selectedGames = new BindableCollection<GameResultModel>();

        /// <summary>
        /// The load more ticket.
        /// </summary>
        private BusyWatcher.BusyWatcherTicket loadMoreTicket;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LatestGamesViewModel"/> class.
        /// </summary>
        /// <param name="events">
        /// The events.
        /// </param>
        /// <param name="gameRepository">
        /// The game repository.
        /// </param>
        /// <param name="deckManager">
        /// The deck manager.
        /// </param>
        /// <param name="dbContext">
        /// The db context.
        /// </param>
        /// <param name="gameManager">
        /// The game manager.
        /// </param>
        [ImportingConstructor]
        public LatestGamesViewModel(IEventAggregator events, IRepository<GameResult> gameRepository, IDeckManager deckManager, Func<HearthStatsDbContext> dbContext, GameManager gameManager)
        {
            this.IsNotifying = false;
            this.events = events;
            this.gameRepository = gameRepository;
            this.deckManager = deckManager;
            this.dbContext = dbContext;
            this.gameManager = gameManager;
            this.Order = 1;
            this.DisplayName = "Games";
            this.events.Subscribe(this);
            this.totals = new GameResultTotalsModel();
            this.gameResultsCV = CollectionViewSource.GetDefaultView(this.gameResults);

            this.gameResultsCV.SortDescriptions.Add(new SortDescription("Started", ListSortDirection.Descending));
            this.servers.Insert(0, new ServerItemModel(string.Empty));

            this.dateFilter.From = DateTime.Now.AddMonths(-1).SetToBeginOfDay();
            this.Busy = new BusyWatcher();
            this.dateFilter.DateChanged += this.DateFilterOnPropertyChanged;
            this.PropertyChanged += this.OnPropertyChanged;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the global data.
        /// </summary>
        [Import]
        public GlobalData GlobalData { get; set; }

        /// <summary>
        /// Gets or sets the current stats.
        /// </summary>
        [Import]
        public FilteredStatsViewModel CurrentStats { get; set; }

        // [Import]
        // public EditGameFlyoutViewModel EditGameViewModel { get; set; }

        // [Import]
        // public CurrentSessionFlyoutViewModel ArenaViewModel { get; set; }

        /// <summary>
        /// Gets the decks.
        /// </summary>
        public IObservableCollection<DeckModel> Decks
        {
            get
            {
                return this.decks;
            }
        }

        /// <summary>
        /// Gets the servers.
        /// </summary>
        public IObservableCollection<ServerItemModel> Servers
        {
            get
            {
                return this.servers;
            }
        }

        /// <summary>
        /// Gets or sets the filter server.
        /// </summary>
        public ServerItemModel FilterServer
        {
            get
            {
                return this.filterServer;
            }

            set
            {
                if (Equals(value, this.filterServer))
                {
                    return;
                }

                this.filterServer = value;
                this.RefreshDecks();
                this.NotifyOfPropertyChange(() => this.FilterServer);
            }
        }

        /// <summary>
        /// Gets the totals.
        /// </summary>
        public GameResultTotalsModel Totals
        {
            get
            {
                return this.totals;
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
        /// Gets or sets the filter hero.
        /// </summary>
        public Hero FilterHero
        {
            get
            {
                return this.filterHero;
            }

            set
            {
                if (Equals(value, this.filterHero))
                {
                    return;
                }

                this.filterHero = value;
                this.NotifyOfPropertyChange(() => this.FilterHero);
            }
        }

        /// <summary>
        /// Gets or sets the filter opponent hero.
        /// </summary>
        public Hero FilterOpponentHero
        {
            get
            {
                return this.filterOpponentHero;
            }

            set
            {
                if (Equals(value, this.filterOpponentHero))
                {
                    return;
                }

                this.filterOpponentHero = value;
                this.NotifyOfPropertyChange(() => this.FilterOpponentHero);
            }
        }

        /// <summary>
        /// Gets or sets the filter deck.
        /// </summary>
        public DeckModel FilterDeck
        {
            get
            {
                return this.filterDeck;
            }

            set
            {
                if (Equals(value, this.filterDeck))
                {
                    return;
                }

                this.filterDeck = value;
                this.NotifyOfPropertyChange(() => this.FilterDeck);
            }
        }

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
        /// Gets or sets the filter game mode.
        /// </summary>
        public string FilterGameMode
        {
            get
            {
                return this.filterGameMode;
            }

            set
            {
                if (value == this.filterGameMode)
                {
                    return;
                }

                this.filterGameMode = value;
                this.NotifyOfPropertyChange(() => this.FilterGameMode);
            }
        }

        /// <summary>
        /// Gets the game modes.
        /// </summary>
        public BindableCollection<string> GameModes
        {
            get
            {
                return this.gameModes;
            }
        }

        /// <summary>
        /// Gets the game results.
        /// </summary>
        public IObservableCollection<GameResultModel> GameResults
        {
            get
            {
                return this.gameResults;
            }
        }

        /// <summary>
        /// Gets the game results cv.
        /// </summary>
        public ICollectionView GameResultsCV
        {
            get
            {
                return this.gameResultsCV;
            }
        }

        /// <summary>
        /// Gets or sets the selected game.
        /// </summary>
        public GameResultModel SelectedGame
        {
            get
            {
                return this.selectedGame;
            }

            set
            {
                if (Equals(value, this.selectedGame))
                {
                    return;
                }

                this.selectedGame = value;
                this.NotifyOfPropertyChange(() => this.SelectedGame);
            }
        }

        /// <summary>
        /// Gets or sets the search.
        /// </summary>
        public string Search
        {
            get
            {
                return this.search;
            }

            set
            {
                if (value == this.search)
                {
                    return;
                }

                this.search = value;
                this.NotifyOfPropertyChange(() => this.Search);
            }
        }

        /// <summary>
        /// Gets the selected games.
        /// </summary>
        public BindableCollection<GameResultModel> SelectedGames
        {
            get
            {
                return this.selectedGames;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the shell.
        /// </summary>
        [Import]
        protected Lazy<IShell> Shell { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The delete selected games.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task DeleteSelectedGames()
        {
            if (this.SelectedGames.Count == 0) return;

            if (MessageBox.Show(string.Format("Delete {0} games?", this.SelectedGames.Count), "Delete games?", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;

            var selectedGames = this.SelectedGames.ToList();
            foreach (var game in selectedGames)
            {
                await this.gameManager.DeleteGame(game.Id);
            }
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Handle(GameResultAdded message)
        {
            this.gameResults.Insert(0, message.GameResult);
            this.SelectedGame = message.GameResult;
            this.RefreshStats();
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Handle(GameResultUpdated message)
        {
            Execute.OnUIThread(
                () =>
                {
                    using (PauseNotify.For(this))
                    {
                        var hasGame = this.gameResults.FirstOrDefault(x => x.Id == message.GameResultId);
                        if (hasGame != null)
                        {
                            var newgame = this.gameRepository.FirstOrDefault(x => x.Id == message.GameResultId);
                            hasGame.MapFrom(newgame);
                            this.RefreshStats();
                        }
                    }
                });
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Handle(GameResultDeleted message)
        {
            var index = this.gameResults.FindIndex(x => x.Id == message.GameId);
            if (index >= 0)
            {
                this.gameResults.RemoveAt(index);

                if (this.SelectedGame != null && this.SelectedGame.Id == message.GameId)
                {
                    if (index > 0)
                    {
                        this.SelectedGame = this.GameResults[index - 1];
                    }

                    if (index == 0 && this.GameResults.Count > 0)
                    {
                        this.SelectedGame = this.GameResults[0];
                    }
                }

                this.RefreshStats();
            }

            // var hasGame = this.gameResults.Any(x => x.Id == message.GameId);
            // if (hasGame)
            // {
            // RefreshData();
            // RefreshStats();
            // }
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Handle(SelectedGameChanged message)
        {
            if (message.Source == this)
            {
                return;
            }

            if (message.Game == null)
            {
                this.SelectedGame = null;
                return;
            }

            if (this.SelectedGame != null && message.Game.Id == this.SelectedGame.Id)
            {
                return;
            }

            var hasGame = this.gameResults.FirstOrDefault(x => x.Id == message.Game.Id);
            if (hasGame != null)
            {
                this.SelectedGame = hasGame;
            }

            // Execute.OnUIThread(
            // () =>
            // {
            // using (PauseNotify.For(this))
            // {
            // }
            // });
        }

        /// <summary>
        /// The refresh data.
        /// </summary>
        public void RefreshData()
        {
            this.needRefresh = true;
            this.LoadMore(true);
            this.RefreshStats();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Called the first time the page's LayoutUpdated event fires after it is navigated to.
        /// </summary>
        /// <param name="view">
        /// </param>
        protected override void OnViewReady(object view)
        {
        }

        /// <summary>
        /// Called when initializing.
        /// </summary>
        protected override async void OnInitialize()
        {
        }

        /// <summary>
        /// The on activate.
        /// </summary>
        protected override void OnActivate()
        {
            Tracker.TrackPageViewAsync("Games", "Games");
        }

        /// <summary>
        /// Called when an attached view's Loaded event fires.
        /// </summary>
        /// <param name="view">
        /// </param>
        protected override void OnViewLoaded(object view)
        {
            if (!this.firstTimeLoaded)
            {
                this.firstTimeLoaded = true;
                this.EnsureInitialized();
                this.RefreshData();
                this.IsNotifying = true;
            }
        }

        /// <summary>
        /// The get filter expression.
        /// </summary>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        private Expression<Func<GameResult, bool>> GetFilterExpression()
        {
            var query = PredicateBuilder.True<GameResult>(); 
            if (this.dateFilter.From != null)
            {
                var filterFromDate = this.dateFilter.From.Value.SetToBeginOfDay();
                query = query.And(x => x.Started >= filterFromDate);
            }

            if (this.dateFilter.To != null)
            {
                var filterToDate = this.dateFilter.To.Value.SetToEndOfDay();
                query = query.And(x => x.Started <= filterToDate);
            }

            if (this.FilterServer != null && !string.IsNullOrEmpty(this.FilterServer.Name))
            {
                var serverName = this.FilterServer.Name;
                query = query.And(x => x.Server == serverName);
            }

            if (!string.IsNullOrWhiteSpace(this.FilterGameMode))
            {
                GameMode gm;
                if (Enum.TryParse(this.FilterGameMode, out gm))
                {
                    query = query.And(x => x.GameMode == gm);
                }
            }

            if (this.FilterDeck != null && this.FilterDeck.Id != Guid.Empty)
            {
                query = query.And(x => x.Deck.Id == this.FilterDeck.Id);
            }

            if (this.FilterHero != null && !string.IsNullOrEmpty(this.FilterHero.Key))
            {
                query = query.And(x => x.Hero.Id == this.FilterHero.Id);
            }

            if (this.FilterOpponentHero != null && !string.IsNullOrEmpty(this.FilterOpponentHero.Key))
            {
                query = query.And(x => x.OpponentHero.Id == this.FilterOpponentHero.Id);
            }

            if (!string.IsNullOrEmpty(this.Search))
            {
                var s = this.Search.ToLowerInvariant().Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var keyword in s)
                {
                    string keyword1 = keyword;
                    query = query.And(x =>
                        x.Notes.ToLower().Contains(keyword1) ||
                        x.Hero.ClassName.ToLower().Contains(keyword1) ||
                        x.OpponentHero.ClassName.ToLower().Contains(keyword1) ||
                        x.Deck.Name.ToLower().Contains(keyword1));
                }
            }

            return query;
        }

        /// <summary>
        /// The refresh stats.
        /// </summary>
        /// <param name="filter">
        /// The filter.
        /// </param>
        private void RefreshStats(Expression<Func<GameResult, bool>> filter = null)
        {
            var expr = filter ?? this.GetFilterExpression();
            Task.Run(() => this.CurrentStats.RefreshFrom(this.dbContext, expr));
            Task.Run(() => this.totals.Update(this.dbContext, expr));
        }

        /// <summary>
        /// The scroll changed.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        public void ScrollChanged(ScrollChangedEventArgs e)
        {
            if (e.VerticalOffset >= e.ExtentHeight - (e.ViewportHeight * 2))
            {
                if (!this.Busy.IsBusy && this.gameResults.Count < this.totalCount)
                {
                    this.needRefresh = true;
                    this.LoadMore();
                }
            }
        }

        /// <summary>
        /// The load more.
        /// </summary>
        /// <param name="clearValues">
        /// The clear values.
        /// </param>
        private void LoadMore(bool clearValues = false)
        {
            this.EnsureInitialized();
            Application.Current.Dispatcher.BeginInvoke((Action)(() =>
            {
                if (this.needRefresh)
                {
                    this.needRefresh = false;
                    if (this.loadMoreTicket != null)
                    {
                        return;
                    }

                    this.loadMoreTicket = this.Busy.GetTicket();
                    Task.Run(
                        async () =>
                        {
                            var oldSelected = this.SelectedGame;
                            if (clearValues)
                            {
                                this.gameResults.Clear();
                            }

                            var newgames = new List<GameResultModel>();
                            var latest = (await this.gameRepository.ToListAsync(
                                query =>
                                {
                                    query = query.Where(this.GetFilterExpression());
                                    query = this.AddOrderByExpression(query);
                                    return query.Skip(clearValues ? 0 : this.gameResults.Count)
                                        .Take(50);

                                })).ToModel();
                            this.totalCount = this.gameRepository.Query(x => x.Where(this.GetFilterExpression()).Count());

                            newgames.AddRange(latest);
                            this.gameResults.AddRange(newgames);
                            if (oldSelected != null)
                            {
                                var foundold = this.gameResults.FirstOrDefault(x => x.Id == oldSelected.Id);
                                if (foundold != null)
                                {
                                    this.SelectedGame = foundold;
                                }
                                else
                                {
                                    this.SelectedGame = null;
                                }
                            }

                            this.loadMoreTicket.Dispose();
                            this.loadMoreTicket = null;
                        });
                }
            }), DispatcherPriority.ContextIdle);
        }

        /// <summary>
        /// The sorting.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        public void Sorting(DataGridSortingEventArgs args)
        {
            this.RefreshData();
        }

        /// <summary>
        /// The add order by expression.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see cref="IQueryable"/>.
        /// </returns>
        private IQueryable<GameResult> AddOrderByExpression(IQueryable<GameResult> query)
        {
            foreach (var sd in this.gameResultsCV.SortDescriptions)
            {
                // if (sd.PropertyName == "Duration")
                // {
                // query = query.OrderBy(sd.PropertyName);
                // }
                if (sd.Direction == ListSortDirection.Ascending)
                {
                    query = query.OrderBy(sd.PropertyName);
                }
                else
                {
                    query = query.OrderBy(string.Format("{0} descending", sd.PropertyName));
                }
            }

            return query;
        }

        /// <summary>
        /// Gets or sets the busy.
        /// </summary>
        public IBusyWatcher Busy { get; set; }

        /// <summary>
        /// The ensure initialized.
        /// </summary>
        private void EnsureInitialized()
        {
            if (this.initDataLoaded)
            {
                return;
            }

            this.initDataLoaded = true;

            var data = this.GlobalData.Get();
            this.heroes.IsNotifying = false;

            // empty hero for 'all'
            this.heroes.Add(new Hero(string.Empty));
            this.heroes.AddRange(data.Heroes.OrderBy(x => x.Name));
            this.heroes.IsNotifying = true;
            this.heroes.Refresh();

            this.RefreshDecks();
        }

        /// <summary>
        /// The refresh decks.
        /// </summary>
        private void RefreshDecks()
        {
            if (this.FilterServer != null && !string.IsNullOrEmpty(this.FilterServer.Name))
            {
                var decks = this.deckManager.GetDecks(this.FilterServer.Name).Select(x => x.ToModel());
                this.decks.Clear();
                this.decks.Add(DeckModel.EmptyEntry);
                this.decks.AddRange(decks);
                if (this.decks.Count > 0)
                {
                    this.FilterDeck = this.decks.First();
                }
            }
            else
            {
                var oldSelDeck = this.FilterDeck;

                var decks = this.deckManager.GetAllDecks().Select(x => x.ToModel());
                this.decks.Clear();
                this.decks.Add(DeckModel.EmptyEntry);
                this.decks.AddRange(decks);
                if (oldSelDeck != null)
                {
                    var newSelectedDeck = this.decks.FirstOrDefault(x => x.Id == oldSelDeck.Id);
                    if (newSelectedDeck != null) this.FilterDeck = newSelectedDeck;
                }
            }
        }

        /// <summary>
        /// The date filter on property changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="eventArgs">
        /// The event args.
        /// </param>
        private void DateFilterOnPropertyChanged(object sender, EventArgs eventArgs)
        {
            this.RefreshData();
        }

        /// <summary>
        /// The on property changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (!PauseNotify.IsPaused(this))
            {
                switch (args.PropertyName)
                {
                    case "SelectedGame":

                        // if (SelectedGame != null)
                        // {
                        // if (SelectedGame.ArenaSession == null)
                        // {
                        // ArenaViewModel.IsOpen = false;
                        // }
                        // else
                        // {
                        // // this is annoying
                        // // ArenaViewModel.Load(SelectedGame.ArenaSession);
                        // }

                        // // EditGameViewModel.Load(SelectedGame);
                        // }
                        this.events.PublishOnUIThread(new SelectedGameChanged(this, this.SelectedGame));
                        break;
                    case "FilterServer":
                    case "FilterHero":
                    case "FilterOpponentHero":
                    case "FilterDeck":
                    case "FilterGameMode":
                        this.RefreshData();
                        break;
                }
            }
        }

        #endregion

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Handle(RefreshAll message)
        {
            this.RefreshData();
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Handle(DeckUpdated message)
        {
            if (message.Deck == null)
            {
                this.RefreshDecks();
                return;
            }

            var found = this.Decks.FirstOrDefault(x => x.Id == message.Deck.Id);
            if (found != null)
            {
                found.MapFrom(message.Deck);
            }

            foreach (var gameResult in this.gameResults)
            {
                if (gameResult.Deck != null && gameResult.Deck.Id == message.Deck.Id)
                {
                    gameResult.Deck = message.Deck;
                }
            }
        }

        // <summary>
        // Handles the message.
        // </summary>
        // <param name="message">The message.</param>
        // public void Handle(DecksUpdated message)
        // {
        // RefreshDecks();
        // }
    }
}