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
using HearthCap.Core.GameCapture.HS;
using HearthCap.Core.GameCapture.HS.Commands;
using HearthCap.Core.GameCapture.HS.Events;
using HearthCap.Data;
using HearthCap.Features.Core;
using HearthCap.Features.GameManager.Events;
using HearthCap.Features.Games;
using HearthCap.Features.Games.Models;
using HearthCap.Shell.Flyouts;
using HearthCap.Shell.Notifications;
using HearthCap.Util;
using MahApps.Metro.Controls;
using Microsoft.WindowsAPICodePack.Dialogs;
using LogManager = NLog.LogManager;

namespace HearthCap.Features.ArenaSessions
{
    [Export(typeof(IFlyout))]
    [Export(typeof(CurrentSessionFlyoutViewModel))]
    public sealed class CurrentSessionFlyoutViewModel : FlyoutViewModel,
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

        private readonly NLog.Logger Log = LogManager.GetCurrentClassLogger();

        #endregion

        #region Fields

        private readonly BindableServerCollection servers = BindableServerCollection.Instance;

        private ServerItemModel selectedServer;

        private readonly IRepository<ArenaSession> arenaRepository;

        private readonly IEventAggregator events;

        private readonly GameManager.GameManager gameManager;

        private readonly IRepository<Hero> heroRepository;

        private DateTime? ended;

        private Hero hero;

        private readonly BindableCollection<Hero> heroes = new BindableCollection<Hero>();

        private bool isEnded;

        private bool lastIsOpen;

        private ArenaSessionModel latestArenaSession;

        private int losses;

