// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BalloonSettingsViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The balloon settings view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.BalloonSettings
{
    using System;
    using System.ComponentModel.Composition;
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

    /// <summary>
    /// The balloon settings view model.
    /// </summary>
    [Export(typeof(ISettingsScreen))]
    public class BalloonSettingsViewModel : SettingsScreen, 
        IHandle<GameModeChanged>, 
        IHandle<DeckDetected>, 
        IHandle<NewRound>
    {
        /// <summary>
        /// The db context.
        /// </summary>
        private readonly Func<HearthStatsDbContext> dbContext;

        /// <summary>
        /// The user preferences.
        /// </summary>
        private readonly UserPreferences userPreferences;

        /// <summary>
        /// The balloon settings.
        /// </summary>
        private readonly BalloonSettings balloonSettings;

        /// <summary>
        /// The show balloons.
        /// </summary>
        private bool showBalloons;

        /// <summary>
        /// The minimize to tray.
        /// </summary>
        private bool minimizeToTray;

        /// <summary>
        /// The events.
        /// </summary>
        private IEventAggregator events;

        /// <summary>
        /// The canpublish.
        /// </summary>
        private bool canpublish;

        /// <summary>
        /// The deck manager.
        /// </summary>
        private IDeckManager deckManager;

        /// <summary>
        /// The sending notification.
        /// </summary>
        private bool sendingNotification;

        /// <summary>
        /// The last game mode.
        /// </summary>
        private GameMode? lastGameMode;

        /// <summary>
        /// The last deck name.
        /// </summary>
        private string lastDeckName;

        /// <summary>
        /// Initializes a new instance of the <see cref="BalloonSettingsViewModel"/> class.
        /// </summary>
        /// <param name="events">
        /// The events.
        /// </param>
        /// <param name="dbContext">
        /// The db context.
        /// </param>
        /// <param name="userPreferences">
        /// The user preferences.
        /// </param>
        /// <param name="balloonSettings">
        /// The balloon settings.
        /// </param>
        /// <param name="deckManager">
        /// The deck manager.
        /// </param>
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

        /// <summary>
        /// Gets the user preferences.
        /// </summary>
        public UserPreferences UserPreferences
        {
            get
            {
                return this.userPreferences;
            }
        }

        /// <summary>
        /// The update settings.
        /// </summary>
        private void UpdateSettings()
        {
            if (PauseNotify.IsPaused(this)) return;
            this.userPreferences.MinimizeToTray = this.MinimizeToTray;
        }

        /// <summary>
        /// The load settings.
        /// </summary>
        private void LoadSettings()
        {
            using (PauseNotify.For(this))
            {
                this.MinimizeToTray = this.userPreferences.MinimizeToTray;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether minimize to tray.
        /// </summary>
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

        /// <summary>
        /// Gets the balloon settings.
        /// </summary>
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
        /// <param name="message">
        /// The message.
        /// </param>
        public void Handle(GameModeChanged message)
        {
            this.lastGameMode = message.GameMode;
            if (!this.sendingNotification)
            {
                this.sendingNotification = true;
                Task.Delay(250).ContinueWith(t => this.ShowModeChangeBalloon());
            }
        }

        /// <summary>
        /// The show mode change balloon.
        /// </summary>
        private void ShowModeChangeBalloon()
        {
            var msg = string.Empty;
            var title = string.Empty;
            if (this.lastGameMode != null)
            {
                msg += string.Format("Game mode: {0}", this.lastGameMode);
                title = "Game mode detected";
            }

            if (!string.IsNullOrEmpty(this.lastDeckName))
            {
                if (!string.IsNullOrEmpty(msg))
                {
                    msg += "\n";
                }

                msg += "Deck: " + this.lastDeckName;
                if (string.IsNullOrEmpty(title))
                {
                    title = "Deck detected";
                }
            }

            this.events.PublishOnBackgroundThread(
                new TrayNotification(title, msg, 3000)
                    {
                        BalloonType = BalloonTypes.GameMode
                    });
            this.lastDeckName = null;
            this.lastGameMode = null;
            this.sendingNotification = false;
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Handle(DeckDetected message)
        {
            var deck = this.deckManager.GetOrCreateDeckBySlot(BindableServerCollection.Instance.DefaultName, message.Key);
            this.lastDeckName = deck != null ? deck.Name : message.Key;
            if (!this.sendingNotification)
            {
                this.sendingNotification = true;
                Task.Delay(250).ContinueWith(t => this.ShowModeChangeBalloon());
            }
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Handle(NewRound message)
        {
            if (message.Current <= 1) return;

            this.events.PublishOnBackgroundThread(new TrayNotification("New round", string.Format("Round #{0}, {1}", message.Current, message.MyTurn ? "your turn" : "enemy turn"), 3000)
                                                 {
                                                     BalloonType = BalloonTypes.GameTurns                                                     
                                                 });
        }
    }
}