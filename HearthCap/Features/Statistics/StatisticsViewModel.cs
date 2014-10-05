// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatisticsViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The statistics view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Statistics
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.Composition;
    using System.Linq;

    using Caliburn.Micro;

    using HearthCap.Data;
    using HearthCap.Features.Analytics;
    using HearthCap.Features.Core;
    using HearthCap.Framework;
    using HearthCap.Shell.Tabs;

    /// <summary>
    /// The statistics view model.
    /// </summary>
    [Export(typeof(ITab))]
    public class StatisticsViewModel : Conductor<IStatsViewModel>.Collection.OneActive, ITab
    {
        /// <summary>
        /// The db context.
        /// </summary>
        private readonly Func<HearthStatsDbContext> dbContext;

        /// <summary>
        /// The game repository.
        /// </summary>
        private readonly IRepository<GameResult> gameRepository;

        /// <summary>
        /// The arena repository.
        /// </summary>
        private readonly IRepository<ArenaSession> arenaRepository;

        /// <summary>
        /// The header.
        /// </summary>
        private string header;

        /// <summary>
        /// The order.
        /// </summary>
        private int order;

        /// <summary>
        /// The filter types.
        /// </summary>
        private readonly BindableCollection<KeyValuePair<Type, string>> filterTypes = new BindableCollection<KeyValuePair<Type, string>> {
                                                                                                new KeyValuePair<Type, string>(typeof(HeroStatViewModel), "Hero vs Hero"), 
                                                                                                new KeyValuePair<Type, string>(typeof(DecksStatViewModel), "Decks vs Hero"), 
                                                                                            };

        /// <summary>
        /// The selected filter type.
        /// </summary>
        private KeyValuePair<Type, string> selectedFilterType;

        /// <summary>
        /// The initialized.
        /// </summary>
        private bool initialized;

        /// <summary>
        /// The filter game mode.
        /// </summary>
        private string filterGameMode;

        /// <summary>
        /// The game modes.
        /// </summary>
        private readonly GameModesStringCollection gameModes = new GameModesStringCollection(true);

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
        private DateFilter dateFilter = new DateFilter {
                                                ShowAllTime = true
                                            };

        /// <summary>
        /// The search.
        /// </summary>
        private string search;

        /// <summary>
        /// Initializes a new instance of the <see cref="StatisticsViewModel"/> class.
        /// </summary>
        /// <param name="dbContext">
        /// The db context.
        /// </param>
        /// <param name="gameRepository">
        /// The game repository.
        /// </param>
        /// <param name="arenaRepository">
        /// The arena repository.
        /// </param>
        /// <param name="statsViewModels">
        /// The stats view models.
        /// </param>
        [ImportingConstructor]
        public StatisticsViewModel(Func<HearthStatsDbContext> dbContext, 
            IRepository<GameResult> gameRepository, 
            IRepository<ArenaSession> arenaRepository, 
            [ImportMany]IEnumerable<IStatsViewModel> statsViewModels)
        {
            this.IsNotifying = false;
            this.dbContext = dbContext;
            this.gameRepository = gameRepository;
            this.arenaRepository = arenaRepository;
            this.DisplayName = this.Header = "Statistics";
            this.Order = 3;
            this.dateFilter.From = DateTime.Now.AddMonths(-1);
            this.dateFilter.DateChanged += this.dateFilter_PropertyChanged;
            this.SelectedFilterType = this.FilterTypes.First();

            foreach (var statsViewModel in statsViewModels)
            {
                this.Items.Add(statsViewModel);
            }

            this.servers.Insert(0, new ServerItemModel(string.Empty));
            this.Busy = new BusyWatcher();
            this.PropertyChanged += this.OnPropertyChanged;
        }

        /// <summary>
        /// The date filter_ property changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="eventArgs">
        /// The event args.
        /// </param>
        void dateFilter_PropertyChanged(object sender, EventArgs eventArgs)
        {
            this.RefreshData();
        }

        /// <summary>
        /// Gets or sets the busy.
        /// </summary>
        public IBusyWatcher Busy { get; set; }

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
        /// Gets or sets the header.
        /// </summary>
        public string Header
        {
            get
            {
                return this.header;
            }

            set
            {
                this.header = value;
                this.NotifyOfPropertyChange(() => this.Header);
            }
        }

        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        public int Order
        {
            get
            {
                return this.order;
            }

            set
            {
                if (value == this.order)
                {
                    return;
                }

                this.order = value;
                this.NotifyOfPropertyChange(() => this.Order);
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
        /// Gets the game modes.
        /// </summary>
        public GameModesStringCollection GameModes
        {
            get
            {
                return this.gameModes;
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
        /// Gets or sets the selected filter type.
        /// </summary>
        public KeyValuePair<Type, string> SelectedFilterType
        {
            get
            {
                return this.selectedFilterType;
            }

            set
            {
                if (value.Equals(this.selectedFilterType))
                {
                    return;
                }

                this.selectedFilterType = value;
                this.NotifyOfPropertyChange(() => this.SelectedFilterType);
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
        /// Gets the filter types.
        /// </summary>
        public BindableCollection<KeyValuePair<Type, string>> FilterTypes
        {
            get
            {
                return this.filterTypes;
            }
        }

        /// <summary>
        /// The refresh data.
        /// </summary>
        public void RefreshData()
        {
            if (this.ActiveItem != null)
            {
                this.ActiveItem.FromDate = this.dateFilter.From;
                this.ActiveItem.ToDate = this.dateFilter.To;
                this.ActiveItem.GameMode = this.FilterGameMode;
                this.ActiveItem.Server = this.FilterServer;
                this.ActiveItem.Search = this.Search;
                this.ActiveItem.RefreshData();
            }
        }

        /// <summary>
        /// Called when initializing.
        /// </summary>
        protected override void OnInitialize()
        {
            this.IsNotifying = true;
        }

        /// <summary>
        /// Called when activating.
        /// </summary>
        protected override void OnActivate()
        {
            Tracker.TrackPageViewAsync("Statistics", "Statistics");
        }

        /// <summary>
        /// Called the first time the page's LayoutUpdated event fires after it is navigated to.
        /// </summary>
        /// <param name="view">
        /// </param>
        protected override void OnViewReady(object view)
        {
            if (this.initialized) return;
            this.initialized = true;

            var item = this.Items.FirstOrDefault(x => x.GetType().IsAssignableFrom(this.SelectedFilterType.Key));
            if (item != null)
            {
                this.ActivateItem(item);
                this.RefreshData();
            }
        }

        /// <summary>
        /// The on property changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "SelectedFilterType":
                    var item = this.Items.FirstOrDefault(x => x.GetType().IsAssignableFrom(this.SelectedFilterType.Key));
                    if (item != null)
                    {
                        this.ActivateItem(item);
                        this.RefreshData();
                    }

                    break;
                case "FilterServer":
                case "FilterFromDate":
                case "FilterToDate":
                case "FilterGameMode":
                    this.RefreshData();
                    break;
            }
        }
    }
}