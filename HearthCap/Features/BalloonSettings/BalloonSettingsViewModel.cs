namespace HearthCap.Features.BalloonSettings
{
    using System;
    using System.ComponentModel.Composition;
    using System.Text;
    using System.Threading.Tasks;

    using Caliburn.Micro;

    using HearthCap.Core.GameCapture.HS.Events;
    using HearthCap.Data;
    using HearthCap.Features.Core;
    using HearthCap.Features.Decks;
    using HearthCap.Features.Settings;
    using HearthCap.Shell.TrayIcon;
    using HearthCap.Shell.UserPreferences;
    using HearthCap.Util;

    [Export(typeof(ISettingsScreen))]
    public class BalloonSettingsViewModel : SettingsScreen,
        IHandle<GameModeChanged>,
        IHandle<DeckDetected>,
        IHandle<NewRound>
    {
        private readonly Func<HearthStatsDbContext> dbContext;

        private readonly UserPreferences userPreferences;

        private readonly BalloonSettings balloonSettings;

        private bool minimizeToTray;

        private IEventAggregator events;

        private IDeckManager deckManager;

        private bool sendingNotification;

        private GameMode? lastGameMode;

        private string lastDeckName;

        [ImportingConstructor]
        public BalloonSettingsViewModel(
            IEventAggregator events,
            Func<HearthStatsDbContext> dbContext,
            UserPreferences userPreferences,
            BalloonSettings balloonSettings, IDeckManager deckManager)
        {
            this.events = events;
            this.dbContext = dbContext;
            this.userPreferences = userPreferences;
            this.balloonSettings = balloonSettings;
            this.deckManager = deckManager;
            this.DisplayName = "Tray icon settings:";
            this.Order = 1;
            events.Subscribe(this);
        }

        public UserPreferences UserPreferences
        {
            get
            {
                return this.userPreferences;
            }
        }

        private void UpdateSettings()
        {
            if (PauseNotify.IsPaused(this)) return;
            userPreferences.MinimizeToTray = this.MinimizeToTray;
        }

        private void LoadSettings()
        {
            using (PauseNotify.For(this))
            {
                this.MinimizeToTray = userPreferences.MinimizeToTray;
            }
        }

        public bool MinimizeToTray
        {
            get
            {
                return this.minimizeToTray;
            }
            set
            {
                if (value.Equals(this.minimizeToTray))
                {
                    return;
                }
                this.minimizeToTray = value;
                this.NotifyOfPropertyChange(() => this.MinimizeToTray);
                this.UpdateSettings();
            }
        }

        public BalloonSettings BalloonSettings
        {
            get
            {
                return this.balloonSettings;
            }
        }

        /// <summary>
        /// Called when initializing.
        /// </summary>
        protected override void OnInitialize()
        {
            this.LoadSettings();
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(GameModeChanged message)
        {
            lastGameMode = message.GameMode;
            if (!sendingNotification)
            {
                this.sendingNotification = true;
                Task.Delay(250).ContinueWith(t => ShowModeChangeBalloon());
            }
        }

        private void ShowModeChangeBalloon()
        {
            var msg = String.Empty;
            var title = String.Empty;
            if (lastGameMode != null)
            {
                msg += String.Format("Game mode: {0}", lastGameMode);
                title = "Game mode detected";
            }
            if (!String.IsNullOrEmpty(lastDeckName))
            {
                if (!String.IsNullOrEmpty(msg))
                {
                    msg += "\n";
                }
                msg += "Deck: " + lastDeckName;
                if (String.IsNullOrEmpty(title))
                {
                    title = "Deck detected";
                }
            }

            events.PublishOnBackgroundThread(
                new TrayNotification(title, msg, 3000)
                    {
                        BalloonType = BalloonTypes.GameMode
                    });
            lastDeckName = null;
            lastGameMode = null;
            sendingNotification = false;
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(DeckDetected message)
        {
            var deck = deckManager.GetOrCreateDeckBySlot(BindableServerCollection.Instance.DefaultName, message.Key);
            this.lastDeckName = deck != null ? deck.Name : message.Key;
            if (!sendingNotification)
            {
                this.sendingNotification = true;
                Task.Delay(250).ContinueWith(t => ShowModeChangeBalloon());
            }
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(NewRound message)
        {
            if (message.Current <= 1) return;

            events.PublishOnBackgroundThread(new TrayNotification("New round",String.Format("Round #{0}, {1}", message.Current, message.MyTurn ? "your turn" : "enemy turn"), 3000)
                                                 {
                                                     BalloonType = BalloonTypes.GameTurns                                                     
                                                 });
        }
    }
}