        private readonly AsyncLock newArenaLock = new AsyncLock();

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
            GameManager.GameManager gameManager)
        {
            this.events = events;
            this.arenaRepository = arenaRepository;
            this.heroRepository = heroRepository;
            this.gameManager = gameManager;
            Name = "arenasession";
            SetPosition(Position.Right);
            this.events.Subscribe(this);
            PropertyChanged += OnPropertyChanged;
            lastIsOpen = IsOpen;
        }

        #endregion

        #region Public Properties

        [Import]
        public GlobalData GlobalData { get; set; }

        public bool ShowScreenshotColumn
        {
            get { return showScreenshotColumn; }
            set
            {
                if (value.Equals(showScreenshotColumn))
                {
                    return;
                }
                showScreenshotColumn = value;
                NotifyOfPropertyChange(() => ShowScreenshotColumn);
            }
        }

        public ServerItemModel SelectedServer
        {
            get { return selectedServer; }
            set
            {
                if (Equals(value, selectedServer))
                {
                    return;
                }
                selectedServer = value;
                NotifyOfPropertyChange(() => SelectedServer);
            }
        }

        public int RewardGold
        {
            get { return rewardGold; }
            set
            {
                if (value == rewardGold)
                {
                    return;
                }
                rewardGold = value;
                NotifyOfPropertyChange(() => RewardGold);
            }
        }

        public int RewardDust
        {
            get { return rewardDust; }
            set
            {
                if (value == rewardDust)
                {
                    return;
                }
                rewardDust = value;
                NotifyOfPropertyChange(() => RewardDust);
            }
        }

        public int RewardPacks
        {
            get { return rewardPacks; }
            set
            {
                if (value == rewardPacks)
                {
                    return;
                }
                rewardPacks = value;
                NotifyOfPropertyChange(() => RewardPacks);
            }
        }

        public string Notes
        {
            get { return notes; }
            set
            {
                if (value == notes)
                {
                    return;
                }
                notes = value;
                NotifyOfPropertyChange(() => Notes);
            }
        }

        public DateTime? Ended
        {
            get { return ended; }
            set
            {
                if (value.Equals(ended))
                {
                    return;
                }
                ended = value;
                NotifyOfPropertyChange(() => Ended);
            }
        }

        public Hero Hero
        {
            get { return hero; }
            set
            {
                if (Equals(value, hero))
                {
                    return;
                }
                hero = value;
                NotifyOfPropertyChange(() => Hero);
            }
        }

        public IObservableCollection<Hero> Heroes
        {
            get { return heroes; }
        }

        public bool IsEnded
        {
            get { return isEnded; }
            set
            {
                if (value.Equals(isEnded))
                {
                    return;
                }
                isEnded = value;
                NotifyOfPropertyChange(() => IsEnded);
            }
        }

        public bool IsLatest
        {
            get
            {
                if (SelectedArenaSession == null
                    || LatestArenaSession == null)
                {
                    return false;
                }

                return Equals(SelectedArenaSession.Id, LatestArenaSession.Id);
            }
        }

        public ArenaSessionModel LatestArenaSession
        {
            get { return latestArenaSession; }
            set
            {
                if (Equals(value, latestArenaSession))
                {
                    return;
                }
                latestArenaSession = value;
                NotifyOfPropertyChange(() => LatestArenaSession);
                NotifyOfPropertyChange(() => IsLatest);
            }
        }

        public int Losses
        {
            get { return losses; }
            set
            {
                if (value == losses)
                {
                    return;
                }
                losses = value;
                NotifyOfPropertyChange(() => Losses);
            }
        }

        public bool Retired
        {
            get { return retired; }
            set
            {
                if (value.Equals(retired))
                {
                    return;
                }
                retired = value;
                NotifyOfPropertyChange(() => Retired);
            }
        }

        public ArenaSessionModel SelectedArenaSession
        {
            get { return selectedArenaSession; }
            set
            {
                if (ReferenceEquals(value, selectedArenaSession))
                {
                    return;
                }
                selectedArenaSession = value;
                //if (value != null)
                //{
                //    this.InitViewModel(value);
                //}
                NotifyOfPropertyChange(() => SelectedArenaSession);
                NotifyOfPropertyChange(() => IsLatest);
            }
        }

        public DateTime Started
        {
            get { return started; }
            set
            {
                if (value.Equals(started))
                {
                    return;
                }
                started = value;
                NotifyOfPropertyChange(() => Started);
            }
        }

        public int Wins
        {
            get { return wins; }
            set
            {
                if (value == wins)
                {
                    return;
                }
                wins = value;
                NotifyOfPropertyChange(() => Wins);
            }
        }

        public BindableServerCollection Servers
        {
            get { return servers; }
        }

        public BitmapImage DeckScreenshot1
        {
            get { return deckScreenshot1; }
            set
            {
                deckScreenshot1 = value;
                NotifyOfPropertyChange(() => DeckScreenshot1);
            }
        }

        public BitmapImage DeckScreenshot2
        {
            get { return deckScreenshot2; }
            set
            {
                deckScreenshot2 = value;
                NotifyOfPropertyChange(() => DeckScreenshot2);
            }
        }

        public bool CanTakeScreenshot
        {
            get { return canTakeScreenshot; }
            set
            {
                if (value.Equals(canTakeScreenshot))
                {
                    return;
                }
                canTakeScreenshot = value;
                NotifyOfPropertyChange(() => CanTakeScreenshot);
            }
        }

        public bool TakingScreenshot
        {
            get { return takingScreenshot; }
            set
            {
                if (value.Equals(takingScreenshot))
                {
                    return;
                }
                takingScreenshot = value;
                NotifyOfPropertyChange(() => TakingScreenshot);
            }
        }

        #endregion

        #region Public Methods and Operators

        public void AddGame()
        {
            // this.IsOpen = false;
            events.PublishOnCurrentThread(new CreateNewGame { ArenaSession = SelectedArenaSession });
        }

        public async Task Delete()
        {
            if (SelectedArenaSession == null)
            {
                return;
            }

            if (MessageBox.Show("Delete this arena?", "Delete this arena?", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                await gameManager.DeleteArenaSession(SelectedArenaSession.Id);
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
            if (SelectedArenaSession == null
                || SelectedArenaSession.Image1 == null)
            {
                return;
            }

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
                    var target = new Bitmap(image1.Width + image2.Width, Math.Max(image1.Height, image2.Height));
                    using (var g = Graphics.FromImage(target))
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
            arenaIdForScreenshot = SelectedArenaSession.Id;
            isSecondScreenshot = false;
            CanTakeScreenshot = false;
            TakingScreenshot = true;
            events.PublishOnBackgroundThread(new RequestArenaDeckScreenshot());
        }

        public void TakeSecondScreenshot()
        {
            arenaIdForScreenshot = SelectedArenaSession.Id;
            isSecondScreenshot = true;
            CanTakeScreenshot = false;
            TakingScreenshot = true;
            events.PublishOnBackgroundThread(new RequestArenaDeckScreenshot());
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
        ///     Handle the message with a Task.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>
        ///     The Task that represents the operation.
        /// </returns>
        public async Task Handle(ArenaHeroDetected message)
        {
            EnsureInitialized();
            detectedHero = message.Hero;

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
        ///     Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(ArenaWinsDetected message)
        {
            detectedWins = message.Wins;
        }

        /// <summary>
        ///     Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(ArenaLossesDetected message)
        {
            detectedLosses = message.Losses;
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
                if (latestArena.IsEnded
                    && latestArena.EndDate != null)
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
                    if (detectedLosses >= 0
                        && detectedLosses < 3)
                    {
                        // last game was not a loss
                        Log.Debug("Correcting last game to be a win, because arena was ended, but losses is {0}.", detectedLosses);
                        var lastgame = latestArena.Games.OrderByDescending(x => x.Started).FirstOrDefault();
                        if (lastgame != null)
                        {
                            lastgame.Victory = true;
                            await gameManager.UpdateGame(lastgame);
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
                LatestArenaSession = latestArena;
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
                        arenaIdForScreenshot = latestArena.Id;
                        TakingScreenshot = true;
                        CanTakeScreenshot = false;
                        Task.Delay(1000).ContinueWith(t => events.PublishOnBackgroundThread(new RequestArenaDeckScreenshot()));
                    }
                }
            }
        }

        /// <summary>
        ///     Handles the message.
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
        ///     Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(ArenaSessionUpdated message)
        {
            if (SelectedArenaSession == null)
            {
                return;
            }

            if (message.ArenaSessionId == SelectedArenaSession.Id)
            {
                var updatedArena = arenaRepository.FirstOrDefault(x => x.Id == message.ArenaSessionId);
                SelectedArenaSession.MapFrom(updatedArena);
                InitViewModel(SelectedArenaSession);
            }
        }

        /// <summary>
        ///     Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(ArenaDrafting message)
        {
        }

        public void Load(ArenaSessionModel arenaSessionModel)
        {
            if (arenaSessionModel == null)
            {
                return;
            }

            SelectedArenaSession = arenaSessionModel;
            InitLatest();
            InitViewModel(SelectedArenaSession);

            IsOpen = true;

            events.PublishOnBackgroundThread(new SelectedArenaSessionChanged(this, arenaSessionModel.Id));
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
            var changeGameHeroes = false;
            if (arena.Hero != null
                && Hero.Id != arena.Hero.Id)
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
            arena.Hero = Hero;
            arena.StartDate = Started;
            arena.EndDate = Ended;
            arena.Retired = Retired;
            arena.Losses = Losses;
            arena.Wins = Wins;
            arena.RewardDust = RewardDust;
            arena.RewardGold = RewardGold;
            arena.RewardPacks = RewardPacks;
            arena.Notes = Notes;
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
            Ended = DateTime.Now;
        }

        /// <summary>The set start time.</summary>
        public void SetStartTime()
        {
            Started = DateTime.Now;
        }

        public async Task ShowCurrent()
        {
            await Task.Run(() => LoadLatest());
        }

        /// <summary>
        ///     Called when a part's imports have been satisfied and it is safe to use.
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

        private void InitViewModel(ArenaSessionModel arenaSession)
        {
            Started = arenaSession.StartDate;
            Ended = arenaSession.EndDate;
            Wins = arenaSession.Wins;
            Losses = arenaSession.Losses;
            Hero = arenaSession.Hero;
            IsEnded = arenaSession.IsEnded;
            Retired = arenaSession.Retired;
            RewardDust = arenaSession.RewardDust;
            RewardGold = arenaSession.RewardGold;
            RewardPacks = arenaSession.RewardPacks;
            Notes = arenaSession.Notes;
            SelectedServer = servers.FirstOrDefault(x => x.Name == arenaSession.Server);

            DeckScreenshot1 = null;
            DeckScreenshot2 = null;
            if (arenaSession.Image1 != null)
            {
                DeckScreenshot1 = CreateBitmapImage(arenaSession.Image1.Image);
            }

            if (arenaSession.Image2 != null)
            {
                DeckScreenshot2 = CreateBitmapImage(arenaSession.Image2.Image);
            }

            NotifyOfPropertyChange(() => IsLatest);

            if (IsLatest && !IsEnded)
            {
                Header = DisplayName = "Running Arena:";
            }
            else if (IsLatest)
            {
                Header = DisplayName = "Last Arena:";
            }
            else
            {
                Header = DisplayName = "Finished Arena:";
            }
        }

        private void EnsureInitialized()
        {
            if (initialized)
            {
                return;
            }
            initialized = true;

            var data = GlobalData.Get();
            heroes.Clear();
            heroes.AddRange(data.Heroes);
            InitLatest();
        }

        private void InitLatest()
        {
            // arena session is always the latest session in the db.
            var serverName = BindableServerCollection.Instance.DefaultName;
            var session = arenaRepository.Query(a => a.Where(x => x.Server == serverName).OrderByDescending(x => x.StartDate).FirstOrDefault().ToModel());
            LatestArenaSession = session;
        }

        /// <summary>The on property changed.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The args.</param>
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            switch (args.PropertyName)
            {
                case "IsOpen":
                    if (!IsOpen && lastIsOpen)
                    {
                        events.PublishOnBackgroundThread(new SelectedArenaSessionChanged(this, null));
                    }
                    //if (IsOpen)
                    //{
                    //    OnSelectedArenaChanged();
                    //}

                    lastIsOpen = IsOpen;
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
                        Hero = await heroRepository.FirstOrDefaultAsync(x => x.Key == detectedHero)
                    };
                arena = await gameManager.AddArenaSession(arena);

                // for web api
                events.PublishOnBackgroundThread(new ArenaSessionStarted(arena.StartDate, arena.Hero.Key, arena.Wins, arena.Losses));
                return arena;
            }
        }

        #endregion

        /// <summary>
        ///     Handles the message.
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
        ///     Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(SelectedGameChanged message)
        {
            if (message.Game != null)
            {
                if (message.Game.ArenaSession == null)
                {
                    IsOpen = false;
                }
            }
        }
    }
}
