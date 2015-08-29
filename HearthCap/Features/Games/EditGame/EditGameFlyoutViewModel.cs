using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using Caliburn.Micro.Recipes.Filters;
using HearthCap.Data;
using HearthCap.Features.ArenaSessions;
using HearthCap.Features.Core;
using HearthCap.Features.Decks;
using HearthCap.Features.Decks.ModelMappers;
using HearthCap.Features.GameManager.Events;
using HearthCap.Features.Games.Models;
using HearthCap.Framework;
using HearthCap.Shell.Dialogs;
using HearthCap.Shell.Flyouts;
using HearthCap.Shell.Notifications;
using MahApps.Metro.Controls;

namespace HearthCap.Features.Games.EditGame
{
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
        private readonly BindableCollection<GameMode> gameModes = new GameModesCollection();

        /// <summary>The go first.</summary>
        private bool goFirst;

        /// <summary>The hero.</summary>
        private Hero hero;

        /// <summary>The heroes.</summary>
        private readonly BindableCollection<Hero> heroes;

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

        private readonly BindableCollection<DeckModel> decks = new BindableCollection<DeckModel>();

        private DeckModel selectedDeck;

        private readonly IDeckManager deckManager;

        private readonly GameManager.GameManager gameManager;

        private ArenaSessionModel arenaSession;

        private bool conceded;

        private readonly BindableServerCollection servers = BindableServerCollection.Instance;

        private ServerItemModel selectedServer;

