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

namespace HearthCap.Features.Statistics
{
    [Export(typeof(ITab))]
    public class StatisticsViewModel : Conductor<IStatsViewModel>.Collection.OneActive, ITab
    {
        private readonly Func<HearthStatsDbContext> dbContext;

        private readonly IRepository<GameResult> gameRepository;

        private readonly IRepository<ArenaSession> arenaRepository;

        private string header;

        private int order;

        private readonly BindableCollection<KeyValuePair<Type, string>> filterTypes = new BindableCollection<KeyValuePair<Type, string>>
            {
                new KeyValuePair<Type, string>(typeof(HeroStatViewModel), "Hero vs Hero"),
                new KeyValuePair<Type, string>(typeof(DecksStatViewModel), "Decks vs Hero")
            };

        private KeyValuePair<Type, string> selectedFilterType;

        private bool initialized;

        private string filterGameMode;

        private readonly GameModesStringCollection gameModes = new GameModesStringCollection(true);

        private readonly IObservableCollection<ServerItemModel> servers = new BindableCollection<ServerItemModel>(BindableServerCollection.Instance);

        private ServerItemModel filterServer;

        private readonly DateFilter dateFilter = new DateFilter
            {
                ShowAllTime = true
            };

        private string search;

        [ImportingConstructor]
        public StatisticsViewModel(Func<HearthStatsDbContext> dbContext,
            IRepository<GameResult> gameRepository,
            IRepository<ArenaSession> arenaRepository,
            [ImportMany] IEnumerable<IStatsViewModel> statsViewModels)
        {
            IsNotifying = false;
            this.dbContext = dbContext;
            this.gameRepository = gameRepository;
            this.arenaRepository = arenaRepository;
            DisplayName = Header = "Statistics";
            Order = 3;
            dateFilter.From = DateTime.Now.AddMonths(-1);
            dateFilter.DateChanged += dateFilter_PropertyChanged;
            SelectedFilterType = FilterTypes.First();

            foreach (var statsViewModel in statsViewModels)
            {
                Items.Add(statsViewModel);
            }
            servers.Insert(0, new ServerItemModel(""));
            Busy = new BusyWatcher();
            PropertyChanged += OnPropertyChanged;
        }

        private void dateFilter_PropertyChanged(object sender, EventArgs eventArgs)
        {
            RefreshData();
        }

        public IBusyWatcher Busy { get; set; }

        public IObservableCollection<ServerItemModel> Servers
        {
            get { return servers; }
        }

        public ServerItemModel FilterServer
        {
            get { return filterServer; }
            set
            {
                if (Equals(value, filterServer))
                {
                    return;
                }
                filterServer = value;
                NotifyOfPropertyChange(() => FilterServer);
            }
        }

        public string Header
        {
            get { return header; }
            set
            {
                header = value;
                NotifyOfPropertyChange(() => Header);
            }
        }

        public int Order
        {
            get { return order; }
            set
            {
                if (value == order)
                {
                    return;
                }
                order = value;
                NotifyOfPropertyChange(() => Order);
            }
        }

        public DateFilter DateFilter
        {
            get { return dateFilter; }
        }

        public GameModesStringCollection GameModes
        {
            get { return gameModes; }
        }

        public string FilterGameMode
        {
            get { return filterGameMode; }
            set
            {
                if (value == filterGameMode)
                {
                    return;
                }
                filterGameMode = value;
                NotifyOfPropertyChange(() => FilterGameMode);
            }
        }

        public KeyValuePair<Type, string> SelectedFilterType
        {
            get { return selectedFilterType; }
            set
            {
                if (value.Equals(selectedFilterType))
                {
                    return;
                }
                selectedFilterType = value;
                NotifyOfPropertyChange(() => SelectedFilterType);
            }
        }

        public string Search
        {
            get { return search; }
            set
            {
                if (value == search)
                {
                    return;
                }
                search = value;
                NotifyOfPropertyChange(() => Search);
            }
        }

        public BindableCollection<KeyValuePair<Type, string>> FilterTypes
        {
            get { return filterTypes; }
        }

        public void RefreshData()
        {
            if (ActiveItem != null)
            {
                ActiveItem.FromDate = dateFilter.From;
                ActiveItem.ToDate = dateFilter.To;
                ActiveItem.GameMode = FilterGameMode;
                ActiveItem.Server = FilterServer;
                ActiveItem.Search = Search;
                ActiveItem.RefreshData();
            }
        }

        /// <summary>
        ///     Called when initializing.
        /// </summary>
        protected override void OnInitialize()
        {
            IsNotifying = true;
        }

        /// <summary>
        ///     Called when activating.
        /// </summary>
        protected override void OnActivate()
        {
            Tracker.TrackPageViewAsync("Statistics", "Statistics");
        }

        /// <summary>
        ///     Called the first time the page's LayoutUpdated event fires after it is navigated to.
        /// </summary>
        /// <param name="view" />
        protected override void OnViewReady(object view)
        {
            if (initialized)
            {
                return;
            }
            initialized = true;

            var item = Items.FirstOrDefault(x => x.GetType().IsAssignableFrom(SelectedFilterType.Key));
            if (item != null)
            {
                ActivateItem(item);
                RefreshData();
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "SelectedFilterType":
                    var item = Items.FirstOrDefault(x => x.GetType().IsAssignableFrom(SelectedFilterType.Key));
                    if (item != null)
                    {
                        ActivateItem(item);
                        RefreshData();
                    }
                    break;
                case "FilterServer":
                case "FilterFromDate":
                case "FilterToDate":
                case "FilterGameMode":
                    RefreshData();
                    break;
            }
        }
    }
}
