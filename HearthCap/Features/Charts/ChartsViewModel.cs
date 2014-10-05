﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChartsViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The charts view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Charts
{
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

    /// <summary>
    /// The charts view model.
    /// </summary>
    [Export(typeof(ITab))]
    public class ChartsViewModel : Conductor<IChartTab>.Collection.OneActive, 
                                   ITab, 
                                   // IHandle<DecksUpdated>,
                                   IHandle<DeckUpdated>
    {
        /// <summary>
        /// The events.
        /// </summary>
        private readonly IEventAggregator events;

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
        /// The deck manager.
        /// </summary>
        private readonly IDeckManager deckManager;

        /// <summary>
        /// The date filter.
        /// </summary>
        private readonly DateFilter dateFilter = new DateFilter {
                                                         ShowAllTime = true
                                                     };

        /// <summary>
        /// The filter server.
        /// </summary>
        private ServerItemModel filterServer;

        /// <summary>
        /// The game modes.
        /// </summary>
        private readonly GameModesStringCollection gameModes = new GameModesStringCollection(true);

        /// <summary>
        /// The filter game mode.
        /// </summary>
        private string filterGameMode;

        /// <summary>
        /// The filter deck.
        /// </summary>
        private DeckModel filterDeck;

        /// <summary>
        /// The filter hero.
        /// </summary>
        private Hero filterHero;

        /// <summary>
        /// The filter opponent hero.
        /// </summary>
        private Hero filterOpponentHero;

        /// <summary>
        /// The heroes.
        /// </summary>
        private BindableCollection<Hero> heroes = new BindableCollection<Hero>();

        /// <summary>
        /// The servers.
        /// </summary>
        private IObservableCollection<ServerItemModel> servers = new BindableCollection<ServerItemModel>(BindableServerCollection.Instance);

        /// <summary>
        /// The decks.
        /// </summary>
        private readonly BindableCollection<DeckModel> decks = new BindableCollection<DeckModel>();

        /// <summary>
        /// The initialized.
        /// </summary>
        private bool initialized;

        /// <summary>
        /// The need refresh.
        /// </summary>
        private bool needRefresh;

        /// <summary>
        /// The search.
        /// </summary>
        private string search;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartsViewModel"/> class.
        /// </summary>
        /// <param name="events">
        /// The events.
        /// </param>
        /// <param name="dbContext">
        /// The db context.
        /// </param>
        /// <param name="gameRepository">
        /// The game repository.
        /// </param>
        /// <param name="arenaRepository">
        /// The arena repository.
        /// </param>
        /// <param name="deckManager">
        /// The deck manager.
        /// </param>
        /// <param name="chartTabs">
        /// The chart tabs.
        /// </param>
        [ImportingConstructor]
        public ChartsViewModel(
            IEventAggregator events, 
            Func<HearthStatsDbContext> dbContext, 
            IRepository<GameResult> gameRepository, 
            IRepository<ArenaSession> arenaRepository, 
            IDeckManager deckManager, 
            [ImportMany]IEnumerable<IChartTab> chartTabs)
        {
            this.events = events;
            this.dbContext = dbContext;
            this.gameRepository = gameRepository;
            this.arenaRepository = arenaRepository;
            this.deckManager = deckManager;
            this.Order = 4;
            this.DisplayName = "Charts";
            this.Busy = new BusyWatcher();
            this.servers.Insert(0, new ServerItemModel(string.Empty));

            this.Items.AddRange(chartTabs.OrderBy(x => x.Order));
            this.ActivateItem(this.Items.FirstOrDefault());

            this.dateFilter.From = DateTime.Now.AddMonths(-1).SetToBeginOfDay();
            this.dateFilter.DateChanged += this.DateFilterOnPropertyChanged;
            this.PropertyChanged += this.OnPropertyChanged;
        }

        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Gets or sets the global data.
        /// </summary>
        [Import]
        public GlobalData GlobalData { get; set; }

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
        /// Gets or sets the busy.
        /// </summary>
        public IBusyWatcher Busy { get; set; }

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
        /// The save to disk.
        /// </summary>
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
                var img = this.CreateScreenshot();
                if (img != null)
                {
                    var encoder = new PngBitmapEncoder();
                    encoder.Interlace = PngInterlaceOption.On;
                    using (var stream = new FileStream(filename, FileMode.Create))
                    {
                        encoder.Frames.Add(BitmapFrame.Create(img));
                        encoder.Save(stream);
                    }

                    this.events.PublishOnBackgroundThread(new SendNotification("Screenshot saved as: " + filename, 3000));
                    Clipboard.SetImage(img);
                }
            }
        }

        /// <summary>
        /// The save to clipboard.
        /// </summary>
        public void SaveToClipboard()
        {
            var img = this.CreateScreenshot();
            if (img != null)
            {
                this.events.PublishOnBackgroundThread(new SendNotification("Screenshot saved to clipboard", 3000));
                Clipboard.SetImage(img);
            }
        }

        /// <summary>
        /// The create screenshot.
        /// </summary>
        /// <returns>
        /// The <see cref="BitmapSource"/>.
        /// </returns>
        private BitmapSource CreateScreenshot()
        {
            var view = (FrameworkElement)this.GetView();
            var charts = view.FindName("ChartsContainer") as FrameworkElement;
            if (charts != null)
            {
                var gennedby = charts.FindName("GeneratedBy") as FrameworkElement;
                gennedby.Visibility = Visibility.Visible;
                charts.UpdateLayout();
                Rect bounds = VisualTreeHelper.GetDescendantBounds(charts);
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
        /// Called when activating.
        /// </summary>
        protected override void OnActivate()
        {
            Tracker.TrackPageViewAsync("Statistics (charts)", "Statistics_Charts");
        }

        /// <summary>
        /// The on view loaded.
        /// </summary>
        /// <param name="view">
        /// The view.
        /// </param>
        protected override async void OnViewLoaded(object view)
        {
            if (this.initialized)
            {
                return;
            }

            this.initialized = true;
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
                var decks = this.deckManager.GetAllDecks().Select(x => x.ToModel());
                this.decks.Clear();
                this.decks.Add(DeckModel.EmptyEntry);
                this.decks.AddRange(decks);
                if (this.decks.Count > 0)
                {
                    this.FilterDeck = this.decks.First();
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
        /// The refresh data.
        /// </summary>
        public void RefreshData()
        {
            this.needRefresh = true;
            this.RefreshDataCore();
        }

        /// <summary>
        /// The refresh data core.
        /// </summary>
        private void RefreshDataCore()
        {
            Application.Current.Dispatcher.BeginInvoke((Action)(() =>
            {
                if (this.needRefresh)
                {
                    this.needRefresh = false;
                    var ticket = this.Busy.GetTicket();
                    var expr = this.GetGamesFilterExpression();
                    var expr2 = this.GetArenasFilterExpression();
                    Task.Run(
                        () =>
                        {
                            this.ActiveItem.RefreshData(expr, expr2);
                            ticket.Dispose();
                        });
                }
            }), DispatcherPriority.ContextIdle);
        }

        /// <summary>
        /// The get games filter expression.
        /// </summary>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        private Expression<Func<GameResult, bool>> GetGamesFilterExpression()
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
        /// The get arenas filter expression.
        /// </summary>
        /// <returns>
        /// The <see cref="Expression"/>.
        /// </returns>
        private Expression<Func<ArenaSession, bool>> GetArenasFilterExpression()
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
            switch (args.PropertyName)
            {
                case "FilterServer":
                case "FilterHero":
                case "FilterOpponentHero":
                case "FilterDeck":
                case "FilterGameMode":
                case "ActiveItem":
                    this.RefreshData();
                    break;
            }
        }

        /// <summary>
        /// The capture screen.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="dpiX">
        /// The dpi x.
        /// </param>
        /// <param name="dpiY">
        /// The dpi y.
        /// </param>
        /// <returns>
        /// The <see cref="BitmapSource"/>.
        /// </returns>
        private static BitmapSource CaptureScreen(Visual target, double dpiX, double dpiY)
        {
            if (target == null)
            {
                return null;
            }

            Rect bounds = VisualTreeHelper.GetDescendantBounds(target);
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)(bounds.Width * dpiX / 96.0), 
                                                            (int)(bounds.Height * dpiY / 96.0), 
                                                            dpiX, 
                                                            dpiY, 
                                                            PixelFormats.Pbgra32);
            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext ctx = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(target);
                ctx.DrawRectangle(vb, null, new Rect(new Point(), bounds.Size));
            }

            rtb.Render(dv);
            return rtb;
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
        }

        // <summary>
        // Handles the message.
        // </summary>
        // <param name="message">The message.</param>
        // public void Handle(DecksUpdated message)
        // {
        // this.RefreshDecks();
        // }
    }
}