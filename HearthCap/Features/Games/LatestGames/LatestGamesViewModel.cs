namespace HearthCap.Features.Games.LatestGames
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.Composition;
    using System.Data.Entity.SqlServer;
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
    using HearthCap.Features.ArenaSessions;
    using HearthCap.Features.Core;
    using HearthCap.Features.Decks;
    using HearthCap.Features.Decks.ModelMappers;
    using HearthCap.Features.GameManager;
    using HearthCap.Features.GameManager.Events;
    using HearthCap.Features.Games.EditGame;
    using HearthCap.Features.Games.Models;
    using HearthCap.Features.Games.Statistics;
    using HearthCap.Framework;
    using HearthCap.Shell;
    using HearthCap.Shell.Events;
    using HearthCap.Shell.Tabs;
    using HearthCap.Util;

    using NLog;

    using Action = System.Action;

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

        private static readonly Logger Log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        #region Fields

        private readonly IDeckManager deckManager;

        private readonly Func<HearthStatsDbContext> dbContext;

        private readonly GameManager gameManager;

        private readonly IEventAggregator events;

        private readonly BindableCollection<string> gameModes =
            new BindableCollection<string>(
                new[]
                    {
                        string.Empty, GameMode.Arena.ToString(), GameMode.Casual.ToString(), GameMode.Challenge.ToString(), GameMode.Practice.ToString(), 
                        GameMode.Ranked.ToString()
                    });

        private readonly IRepository<GameResult> gameRepository;

        private readonly BindableCollection<GameResultModel> gameResults = new BindableCollection<GameResultModel>();

        private DeckModel filterDeck;

        private string filterGameMode;

        private bool initDataLoaded;

        private GameResultModel selectedGame;

        private bool firstTimeLoaded = false;

        private Hero filterHero;

        private readonly BindableCollection<Hero> heroes = new BindableCollection<Hero>();

        private GameResultTotalsModel totals;

        private Hero filterOpponentHero;

        private bool needRefresh = true;

        private ICollectionView gameResultsCV;

        private IObservableCollection<ServerItemModel> servers = new BindableCollection<ServerItemModel>(BindableServerCollection.Instance);

        private BindableCollection<DeckModel> decks = new BindableCollection<DeckModel>();

        private ServerItemModel filterServer;

        private readonly DateFilter dateFilter = new DateFilter() { ShowAllTime = true };

        private int currentMax;

        private int totalCount;

        private string search;

        private BindableCollection<GameResultModel> selectedGames = new BindableCollection<GameResultModel>();

        private BusyWatcher.BusyWatcherTicket loadMoreTicket;

        #endregion

        #region Constructors and Destructors

        [ImportingConstructor]
        public LatestGamesViewModel(IEventAggregator events, IRepository<GameResult> gameRepository, IDeckManager deckManager, Func<HearthStatsDbContext> dbContext, GameManager gameManager)
        {
            IsNotifying = false;
            this.events = events;
            this.gameRepository = gameRepository;
            this.deckManager = deckManager;
            this.dbContext = dbContext;
            this.gameManager = gameManager;
            this.Order = 1;
            this.DisplayName = "Games";
            this.events.Subscribe(this);
            this.totals = new GameResultTotalsModel();
            gameResultsCV = CollectionViewSource.GetDefaultView(this.gameResults);

            gameResultsCV.SortDescriptions.Add(new SortDescription("Started", ListSortDirection.Descending));
            servers.Insert(0, new ServerItemModel(""));

            dateFilter.From = DateTime.Now.AddMonths(-1).SetToBeginOfDay();
            Busy = new BusyWatcher();
            dateFilter.DateChanged += DateFilterOnPropertyChanged;
            this.PropertyChanged += this.OnPropertyChanged;
        }

        #endregion

        #region Public Properties

        [Import]
        public GlobalData GlobalData { get; set; }

        [Import]
        public FilteredStatsViewModel CurrentStats { get; set; }

        //[Import]
        //public EditGameFlyoutViewModel EditGameViewModel { get; set; }

        //[Import]
        //public CurrentSessionFlyoutViewModel ArenaViewModel { get; set; }

        public IObservableCollection<DeckModel> Decks
        {
            get
            {
                return this.decks;
            }
        }

        public IObservableCollection<ServerItemModel> Servers
        {
            get
            {
                return servers;
            }
        }

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
                RefreshDecks();
                this.NotifyOfPropertyChange(() => this.FilterServer);
            }
        }

        public GameResultTotalsModel Totals
        {
            get
            {
                return totals;
            }
        }

        public IObservableCollection<Hero> Heroes
        {
            get
            {
                return heroes;
            }
        }

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

        public DateFilter DateFilter
        {
            get
            {
                return dateFilter;
            }
        }

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

        public BindableCollection<string> GameModes
        {
            get
            {
                return this.gameModes;
            }
        }

        public IObservableCollection<GameResultModel> GameResults
        {
            get
            {
                return this.gameResults;
            }
        }

        public ICollectionView GameResultsCV
        {
            get
            {
                return gameResultsCV;
            }
        }

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

        public BindableCollection<GameResultModel> SelectedGames
        {
            get
            {
                return selectedGames;
            }
        }

        #endregion

        #region Properties

        [Import]
        protected Lazy<IShell> Shell { get; set; }

        #endregion

        #region Public Methods and Operators

        public async Task DeleteSelectedGames()
        {
            if (SelectedGames.Count == 0) return;

            if (MessageBox.Show(string.Format("Delete {0} games?", SelectedGames.Count), "Delete games?", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;

            var selectedGames = SelectedGames.ToList();
            foreach (var game in selectedGames)
            {
                await gameManager.DeleteGame(game.Id);
            }
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(GameResultAdded message)
        {
            this.gameResults.Insert(0, message.GameResult);
            this.SelectedGame = message.GameResult;
            RefreshStats();
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
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
                            var newgame = gameRepository.FirstOrDefault(x => x.Id == message.GameResultId);
                            hasGame.MapFrom(newgame);
                            RefreshStats();
                        }
                    }
                });
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(GameResultDeleted message)
        {
            var index = this.gameResults.FindIndex(x => x.Id == message.GameId);
            if (index >= 0)
            {
                this.gameResults.RemoveAt(index);

                if (SelectedGame != null && SelectedGame.Id == message.GameId)
                {
                    if (index > 0)
                    {
                        SelectedGame = this.GameResults[index - 1];
                    }
                    if (index == 0 && this.GameResults.Count > 0)
                    {
                        SelectedGame = this.GameResults[0];
                    }
                }

                RefreshStats();
            }
            //var hasGame = this.gameResults.Any(x => x.Id == message.GameId);
            //if (hasGame)
            //{
            //    RefreshData();
            //    RefreshStats();
            //}
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(SelectedGameChanged message)
        {
            if (message.Source == this)
            {
                return;
            }

            if (message.Game == null)
            {
                SelectedGame = null;
                return;
            }

            if (SelectedGame != null && message.Game.Id == SelectedGame.Id)
            {
                return;
            }

            var hasGame = this.gameResults.FirstOrDefault(x => x.Id == message.Game.Id);
            if (hasGame != null)
            {
                this.SelectedGame = hasGame;
            }
            //Execute.OnUIThread(
            //    () =>
            //    {
            //        using (PauseNotify.For(this))
            //        {
            //        }
            //    });
        }

        public void RefreshData()
        {
            needRefresh = true;
            LoadMore(true);
            RefreshStats();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Called the first time the page's LayoutUpdated event fires after it is navigated to.
        /// </summary>
        /// <param name="view"/>
        protected override void OnViewReady(object view)
        {
        }

        /// <summary>
        /// Called when initializing.
        /// </summary>
        protected override async void OnInitialize()
        {
        }

        protected override void OnActivate()
        {
            Tracker.TrackPageViewAsync("Games", "Games");
        }

        /// <summary>
        /// Called when an attached view's Loaded event fires.
        /// </summary>
        /// <param name="view"/>
        protected override void OnViewLoaded(object view)
        {
            if (!firstTimeLoaded)
            {
                firstTimeLoaded = true;
                EnsureInitialized();
                RefreshData();
                IsNotifying = true;
            }
        }

        private Expression<Func<GameResult, bool>> GetFilterExpression()
        {
            var query = PredicateBuilder.True<GameResult>(); ;
            if (dateFilter.From != null)
            {
                var filterFromDate = dateFilter.From.Value.SetToBeginOfDay();
                query = query.And(x => x.Started >= filterFromDate);
            }
            if (dateFilter.To != null)
            {
                var filterToDate = dateFilter.To.Value.SetToEndOfDay();
                query = query.And(x => x.Started <= filterToDate);
            }

            if (this.FilterServer != null && !String.IsNullOrEmpty(this.FilterServer.Name))
            {
                var serverName = this.FilterServer.Name;
                query = query.And(x => x.Server == serverName);
            }

            if (!string.IsNullOrWhiteSpace(FilterGameMode))
            {
                GameMode gm;
                if (Enum.TryParse(FilterGameMode, out gm))
                {
                    query = query.And(x => x.GameMode == gm);
                }
            }

            if (FilterDeck != null && FilterDeck.Id != Guid.Empty)
            {
                query = query.And(x => x.Deck.Id == FilterDeck.Id);
            }

            if (FilterHero != null && !String.IsNullOrEmpty(FilterHero.Key))
            {
                query = query.And(x => x.Hero.Id == FilterHero.Id);
            }

            if (FilterOpponentHero != null && !String.IsNullOrEmpty(FilterOpponentHero.Key))
            {
                query = query.And(x => x.OpponentHero.Id == FilterOpponentHero.Id);
            }
            if (!String.IsNullOrEmpty(Search))
            {
                var s = Search.ToLowerInvariant().Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
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

        private void RefreshStats(Expression<Func<GameResult, bool>> filter = null)
        {
            var expr = filter ?? GetFilterExpression();
            Task.Run(() => CurrentStats.RefreshFrom(dbContext, expr));
            Task.Run(() => this.totals.Update(dbContext, expr));
        }

        public void ScrollChanged(ScrollChangedEventArgs e)
        {
            if (e.VerticalOffset >= e.ExtentHeight - (e.ViewportHeight * 2))
            {
                if (!Busy.IsBusy && gameResults.Count < totalCount)
                {
                    needRefresh = true;
                    LoadMore();
                }
            }
        }

        private void LoadMore(bool clearValues = false)
        {
            EnsureInitialized();
            Application.Current.Dispatcher.BeginInvoke((Action)(() =>
            {
                if (needRefresh)
                {
                    needRefresh = false;
                    if (loadMoreTicket != null)
                    {
                        return;
                    }
                    this.loadMoreTicket = Busy.GetTicket();
                    Task.Run(
                        async () =>
                        {
                            var oldSelected = SelectedGame;
                            if (clearValues)
                            {
                                this.gameResults.Clear();
                            }
                            var newgames = new List<GameResultModel>();
                            var latest = (await gameRepository.ToListAsync(
                                query =>
                                {
                                    query = query.Where(this.GetFilterExpression());
                                    query = AddOrderByExpression(query);
                                    return query.Skip(clearValues ? 0 : this.gameResults.Count)
                                        .Take(50);

                                })).ToModel();
                            totalCount = gameRepository.Query(x => x.Where(GetFilterExpression()).Count());

                            newgames.AddRange(latest);
                            this.gameResults.AddRange(newgames);
                            if (oldSelected != null)
                            {
                                var foundold = gameResults.FirstOrDefault(x => x.Id == oldSelected.Id);
                                if (foundold != null)
                                {
                                    SelectedGame = foundold;
                                }
                                else
                                {
                                    SelectedGame = null;
                                }
                            }
                            loadMoreTicket.Dispose();
                            loadMoreTicket = null;
                        });
                }
            }), DispatcherPriority.ContextIdle);
        }

        public void Sorting(DataGridSortingEventArgs args)
        {
            RefreshData();
        }

        private IQueryable<GameResult> AddOrderByExpression(IQueryable<GameResult> query)
        {
            foreach (var sd in gameResultsCV.SortDescriptions)
            {
                //if (sd.PropertyName == "Duration")
                //{
                //    query = query.OrderBy(sd.PropertyName);
                //}
                if (sd.Direction == ListSortDirection.Ascending)
                {
                    query = query.OrderBy(sd.PropertyName);
                }
                else
                {
                    query = query.OrderBy(String.Format("{0} descending", sd.PropertyName));
                }
            }
            return query;
        }

        public IBusyWatcher Busy { get; set; }

        private void EnsureInitialized()
        {
            if (initDataLoaded)
            {
                return;
            }
            initDataLoaded = true;

            var data = GlobalData.Get();
            heroes.IsNotifying = false;
            // empty hero for 'all'
            heroes.Add(new Hero(""));
            heroes.AddRange(data.Heroes.OrderBy(x => x.Name));
            heroes.IsNotifying = true;
            heroes.Refresh();

            RefreshDecks();
        }

        private void RefreshDecks()
        {
            if (FilterServer != null && !String.IsNullOrEmpty(FilterServer.Name))
            {
                var decks = deckManager.GetDecks(FilterServer.Name).Select(x => x.ToModel());
                this.decks.Clear();
                this.decks.Add(DeckModel.EmptyEntry);
                this.decks.AddRange(decks);
                if (this.decks.Count > 0)
                {
                    FilterDeck = this.decks.First();
                }
            }
            else
            {
                var oldSelDeck = FilterDeck;

                var decks = deckManager.GetAllDecks().Select(x => x.ToModel());
                this.decks.Clear();
                this.decks.Add(DeckModel.EmptyEntry);
                this.decks.AddRange(decks);
                if (oldSelDeck != null)
                {
                    var newSelectedDeck = this.decks.FirstOrDefault(x => x.Id == oldSelDeck.Id);
                    if (newSelectedDeck != null) FilterDeck = newSelectedDeck;
                }
            }
        }

        private void DateFilterOnPropertyChanged(object sender, EventArgs eventArgs)
        {
            RefreshData();
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (!PauseNotify.IsPaused(this))
            {
                switch (args.PropertyName)
                {
                    case "SelectedGame":
                        //if (SelectedGame != null)
                        //{
                        //    if (SelectedGame.ArenaSession == null)
                        //    {
                        //        ArenaViewModel.IsOpen = false;
                        //    }
                        //    else
                        //    {
                        //        // this is annoying
                        //        // ArenaViewModel.Load(SelectedGame.ArenaSession);
                        //    }

                        //    // EditGameViewModel.Load(SelectedGame);
                        //}
                        this.events.PublishOnUIThread(new SelectedGameChanged(this, SelectedGame));
                        break;
                    case "FilterServer":
                    case "FilterHero":
                    case "FilterOpponentHero":
                    case "FilterDeck":
                    case "FilterGameMode":
                        RefreshData();
                        break;
                }
            }
        }

        #endregion

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(RefreshAll message)
        {
            RefreshData();
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
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

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        //public void Handle(DecksUpdated message)
        //{
        //    RefreshDecks();
        //}
    }
}