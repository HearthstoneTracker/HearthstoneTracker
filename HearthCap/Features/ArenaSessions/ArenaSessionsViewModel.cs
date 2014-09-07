namespace HearthCap.Features.ArenaSessions
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
    using HearthCap.Features.GameManager;
    using HearthCap.Features.GameManager.Events;
    using HearthCap.Features.Games;
    using HearthCap.Features.Games.EditGame;
    using HearthCap.Features.Games.Models;
    using HearthCap.Features.ArenaSessions.Statistics;
    using HearthCap.Framework;
    using HearthCap.Shell;
    using HearthCap.Shell.Events;
    using HearthCap.Shell.Tabs;
    using HearthCap.UI.Behaviors.DragDrop;
    using HearthCap.Util;

    using Action = System.Action;

    [Export(typeof(ITab))]
    [Export(typeof(ArenaSessionsViewModel))]
    public class ArenaSessionsViewModel : TabViewModel,
                                          IHandle<ArenaSessionAdded>,
                                          IHandle<ArenaSessionUpdated>,
                                          IHandle<ArenaSessionDeleted>,
                                          IHandle<GameResultDeleted>,
                                          IHandle<GameResultAdded>,
                                          IHandle<GameResultUpdated>,
                                          IHandle<SelectedGameChanged>,
                                          IHandle<RefreshAll>,
                                          IHandle<SelectedArenaSessionChanged>
    {
        #region Fields

        private readonly IRepository<ArenaSession> arenaRepository;

        private readonly IRepository<GameResult> gameRepository;

        private readonly Func<HearthStatsDbContext> dbContext;

        private readonly GameManager gameManager;

        private readonly BindableCollection<ArenaSessionModel> arenaSessions = new BindableCollection<ArenaSessionModel>();

        private readonly IEventAggregator events;

        private ICollectionView arenaSessionsCV;

        private ArenaSessionModel selectedArenaSession;

        private GameResultModel selectedGame;

        private bool firstTimeReady = true;

        private readonly BindableCollection<Hero> heroes = new BindableCollection<Hero>();

        private Hero filterHero;

        private readonly ArenaSessionTotalsModel totals;

        private bool needRefresh = true;

        private IObservableCollection<ServerItemModel> servers = new BindableCollection<ServerItemModel>(BindableServerCollection.Instance);

        private ServerItemModel filterServer;

        private DateFilter dateFilter = new DateFilter() { ShowAllTime = true };

        private int totalCount;

        private string search;

        private bool needStatsRefresh;

        private BindableCollection<GameResultModel> selectedGames = new BindableCollection<GameResultModel>();

        private BusyWatcher.BusyWatcherTicket loadMoreTicket;

        #endregion

        #region Constructors and Destructors

        [ImportingConstructor]
        public ArenaSessionsViewModel(
            IEventAggregator events,
            IRepository<ArenaSession> arenaRepository,
            IRepository<GameResult> gameRepository,
            Func<HearthStatsDbContext> dbContext,
            GameManager gameManager)
        {
            IsNotifying = false;
            this.events = events;
            this.arenaRepository = arenaRepository;
            this.gameRepository = gameRepository;
            this.dbContext = dbContext;
            this.gameManager = gameManager;
            this.Order = 2;
            this.DisplayName = "Arenas";

            this.events.Subscribe(this);
            arenaSessionsCV = CollectionViewSource.GetDefaultView(this.arenaSessions);
            arenaSessionsCV.SortDescriptions.Add(new SortDescription("StartDate", ListSortDirection.Descending));
            this.dateFilter.From = DateTime.Now.AddMonths(-1).SetToBeginOfDay();
            this.totals = new ArenaSessionTotalsModel();
            servers.Insert(0, new ServerItemModel(""));
            Busy = new BusyWatcher();
            ItemsDragDropCommand = new RelayCommand<DataGridDragDropEventArgs>(
                                   (args) => DragDropItem(args),
                                   (args) => args != null &&
                                             args.TargetObject != null &&
                                             args.DroppedObject != null &&
                                             args.Effects != System.Windows.DragDropEffects.None);
            this.PropertyChanged += this.OnPropertyChanged;
        }

        //public BindableCollection<GameResultModel> SelectedGames
        //{
        //    get
        //    {
        //        return selectedGames;
        //    }
        //}

        private void DragDropItem(DataGridDragDropEventArgs args)
        {
            var arena = args.TargetObject as ArenaSessionModel;
            var game = args.DroppedObject as GameResultModel;
            if (arena == null || game == null || Equals(game.ArenaSession, arena))
                return;


            // move game to arena
            gameManager.MoveGameToArena(game, arena);
        }

        //public async Task DeleteSelectedGames()
        //{
        //    if (SelectedGames.Count == 0) return;

        //    if (MessageBox.Show(string.Format("Delete {0} games?", SelectedGames.Count), "Delete games?", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
        //        return;

        //    var selectedGames = SelectedGames.ToList();
        //    foreach (var game in selectedGames)
        //    {
        //        await gameManager.DeleteGame(game.Id);
        //    }
        //}

        public RelayCommand<DataGridDragDropEventArgs> ItemsDragDropCommand { get; set; }

        private void DateFilterOnPropertyChanged(object sender, EventArgs eventArgs)
        {
            RefreshData();
        }

        private void RefreshStats(Expression<Func<ArenaSession, bool>> filter = null)
        {
            needStatsRefresh = true;
            var expr = filter ?? GetFilterExpression();
            Application.Current.Dispatcher.BeginInvoke(
                (Action)(() =>
                    {
                        if (needStatsRefresh)
                        {
                            needStatsRefresh = false;
                            Task.Run(() => this.totals.Update(dbContext, expr));
                            Task.Run(() => CurrentStats.RefreshFrom(dbContext, expr));
                        }
                    }), DispatcherPriority.ContextIdle);
        }

        #endregion

        #region Public Properties

        [Import]
        public FilteredStatsViewModel CurrentStats { get; set; }

        [Import]
        public EditGameFlyoutViewModel EditGameViewModel { get; set; }

        [Import]
        public CurrentSessionFlyoutViewModel ArenaViewModel { get; set; }

        [Import]
        public GlobalData GlobalData { get; set; }

        public IBusyWatcher Busy { get; set; }

        public ICollectionView ArenaSessions
        {
            get
            {
                return arenaSessionsCV;
            }
        }

        public ArenaSessionModel SelectedArenaSession
        {
            get
            {
                return this.selectedArenaSession;
            }
            set
            {
                if (Equals(value, this.selectedArenaSession))
                {
                    return;
                }
                // var tmp = SelectedGame;
                // SelectedGame = null;
                this.selectedArenaSession = value;
                this.NotifyOfPropertyChange(() => this.SelectedArenaSession);
                // SelectedGame = tmp;
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

        public IObservableCollection<ArenaSessionModel> _ArenaSessions
        {
            get
            {
                return this.arenaSessions;
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

        public DateFilter DateFilter
        {
            get
            {
                return dateFilter;
            }
        }

        #endregion

        #region Properties

        [Import]
        protected CurrentSessionFlyoutViewModel CurrentSessionFlyout { get; set; }

        [Import]
        protected EditGameFlyoutViewModel EditGameFlyout { get; set; }

        [Import]
        protected Lazy<IShell> Shell { get; set; }

        public ArenaSessionTotalsModel Totals
        {
            get
            {
                return this.totals;
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
                this.NotifyOfPropertyChange(() => this.FilterServer);
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
        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(ArenaSessionAdded message)
        {
            if (arenaSessions.Count > 0)
            {
                Execute.OnUIThread(
                    () =>
                    {
                        using (PauseNotify.For(this))
                        {
                            this.arenaSessions.Insert(0, message.ArenaSession);
                            this.SelectedArenaSession = message.ArenaSession;
                            RefreshStats();
                        }
                    });
            }
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(ArenaSessionUpdated message)
        {
            var oldSelectedGame = SelectedGame;
            var selectedArena = SelectedArenaSession;
            Execute.OnUIThread(
                () =>
                {
                    using (PauseNotify.For(this))
                    {
                        var arena = this.arenaSessions.FirstOrDefault(x => x.Id == message.ArenaSessionId);
                        var updatedArena = arenaRepository.FirstOrDefault(x => x.Id == message.ArenaSessionId);
                        if (arena != null)
                        {
                            arena.MapFrom(updatedArena);
                            this.SelectedArenaSession = selectedArena;
                            this.SelectedGame = oldSelectedGame;
                            RefreshStats();
                        }
                    }
                });
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(ArenaSessionDeleted message)
        {
            Execute.OnUIThread(
                () =>
                {
                    using (PauseNotify.For(this))
                    {
                        var index = this._ArenaSessions.FindIndex(x => x.Id == message.ArenaSession.Id);
                        if (index >= 0)
                        {
                            this._ArenaSessions.RemoveAt(index);
                            if (SelectedArenaSession != null && SelectedArenaSession.Id == message.ArenaSession.Id)
                            {
                                if (index > 0)
                                {
                                    SelectedArenaSession = this._ArenaSessions[index - 1];
                                }
                                if (index == 0 && this._ArenaSessions.Count > 0)
                                {
                                    SelectedArenaSession = this._ArenaSessions[0];
                                }
                            }
                            RefreshStats();
                        }
                    }
                });
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
                //if (!ArenaViewModel.IsOpen && SelectedGame != null)
                //{
                //    SelectedArenaSession = SelectedGame.ArenaSession;
                //    if (IsActive)
                //    {
                //        ArenaViewModel.Load(SelectedArenaSession);
                //    }
                //}
                SelectedGame = null;
                if (!ArenaViewModel.IsOpen)
                {
                    SelectedArenaSession = null;
                }
                return;
            }

            bool found = false;
            foreach (var arena in arenaSessions)
            {
                foreach (var game in arena.Games)
                {
                    if (game.Id == message.Game.Id)
                    {
                        SelectedArenaSession = arena;
                        SelectedGame = game;
                        found = true;
                        break;
                    }
                }
                if (found)
                {
                    break;
                }
            }
            //Execute.OnUIThread(
            //    () =>
            //    {
            //        using (PauseNotify.For(this))
            //        {
            //        }
            //    });
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(SelectedArenaSessionChanged message)
        {
            if (message.Source == this)
            {
                return;
            }

            Execute.OnUIThread(
                () =>
                {
                    using (PauseNotify.For(this))
                    {
                        var selected = this.arenaSessions.FirstOrDefault(x => x.Id == message.Id);
                        this.SelectedArenaSession = selected;
                    }
                });
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(GameResultDeleted message)
        {
            if (message.ArenaId == null) return;

            if (message.ArenaId != null)
            {
                Execute.OnUIThread(
                    () =>
                    {
                        using (PauseNotify.For(this))
                        {
                            int index;
                            var game = GetGameResult(message.ArenaId.Value, message.GameId, out index);
                            if (game == null) return;
                            SelectedArenaSession = game.ArenaSession;
                            if (index >= 0)
                            {
                                game.ArenaSession.Games.Remove(game);

                                //if (SelectedGame != null &&
                                //    SelectedGame.Id == game.Id)
                                //{
                                //    if (index > 0)
                                //    {
                                //        SelectedGame = game.ArenaSession.Games[index - 1];
                                //    }
                                //    if (index == 0 && game.ArenaSession.Games.Count > 0)
                                //    {
                                //        SelectedGame = game.ArenaSession.Games[0];
                                //    }
                                //}
                            }
                            RefreshStats();
                        }
                    });
            }
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(GameResultAdded message)
        {
            if (message.GameResult.ArenaSessionId == null) return;

            var arena = this.arenaSessions.FirstOrDefault(x => x.Id == message.GameResult.ArenaSessionId);
            if (arena != null)
            {
                Execute.OnUIThread(
                    () =>
                    {
                        using (PauseNotify.For(this))
                        {
                            var game = arena.Games.FirstOrDefault(x => x.Id == message.GameResult.Id);
                            if (game == null)
                            {
                                arena.Games.Insert(0, message.GameResult);
                                RefreshStats();
                            }
                            // arena.MapFrom(message.GameResult.ArenaSession);
                            SelectedGame = arena.Games.FirstOrDefault(x => x.Id == message.GameResult.Id);
                        }
                    });
            }
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(GameResultUpdated message)
        {
            if (message.ArenaSessionId == null) return;
            var selected = SelectedGame;
            int index;
            var game = GetGameResult(message.ArenaSessionId.Value, message.GameResultId, out index);
            if (game != null)
            {
                Execute.OnUIThread(
                    () =>
                    {
                        using (PauseNotify.For(this))
                        {
                            var newgame = gameRepository.FirstOrDefault(x => x.Id == message.GameResultId);
                            game.MapFrom(newgame);
                            SelectedGame = selected;
                            RefreshStats();
                        }
                    });
            }
        }

        private GameResultModel GetGameResult(Guid arenaId, Guid gameId, out int index)
        {
            index = -1;
            GameResultModel found = null;
            foreach (var arena in arenaSessions)
            {
                index++;
                if (arena.Id != arenaId)
                    continue;
                found = arena.Games.FirstOrDefault(x => x.Id == gameId);
                if (found != null)
                    break;
            }
            return found;
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
        /// Called when initializing.
        /// </summary>
        protected override async void OnInitialize()
        {
            this.dateFilter.DateChanged += DateFilterOnPropertyChanged;
        }

        /// <summary>
        /// Called when activating.
        /// </summary>
        protected override void OnActivate()
        {
            Tracker.TrackPageViewAsync("Arenas", "Arenas");
        }

        /// <summary>
        /// Called when an attached view's Loaded event fires.
        /// </summary>
        /// <param name="view"/>
        protected override async void OnViewLoaded(object view)
        {
            IsNotifying = true;
            if (firstTimeReady)
            {
                firstTimeReady = false;
                await this.LoadInitialData();
                RefreshData();
            }
        }

        /// <summary>
        /// Called the first time the page's LayoutUpdated event fires after it is navigated to.
        /// </summary>
        /// <param name="view"/>
        protected override async void OnViewReady(object view)
        {

        }

        private async Task LoadInitialData()
        {
            var data = await GlobalData.GetAsync();
            heroes.IsNotifying = false;
            // empty hero for 'all'
            heroes.Add(new Hero(""));
            heroes.AddRange(data.Heroes.OrderBy(x => x.Name));
            heroes.IsNotifying = true;
            heroes.Refresh();
        }

        public void ScrollChanged(ScrollChangedEventArgs e)
        {
            if (e.VerticalOffset >= e.ExtentHeight - (e.ViewportHeight * 2))
            {
                if (!Busy.IsBusy && arenaSessions.Count < totalCount)
                {
                    needRefresh = true;
                    LoadMore();
                }
            }
        }

        private async Task LoadMore(bool clearValues = false)
        {
            await Application.Current.Dispatcher.BeginInvoke(
                (Action)(() =>
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
                                    if (clearValues)
                                    {
                                        this.arenaSessions.Clear();
                                    }
                                    var newarenas = new List<ArenaSessionModel>();
                                    var result = await arenaRepository.ToListAsync(
                                        query =>
                                        {
                                            query = query.Where(GetFilterExpression());
                                            query = AddOrderByExpression(query);
                                            return query.Skip(clearValues ? 0 : this.arenaSessions.Count)
                                                .Take(50);
                                        });
                                    totalCount = arenaRepository.Query(x => x.Where(GetFilterExpression()).Count());

                                    foreach (var arena in result.ToModel())
                                    {
                                        newarenas.Add(arena);
                                        if (ArenaViewModel.IsOpen &&
                                            ArenaViewModel.SelectedArenaSession != null &&
                                            SelectedArenaSession != null &&
                                            SelectedArenaSession.Id == arena.Id)
                                        {
                                            this.SelectedArenaSession = arena;
                                        }
                                        if (SelectedGame != null &&
                                            EditGameViewModel.IsOpen &&
                                                EditGameViewModel.SelectedGame != null &&
                                                SelectedGame != null)
                                        {
                                            var hasgame = arena.Games.FirstOrDefault(x => x.Id == SelectedGame.Id);
                                            if (hasgame != null)
                                            {
                                                SelectedGame = hasgame;
                                            }
                                        }
                                    }

                                    this.arenaSessions.AddRange(newarenas);
                                    loadMoreTicket.Dispose();
                                    loadMoreTicket = null;

                                    // does not work nicely
                                    //if (EditGameFlyout.IsOpen && EditGameFlyout.SelectedGame != null)
                                    //{
                                    //    Handle(new SelectedGameChanged(EditGameFlyout, EditGameFlyout.SelectedGame.Id));
                                    //}
                                    //else
                                    //{
                                    //    if (ArenaViewModel.IsOpen && ArenaViewModel.SelectedArenaSession != null)
                                    //    {
                                    //        var selected = this.arenaSessions.FirstOrDefault(x => x.Id == ArenaViewModel.SelectedArenaSession.Id);
                                    //        this.SelectedArenaSession = selected;
                                    //    }                                        
                                    //}
                                });
                        }
                    }),
                DispatcherPriority.ContextIdle);
        }

        public void Sorting(DataGridSortingEventArgs args)
        {
            RefreshData();
        }

        private IQueryable<ArenaSession> AddOrderByExpression(IQueryable<ArenaSession> query)
        {
            foreach (var sd in arenaSessionsCV.SortDescriptions)
            {
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

        private Expression<Func<ArenaSession, bool>> GetFilterExpression()
        {
            var query = PredicateBuilder.True<ArenaSession>(); ;
            if (dateFilter.From != null)
            {
                var filterFromDate = dateFilter.From.Value.SetToBeginOfDay();
                query = query.And(x => x.StartDate >= filterFromDate);
            }
            if (dateFilter.To != null)
            {
                var filterToDate = dateFilter.To.Value.SetToEndOfDay();
                query = query.And(x => x.EndDate <= filterToDate);
            }

            if (this.FilterServer != null && !String.IsNullOrEmpty(this.FilterServer.Name))
            {
                var serverName = this.FilterServer.Name;
                query = query.And(x => x.Server == serverName);
            }

            if (FilterHero != null && !String.IsNullOrEmpty(FilterHero.Key))
            {
                query = query.And(x => x.Hero.Id == FilterHero.Id);
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
                        x.Games.Any(g => g.OpponentHero.ClassName.ToLower().Contains(keyword1)) ||
                        x.Games.Any(g => g.Notes.ToLower().Contains(keyword1)));
                }
            }

            return query;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            bool not = IsNotifying;

            if (!PauseNotify.IsPaused(this))
            {
                switch (args.PropertyName)
                {
                    case "SelectedGame":
                        if (SelectedGame != null)
                        {
                            EditGameViewModel.Load(SelectedGame);
                        }
                        break;
                    case "SelectedArenaSession":
                        if (SelectedArenaSession != null)
                        {
                            ArenaViewModel.Load(SelectedArenaSession);
                            EditGameViewModel.IsOpen = false;
                        }
                        else
                        {
                            ArenaViewModel.IsOpen = false;
                        }
                        break;
                    case "FilterServer":
                    case "FilterHero":
                    case "FilterFromDate":
                    case "FilterToDate":
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
    }
}