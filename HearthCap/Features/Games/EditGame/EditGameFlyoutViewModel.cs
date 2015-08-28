namespace HearthCap.Features.Games.EditGame
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.Composition;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;

    using Caliburn.Micro;
    using Caliburn.Micro.Recipes.Filters;

    using HearthCap.Core.GameCapture;
    using HearthCap.Data;
    using HearthCap.Features.ArenaSessions;
    using HearthCap.Features.Core;
    using HearthCap.Features.Decks;
    using HearthCap.Features.Decks.ModelMappers;
    using HearthCap.Features.GameManager;
    using HearthCap.Features.GameManager.Events;
    using HearthCap.Features.Games.Models;
    using HearthCap.Framework;
    using HearthCap.Shell.Dialogs;
    using HearthCap.Shell.Flyouts;
    using HearthCap.Shell.Notifications;

    using MahApps.Metro.Controls;

    /// <summary>The edit game flyout view model.</summary>
    [Export(typeof(IFlyout))]
    [Export(typeof(EditGameFlyoutViewModel))]
    public class EditGameFlyoutViewModel :
        FlyoutViewModel,
        IHandle<SelectedGameChanged>,
        IHandle<CreateNewGame>,
        IHandle<GameResultAdded>,
        IHandle<GameResultUpdated>,
        // IHandle<DecksUpdated>,
        IHandle<DeckUpdated>
    {
        #region Constants

        /// <summary>The display name_edit.</summary>
        private const string displayName_edit = "Edit game:";

        /// <summary>The display name_new.</summary>
        private const string displayName_new = "New game:";

        #endregion

        #region Fields

        /// <summary>The capture engine.</summary>

        /// <summary>The dialog manager.</summary>
        private readonly IDialogManager dialogManager;

        /// <summary>The events.</summary>
        private readonly IEventAggregator events;

        private readonly IRepository<ArenaSession> arenaRepository;

        private readonly IRepository<GameResult> gameRepository;

        /// <summary>The end time.</summary>
        private DateTime endTime;

        /// <summary>The game mode.</summary>
        private GameMode gameMode;

        /// <summary>The game modes.</summary>
        private BindableCollection<GameMode> gameModes = new GameModesCollection();

        /// <summary>The go first.</summary>
        private bool goFirst;

        /// <summary>The hero.</summary>
        private Hero hero;

        /// <summary>The heroes.</summary>
        private BindableCollection<Hero> heroes;

        /// <summary>The last game id.</summary>
        private Guid? lastGameId;

        /// <summary>The last is open.</summary>
        private bool lastIsOpen;

        /// <summary>The notes.</summary>
        private string notes;

        /// <summary>The opponent hero.</summary>
        private Hero opponentHero;

        /// <summary>The selected game.</summary>
        private GameResultModel selectedGame;

        /// <summary>The start time.</summary>
        private DateTime startTime;

        /// <summary>The turns.</summary>
        private int turns;

        /// <summary>The victory.</summary>
        private bool victory;

        private BindableCollection<DeckModel> decks = new BindableCollection<DeckModel>();

        private DeckModel selectedDeck;

        private IDeckManager deckManager;

        private readonly GameManager gameManager;

        private ArenaSessionModel arenaSession;

        private bool conceded;

        private readonly BindableServerCollection servers = BindableServerCollection.Instance;

        private ServerItemModel selectedServer;

        private bool initialized;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="EditGameFlyoutViewModel"/> class.</summary>
        /// <param name="dialogManager">The dialog manager.</param>
        /// <param name="events">The events.</param>
        /// <param name="captureEngine">The capture engine.</param>
        [ImportingConstructor]
        public EditGameFlyoutViewModel(
            IDialogManager dialogManager,
            IEventAggregator events,
            IRepository<ArenaSession> arenaRepository,
            IRepository<GameResult> gameRepository,
            IDeckManager deckManager,
            GameManager gameManager)
        {
            this.dialogManager = dialogManager;
            this.events = events;
            this.arenaRepository = arenaRepository;
            this.gameRepository = gameRepository;
            this.deckManager = deckManager;
            this.gameManager = gameManager;
            this.Name = Flyouts.EditGame;
            this.Header = displayName_new;
            SetPosition(Position.Right);
            this.heroes = new BindableCollection<Hero>();
            events.Subscribe(this);

            this.GameMode = GameMode.Practice;
            this.StartTime = DateTime.Now;
            this.EndTime = DateTime.Now;
            this.PropertyChanged += this.OnPropertyChanged;
            this.lastIsOpen = this.IsOpen;

            // yeah lol :p
            gameModes.Remove(GameMode.Arena);
            selectedServer = servers.Default;
            Busy = new BusyWatcher();
        }

        #endregion

        #region Public Properties

        [Import]
        public CurrentSessionFlyoutViewModel ArenaViewModel { get; set; }

        /// <summary>Gets or sets the busy.</summary>
        public IBusyWatcher Busy { get; set; }

        public IObservableCollection<DeckModel> Decks
        {
            get
            {
                return this.decks;
            }
        }

        /// <summary>Gets a value indicating whether can reset.</summary>
        public bool CanReset
        {
            get
            {
                return this.SelectedGame != null;
            }
        }

        /// <summary>Gets a value indicating whether can save.</summary>
        public bool CanSave
        {
            get
            {
                return this.Hero != null &&
                    this.OpponentHero != null &&
                    SelectedServer != null &&
                    (ArenaSession != null || ArenaSession == null && SelectedDeck != null);
            }
        }

        public bool CanSaveAsNew
        {
            get
            {
                return (LastGameId == null ||
                       (SelectedGame != null && SelectedGame.ArenaSession == null) ||
                       (SelectedGame != null &&
                        SelectedGame.ArenaSession != null &&
                        !SelectedGame.ArenaSession.IsEnded));
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
                RefreshDecks();
                this.NotifyOfPropertyChange(() => this.SelectedServer);
                this.NotifyOfPropertyChange(() => this.CanSave);
                this.NotifyOfPropertyChange(() => this.CanSaveAsNew);
            }
        }

        public DeckModel SelectedDeck
        {
            get
            {
                return this.selectedDeck;
            }
            set
            {
                if (Equals(value, this.selectedDeck))
                {
                    return;
                }
                this.selectedDeck = value;
                this.NotifyOfPropertyChange(() => this.SelectedDeck);
                this.NotifyOfPropertyChange(() => this.CanSave);
                this.NotifyOfPropertyChange(() => this.CanSaveAsNew);
            }
        }

        /// <summary>Gets or sets the end time.</summary>
        public DateTime EndTime
        {
            get
            {
                return this.endTime;
            }

            set
            {
                if (value.Equals(this.endTime))
                {
                    return;
                }

                this.endTime = value;
                this.NotifyOfPropertyChange(() => this.EndTime);
            }
        }

        /// <summary>Gets or sets the game mode.</summary>
        public GameMode GameMode
        {
            get
            {
                return this.gameMode;
            }

            set
            {
                if (value == this.gameMode)
                {
                    return;
                }

                this.gameMode = value;
                this.NotifyOfPropertyChange(() => this.GameMode);
                this.NotifyOfPropertyChange(() => this.CanSave);
            }
        }

        /// <summary>Gets the game modes.</summary>
        public IObservableCollection<GameMode> GameModes
        {
            get
            {
                return this.gameModes;
            }
        }

        /// <summary>Gets or sets the global data.</summary>
        [Import]
        public GlobalData GlobalData { get; set; }

        /// <summary>Gets or sets a value indicating whether go first.</summary>
        public bool GoFirst
        {
            get
            {
                return this.goFirst;
            }

            set
            {
                if (value.Equals(this.goFirst))
                {
                    return;
                }

                this.goFirst = value;
                this.NotifyOfPropertyChange(() => this.GoFirst);
            }
        }

        /// <summary>Gets or sets the hero.</summary>
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
                this.NotifyOfPropertyChange(() => this.CanSave);
                this.NotifyOfPropertyChange(() => this.CanSaveAsNew);
            }
        }

        /// <summary>Gets the heroes.</summary>
        public IObservableCollection<Hero> Heroes
        {
            get
            {
                return this.heroes;
            }
        }

        /// <summary>Gets or sets the last game id.</summary>
        public Guid? LastGameId
        {
            get
            {
                return this.lastGameId;
            }

            set
            {
                if (value.Equals(this.lastGameId))
                {
                    return;
                }

                this.lastGameId = value;
                this.NotifyOfPropertyChange(() => this.LastGameId);
                this.NotifyOfPropertyChange(() => CanSave);
                this.NotifyOfPropertyChange(() => CanSaveAsNew);
            }
        }

        /// <summary>Gets or sets the notes.</summary>
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

        /// <summary>Gets or sets the opponent hero.</summary>
        public Hero OpponentHero
        {
            get
            {
                return this.opponentHero;
            }

            set
            {
                if (value == this.opponentHero)
                {
                    return;
                }

                this.opponentHero = value;
                this.NotifyOfPropertyChange(() => this.OpponentHero);
                this.NotifyOfPropertyChange(() => this.CanSave);
                this.NotifyOfPropertyChange(() => this.CanSaveAsNew);
            }
        }

        /// <summary>Gets or sets the selected game.</summary>
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
                this.NotifyOfPropertyChange(() => this.CanSave);
                this.NotifyOfPropertyChange(() => this.CanSaveAsNew);
            }
        }

        /// <summary>Gets or sets the start time.</summary>
        public DateTime StartTime
        {
            get
            {
                return this.startTime;
            }

            set
            {
                if (value.Equals(this.startTime))
                {
                    return;
                }

                this.startTime = value;
                this.NotifyOfPropertyChange(() => this.StartTime);
            }
        }

        /// <summary>Gets or sets the turns.</summary>
        public int Turns
        {
            get
            {
                return this.turns;
            }

            set
            {
                if (value == this.turns || value < 1)
                {
                    return;
                }

                this.turns = value;
                this.NotifyOfPropertyChange(() => this.Turns);
            }
        }

        /// <summary>Gets or sets a value indicating whether victory.</summary>
        public bool Victory
        {
            get
            {
                return this.victory;
            }

            set
            {
                if (value.Equals(this.victory))
                {
                    return;
                }

                this.victory = value;
                this.NotifyOfPropertyChange(() => this.Victory);
            }
        }

        public bool Conceded
        {
            get
            {
                return this.conceded;
            }
            set
            {
                if (value.Equals(this.conceded))
                {
                    return;
                }
                this.conceded = value;
                this.NotifyOfPropertyChange(() => this.Conceded);
            }
        }

        #endregion

        #region Public Methods and Operators

        public void ViewArena()
        {
            if (ArenaSession == null) return;
            IsOpen = false;
            // events.PublishOnUIThread(new SelectedArenaSessionChanged(this, ArenaSession.Id));
            ArenaViewModel.Load(ArenaSession);
        }

        public async Task AssignToArena()
        {
            var latestArena = await arenaRepository.QueryAsync(x => x.OrderByDescending(e => e.StartDate).FirstOrDefaultAsync());
            if (latestArena == null)
            {
                return;
            }

            await gameManager.AssignGameToArena(this.SelectedGame, latestArena.ToModel());
            LoadGameResult(SelectedGame);
        }

        /// <summary>Handles the message.</summary>
        /// <param name="message">The message.</param>
        public void Handle(SelectedGameChanged message)
        {
            if (message.Source == this)
            {
                return;
            }

            var game = message.Game;
            if (game == null)
            {
                this.IsOpen = false;
                return;
            }

            this.Load(game);
        }

        /// <summary>Handles the message.</summary>
        /// <param name="message">The message.</param>
        public void Handle(CreateNewGame message)
        {
            this.events.PublishOnUIThread(new SelectedGameChanged(this, null));

            Execute.OnUIThread(
                () =>
                {
                    this.Clear();
                    this.IsOpen = true;
                    if (message.ArenaSession != null)
                    {
                        this.ArenaSession = message.ArenaSession;
                        this.Hero = message.ArenaSession.Hero;
                        this.GameMode = GameMode.Arena;
                    }
                });
        }

        public ArenaSessionModel ArenaSession
        {
            get
            {
                return this.arenaSession;
            }
            set
            {
                if (Equals(value, this.arenaSession))
                {
                    return;
                }
                this.arenaSession = value;
                this.NotifyOfPropertyChange(() => this.ArenaSession);
            }
        }

        public BindableServerCollection Servers
        {
            get
            {
                return this.servers;
            }
        }

        /// <summary>The reset.</summary>
        [Preview("CanReset")]
        [Dependencies("SelectedGame")]
        public void Reset()
        {
            if (this.SelectedGame != null)
            {
                this.LoadGameResult(this.SelectedGame);
            }
        }

        /// <summary>The save.</summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Preview("CanSave")]
        [Dependencies("Hero", "OpponentHero", "GameMode", "SelectedServer", "ArenaSession", "SelectedDeck")]
        public async Task Save()
        {
            using (var busy = this.Busy.GetTicket())
            {
                GameResultModel gameResult = null;
                var added = false;
                if (this.LastGameId == null)
                {
                    gameResult = new GameResultModel();
                    added = true;
                }
                else
                {
                    // gameResult = (await gameRepository.FirstOrDefaultAsync(x => x.Id == this.LastGameId)).ToModel();
                    gameResult = SelectedGame;
                }

                this.SetGameResult(gameResult);

                if (this.ArenaSession != null)
                {
                    gameResult.ArenaSession = ArenaSession;
                }

                if (added)
                {
                    await gameManager.AddGame(gameResult);
                }
                else
                {
                    await gameManager.UpdateGame(gameResult);
                }

                events.PublishOnBackgroundThread(new SendNotification("Game successfully saved."));
                this.LastGameId = gameResult.Id;
                LoadGameResult(gameResult);
            }
        }

        /// <summary>The save as new.</summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [Dependencies("CanSave", "Hero", "OpponentHero", "GameMode", "SelectedServer", "ArenaSession", "SelectedDeck")]
        [Preview("CanSave")]
        public async Task SaveAsNew()
        {
            this.LastGameId = null;

            await this.Save();
        }

        public async void Delete()
        {
            if (MessageBox.Show("Delete this game?", "Are you sure?", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                return;
            }
            IsOpen = false;
            await DeleteAsync();
            events.PublishOnBackgroundThread(new SendNotification("Game successfully deleted."));
        }

        private async Task DeleteAsync()
        {
            if (SelectedGame == null) return;

            await gameManager.DeleteGame(SelectedGame.Id);
            Clear();
        }

        public void ViewDeck()
        {
            events.PublishOnBackgroundThread(new SelectDeck(SelectedDeck));
        }

        /// <summary>The set end time.</summary>
        public void SetEndTime()
        {
            this.EndTime = DateTime.Now;
        }

        /// <summary>The set start time.</summary>
        public void SetStartTime()
        {
            this.StartTime = DateTime.Now;
        }

        #endregion

        #region Methods

        /// <summary>The on initialize.</summary>
        protected override void OnInitialize()
        {
            this.EnsureInitialized();
        }

        /// <summary>The clear.</summary>
        private void Clear()
        {
            this.SelectedGame = null;
            this.LastGameId = null;
            this.StartTime = DateTime.Now;
            this.EndTime = DateTime.Now;
            this.Hero = null;
            this.OpponentHero = null;
            this.GoFirst = true;
            this.Victory = false;
            this.Turns = 1;
            this.ArenaSession = null;
            this.Conceded = false;
            SelectedServer = servers.Default;

            if (this.decks.Count > 0)
            {
                this.SelectedDeck = this.decks.First();
            }
        }

        /// <summary>The load data.</summary>
        /// <returns>The <see cref="Task"/>.</returns>
        private void EnsureInitialized()
        {
            if (initialized) return;
            initialized = true;
            var data = this.GlobalData.Get();
            this.heroes.Clear();
            this.heroes.AddRange(data.Heroes);
            RefreshDecks();
        }

        private void RefreshDecks()
        {
            if (SelectedServer == null || String.IsNullOrEmpty(SelectedServer.Name))
            {
                return;
            }
            var current = SelectedDeck;
            var decks = deckManager.GetDecks(SelectedServer.Name).Select(x => x.ToModel());
            this.decks.Clear();
            this.decks.AddRange(decks);
            if (current != null)
            {
                SelectedDeck = Decks.FirstOrDefault(x => x.Id == current.Id);
            }
            else
            {
                SelectedDeck = Decks.FirstOrDefault();
            }
        }

        private void LoadGameResult(GameResultModel game)
        {
            EnsureInitialized();
            this.SelectedGame = game;
            this.Hero = game.Hero;
            this.OpponentHero = game.OpponentHero;
            this.StartTime = game.Started;
            this.EndTime = game.Stopped;
            this.Victory = game.Victory;
            this.GoFirst = game.GoFirst;
            this.GameMode = game.GameMode;
            // force notify even if not changed
            NotifyOfPropertyChange(() => GameMode);
            this.Notes = game.Notes;
            this.Turns = game.Turns;
            this.ArenaSession = game.ArenaSession;
            this.LastGameId = game.Id;
            this.Conceded = game.Conceded;
            this.SelectedServer = servers.FirstOrDefault(x => x.Name == game.Server);

            if (game.Deck != null)
            {
                if (game.Deck.Deleted && Decks.All(x => x.Id != game.Deck.Id))
                {
                    var model = game.Deck.ToModel();
                    model.Name += " (deleted)";
                    Decks.Insert(0, model);
                }

                this.SelectedDeck = Decks.FirstOrDefault(x => x.Id == game.Deck.Id);
            }

            this.NotifyOfPropertyChange(() => this.CanSaveAsNew);
            //Execute.OnUIThread(
            //    () =>
            //    {
            //        var v = (UIElement)this.GetView();
            //        Panel.SetZIndex(v, 10);
            //    });
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
                        this.events.PublishOnUIThread(new SelectedGameChanged(this, null));
                    }

                    this.lastIsOpen = this.IsOpen;
                    break;
                case "LastGameId":
                    if (this.LastGameId == null)
                    {
                        this.Header = displayName_new;
                    }
                    else
                    {
                        this.Header = displayName_edit;
                    }

                    break;
            }
        }

        /// <summary>The set game result.</summary>
        /// <param name="context">The context.</param>
        /// <param name="game">The game.</param>
        private void SetGameResult(GameResultModel game)
        {
            game.GameMode = this.GameMode;
            if (this.SelectedDeck != null)
            {
                game.Deck = deckManager.GetDeckById(this.SelectedDeck.Id);
            }
            game.GameMode = this.GameMode;
            game.GoFirst = this.GoFirst;
            game.Hero = this.Hero;
            game.OpponentHero = this.OpponentHero;
            game.Notes = this.Notes;
            game.Started = this.StartTime;
            game.Stopped = this.EndTime;
            game.Victory = this.Victory;
            game.Turns = this.Turns;
            game.Notes = this.Notes;
            game.Conceded = this.Conceded;
            game.Server = this.SelectedServer.Name;
        }

        #endregion

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(GameResultAdded message)
        {
            if (message.Source == this)
            {
                return;
            }

            LoadGameResult(message.GameResult);
        }

        public void Load(GameResultModel gameResultModel)
        {
            LoadGameResult(gameResultModel);
            //if (IsOpen)
            //{
            //    IsOpen = false;
            //}
            IsOpen = true;
            this.events.PublishOnUIThread(new SelectedGameChanged(this, gameResultModel));
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(GameResultUpdated message)
        {
            if (message.GameResultId == this.LastGameId)
            {
                var newgame = gameRepository.FirstOrDefault(x => x.Id == message.GameResultId);
                LoadGameResult(newgame.ToModel());
            }
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
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

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        //public void Handle(DecksUpdated message)
        //{
        //    if (SelectedServer != null && SelectedServer.Name == message.Server)
        //    {
        //        var oldSelected = SelectedDeck;
        //        RefreshDecks();
        //        if (oldSelected != null)
        //        {
        //            SelectedDeck = Decks.FirstOrDefault(x => x.Id == oldSelected.Id);
        //        }
        //    }
        //}
    }
}