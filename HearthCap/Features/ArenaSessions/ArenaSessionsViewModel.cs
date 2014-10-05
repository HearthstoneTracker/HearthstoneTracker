// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArenaSessionsViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The arena sessions view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

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
    using HearthCap.Features.ArenaSessions.Statistics;
    using HearthCap.Features.Core;
    using HearthCap.Features.GameManager;
    using HearthCap.Features.GameManager.Events;
    using HearthCap.Features.Games;
    using HearthCap.Features.Games.EditGame;
    using HearthCap.Features.Games.Models;
    using HearthCap.Framework;
    using HearthCap.Shell;
    using HearthCap.Shell.Events;
    using HearthCap.Shell.Tabs;
    using HearthCap.UI.Behaviors.DragDrop;
    using HearthCap.Util;

    using Action = System.Action;

    /// <summary>
    /// The arena sessions view model.
    /// </summary>
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

        /// <summary>
        /// The arena repository.
        /// </summary>
        private readonly IRepository<ArenaSession> arenaRepository;

        /// <summary>
        /// The game repository.
        /// </summary>
        private readonly IRepository<GameResult> gameRepository;

        /// <summary>
        /// The db context.
        /// </summary>
        private readonly Func<HearthStatsDbContext> dbContext;

        /// <summary>
        /// The game manager.
        /// </summary>
        private readonly GameManager gameManager;

        /// <summary>
        /// The arena sessions.
        /// </summary>
        private readonly BindableCollection<ArenaSessionModel> arenaSessions = new BindableCollection<ArenaSessionModel>();

        /// <summary>
        /// The events.
        /// </summary>
        private readonly IEventAggregator events;

        /// <summary>
        /// The arena sessions cv.
        /// </summary>
        private ICollectionView arenaSessionsCV;

        /// <summary>
        /// The selected arena session.
        /// </summary>
        private ArenaSessionModel selectedArenaSession;

        /// <summary>
        /// The selected game.
        /// </summary>
        private GameResultModel selectedGame;

        /// <summary>
        /// The first time ready.
        /// </summary>
        private bool firstTimeReady = true;

        /// <summary>
        /// The heroes.
        /// </summary>
        private readonly BindableCollection<Hero> heroes = new BindableCollection<Hero>();

        /// <summary>
        /// The filter hero.
        /// </summary>
        private Hero filterHero;

        /// <summary>
        /// The totals.
        /// </summary>
        private readonly ArenaSessionTotalsModel totals;

        /// <summary>
        /// The need refresh.
        /// </summary>
        private bool needRefresh = true;

        /// <summary>
        /// The servers.
        /// </summary>
        private IObservableCollection<ServerItemModel> servers = new BindableCollection<ServerItemModel>(BindableServerCollection.Instance);

        /// <summary>
        /// The filter server.
        /// </summary>
        private ServerItemModel filterServer;

        /// <summary>
        /// The date filter.
        /// </summary>
        private DateFilter dateFilter = new DateFilter { ShowAllTime = true };

        /// <summary>
        /// The total count.
        /// </summary>
        private int totalCount;

        /// <summary>
        /// The search.
        /// </summary>
        private string search;

        /// <summary>
        /// The need stats refresh.
        /// </summary>
        private bool needStatsRefresh;

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
        /// Initializes a new instance of the <see cref="ArenaSessionsViewModel"/> class.
        /// </summary>
        /// <param name="events">
        /// The events.
        /// </param>
        /// <param name="arenaRepository">
        /// The arena repository.
        /// </param>
        /// <param name="gameRepository">
        /// The game repository.
        /// </param>
        /// <param name="dbContext">
        /// The db context.
        /// </param>
        /// <param name="gameManager">
        /// The game manager.
        /// </param>
        [ImportingConstructor]
        public ArenaSessionsViewModel(
            IEventAggregator events, 
            IRepository<ArenaSession> arenaRepository, 
            IRepository<GameResult> gameRepository, 
            Func<HearthStatsDbContext> dbContext, 
            GameManager gameManager)
        {
            this.IsNotifying = false;
            this.events = events;
            this.arenaRepository = arenaRepository;
            this.gameRepository = gameRepository;
            this.dbContext = dbContext;
            this.gameManager = gameManager;
            this.Order = 2;
            this.DisplayName = "Arenas";

            this.events.Subscribe(this);
            this.arenaSessionsCV = CollectionViewSource.GetDefaultView(this.arenaSessions);
            this.arenaSessionsCV.SortDescriptions.Add(new SortDescription("StartDate", ListSortDirection.Descending));
            this.dateFilter.From = DateTime.Now.AddMonths(-1).SetToBeginOfDay();
            this.totals = new ArenaSessionTotalsModel();
            this.servers.Insert(0, new ServerItemModel(string.Empty));
            this.Busy = new BusyWatcher();
            this.ItemsDragDropCommand = new RelayCommand<DataGridDragDropEventArgs>(
                                   args => this.DragDropItem(args), 
                                   args => args != null &&
                                             args.TargetObject != null &&
                                             args.DroppedObject != null &&
                                             args.Effects != DragDropEffects.None);
            this.PropertyChanged += this.OnPropertyChanged;
        }

        // public BindableCollection<GameResultModel> SelectedGames
        // {
        // get
        // {
        // return selectedGames;
        // }
        // }

        /// <summary>
        /// The drag drop item.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        private void DragDropItem(DataGridDragDropEventArgs args)
        {
            var arena = args.TargetObject as ArenaSessionModel;
            var game = args.DroppedObject as GameResultModel;
            if (arena == null || game == null || Equals(game.ArenaSession, arena))
                return;


            // move game to arena
            this.gameManager.MoveGameToArena(game, arena);
        }

        // public async Task DeleteSelectedGames()
        // {
        // if (SelectedGames.Count == 0) return;

        // if (MessageBox.Show(string.Format("Delete {0} games?", SelectedGames.Count), "Delete games?", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
        // return;

        // var selectedGames = SelectedGames.ToList();
        // foreach (var game in selectedGames)
        // {
        // await gameManager.DeleteGame(game.Id);
        // }
        // }

        /// <summary>
        /// Gets or sets the items drag drop command.
        /// </summary>
        public RelayCommand<DataGridDragDropEventArgs> ItemsDragDropCommand { get; set; }

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
        /// The refresh stats.
        /// </summary>
        /// <param name="filter">
        /// The filter.
        /// </param>
        private void RefreshStats(Expression<Func<ArenaSession, bool>> filter = null)
        {
            this.needStatsRefresh = true;
            var expr = filter ?? this.GetFilterExpression();
            Application.Current.Dispatcher.BeginInvoke(
                (Action)(() =>
                    {
                        if (this.needStatsRefresh)
                        {
                            this.needStatsRefresh = false;
                            Task.Run(() => this.totals.Update(this.dbContext, expr));
                            Task.Run(() => this.CurrentStats.RefreshFrom(this.dbContext, expr));
                        }
                    }), DispatcherPriority.ContextIdle);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the current stats.
        /// </summary>
        [Import]
        public FilteredStatsViewModel CurrentStats { get; set; }

        /// <summary>
        /// Gets or sets the edit game view model.
        /// </summary>
        [Import]
        public EditGameFlyoutViewModel EditGameViewModel { get; set; }

        /// <summary>
        /// Gets or sets the arena view model.
        /// </summary>
        [Import]
        public CurrentSessionFlyoutViewModel ArenaViewModel { get; set; }

        /// <summary>
        /// Gets or sets the global data.
        /// </summary>
        [Import]
        public GlobalData GlobalData { get; set; }

        /// <summary>
        /// Gets or sets the busy.
        /// </summary>
        public IBusyWatcher Busy { get; set; }

        /// <summary>
        /// Gets the arena sessions.
        /// </summary>
        public ICollectionView ArenaSessions
        {
            get
            {
                return this.arenaSessionsCV;
            }
        }

        /// <summary>
        /// Gets or sets the selected arena session.
        /// </summary>
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
        /// Gets the _ arena sessions.
        /// </summary>
        public IObservableCollection<ArenaSessionModel> _ArenaSessions
        {
            get
            {
                return this.arenaSessions;
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
        /// Gets the date filter.
        /// </summary>
        public DateFilter DateFilter
        {
            get
            {
                return this.dateFilter;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the current session flyout.
        /// </summary>
        [Import]
        protected CurrentSessionFlyoutViewModel CurrentSessionFlyout { get; set; }

        /// <summary>
        /// Gets or sets the edit game flyout.
        /// </summary>
        [Import]
        protected EditGameFlyoutViewModel EditGameFlyout { get; set; }

        /// <summary>
        /// Gets or sets the shell.
        /// </summary>
        [Import]
        protected Lazy<IShell> Shell { get; set; }

        /// <summary>
        /// Gets the totals.
        /// </summary>
        public ArenaSessionTotalsModel Totals
        {
            get
            {
                return this.totals;
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
                this.NotifyOfPropertyChange(() => this.FilterServer);
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
        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Handle(ArenaSessionAdded message)
        {
            if (this.arenaSessions.Count > 0)
            {
                Execute.OnUIThread(
                    () =>
                    {
                        using (PauseNotify.For(this))
                        {
                            this.arenaSessions.Insert(0, message.ArenaSession);
                            this.SelectedArenaSession = message.ArenaSession;
                            this.RefreshStats();
                        }
                    });
            }
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Handle(ArenaSessionUpdated message)
        {
            var oldSelectedGame = this.SelectedGame;
            var selectedArena = this.SelectedArenaSession;
            Execute.OnUIThread(
                () =>
                {
                    using (PauseNotify.For(this))
                    {
                        var arena = this.arenaSessions.FirstOrDefault(x => x.Id == message.ArenaSessionId);
                        var updatedArena = this.arenaRepository.FirstOrDefault(x => x.Id == message.ArenaSessionId);
                        if (arena != null)
                        {
                            arena.MapFrom(updatedArena);
                            this.SelectedArenaSession = selectedArena;
                            this.SelectedGame = oldSelectedGame;
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
                            if (this.SelectedArenaSession != null && this.SelectedArenaSession.Id == message.ArenaSession.Id)
                            {
                                if (index > 0)
                                {
                                    this.SelectedArenaSession = this._ArenaSessions[index - 1];
                                }

                                if (index == 0 && this._ArenaSessions.Count > 0)
                                {
                                    this.SelectedArenaSession = this._ArenaSessions[0];
                                }
                            }

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
        public void Handle(SelectedGameChanged message)
        {
            if (message.Source == this)
            {
                return;
            }

            if (message.Game == null)
            {
                // if (!ArenaViewModel.IsOpen && SelectedGame != null)
                // {
                // SelectedArenaSession = SelectedGame.ArenaSession;
                // if (IsActive)
                // {
                // ArenaViewModel.Load(SelectedArenaSession);
                // }
                // }
                this.SelectedGame = null;
                if (!this.ArenaViewModel.IsOpen)
                {
                    this.SelectedArenaSession = null;
                }

                return;
            }

            bool found = false;
            foreach (var arena in this.arenaSessions)
            {
                foreach (var game in arena.Games)
                {
                    if (game.Id == message.Game.Id)
                    {
                        this.SelectedArenaSession = arena;
                        this.SelectedGame = game;
                        found = true;
                        break;
                    }
                }

                if (found)
                {
                    break;
                }
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
        /// Handles the message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
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
        /// <param name="message">
        /// The message.
        /// </param>
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
                            var game = this.GetGameResult(message.ArenaId.Value, message.GameId, out index);
                            if (game == null) return;
                            this.SelectedArenaSession = game.ArenaSession;
                            if (index >= 0)
                            {
                                game.ArenaSession.Games.Remove(game);

                                // if (SelectedGame != null &&
                                // SelectedGame.Id == game.Id)
                                // {
                                // if (index > 0)
                                // {
                                // SelectedGame = game.ArenaSession.Games[index - 1];
                                // }
                                // if (index == 0 && game.ArenaSession.Games.Count > 0)
                                // {
                                // SelectedGame = game.ArenaSession.Games[0];
                                // }
                                // }
                            }

                            this.RefreshStats();
                        }
                    });
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
                                this.RefreshStats();
                            }

                            // arena.MapFrom(message.GameResult.ArenaSession);
                            this.SelectedGame = arena.Games.FirstOrDefault(x => x.Id == message.GameResult.Id);
                        }
                    });
            }
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Handle(GameResultUpdated message)
        {
            if (message.ArenaSessionId == null) return;
            var selected = this.SelectedGame;
            int index;
            var game = this.GetGameResult(message.ArenaSessionId.Value, message.GameResultId, out index);
            if (game != null)
            {
                Execute.OnUIThread(
                    () =>
                    {
                        using (PauseNotify.For(this))
                        {
                            var newgame = this.gameRepository.FirstOrDefault(x => x.Id == message.GameResultId);
                            game.MapFrom(newgame);
                            this.SelectedGame = selected;
                            this.RefreshStats();
                        }
                    });
            }
        }

        /// <summary>
        /// The get game result.
        /// </summary>
        /// <param name="arenaId">
        /// The arena id.
        /// </param>
        /// <param name="gameId">
        /// The game id.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="GameResultModel"/>.
        /// </returns>
        private GameResultModel GetGameResult(Guid arenaId, Guid gameId, out int index)
        {
            index = -1;
            GameResultModel found = null;
            foreach (var arena in this.arenaSessions)
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
        /// Called when initializing.
        /// </summary>
        protected override async void OnInitialize()
        {
            this.dateFilter.DateChanged += this.DateFilterOnPropertyChanged;
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
        /// <param name="view">
        /// </param>
        protected override async void OnViewLoaded(object view)
        {
            this.IsNotifying = true;
            if (this.firstTimeReady)
            {
                this.firstTimeReady = false;
                await this.LoadInitialData();
                this.RefreshData();
            }
        }

        /// <summary>
        /// Called the first time the page's LayoutUpdated event fires after it is navigated to.
        /// </summary>
        /// <param name="view">
        /// </param>
        protected override async void OnViewReady(object view)
        {

        }

        /// <summary>
        /// The load initial data.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task LoadInitialData()
        {
            var data = await this.GlobalData.GetAsync();
            this.heroes.IsNotifying = false;

            // empty hero for 'all'
            this.heroes.Add(new Hero(string.Empty));
            this.heroes.AddRange(data.Heroes.OrderBy(x => x.Name));
            this.heroes.IsNotifying = true;
            this.heroes.Refresh();
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
                if (!this.Busy.IsBusy && this.arenaSessions.Count < this.totalCount)
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
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task LoadMore(bool clearValues = false)
        {
            await Application.Current.Dispatcher.BeginInvoke(
                (Action)(() =>
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
                                    if (clearValues)
                                    {
                                        this.arenaSessions.Clear();
                                    }

                                    var newarenas = new List<ArenaSessionModel>();
                                    var result = await this.arenaRepository.ToListAsync(
                                        query =>
                                        {
                                            query = query.Where(this.GetFilterExpression());
                                            query = this.AddOrderByExpression(query);
                                            return query.Skip(clearValues ? 0 : this.arenaSessions.Count)
                                                .Take(50);
                                        });
                                    this.totalCount = this.arenaRepository.Query(x => x.Where(this.GetFilterExpression()).Count());

                                    foreach (var arena in result.ToModel())
                                    {
                                        newarenas.Add(arena);
                                        if (this.ArenaViewModel.IsOpen &&
                                            this.ArenaViewModel.SelectedArenaSession != null &&
                                            this.SelectedArenaSession != null &&
                                            this.SelectedArenaSession.Id == arena.Id)
                                        {
                                            this.SelectedArenaSession = arena;
                                        }

                                        if (this.SelectedGame != null &&
                                            this.EditGameViewModel.IsOpen &&
                                                this.EditGameViewModel.SelectedGame != null &&
                                                this.SelectedGame != null)
                                        {
                                            var hasgame = arena.Games.FirstOrDefault(x => x.Id == this.SelectedGame.Id);
                                            if (hasgame != null)
                                            {
                                                this.SelectedGame = hasgame;
                                            }
                                        }
                                    }

                                    this.arenaSessions.AddRange(newarenas);
                                    this.loadMoreTicket.Dispose();
                                    this.loadMoreTicket = null;

                                    // does not work nicely
                                    // if (EditGameFlyout.IsOpen && EditGameFlyout.SelectedGame != null)
                                    // {
                                    // Handle(new SelectedGameChanged(EditGameFlyout, EditGameFlyout.SelectedGame.Id));
                                    // }
                                    // else
                                    // {
                                    // if (ArenaViewModel.IsOpen && ArenaViewModel.SelectedArenaSession != null)
                                    // {
                                    // var selected = this.arenaSessions.FirstOrDefault(x => x.Id == ArenaViewModel.SelectedArenaSession.Id);
                                    // this.SelectedArenaSession = selected;
                                    // }                                        
                                    // }
                                });
                        }
                    }), 
                DispatcherPriority.ContextIdle);
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
        private IQueryable<ArenaSession> AddOrderByExpression(IQueryable<ArenaSession> query)
        {
            foreach (var sd in this.arenaSessionsCV.SortDescriptions)
            {
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
        /// The get filter expression.
        /// </summary>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        private Expression<Func<ArenaSession, bool>> GetFilterExpression()
        {
            var query = PredicateBuilder.True<ArenaSession>(); 
            if (this.dateFilter.From != null)
            {
                var filterFromDate = this.dateFilter.From.Value.SetToBeginOfDay();
                query = query.And(x => x.StartDate >= filterFromDate);
            }

            if (this.dateFilter.To != null)
            {
                var filterToDate = this.dateFilter.To.Value.SetToEndOfDay();
                query = query.And(x => x.EndDate <= filterToDate);
            }

            if (this.FilterServer != null && !string.IsNullOrEmpty(this.FilterServer.Name))
            {
                var serverName = this.FilterServer.Name;
                query = query.And(x => x.Server == serverName);
            }

            if (this.FilterHero != null && !string.IsNullOrEmpty(this.FilterHero.Key))
            {
                query = query.And(x => x.Hero.Id == this.FilterHero.Id);
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
                        x.Games.Any(g => g.OpponentHero.ClassName.ToLower().Contains(keyword1)) ||
                        x.Games.Any(g => g.Notes.ToLower().Contains(keyword1)));
                }
            }

            return query;
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
            bool not = this.IsNotifying;

            if (!PauseNotify.IsPaused(this))
            {
                switch (args.PropertyName)
                {
                    case "SelectedGame":
                        if (this.SelectedGame != null)
                        {
                            this.EditGameViewModel.Load(this.SelectedGame);
                        }

                        break;
                    case "SelectedArenaSession":
                        if (this.SelectedArenaSession != null)
                        {
                            this.ArenaViewModel.Load(this.SelectedArenaSession);
                            this.EditGameViewModel.IsOpen = false;
                        }
                        else
                        {
                            this.ArenaViewModel.IsOpen = false;
                        }

                        break;
                    case "FilterServer":
                    case "FilterHero":
                    case "FilterFromDate":
                    case "FilterToDate":
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
    }
}