// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CurrentSessionFlyoutViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The current session flyout view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

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

    using LogManager = NLog.LogManager;

    /// <summary>
    /// The current session flyout view model.
    /// </summary>
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

        /// <summary>
        /// The log.
        /// </summary>
        private Logger Log = LogManager.GetCurrentClassLogger();

        #endregion

        #region Fields

        /// <summary>
        /// The servers.
        /// </summary>
        private readonly BindableServerCollection servers = BindableServerCollection.Instance;

        /// <summary>
        /// The selected server.
        /// </summary>
        private ServerItemModel selectedServer;

        /// <summary>
        /// The arena repository.
        /// </summary>
        private readonly IRepository<ArenaSession> arenaRepository;

        /// <summary>
        /// The events.
        /// </summary>
        private readonly IEventAggregator events;

        /// <summary>
        /// The game manager.
        /// </summary>
        private readonly GameManager gameManager;

        /// <summary>
        /// The hero repository.
        /// </summary>
        private readonly IRepository<Hero> heroRepository;

        /// <summary>
        /// The ended.
        /// </summary>
        private DateTime? ended;

        /// <summary>
        /// The hero.
        /// </summary>
        private Hero hero;

        /// <summary>
        /// The heroes.
        /// </summary>
        private BindableCollection<Hero> heroes = new BindableCollection<Hero>();

        /// <summary>
        /// The is ended.
        /// </summary>
        private bool isEnded;

        /// <summary>
        /// The last is open.
        /// </summary>
        private bool lastIsOpen;

        /// <summary>
        /// The latest arena session.
        /// </summary>
        private ArenaSessionModel latestArenaSession;

        /// <summary>
        /// The losses.
        /// </summary>
        private int losses;

        /// <summary>
        /// The new arena lock.
        /// </summary>
        private AsyncLock newArenaLock = new AsyncLock();

        /// <summary>
        /// The retired.
        /// </summary>
        private bool retired;

        /// <summary>
        /// The selected arena session.
        /// </summary>
        private ArenaSessionModel selectedArenaSession;

        /// <summary>
        /// The started.
        /// </summary>
        private DateTime started;

        /// <summary>
        /// The wins.
        /// </summary>
        private int wins;

        /// <summary>
        /// The initialized.
        /// </summary>
        private bool initialized;

        /// <summary>
        /// The reward gold.
        /// </summary>
        private int rewardGold;

        /// <summary>
        /// The reward dust.
        /// </summary>
        private int rewardDust;

        /// <summary>
        /// The reward packs.
        /// </summary>
        private int rewardPacks;

        /// <summary>
        /// The notes.
        /// </summary>
        private string notes;

        /// <summary>
        /// The arena id for screenshot.
        /// </summary>
        private Guid arenaIdForScreenshot;

        /// <summary>
        /// The deck screenshot 1.
        /// </summary>
        private BitmapImage deckScreenshot1;

        /// <summary>
        /// The deck screenshot 2.
        /// </summary>
        private BitmapImage deckScreenshot2;

        /// <summary>
        /// The is second screenshot.
        /// </summary>
        private bool isSecondScreenshot;

        /// <summary>
        /// The can take screenshot.
        /// </summary>
        private bool canTakeScreenshot;

        /// <summary>
        /// The taking screenshot.
        /// </summary>
        private bool takingScreenshot;

        /// <summary>
        /// The show screenshot column.
        /// </summary>
        private bool showScreenshotColumn;

        /// <summary>
        /// The handling arena detect.
        /// </summary>
        private bool handlingArenaDetect;

        /// <summary>
        /// The detected hero.
        /// </summary>
        private string detectedHero;

        /// <summary>
        /// The detected wins.
        /// </summary>
        private int detectedWins = -1;

        /// <summary>
        /// The detected losses.
        /// </summary>
        private int detectedLosses = -1;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CurrentSessionFlyoutViewModel"/> class.
        /// </summary>
        /// <param name="events">
        /// The events.
        /// </param>
        /// <param name="arenaRepository">
        /// The arena repository.
        /// </param>
        /// <param name="heroRepository">
        /// The hero repository.
        /// </param>
        /// <param name="gameManager">
        /// The game manager.
        /// </param>
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
            this.SetPosition(Position.Right);
            this.events.Subscribe(this);
            this.PropertyChanged += this.OnPropertyChanged;
            this.lastIsOpen = this.IsOpen;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the global data.
        /// </summary>
        [Import]
        public GlobalData GlobalData { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether show screenshot column.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the selected server.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the reward gold.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the reward dust.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the reward packs.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the notes.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the ended.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the hero.
        /// </summary>
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
        /// Gets or sets a value indicating whether is ended.
        /// </summary>
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

        /// <summary>
        /// Gets a value indicating whether is latest.
        /// </summary>
        public bool IsLatest
        {
            get
            {
                if (this.SelectedArenaSession == null || this.LatestArenaSession == null) return false;

                return Equals(this.SelectedArenaSession.Id, this.LatestArenaSession.Id);
            }
        }

        /// <summary>
        /// Gets or sets the latest arena session.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the losses.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether retired.
        /// </summary>
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
                if (ReferenceEquals(value, this.selectedArenaSession))
                {
                    return;
                }

                this.selectedArenaSession = value;

                // if (value != null)
                // {
                // this.InitViewModel(value);
                // }
                this.NotifyOfPropertyChange(() => this.SelectedArenaSession);
                this.NotifyOfPropertyChange(() => this.IsLatest);
            }
        }

        /// <summary>
        /// Gets or sets the started.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the wins.
        /// </summary>
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

        /// <summary>
        /// Gets the servers.
        /// </summary>
        public BindableServerCollection Servers
        {
            get
            {
                return this.servers;
            }
        }

        /// <summary>
        /// Gets or sets the deck screenshot 1.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the deck screenshot 2.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether can take screenshot.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether taking screenshot.
        /// </summary>
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

        /// <summary>
        /// The add game.
        /// </summary>
        public void AddGame()
        {
            // this.IsOpen = false;
            this.events.PublishOnCurrentThread(new CreateNewGame { ArenaSession = this.SelectedArenaSession });
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task Delete()
        {
            if (this.SelectedArenaSession == null)
            {
                return;
            }

            if (MessageBox.Show("Delete this arena?", "Delete this arena?", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                await this.gameManager.DeleteArenaSession(this.SelectedArenaSession.Id);
                this.events.PublishOnBackgroundThread(new SendNotification("Arena successfully deleted."));
                this.LoadLatest();
            }
        }

        /// <summary>
        /// The cancel take screenshot.
        /// </summary>
        public void CancelTakeScreenshot()
        {
            this.isSecondScreenshot = false;
            this.CanTakeScreenshot = true;
            this.TakingScreenshot = false;
        }

        /// <summary>
        /// The save screenshot to disk.
        /// </summary>
        public void SaveScreenshotToDisk()
        {
            if (this.SelectedArenaSession == null || this.SelectedArenaSession.Image1 == null)
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
                using (var ms = new MemoryStream(this.SelectedArenaSession.Image1.Image))
                {
                    image1 = new Bitmap(new Bitmap(ms));
                }

                if (this.SelectedArenaSession.Image2 != null)
                {
                    using (var ms = new MemoryStream(this.SelectedArenaSession.Image2.Image))
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

        /// <summary>
        /// The take first screenshot.
        /// </summary>
        public void TakeFirstScreenshot()
        {
            this.arenaIdForScreenshot = this.SelectedArenaSession.Id;
            this.isSecondScreenshot = false;
            this.CanTakeScreenshot = false;
            this.TakingScreenshot = true;
            this.events.PublishOnBackgroundThread(new RequestArenaDeckScreenshot());
        }

        /// <summary>
        /// The take second screenshot.
        /// </summary>
        public void TakeSecondScreenshot()
        {
            this.arenaIdForScreenshot = this.SelectedArenaSession.Id;
            this.isSecondScreenshot = true;
            this.CanTakeScreenshot = false;
            this.TakingScreenshot = true;
            this.events.PublishOnBackgroundThread(new RequestArenaDeckScreenshot());
        }

        /// <summary>
        /// The toggle screenshot.
        /// </summary>
        public void ToggleScreenshot()
        {
            this.ShowScreenshotColumn = !this.ShowScreenshotColumn;
        }

        /// <summary>
        /// The merge.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
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

            var previousArena = this.arenaRepository.Query(a => a.Where(x => x.StartDate < this.SelectedArenaSession.StartDate).OrderByDescending(x => x.StartDate).FirstOrDefault().ToModel());
            if (previousArena == null)
            {
                MessageBox.Show("No previous arena found", "Not found", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (previousArena.Hero.Id != this.SelectedArenaSession.Hero.Id)
            {
                MessageBox.Show(string.Format("Cannot merge because previous arena hero is not a {0}.", this.SelectedArenaSession.Hero.ClassName), "Cannot merge", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            await this.gameManager.MergeArenas(this.SelectedArenaSession, previousArena);
            previousArena = this.arenaRepository.Query(a => a.FirstOrDefault(x => x.Id == previousArena.Id).ToModel());

            this.Load(previousArena);
        }

        /// <summary>
        /// Handle the message with a Task.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// The Task that represents the operation.
        /// </returns>
        public async Task Handle(ArenaHeroDetected message)
        {
            this.EnsureInitialized();
            this.detectedHero = message.Hero;

            if (string.IsNullOrEmpty(this.detectedHero))
            {
                this.Log.Debug("Detected hero is null or empty, ignoring.");
                return;
            }

            if (!this.handlingArenaDetect)
            {
                this.handlingArenaDetect = true;
                await Task.Delay(500).ContinueWith(async t => await this.HandleArenaDetect());
            }
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Handle(ArenaWinsDetected message)
        {
            this.detectedWins = message.Wins;
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Handle(ArenaLossesDetected message)
        {
            this.detectedLosses = message.Losses;
        }

        /// <summary>
        /// The handle arena detect.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task HandleArenaDetect()
        {
            if (!GlobalLocks.NewArenaLock.Wait(1000))
            {
                this.Log.Debug("Waited for NewArenaLock lock");
                GlobalLocks.NewArenaLock.Wait();
            }

            // arena session is always the latest session in the db.
            var serverName = BindableServerCollection.Instance.DefaultName;
            var latestArena = this.arenaRepository.Query(a => a.Where(x => x.Server == serverName).OrderByDescending(x => x.StartDate).FirstOrDefault().ToModel());

            try
            {
                if (latestArena == null)
                {
                    latestArena = await this.TryCreateArenaSession(this.detectedHero);
                    return;
                }

                // weird case
                if (latestArena.IsEnded && latestArena.EndDate != null)
                {
                    var diff = DateTime.Now.Subtract(latestArena.EndDate.Value);
                    if (diff < TimeSpan.FromMinutes(5))
                    {
                        this.Log.Debug("Latest arena is ended (and ended less then 5 minutes ago). Ignoring...");
                        return;

                        // Log.Debug("Latest arena is ended (and started more then 5 minutes ago). Creating new arena");
                        // latestArena = await TryCreateArenaSession(detectedHero);
                        // return;
                    }
                }

                // new arena started because new hero
                if (this.detectedHero != latestArena.Hero.Key)
                {
                    this.Log.Debug("detectedHero ({0}) != latestArena.Hero.Key ({1}), starting new arena.", this.detectedHero, latestArena.Hero.Key);
                    if (!latestArena.IsEnded)
                    {
                        this.Log.Debug("Retiring, previous arena.");

                        // retired
                        await this.Retire(latestArena);
                    }

                    latestArena = await this.TryCreateArenaSession(this.detectedHero);
                    return;
                }

                // check if we need to correct last game
                if (latestArena.IsEnded)
                {
                    if (this.detectedLosses >= 0 && this.detectedLosses < 3)
                    {
                        // last game was not a loss
                        this.Log.Debug("Correcting last game to be a win, because arena was ended, but losses is {0}.", this.detectedLosses);
                        var lastgame = latestArena.Games.OrderByDescending(x => x.Started).FirstOrDefault();
                        if (lastgame != null)
                        {
                            lastgame.Victory = true;
                            await this.gameManager.UpdateGame(lastgame);
                            return;
                        }
                    }
                }

                // doesn't work very well.
                // if ((detectedLosses >= 0 && detectedLosses < latestArena.Losses) ||
                // (detectedWins >= 0 && detectedWins < latestArena.Wins))
                // {
                // Log.Debug(
                // "Detected wins/losses ({0}/{1}) smaller then last ({2}/{3}). Starting new arena.",
                // detectedWins,
                // detectedLosses,
                // latestArena.Wins,
                // latestArena.Losses);
                // latestArena = await TryCreateArenaSession(detectedHero);
                // return;
                // }
            }
            finally
            {
                this.LatestArenaSession = latestArena;
                this.detectedHero = null;
                this.detectedWins = -1;
                this.detectedLosses = -1;
                this.handlingArenaDetect = false;
                if (latestArena != null)
                {
                    this.Load(latestArena);

                    if (latestArena.Image1 == null)
                    {
                        // add screenshot async
                        this.arenaIdForScreenshot = latestArena.Id;
                        this.TakingScreenshot = true;
                        this.CanTakeScreenshot = false;
                        Task.Delay(1000).ContinueWith(t => this.events.PublishOnBackgroundThread(new RequestArenaDeckScreenshot()));
                    }
                }
            }
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Handle(GameModeChanged message)
        {
            if (message.GameMode == GameMode.Arena)
            {
                this.LoadLatest();
            }

            this.CanTakeScreenshot = message.GameMode == GameMode.Arena;
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Handle(ArenaSessionUpdated message)
        {
            if (this.SelectedArenaSession == null) return;

            if (message.ArenaSessionId == this.SelectedArenaSession.Id)
            {
                var updatedArena = this.arenaRepository.FirstOrDefault(x => x.Id == message.ArenaSessionId);
                this.SelectedArenaSession.MapFrom(updatedArena);
                this.InitViewModel(this.SelectedArenaSession);
            }
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Handle(ArenaDrafting message)
        {
        }

        /// <summary>
        /// The load.
        /// </summary>
        /// <param name="arenaSessionModel">
        /// The arena session model.
        /// </param>
        public void Load(ArenaSessionModel arenaSessionModel)
        {
            if (arenaSessionModel == null)
                return;

            this.SelectedArenaSession = arenaSessionModel;
            this.InitLatest();
            this.InitViewModel(this.SelectedArenaSession);
            if (this.IsOpen)
            {
                this.IsOpen = false;
            }

            this.IsOpen = true;
            this.events.PublishOnBackgroundThread(new SelectedArenaSessionChanged(this, arenaSessionModel.Id));
        }

        /// <summary>
        /// The can save.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool CanSave()
        {
            return this.Hero != null && this.SelectedServer != null;
        }

        /// <summary>
        /// The save.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Dependencies("Hero", "SelectedServer")]
        public async Task Save()
        {
            if (this.SelectedArenaSession == null)
            {
                return;
            }

            var arena = this.SelectedArenaSession;
            bool changeGameHeroes = false;
            if (arena.Hero != null && this.Hero.Id != arena.Hero.Id)
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
            arena.Server = this.SelectedServer.Name;

            await this.gameManager.UpdateArenaSession(this.SelectedArenaSession);
            if (changeGameHeroes)
            {
                foreach (var game in this.SelectedArenaSession.Games)
                {
                    game.Hero = this.Hero;
                    await this.gameManager.UpdateGame(game);
                }
            }

            this.InitLatest();
            this.InitViewModel(this.SelectedArenaSession);
            this.events.PublishOnBackgroundThread(new SendNotification("Arena successfully saved."));
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

        /// <summary>
        /// The show current.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task ShowCurrent()
        {
            this.LoadLatest();
        }


        /// <summary>
        /// Called when a part's imports have been satisfied and it is safe to use.
        /// </summary>
        public void OnImportsSatisfied()
        {
            this.EnsureInitialized();
        }

        /// <summary>
        /// The load latest.
        /// </summary>
        public void LoadLatest()
        {
            this.InitLatest();
            if (this.LatestArenaSession != null)
            {
                this.Load(this.LatestArenaSession);
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

        /// <summary>
        /// The init view model.
        /// </summary>
        /// <param name="arenaSession">
        /// The arena session.
        /// </param>
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
            this.SelectedServer = this.servers.FirstOrDefault(x => x.Name == arenaSession.Server);

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

            this.NotifyOfPropertyChange(() => this.IsLatest);

            if (this.IsLatest && !this.IsEnded)
            {
                this.Header = this.DisplayName = "Running Arena:";
            }
            else if (this.IsLatest)
            {
                this.Header = this.DisplayName = "Last Arena:";
            }
            else
            {
                this.Header = this.DisplayName = "Finished Arena:";
            }
        }

        /// <summary>
        /// The ensure initialized.
        /// </summary>
        private void EnsureInitialized()
        {
            if (this.initialized) return;
            this.initialized = true;

            var data = this.GlobalData.Get();
            this.heroes.Clear();
            this.heroes.AddRange(data.Heroes);
            this.InitLatest();
        }

        /// <summary>
        /// The init latest.
        /// </summary>
        private void InitLatest()
        {
            // arena session is always the latest session in the db.
            var serverName = BindableServerCollection.Instance.DefaultName;
            var session = this.arenaRepository.Query(a => a.Where(x => x.Server == serverName).OrderByDescending(x => x.StartDate).FirstOrDefault().ToModel());
            this.LatestArenaSession = session;
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
                case "IsOpen":
                    if (!this.IsOpen && this.lastIsOpen)
                    {
                        this.events.PublishOnBackgroundThread(new SelectedArenaSessionChanged(this, null));
                    }

                    // if (IsOpen)
                    // {
                    // OnSelectedArenaChanged();
                    // }
                    this.lastIsOpen = this.IsOpen;
                    break;
            }
        }

        // private void OnSelectedArenaChanged()
        // {
        // if (!PauseNotify.IsPaused(this))
        // {
        // this.events.PublishOnBackgroundThread(new SelectedArenaSessionChanged(this, this.SelectedArenaSession != null ? this.SelectedArenaSession.Id : (Guid?)null));
        // }
        // }

        /// <summary>
        /// The retire.
        /// </summary>
        /// <param name="arenaSession">
        /// The arena session.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task Retire(ArenaSessionModel arenaSession)
        {
            await this.gameManager.Retire(arenaSession);
        }

        /// <summary>
        /// The try create arena session.
        /// </summary>
        /// <param name="detectedHero">
        /// The detected hero.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task<ArenaSessionModel> TryCreateArenaSession(string detectedHero)
        {
            if (detectedHero == null)
            {
                this.Log.Warn("TryCreateArenaSession called without hero");
                return null;
            }

            using (await this.newArenaLock.LockAsync())
            {
                var arena = new ArenaSessionModel
                                {
                                    Hero = await this.heroRepository.FirstOrDefaultAsync(x => x.Key == detectedHero), 
                                };
                arena = await this.gameManager.AddArenaSession(arena);

                // for web api
                this.events.PublishOnBackgroundThread(new ArenaSessionStarted(arena.StartDate, arena.Hero.Key, arena.Wins, arena.Losses));
                return arena;
            }
        }

        #endregion

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task Handle(ArenaDeckScreenshotTaken message)
        {
            try
            {
                var arena = this.arenaRepository.Query(x => x.FirstOrDefault(a => a.Id == this.arenaIdForScreenshot));
                if (arena == null)
                {
                    return;
                }

                ArenaDeckImage arenaImage;
                if (!this.isSecondScreenshot)
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

                await this.gameManager.UpdateArenaSession(arena.ToModel());
                if (!this.isSecondScreenshot)
                {
                    this.DeckScreenshot1 = CreateBitmapImage(message.Image);
                }
                else
                {
                    this.DeckScreenshot2 = CreateBitmapImage(message.Image);
                }
            }
            finally
            {
                message.Image.Dispose();
                this.CanTakeScreenshot = true;
                this.TakingScreenshot = false;
                this.isSecondScreenshot = false;
            }
        }

        /// <summary>
        /// The create bitmap image.
        /// </summary>
        /// <param name="image">
        /// The image.
        /// </param>
        /// <returns>
        /// The <see cref="BitmapImage"/>.
        /// </returns>
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

        /// <summary>
        /// The create bitmap image.
        /// </summary>
        /// <param name="image">
        /// The image.
        /// </param>
        /// <returns>
        /// The <see cref="BitmapImage"/>.
        /// </returns>
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
        /// <param name="message">
        /// The message.
        /// </param>
        public void Handle(SelectedGameChanged message)
        {

            if (message.Game != null)
            {
                if (message.Game.ArenaSession == null)
                {
                    this.IsOpen = false;
                    return;
                }
            }
        }
    }
}