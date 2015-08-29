using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Caliburn.Micro;
using HearthCap.Data;
using HearthCap.Features.Analytics;
using HearthCap.Features.Core;
using HearthCap.Features.Decks;
using HearthCap.Features.Decks.ModelMappers;
using HearthCap.Features.Games.Models;
using HearthCap.Framework;
using HearthCap.Shell.Notifications;
using HearthCap.Shell.Tabs;
using HearthCap.Util;
using Microsoft.WindowsAPICodePack.Dialogs;
using Action = System.Action;

namespace HearthCap.Features.Charts
{
    [Export(typeof(ITab))]
    public class ChartsViewModel : Conductor<IChartTab>.Collection.OneActive, ITab,
        // IHandle<DecksUpdated>,
        IHandle<DeckUpdated>
    {
        private readonly IEventAggregator events;

        private readonly Func<HearthStatsDbContext> dbContext;

        private readonly IRepository<GameResult> gameRepository;

        private readonly IRepository<ArenaSession> arenaRepository;

        private readonly IDeckManager deckManager;

        private readonly DateFilter dateFilter = new DateFilter
            {
                ShowAllTime = true
            };

        private ServerItemModel filterServer;

        private readonly GameModesStringCollection gameModes = new GameModesStringCollection(true);

        private string filterGameMode;

        private DeckModel filterDeck;

        private Hero filterHero;

        private Hero filterOpponentHero;

        private readonly BindableCollection<Hero> heroes = new BindableCollection<Hero>();

        private readonly IObservableCollection<ServerItemModel> servers = new BindableCollection<ServerItemModel>(BindableServerCollection.Instance);

        private readonly BindableCollection<DeckModel> decks = new BindableCollection<DeckModel>();

        private bool initialized;

        private bool needRefresh;

        private string search;

        [ImportingConstructor]
        public ChartsViewModel(
            IEventAggregator events,
            Func<HearthStatsDbContext> dbContext,
            IRepository<GameResult> gameRepository,
            IRepository<ArenaSession> arenaRepository,
            IDeckManager deckManager,
            [ImportMany] IEnumerable<IChartTab> chartTabs)
        {
            this.events = events;
            this.dbContext = dbContext;
            this.gameRepository = gameRepository;
            this.arenaRepository = arenaRepository;
            this.deckManager = deckManager;
            Order = 4;
            DisplayName = "Charts";
            Busy = new BusyWatcher();
            servers.Insert(0, new ServerItemModel(""));

            Items.AddRange(chartTabs.OrderBy(x => x.Order));
            ActivateItem(Items.FirstOrDefault());

            dateFilter.From = DateTime.Now.AddMonths(-1).SetToBeginOfDay();
            dateFilter.DateChanged += DateFilterOnPropertyChanged;
            PropertyChanged += OnPropertyChanged;
        }

        public int Order { get; set; }

        [Import]
        public GlobalData GlobalData { get; set; }

        public DateFilter DateFilter
        {
            get { return dateFilter; }
        }

        public IBusyWatcher Busy { get; set; }

        public IObservableCollection<Hero> Heroes
        {
            get { return heroes; }
        }

        public IObservableCollection<ServerItemModel> Servers
        {
            get { return servers; }
        }

        public Hero FilterHero
        {
            get { return filterHero; }
            set
            {
                if (Equals(value, filterHero))
                {
                    return;
                }
                filterHero = value;
                NotifyOfPropertyChange(() => FilterHero);
            }
        }

        public Hero FilterOpponentHero
        {
            get { return filterOpponentHero; }
            set
            {
                if (Equals(value, filterOpponentHero))
                {
                    return;
                }
                filterOpponentHero = value;
                NotifyOfPropertyChange(() => FilterOpponentHero);
            }
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
                RefreshDecks();
                NotifyOfPropertyChange(() => FilterServer);
            }
        }

        public IObservableCollection<DeckModel> Decks
        {
            get { return decks; }
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

        public DeckModel FilterDeck
        {
            get { return filterDeck; }
            set
            {
                if (Equals(value, filterDeck))
                {
                    return;
                }
                filterDeck = value;
                NotifyOfPropertyChange(() => FilterDeck);
            }
        }

        public void SaveToDisk()
        {
            var dialog = new CommonSaveFileDialog
                {
                    OverwritePrompt = true,
                    DefaultExtension = ".png",
                    DefaultFileName = "Untitled.png",
                    EnsureValidNames = true,
                    Title = "Save statistics charts",
                    AllowPropertyEditing = false,
                    RestoreDirectory = true,
                    Filters =
                        {
                            new CommonFileDialogFilter("PNG", ".png")
                        }
                };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var filename = dialog.FileName;
                var img = CreateScreenshot();
                if (img != null)
                {
                    var encoder = new PngBitmapEncoder();
                    encoder.Interlace = PngInterlaceOption.On;
                    using (var stream = new FileStream(filename, FileMode.Create))
                    {
                        encoder.Frames.Add(BitmapFrame.Create(img));
                        encoder.Save(stream);
                    }
                    events.PublishOnBackgroundThread(new SendNotification("Screenshot saved as: " + filename, 3000));
                    Clipboard.SetImage(img);
                }
            }
        }

        public void SaveToClipboard()
        {
            var img = CreateScreenshot();
            if (img != null)
            {
                events.PublishOnBackgroundThread(new SendNotification("Screenshot saved to clipboard", 3000));
                Clipboard.SetImage(img);
            }
        }

