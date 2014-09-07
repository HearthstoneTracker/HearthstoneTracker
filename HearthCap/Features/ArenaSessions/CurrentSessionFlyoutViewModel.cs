namespace HearthCap.Features.ArenaSessions
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.Composition;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Media.Imaging;

    using Caliburn.Micro;
    using Caliburn.Micro.Recipes.Filters;

    using HearthCap.Core.GameCapture;
    using HearthCap.Core.GameCapture.HS;
    using HearthCap.Core.GameCapture.HS.Commands;
    using HearthCap.Core.GameCapture.HS.Events;
    using HearthCap.Data;
    using HearthCap.Features.Core;
    using HearthCap.Features.GameManager;
    using HearthCap.Features.GameManager.Events;
    using HearthCap.Features.Games;
    using HearthCap.Features.Games.Models;
    using HearthCap.Shell.Flyouts;
    using HearthCap.Shell.Notifications;
    using HearthCap.Util;

    using MahApps.Metro.Controls;

    using Microsoft.WindowsAPICodePack.Dialogs;

    using NLog;

    [Export(typeof(IFlyout))]
    [Export(typeof(CurrentSessionFlyoutViewModel))]
    public class CurrentSessionFlyoutViewModel : FlyoutViewModel,
                                                 IPartImportsSatisfiedNotification,
                                                 IHandleWithTask<ArenaHeroDetected>,
                                                 IHandle<GameModeChanged>,
                                                 IHandle<ArenaDrafting>,
                                                 IHandle<ArenaSessionUpdated>,
                                                 IHandleWithTask<ArenaDeckScreenshotTaken>,
                                                 IHandle<ArenaWinsDetected>,
                                                 IHandle<ArenaLossesDetected>,
                                                 IHandle<SelectedGameChanged>
    {
        #region Static Fields

        private Logger Log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        #region Fields
        private readonly BindableServerCollection servers = BindableServerCollection.Instance;

        private ServerItemModel selectedServer;

        private readonly IRepository<ArenaSession> arenaRepository;

        private readonly IEventAggregator events;

        private readonly GameManager gameManager;

        private readonly IRepository<Hero> heroRepository;

        private DateTime? ended;

        private Hero hero;

        private BindableCollection<Hero> heroes = new BindableCollection<Hero>();

        private bool isEnded;

        private bool lastIsOpen;

        private ArenaSessionModel latestArenaSession;

        private int losses;

        private AsyncLock newArenaLock = new AsyncLock();

        private bool retired;

        private ArenaSessionModel selectedArenaSession;

        private DateTime started;

        private int wins;

        private bool initialized;

        private int rewardGold;

        private int rewardDust;

        private int rewardPacks;

        private string notes;

        private Guid arenaIdForScreenshot;

        private BitmapImage deckScreenshot1;

        private BitmapImage deckScreenshot2;

        private bool isSecondScreenshot;

        private bool canTakeScreenshot;

        private bool takingScreenshot;

        private bool showScreenshotColumn;

        private bool handlingArenaDetect;

        private string detectedHero;

        private int detectedWins = -1;

        private int detectedLosses = -1;

        #endregion

        #region Constructors and Destructors

        [ImportingConstructor]
        public CurrentSessionFlyoutViewModel(
            IEventAggregator events,
            IRepository<ArenaSession> arenaRepository,
            IRepository<Hero> heroRepository,
            GameManager gameManager)
        {
            this.events = events;
            this.arenaRepository = arenaRepository;
            this.heroRepository = heroRepository;
            this.gameManager = gameManager;
            this.Name = "arenasession";
            SetPosition(Position.Right);
            this.events.Subscribe(this);
            this.PropertyChanged += this.OnPropertyChanged;
            this.lastIsOpen = this.IsOpen;
        }

        #endregion

        #region Public Properties

        [Import]
        public GlobalData GlobalData { get; set; }

        public bool ShowScreenshotColumn
        {
            get
            {
                return this.showScreenshotColumn;
            }
            set
            {
                if (value.Equals(this.showScreenshotColumn))
                {
                    return;
                }
                this.showScreenshotColumn = value;
                this.NotifyOfPropertyChange(() => this.ShowScreenshotColumn);
            }
        }

        public ServerItemModel SelectedServer
        {
            get
            {
                return this.selectedServer;
            }
            set
            {
                if (Equals(value, this.selectedServer))
                {
                    return;
                }
                this.selectedServer = value;
                this.NotifyOfPropertyChange(() => this.SelectedServer);
            }
        }

        public int RewardGold
        {
            get
            {
                return this.rewardGold;
            }
            set
            {
                if (value == this.rewardGold)
                {
                    return;
                }
                this.rewardGold = value;
                this.NotifyOfPropertyChange(() => this.RewardGold);
            }
        }

        public int RewardDust
        {
            get
            {
                return this.rewardDust;
            }
            set
            {
                if (value == this.rewardDust)
                {
                    return;
                }
                this.rewardDust = value;
                this.NotifyOfPropertyChange(() => this.RewardDust);
            }
        }

        public int RewardPacks
        {
            get
            {
                return this.rewardPacks;
            }
            set
            {
                if (value == this.rewardPacks)
                {
                    return;
                }
                this.rewardPacks = value;
                this.NotifyOfPropertyChange(() => this.RewardPacks);
            }
        }

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

        public DateTime? Ended
        {
            get
            {
                return this.ended;
            }
            set
            {
                if (value.Equals(this.ended))
                {
                    return;
                }
                this.ended = value;
                this.NotifyOfPropertyChange(() => this.Ended);
            }
        }

        public Hero Hero
        {
            get
            {
                return this.hero;
            }
            set
            {
                if (Equals(value, this.hero))
                {
                    return;
                }
                this.hero = value;
                this.NotifyOfPropertyChange(() => this.Hero);
            }
        }

        public IObservableCollection<Hero> Heroes
        {
            get
            {
                return heroes;
            }
        }

        public bool IsEnded
        {
            get
            {
                return this.isEnded;
            }
            set
            {
                if (value.Equals(this.isEnded))
                {
                    return;
                }
                this.isEnded = value;
                this.NotifyOfPropertyChange(() => this.IsEnded);
            }
        }

        public bool IsLatest
        {
            get
            {
                if (SelectedArenaSession == null || LatestArenaSession == null) return false;

                return Equals(this.SelectedArenaSession.Id, this.LatestArenaSession.Id);
            }
        }

        public ArenaSessionModel LatestArenaSession
        {
            get
            {
                return this.latestArenaSession;
            }
            set
            {
                if (Equals(value, this.latestArenaSession))
                {
                    return;
                }
                this.latestArenaSession = value;
                this.NotifyOfPropertyChange(() => this.LatestArenaSession);
                this.NotifyOfPropertyChange(() => this.IsLatest);
            }
        }

        public int Losses
        {
            get
            {
                return this.losses;
            }
            set
            {
                if (value == this.losses)
                {
                    return;
                }
                this.losses = value;
                this.NotifyOfPropertyChange(() => this.Losses);
            }
        }

        public bool Retired
        {
            get
            {
                return this.retired;
            }
            set
            {
                if (value.Equals(this.retired))
                {
                    return;
                }
                this.retired = value;
                this.NotifyOfPropertyChange(() => this.Retired);
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
                if (ReferenceEquals(value, this.selectedArenaSession))
                {
                    return;
                }
                this.selectedArenaSession = value;
                //if (value != null)
                //{
                //    this.InitViewModel(value);
                //}
                this.NotifyOfPropertyChange(() => this.SelectedArenaSession);
                this.NotifyOfPropertyChange(() => this.IsLatest);
            }
        }

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

        public int Wins
        {
            get
            {
                return this.wins;
            }
            set
            {
                if (value == this.wins)
                {
                    return;
                }
                this.wins = value;
                this.NotifyOfPropertyChange(() => this.Wins);
            }
        }

        public BindableServerCollection Servers
        {
            get
            {
                return this.servers;
            }
        }

        public BitmapImage DeckScreenshot1
        {
            get
            {
                return this.deckScreenshot1;
            }
            set
            {
                this.deckScreenshot1 = value;
                this.NotifyOfPropertyChange(() => this.DeckScreenshot1);
            }
        }

        public BitmapImage DeckScreenshot2
        {
            get
            {
                return this.deckScreenshot2;
            }
            set
            {
                this.deckScreenshot2 = value;
                this.NotifyOfPropertyChange(() => this.DeckScreenshot2);
            }
        }

        public bool CanTakeScreenshot
        {
            get
            {
                return this.canTakeScreenshot;
            }
            set
            {
                if (value.Equals(this.canTakeScreenshot))
                {
                    return;
                }
                this.canTakeScreenshot = value;
                this.NotifyOfPropertyChange(() => this.CanTakeScreenshot);
            }
        }

        public bool TakingScreenshot
        {
            get
            {
                return this.takingScreenshot;
            }
            set
            {
                if (value.Equals(this.takingScreenshot))
                {
                    return;
                }
                this.takingScreenshot = value;
                this.NotifyOfPropertyChange(() => this.TakingScreenshot);
            }
        }

        #endregion

        #region Public Methods and Operators

        public void AddGame()
        {
            // this.IsOpen = false;
            events.PublishOnCurrentThread(new CreateNewGame() { ArenaSession = this.SelectedArenaSession });
        }

        public async Task Delete()
        {
            if (SelectedArenaSession == null)
            {
                return;
            }

            if (MessageBox.Show("Delete this arena?", "Delete this arena?", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                await gameManager.DeleteArenaSession(this.SelectedArenaSession.Id);
                events.PublishOnBackgroundThread(new SendNotification("Arena successfully deleted."));
                LoadLatest();
            }
        }

        public void CancelTakeScreenshot()
        {
            isSecondScreenshot = false;
            CanTakeScreenshot = true;
            TakingScreenshot = false;
        }

        public void SaveScreenshotToDisk()
        {
            if (SelectedArenaSession == null || SelectedArenaSession.Image1 == null)
                return;

            var defaultFilename = "arena.png";
            var dialog = new CommonSaveFileDialog
            {
                OverwritePrompt = true,
                DefaultExtension = ".png",
                DefaultFileName = defaultFilename,
                EnsureValidNames = true,
                Title = "Save deck screenshot",
                AllowPropertyEditing = false,
                RestoreDirectory = true,
                Filters =
                                     {
                                         new CommonFileDialogFilter("PNG", ".png")
                                     }
            };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                Bitmap image1 = null;
                Bitmap image2 = null;
                using (var ms = new MemoryStream(SelectedArenaSession.Image1.Image))
                {
                    image1 = new Bitmap(new Bitmap(ms));
                }

                if (SelectedArenaSession.Image2 != null)
                {
                    using (var ms = new MemoryStream(SelectedArenaSession.Image2.Image))
                    {
                        image2 = new Bitmap(new Bitmap(ms));
                    }
                }

                var filename = dialog.FileName;
                if (image2 == null)
                {
                    image1.Save(filename, ImageFormat.Png);
                }
                else
                {
                    Bitmap target = new Bitmap(image1.Width + image2.Width, Math.Max(image1.Height, image2.Height));
                    using (Graphics g = Graphics.FromImage(target))
                    {
                        g.DrawImage(image1, 0, 0);
                        g.DrawImage(image2, image1.Width, 0);
                    }
                    target.Save(filename, ImageFormat.Png);
                }
            }
        }

        public void TakeFirstScreenshot()
        {
            this.arenaIdForScreenshot = SelectedArenaSession.Id;
            this.isSecondScreenshot = false;
            CanTakeScreenshot = false;
            TakingScreenshot = true;
            this.events.PublishOnBackgroundThread(new RequestArenaDeckScreenshot());
        }

        public void TakeSecondScreenshot()
        {
            this.arenaIdForScreenshot = SelectedArenaSession.Id;
            this.isSecondScreenshot = true;
            CanTakeScreenshot = false;
            TakingScreenshot = true;
            this.events.PublishOnBackgroundThread(new RequestArenaDeckScreenshot());
        }

        public void ToggleScreenshot()
        {
            ShowScreenshotColumn = !ShowScreenshotColumn;
        }

        public async Task Merge()
        {
            if (MessageBox.Show(
                "Delete this arena and move all games to the previous arena?",
                "Merge arenas?",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question) != MessageBoxResult.Yes)
            {
                return;
            }

            var previousArena = arenaRepository.Query(a => a.Where(x => x.StartDate < SelectedArenaSession.StartDate).OrderByDescending(x => x.StartDate).FirstOrDefault().ToModel());
            if (previousArena == null)
            {
                MessageBox.Show("No previous arena found", "Not found", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (previousArena.Hero.Id != SelectedArenaSession.Hero.Id)
            {
                MessageBox.Show(string.Format("Cannot merge because previous arena hero is not a {0}.", SelectedArenaSession.Hero.ClassName), "Cannot merge", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            await gameManager.MergeArenas(SelectedArenaSession, previousArena);
            previousArena = arenaRepository.Query(a => a.FirstOrDefault(x => x.Id == previousArena.Id).ToModel());

            Load(previousArena);
        }
        /// <summary>
        /// Handle the message with a Task.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>
        /// The Task that represents the operation.
        /// </returns>
        public async Task Handle(ArenaHeroDetected message)
        {
            EnsureInitialized();
            this.detectedHero = message.Hero;

            if (String.IsNullOrEmpty(detectedHero))
            {
                Log.Debug("Detected hero is null or empty, ignoring.");
                return;
            }

            if (!handlingArenaDetect)
            {
                handlingArenaDetect = true;
                await Task.Delay(500).ContinueWith(async t => await HandleArenaDetect());
            }
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(ArenaWinsDetected message)
        {
            this.detectedWins = message.Wins;
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(ArenaLossesDetected message)
        {
            this.detectedLosses = message.Losses;
        }

        private async Task HandleArenaDetect()
        {
            if (!GlobalLocks.NewArenaLock.Wait(1000))
            {
                Log.Debug("Waited for NewArenaLock lock");
                GlobalLocks.NewArenaLock.Wait();
            }

            // arena session is always the latest session in the db.
            var serverName = BindableServerCollection.Instance.DefaultName;
            var latestArena = arenaRepository.Query(a => a.Where(x => x.Server == serverName).OrderByDescending(x => x.StartDate).FirstOrDefault().ToModel());

            try
            {
                if (latestArena == null)
                {
                    latestArena = await TryCreateArenaSession(detectedHero);
                    return;
                }

                // weird case
                if (latestArena.IsEnded && latestArena.EndDate != null)
                {
                    var diff = DateTime.Now.Subtract(latestArena.EndDate.Value);
                    if (diff < TimeSpan.FromMinutes(5))
                    {
                        Log.Debug("Latest arena is ended (and ended less then 5 minutes ago). Ignoring...");
                        return;
                        //Log.Debug("Latest arena is ended (and started more then 5 minutes ago). Creating new arena");
                        //latestArena = await TryCreateArenaSession(detectedHero);
                        //return;
                    }
                }

                // new arena started because new hero
                if (detectedHero != latestArena.Hero.Key)
                {
                    Log.Debug("detectedHero ({0}) != latestArena.Hero.Key ({1}), starting new arena.", detectedHero, latestArena.Hero.Key);
                    if (!latestArena.IsEnded)
                    {
                        Log.Debug("Retiring, previous arena.");
                        // retired
                        await Retire(latestArena);
                    }

                    latestArena = await TryCreateArenaSession(detectedHero);
                    return;
                }

                // check if we need to correct last game
                if (latestArena.IsEnded)
                {
                    if (detectedLosses >= 0 && detectedLosses < 3)
                    {
                        // last game was not a loss
                        Log.Debug("Correcting last game to be a win, because arena was ended, but losses is {0}.", detectedLosses);
                        var lastgame = latestArena.Games.OrderByDescending(x => x.Started).FirstOrDefault();
                        if (lastgame != null)
                        {
                            lastgame.Victory = true;
                            await gameManager.UpdateGame(lastgame);
                            return;
                        }
                    }
                }

                // doesn't work very well.
                //if ((detectedLosses >= 0 && detectedLosses < latestArena.Losses) ||
                //    (detectedWins >= 0 && detectedWins < latestArena.Wins))
                //{
                //    Log.Debug(
                //        "Detected wins/losses ({0}/{1}) smaller then last ({2}/{3}). Starting new arena.",
                //        detectedWins,
                //        detectedLosses,
                //        latestArena.Wins,
                //        latestArena.Losses);
                //    latestArena = await TryCreateArenaSession(detectedHero);
                //    return;
                //}

            }
            finally
            {
                this.LatestArenaSession = latestArena;
                detectedHero = null;
                detectedWins = -1;
                detectedLosses = -1;
                handlingArenaDetect = false;
                if (latestArena != null)
                {
                    Load(latestArena);

                    if (latestArena.Image1 == null)
                    {
                        // add screenshot async
                        this.arenaIdForScreenshot = latestArena.Id;
                        TakingScreenshot = true;
                        CanTakeScreenshot = false;
                        Task.Delay(1000).ContinueWith(t => this.events.PublishOnBackgroundThread(new RequestArenaDeckScreenshot()));
                    }
                }
            }
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(GameModeChanged message)
        {
            if (message.GameMode == GameMode.Arena)
            {
                LoadLatest();
            }

            CanTakeScreenshot = message.GameMode == GameMode.Arena;
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(ArenaSessionUpdated message)
        {
            if (this.SelectedArenaSession == null) return;

            if (message.ArenaSessionId == this.SelectedArenaSession.Id)
            {
                var updatedArena = arenaRepository.FirstOrDefault(x => x.Id == message.ArenaSessionId);
                SelectedArenaSession.MapFrom(updatedArena);
                InitViewModel(SelectedArenaSession);
            }
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(ArenaDrafting message)
        {
        }

        public void Load(ArenaSessionModel arenaSessionModel)
        {
            if (arenaSessionModel == null)
                return;

            SelectedArenaSession = arenaSessionModel;
            InitLatest();
            InitViewModel(SelectedArenaSession);
            if (IsOpen)
            {
                IsOpen = false;
            }
            IsOpen = true;
            this.events.PublishOnBackgroundThread(new SelectedArenaSessionChanged(this, arenaSessionModel.Id));
        }

        public bool CanSave()
        {
            return Hero != null && SelectedServer != null;
        }

        [Dependencies("Hero", "SelectedServer")]
        public async Task Save()
        {
            if (SelectedArenaSession == null)
            {
                return;
            }

            var arena = SelectedArenaSession;
            bool changeGameHeroes = false;
            if (arena.Hero != null && Hero.Id != arena.Hero.Id)
            {
                if (MessageBox.Show(
                    "Changing arena hero, will change the hero for all games in this arena.\nAre you sure you want to save?",
                    "Change arena hero?",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question,
                    MessageBoxResult.Yes) == MessageBoxResult.No)
                {
                    return;
                }
                changeGameHeroes = true;
            }
            arena.Hero = this.Hero;
            arena.StartDate = this.Started;
            arena.EndDate = this.Ended;
            arena.Retired = this.Retired;
            arena.Losses = this.Losses;
            arena.Wins = this.Wins;
            arena.RewardDust = this.RewardDust;
            arena.RewardGold = this.RewardGold;
            arena.RewardPacks = this.RewardPacks;
            arena.Notes = this.Notes;
            arena.Server = SelectedServer.Name;

            await gameManager.UpdateArenaSession(SelectedArenaSession);
            if (changeGameHeroes)
            {
                foreach (var game in SelectedArenaSession.Games)
                {
                    game.Hero = Hero;
                    await gameManager.UpdateGame(game);
                }
            }
            InitLatest();
            InitViewModel(SelectedArenaSession);
            events.PublishOnBackgroundThread(new SendNotification("Arena successfully saved."));
        }

        /// <summary>The set end time.</summary>
        public void SetEndTime()
        {
            this.Ended = DateTime.Now;
        }

        /// <summary>The set start time.</summary>
        public void SetStartTime()
        {
            this.Started = DateTime.Now;
        }

        public async Task ShowCurrent()
        {
            LoadLatest();
        }


        /// <summary>
        /// Called when a part's imports have been satisfied and it is safe to use.
        /// </summary>
        public void OnImportsSatisfied()
        {
            EnsureInitialized();
        }


        public void LoadLatest()
        {
            InitLatest();
            if (LatestArenaSession != null)
            {
                Load(LatestArenaSession);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Called when initializing.
        /// </summary>
        protected override async void OnInitialize()
        {
        }

        private void InitViewModel(ArenaSessionModel arenaSession)
        {
            this.Started = arenaSession.StartDate;
            this.Ended = arenaSession.EndDate;
            this.Wins = arenaSession.Wins;
            this.Losses = arenaSession.Losses;
            this.Hero = arenaSession.Hero;
            this.IsEnded = arenaSession.IsEnded;
            this.Retired = arenaSession.Retired;
            this.RewardDust = arenaSession.RewardDust;
            this.RewardGold = arenaSession.RewardGold;
            this.RewardPacks = arenaSession.RewardPacks;
            this.Notes = arenaSession.Notes;
            this.SelectedServer = servers.FirstOrDefault(x => x.Name == arenaSession.Server);

            this.DeckScreenshot1 = null;
            this.DeckScreenshot2 = null;
            if (arenaSession.Image1 != null)
            {
                this.DeckScreenshot1 = CreateBitmapImage(arenaSession.Image1.Image);
            }

            if (arenaSession.Image2 != null)
            {
                this.DeckScreenshot2 = CreateBitmapImage(arenaSession.Image2.Image);
            }

            NotifyOfPropertyChange(() => IsLatest);

            if (IsLatest && !IsEnded)
            {
                this.Header = this.DisplayName = "Running Arena:";
            }
            else if (IsLatest)
            {
                this.Header = this.DisplayName = "Last Arena:";
            }
            else
            {
                this.Header = this.DisplayName = "Finished Arena:";
            }
        }

        private void EnsureInitialized()
        {
            if (initialized) return;
            initialized = true;

            var data = this.GlobalData.Get();
            this.heroes.Clear();
            this.heroes.AddRange(data.Heroes);
            InitLatest();
        }

        private void InitLatest()
        {
            // arena session is always the latest session in the db.
            var serverName = BindableServerCollection.Instance.DefaultName;
            var session = arenaRepository.Query(a => a.Where(x => x.Server == serverName).OrderByDescending(x => x.StartDate).FirstOrDefault().ToModel());
            this.LatestArenaSession = session;
        }

        /// <summary>The on property changed.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The args.</param>
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            switch (args.PropertyName)
            {
                case "IsOpen":
                    if (!this.IsOpen && this.lastIsOpen)
                    {
                        this.events.PublishOnBackgroundThread(new SelectedArenaSessionChanged(this, null));
                    }
                    //if (IsOpen)
                    //{
                    //    OnSelectedArenaChanged();
                    //}

                    this.lastIsOpen = this.IsOpen;
                    break;
            }
        }

        //private void OnSelectedArenaChanged()
        //{
        //    if (!PauseNotify.IsPaused(this))
        //    {
        //        this.events.PublishOnBackgroundThread(new SelectedArenaSessionChanged(this, this.SelectedArenaSession != null ? this.SelectedArenaSession.Id : (Guid?)null));
        //    }
        //}

        private async Task Retire(ArenaSessionModel arenaSession)
        {
            await gameManager.Retire(arenaSession);
        }

        private async Task<ArenaSessionModel> TryCreateArenaSession(string detectedHero)
        {
            if (detectedHero == null)
            {
                Log.Warn("TryCreateArenaSession called without hero");
                return null;
            }

            using (await newArenaLock.LockAsync())
            {
                var arena = new ArenaSessionModel
                                {
                                    Hero = await heroRepository.FirstOrDefaultAsync(x => x.Key == detectedHero),
                                };
                arena = await gameManager.AddArenaSession(arena);

                // for web api
                events.PublishOnBackgroundThread(new ArenaSessionStarted(arena.StartDate, arena.Hero.Key, arena.Wins, arena.Losses));
                return arena;
            }
        }

        #endregion

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public async Task Handle(ArenaDeckScreenshotTaken message)
        {
            try
            {
                var arena = arenaRepository.Query(x => x.FirstOrDefault(a => a.Id == arenaIdForScreenshot));
                if (arena == null)
                {
                    return;
                }
                ArenaDeckImage arenaImage;
                if (!isSecondScreenshot)
                {
                    if (arena.Image1 == null)
                    {
                        arena.Image1 = new ArenaDeckImage();
                    }
                    arenaImage = arena.Image1;
                }
                else
                {
                    if (arena.Image2 == null)
                    {
                        arena.Image2 = new ArenaDeckImage();
                    }
                    arenaImage = arena.Image2;
                }

                using (var ms = new MemoryStream())
                {
                    message.Image.Save(ms, ImageFormat.Bmp);
                    ms.Position = 0;
                    arenaImage.Image = ms.ToArray();
                }

                await gameManager.UpdateArenaSession(arena.ToModel());
                if (!isSecondScreenshot)
                {
                    DeckScreenshot1 = CreateBitmapImage(message.Image);
                }
                else
                {
                    DeckScreenshot2 = CreateBitmapImage(message.Image);
                }
            }
            finally
            {
                message.Image.Dispose();
                CanTakeScreenshot = true;
                TakingScreenshot = false;
                isSecondScreenshot = false;
            }
        }

        private BitmapImage CreateBitmapImage(byte[] image)
        {
            var ms = new MemoryStream(image);
            var bi = new BitmapImage();
            bi.BeginInit();
            bi.CacheOption = BitmapCacheOption.OnLoad;
            bi.StreamSource = ms;
            bi.EndInit();
            bi.Freeze();
            ms.Dispose();
            return bi;
        }

        private BitmapImage CreateBitmapImage(Bitmap image)
        {
            var ms = new MemoryStream();
            var bi = new BitmapImage();
            bi.BeginInit();
            bi.CacheOption = BitmapCacheOption.OnLoad;
            image.Save(ms, ImageFormat.Bmp);
            ms.Seek(0, SeekOrigin.Begin);
            bi.StreamSource = ms;
            bi.EndInit();
            ms.Dispose();
            bi.Freeze();
            return bi;
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(SelectedGameChanged message)
        {

            if (message.Game != null)
            {
                if (message.Game.ArenaSession == null)
                {
                    IsOpen = false;
                    return;
                }
            }
        }
    }
}