        private bool initialized;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="EditGameFlyoutViewModel" /> class.</summary>
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
            GameManager.GameManager gameManager)
        {
            this.dialogManager = dialogManager;
            this.events = events;
            this.arenaRepository = arenaRepository;
            this.gameRepository = gameRepository;
            this.deckManager = deckManager;
            this.gameManager = gameManager;
            Name = Flyouts.EditGame;
            Header = displayName_new;
            SetPosition(Position.Right);
            heroes = new BindableCollection<Hero>();
            events.Subscribe(this);

            GameMode = GameMode.Practice;
            StartTime = DateTime.Now;
            EndTime = DateTime.Now;
            PropertyChanged += OnPropertyChanged;
            lastIsOpen = IsOpen;

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
            get { return decks; }
        }

        /// <summary>Gets a value indicating whether can reset.</summary>
        public bool CanReset
        {
            get { return SelectedGame != null; }
        }

        /// <summary>Gets a value indicating whether can save.</summary>
        public bool CanSave
        {
            get
            {
                return Hero != null &&
                       OpponentHero != null &&
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
            get { return selectedServer; }
            set
            {
                if (Equals(value, selectedServer))
                {
                    return;
                }
                selectedServer = value;
                RefreshDecks();
                NotifyOfPropertyChange(() => SelectedServer);
                NotifyOfPropertyChange(() => CanSave);
                NotifyOfPropertyChange(() => CanSaveAsNew);
            }
        }

        public DeckModel SelectedDeck
        {
            get { return selectedDeck; }
            set
            {
                if (Equals(value, selectedDeck))
                {
                    return;
                }
                selectedDeck = value;
                NotifyOfPropertyChange(() => SelectedDeck);
                NotifyOfPropertyChange(() => CanSave);
                NotifyOfPropertyChange(() => CanSaveAsNew);
            }
        }

        /// <summary>Gets or sets the end time.</summary>
        public DateTime EndTime
        {
            get { return endTime; }

            set
            {
                if (value.Equals(endTime))
                {
                    return;
                }

                endTime = value;
                NotifyOfPropertyChange(() => EndTime);
            }
        }

        /// <summary>Gets or sets the game mode.</summary>
        public GameMode GameMode
        {
            get { return gameMode; }

            set
            {
                if (value == gameMode)
                {
                    return;
                }

                gameMode = value;
                NotifyOfPropertyChange(() => GameMode);
                NotifyOfPropertyChange(() => CanSave);
            }
        }

        /// <summary>Gets the game modes.</summary>
        public IObservableCollection<GameMode> GameModes
        {
            get { return gameModes; }
        }

        /// <summary>Gets or sets the global data.</summary>
        [Import]
        public GlobalData GlobalData { get; set; }

        /// <summary>Gets or sets a value indicating whether go first.</summary>
        public bool GoFirst
        {
            get { return goFirst; }

            set
            {
                if (value.Equals(goFirst))
                {
                    return;
                }

                goFirst = value;
                NotifyOfPropertyChange(() => GoFirst);
            }
        }

        /// <summary>Gets or sets the hero.</summary>
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
                NotifyOfPropertyChange(() => CanSave);
                NotifyOfPropertyChange(() => CanSaveAsNew);
            }
        }

        /// <summary>Gets the heroes.</summary>
        public IObservableCollection<Hero> Heroes
        {
            get { return heroes; }
        }

        /// <summary>Gets or sets the last game id.</summary>
        public Guid? LastGameId
        {
            get { return lastGameId; }

            set
            {
                if (value.Equals(lastGameId))
                {
                    return;
                }

                lastGameId = value;
                NotifyOfPropertyChange(() => LastGameId);
                NotifyOfPropertyChange(() => CanSave);
                NotifyOfPropertyChange(() => CanSaveAsNew);
            }
        }

        /// <summary>Gets or sets the notes.</summary>
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

        /// <summary>Gets or sets the opponent hero.</summary>
        public Hero OpponentHero
        {
            get { return opponentHero; }

            set
            {
                if (value == opponentHero)
                {
                    return;
                }

                opponentHero = value;
                NotifyOfPropertyChange(() => OpponentHero);
                NotifyOfPropertyChange(() => CanSave);
                NotifyOfPropertyChange(() => CanSaveAsNew);
            }
        }

        /// <summary>Gets or sets the selected game.</summary>
        public GameResultModel SelectedGame
        {
            get { return selectedGame; }

            set
            {
                if (Equals(value, selectedGame))
                {
                    return;
                }

                selectedGame = value;
                NotifyOfPropertyChange(() => SelectedGame);
                NotifyOfPropertyChange(() => CanSave);
                NotifyOfPropertyChange(() => CanSaveAsNew);
            }
        }

        /// <summary>Gets or sets the start time.</summary>
        public DateTime StartTime
        {
            get { return startTime; }

            set
            {
                if (value.Equals(startTime))
                {
                    return;
                }

                startTime = value;
                NotifyOfPropertyChange(() => StartTime);
            }
        }

        /// <summary>Gets or sets the turns.</summary>
        public int Turns
        {
            get { return turns; }

            set
            {
                if (value == turns
                    || value < 1)
                {
                    return;
                }

                turns = value;
                NotifyOfPropertyChange(() => Turns);
            }
        }

        /// <summary>Gets or sets a value indicating whether victory.</summary>
        public bool Victory
        {
            get { return victory; }

            set
            {
                if (value.Equals(victory))
                {
                    return;
                }

                victory = value;
                NotifyOfPropertyChange(() => Victory);
            }
        }

        public bool Conceded
        {
            get { return conceded; }
            set
            {
                if (value.Equals(conceded))
                {
                    return;
                }
                conceded = value;
                NotifyOfPropertyChange(() => Conceded);
            }
        }

        #endregion

        #region Public Methods and Operators

        public void ViewArena()
        {
            if (ArenaSession == null)
            {
                return;
            }
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

            await gameManager.AssignGameToArena(SelectedGame, latestArena.ToModel());
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
                IsOpen = false;
                return;
            }

            Load(game);
        }

        /// <summary>Handles the message.</summary>
        /// <param name="message">The message.</param>
        public void Handle(CreateNewGame message)
        {
            events.PublishOnUIThread(new SelectedGameChanged(this, null));

            Execute.OnUIThread(
                () =>
                    {
                        Clear();
                        IsOpen = true;
                        if (message.ArenaSession != null)
                        {
                            ArenaSession = message.ArenaSession;
                            Hero = message.ArenaSession.Hero;
                            GameMode = GameMode.Arena;
                        }
                    });
        }

        public ArenaSessionModel ArenaSession
        {
            get { return arenaSession; }
            set
            {
                if (Equals(value, arenaSession))
                {
                    return;
                }
                arenaSession = value;
                NotifyOfPropertyChange(() => ArenaSession);
            }
        }

        public BindableServerCollection Servers
        {
            get { return servers; }
        }

        /// <summary>The reset.</summary>
        [Preview("CanReset")]
        [Dependencies("SelectedGame")]
        public void Reset()
        {
            if (SelectedGame != null)
            {
                LoadGameResult(SelectedGame);
            }
        }

        /// <summary>The save.</summary>
        /// <returns>The <see cref="Task" />.</returns>
        [Preview("CanSave")]
        [Dependencies("Hero", "OpponentHero", "GameMode", "SelectedServer", "ArenaSession", "SelectedDeck")]
        public async Task Save()
        {
            using (var busy = Busy.GetTicket())
            {
                GameResultModel gameResult = null;
                var added = false;
                if (LastGameId == null)
                {
                    gameResult = new GameResultModel();
                    added = true;
                }
                else
                {
                    // gameResult = (await gameRepository.FirstOrDefaultAsync(x => x.Id == this.LastGameId)).ToModel();
                    gameResult = SelectedGame;
                }

                SetGameResult(gameResult);

                if (ArenaSession != null)
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
                LastGameId = gameResult.Id;
                LoadGameResult(gameResult);
            }
        }

        /// <summary>The save as new.</summary>
        /// <returns>The <see cref="Task" />.</returns>
        [Dependencies("CanSave", "Hero", "OpponentHero", "GameMode", "SelectedServer", "ArenaSession", "SelectedDeck")]
        [Preview("CanSave")]
        public async Task SaveAsNew()
        {
            LastGameId = null;

            await Save();
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
            if (SelectedGame == null)
            {
                return;
            }

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
            EndTime = DateTime.Now;
        }

        /// <summary>The set start time.</summary>
        public void SetStartTime()
        {
            StartTime = DateTime.Now;
        }

        #endregion

        #region Methods

        /// <summary>The on initialize.</summary>
        protected override void OnInitialize()
        {
            EnsureInitialized();
        }

        /// <summary>The clear.</summary>
        private void Clear()
        {
            SelectedGame = null;
            LastGameId = null;
            StartTime = DateTime.Now;
            EndTime = DateTime.Now;
            Hero = null;
            OpponentHero = null;
            GoFirst = true;
            Victory = false;
            Turns = 1;
            ArenaSession = null;
            Conceded = false;
            SelectedServer = servers.Default;

            if (decks.Count > 0)
            {
                SelectedDeck = decks.First();
            }
        }

        /// <summary>The load data.</summary>
        /// <returns>The <see cref="Task" />.</returns>
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
            RefreshDecks();
        }

        private void RefreshDecks()
        {
            if (SelectedServer == null
                || String.IsNullOrEmpty(SelectedServer.Name))
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
            SelectedGame = game;
            Hero = game.Hero;
            OpponentHero = game.OpponentHero;
            StartTime = game.Started;
            EndTime = game.Stopped;
            Victory = game.Victory;
            GoFirst = game.GoFirst;
            GameMode = game.GameMode;
            // force notify even if not changed
            NotifyOfPropertyChange(() => GameMode);
            Notes = game.Notes;
            Turns = game.Turns;
            ArenaSession = game.ArenaSession;
            LastGameId = game.Id;
            Conceded = game.Conceded;
            SelectedServer = servers.FirstOrDefault(x => x.Name == game.Server);

            if (game.Deck != null)
            {
                if (game.Deck.Deleted
                    && Decks.All(x => x.Id != game.Deck.Id))
                {
                    var model = game.Deck.ToModel();
                    model.Name += " (deleted)";
                    Decks.Insert(0, model);
                }

                SelectedDeck = Decks.FirstOrDefault(x => x.Id == game.Deck.Id);
            }

            NotifyOfPropertyChange(() => CanSaveAsNew);
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
                    if (!IsOpen && lastIsOpen)
                    {
                        events.PublishOnUIThread(new SelectedGameChanged(this, null));
                    }

                    lastIsOpen = IsOpen;
                    break;
                case "LastGameId":
                    if (LastGameId == null)
                    {
                        Header = displayName_new;
                    }
                    else
                    {
                        Header = displayName_edit;
                    }

                    break;
            }
        }

        /// <summary>The set game result.</summary>
        /// <param name="context">The context.</param>
        /// <param name="game">The game.</param>
        private void SetGameResult(GameResultModel game)
        {
            game.GameMode = GameMode;
            if (SelectedDeck != null)
            {
                game.Deck = deckManager.GetDeckById(SelectedDeck.Id);
            }
            game.GameMode = GameMode;
            game.GoFirst = GoFirst;
            game.Hero = Hero;
            game.OpponentHero = OpponentHero;
            game.Notes = Notes;
            game.Started = StartTime;
            game.Stopped = EndTime;
            game.Victory = Victory;
            game.Turns = Turns;
            game.Notes = Notes;
            game.Conceded = Conceded;
            game.Server = SelectedServer.Name;
        }

        #endregion

        /// <summary>
        ///     Handles the message.
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
            events.PublishOnUIThread(new SelectedGameChanged(this, gameResultModel));
        }

        /// <summary>
        ///     Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(GameResultUpdated message)
        {
            if (message.GameResultId == LastGameId)
            {
                var newgame = gameRepository.FirstOrDefault(x => x.Id == message.GameResultId);
                LoadGameResult(newgame.ToModel());
            }
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