        private BitmapSource CreateScreenshot()
        {
            var view = (FrameworkElement)GetView();
            var charts = view.FindName("ChartsContainer") as FrameworkElement;
            if (charts != null)
            {
                var gennedby = charts.FindName("GeneratedBy") as FrameworkElement;
                gennedby.Visibility = Visibility.Visible;
                charts.UpdateLayout();
                var bounds = VisualTreeHelper.GetDescendantBounds(charts);
                var img = CaptureScreen(charts, 192, 192);
                var resized = new TransformedBitmap(
                    img,
                    new ScaleTransform(
                        bounds.Width / img.PixelWidth,
                        bounds.Height / img.PixelHeight,
                        0,
                        0));
                gennedby.Visibility = Visibility.Collapsed;
                charts.UpdateLayout();
                return resized;
            }
            return null;
        }

        /// <summary>
        ///     Called when activating.
        /// </summary>
        protected override void OnActivate()
        {
            Tracker.TrackPageViewAsync("Statistics (charts)", "Statistics_Charts");
        }

        protected override void OnViewLoaded(object view)
        {
            if (initialized)
            {
                return;
            }
            initialized = true;
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
            if (FilterServer != null
                && !String.IsNullOrEmpty(FilterServer.Name))
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
                var decks = deckManager.GetAllDecks().Select(x => x.ToModel());
                this.decks.Clear();
                this.decks.Add(DeckModel.EmptyEntry);
                this.decks.AddRange(decks);
                if (this.decks.Count > 0)
                {
                    FilterDeck = this.decks.First();
                }
            }
        }

        private void DateFilterOnPropertyChanged(object sender, EventArgs eventArgs)
        {
            RefreshData();
        }

        public void RefreshData()
        {
            needRefresh = true;
            RefreshDataCore();
        }

        private void RefreshDataCore()
        {
            Application.Current.Dispatcher.BeginInvoke((Action)(() =>
                {
                    if (needRefresh)
                    {
                        needRefresh = false;
                        var ticket = Busy.GetTicket();
                        var expr = GetGamesFilterExpression();
                        var expr2 = GetArenasFilterExpression();
                        Task.Run(
                            () =>
                                {
                                    ActiveItem.RefreshData(expr, expr2);
                                    ticket.Dispose();
                                });
                    }
                }), DispatcherPriority.ContextIdle);
        }

        private Expression<Func<GameResult, bool>> GetGamesFilterExpression()
        {
            var query = PredicateBuilder.True<GameResult>();
            ;
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

            if (FilterServer != null
                && !String.IsNullOrEmpty(FilterServer.Name))
            {
                var serverName = FilterServer.Name;
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

            if (FilterDeck != null
                && FilterDeck.Id != Guid.Empty)
            {
                query = query.And(x => x.Deck.Id == FilterDeck.Id);
            }

            if (FilterHero != null
                && !String.IsNullOrEmpty(FilterHero.Key))
            {
                query = query.And(x => x.Hero.Id == FilterHero.Id);
            }

            if (FilterOpponentHero != null
                && !String.IsNullOrEmpty(FilterOpponentHero.Key))
            {
                query = query.And(x => x.OpponentHero.Id == FilterOpponentHero.Id);
            }

            if (!String.IsNullOrEmpty(Search))
            {
                var s = Search.ToLowerInvariant().Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var keyword in s)
                {
                    var keyword1 = keyword;
                    query = query.And(x =>
                        x.Notes.ToLower().Contains(keyword1) ||
                        x.Hero.ClassName.ToLower().Contains(keyword1) ||
                        x.OpponentHero.ClassName.ToLower().Contains(keyword1) ||
                        x.Deck.Name.ToLower().Contains(keyword1));
                }
            }

            return query;
        }

        private Expression<Func<ArenaSession, bool>> GetArenasFilterExpression()
        {
            var query = PredicateBuilder.True<ArenaSession>();
            ;
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

            if (FilterServer != null
                && !String.IsNullOrEmpty(FilterServer.Name))
            {
                var serverName = FilterServer.Name;
                query = query.And(x => x.Server == serverName);
            }

            if (FilterHero != null
                && !String.IsNullOrEmpty(FilterHero.Key))
            {
                query = query.And(x => x.Hero.Id == FilterHero.Id);
            }

            if (!String.IsNullOrEmpty(Search))
            {
                var s = Search.ToLowerInvariant().Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var keyword in s)
                {
                    var keyword1 = keyword;
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
            switch (args.PropertyName)
            {
                case "FilterServer":
                case "FilterHero":
                case "FilterOpponentHero":
                case "FilterDeck":
                case "FilterGameMode":
                case "ActiveItem":
                    RefreshData();
                    break;
            }
        }

        private static BitmapSource CaptureScreen(Visual target, double dpiX, double dpiY)
        {
            if (target == null)
            {
                return null;
            }
            var bounds = VisualTreeHelper.GetDescendantBounds(target);
            var rtb = new RenderTargetBitmap((int)(bounds.Width * dpiX / 96.0),
                (int)(bounds.Height * dpiY / 96.0),
                dpiX,
                dpiY,
                PixelFormats.Pbgra32);
            var dv = new DrawingVisual();
            using (var ctx = dv.RenderOpen())
            {
                var vb = new VisualBrush(target);
                ctx.DrawRectangle(vb, null, new Rect(new Point(), bounds.Size));
            }
            rtb.Render(dv);
            return rtb;
        }

        /// <summary>
        ///     Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(DeckUpdated message)
        {
            if (message.Deck == null)
            {
                RefreshDecks();
                return;
            }

            var found = Decks.FirstOrDefault(x => x.Id == message.Deck.Id);
            if (found != null)
            {
                found.MapFrom(message.Deck);
            }
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        //public void Handle(DecksUpdated message)
        //{
        //    this.RefreshDecks();
        //}
    }
}
