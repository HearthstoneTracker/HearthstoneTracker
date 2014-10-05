// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatusViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The status view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Status
{
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Windows.Media;

    using Caliburn.Micro;

    using HearthCap.Core.GameCapture.EngineEvents;
    using HearthCap.Core.GameCapture.HS.Events;
    using HearthCap.Data;
    using HearthCap.Features.Core;
    using HearthCap.Features.Decks;

    /// <summary>
    /// The status view model.
    /// </summary>
    [Export(typeof(StatusViewModel))]
    public class StatusViewModel : Screen, 
        IHandle<GameModeChanged>, 
        IHandle<DeckDetected>, 
        IHandle<WindowNotFound>, 
        IHandle<WindowFound>, 
        IHandle<HeroDetected>, 
        IHandle<OpponentHeroDetected>, 
        IHandle<GameEnded>, 
        IHandle<NewRound>, 
        IHandle<GameStarted>, 
        IHandle<WindowMinimized>
    {
        /// <summary>
        /// The events.
        /// </summary>
        private readonly IEventAggregator events;

        /// <summary>
        /// The deck manager.
        /// </summary>
        private readonly IDeckManager deckManager;

        /// <summary>
        /// The window minimized.
        /// </summary>
        private bool windowMinimized;

        /// <summary>
        /// The game mode.
        /// </summary>
        private GameMode gameMode;

        /// <summary>
        /// The deck.
        /// </summary>
        private string deck;

        /// <summary>
        /// The hero.
        /// </summary>
        private Hero hero;

        /// <summary>
        /// The opponent hero.
        /// </summary>
        private Hero opponentHero;

        /// <summary>
        /// The height.
        /// </summary>
        private decimal height;

        /// <summary>
        /// The my turn.
        /// </summary>
        private bool myTurn;

        /// <summary>
        /// The turns.
        /// </summary>
        private int turns;

        /// <summary>
        /// The is in game.
        /// </summary>
        private bool isInGame;

        /// <summary>
        /// The window lost.
        /// </summary>
        private bool windowLost;

        /// <summary>
        /// The window found.
        /// </summary>
        private bool windowFound;

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusViewModel"/> class.
        /// </summary>
        /// <param name="events">
        /// The events.
        /// </param>
        /// <param name="deckManager">
        /// The deck manager.
        /// </param>
        [ImportingConstructor]
        public StatusViewModel(IEventAggregator events, IDeckManager deckManager)
        {
            this.events = events;
            this.deckManager = deckManager;
            events.Subscribe(this);
        }

        /// <summary>
        /// Gets or sets the global data.
        /// </summary>
        [Import]
        protected GlobalData GlobalData { get; set; }

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
                this.NotifyOfPropertyChange(() => this.HeroBrush);
            }
        }

        /// <summary>
        /// Gets or sets the opponent hero.
        /// </summary>
        public Hero OpponentHero
        {
            get
            {
                return this.opponentHero;
            }

            set
            {
                if (Equals(value, this.opponentHero))
                {
                    return;
                }

                this.opponentHero = value;
                this.NotifyOfPropertyChange(() => this.OpponentHero);
                this.NotifyOfPropertyChange(() => this.OpponentHeroBrush);
            }
        }

        /// <summary>
        /// Gets the hero brush.
        /// </summary>
        public Brush HeroBrush
        {
            get
            {
                return this.Hero.GetBrush();
            }
        }

        /// <summary>
        /// Gets the opponent hero brush.
        /// </summary>
        public Brush OpponentHeroBrush
        {
            get
            {
                return this.OpponentHero.GetBrush();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether window minimized.
        /// </summary>
        public bool WindowMinimized
        {
            get
            {
                return this.windowMinimized;
            }

            set
            {
                if (value.Equals(this.windowMinimized))
                {
                    return;
                }

                this.windowMinimized = value;
                if (this.windowMinimized)
                {
                    this.WindowLost = false;
                    this.WindowFound = false;
                }
 
                this.NotifyOfPropertyChange(() => this.WindowMinimized);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether window found.
        /// </summary>
        public bool WindowFound
        {
            get
            {
                return this.windowFound;
            }

            set
            {
                if (value.Equals(this.windowFound))
                {
                    return;
                }

                this.windowFound = value;
                if (this.windowFound)
                {
                    this.WindowLost = false;
                    this.WindowMinimized = false;
                }
 
                this.NotifyOfPropertyChange(() => this.WindowFound);
                this.NotifyOfPropertyChange(() => this.ShowDeck);                
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether window lost.
        /// </summary>
        public bool WindowLost
        {
            get
            {
                return this.windowLost;
            }

            set
            {
                if (value.Equals(this.windowLost))
                {
                    return;
                }

                this.windowLost = value;
                if (this.windowLost)
                {
                    this.WindowFound = false;
                    this.WindowMinimized = false;
                }

                this.NotifyOfPropertyChange(() => this.WindowLost);
            }
        }

        /// <summary>
        /// Gets a value indicating whether is unknown game mode.
        /// </summary>
        public bool IsUnknownGameMode
        {
            get
            {
                return this.GameMode == GameMode.Unknown;
            }
        }

        /// <summary>
        /// Gets a value indicating whether show deck.
        /// </summary>
        public bool ShowDeck
        {
            get
            {
                return this.GameMode != GameMode.Arena && 
                    this.GameMode != GameMode.Unknown  &&
                    this.WindowFound;
            }
        }

        /// <summary>
        /// Gets or sets the game mode.
        /// </summary>
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
                this.NotifyOfPropertyChange(() => this.IsUnknownGameMode);
                this.NotifyOfPropertyChange(() => this.ShowDeck);
            }
        }

        /// <summary>
        /// Gets or sets the deck.
        /// </summary>
        public string Deck
        {
            get
            {
                return this.deck;
            }

            set
            {
                if (value == this.deck)
                {
                    return;
                }

                this.deck = value;
                this.NotifyOfPropertyChange(() => this.Deck);
                this.NotifyOfPropertyChange(() => this.IsUnknownDeck);
            }
        }

        /// <summary>
        /// Gets a value indicating whether is unknown deck.
        /// </summary>
        public bool IsUnknownDeck
        {
            get
            {
                return string.IsNullOrEmpty(this.Deck);
            }
        }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        public decimal Height
        {
            get
            {
                return this.height;
            }

            set
            {
                if (value == this.height)
                {
                    return;
                }

                this.height = value;
                this.NotifyOfPropertyChange(() => this.Height);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether my turn.
        /// </summary>
        public bool MyTurn
        {
            get
            {
                return this.myTurn;
            }

            set
            {
                if (value.Equals(this.myTurn))
                {
                    return;
                }

                this.myTurn = value;
                this.NotifyOfPropertyChange(() => this.MyTurn);
            }
        }

        /// <summary>
        /// Gets or sets the turns.
        /// </summary>
        public int Turns
        {
            get
            {
                return this.turns;
            }

            set
            {
                if (value == this.turns)
                {
                    return;
                }

                this.turns = value;
                this.NotifyOfPropertyChange(() => this.Turns);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is in game.
        /// </summary>
        public bool IsInGame
        {
            get
            {
                return this.isInGame;
            }

            set
            {
                if (value.Equals(this.isInGame))
                {
                    return;
                }

                this.isInGame = value;
                this.NotifyOfPropertyChange(() => this.IsInGame);
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
            this.WindowFound = true;
            this.GameMode = message.GameMode;
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Handle(DeckDetected message)
        {
            this.WindowFound = true;
            var deck = this.deckManager.GetOrCreateDeckBySlot(BindableServerCollection.Instance.DefaultName, message.Key);
            if (deck != null)
            {
                this.Deck = deck.Name;
            }
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Handle(WindowNotFound message)
        {
            this.WindowLost = true;
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Handle(WindowFound message)
        {
            this.WindowFound = true;
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Handle(WindowMinimized message)
        {
            this.WindowMinimized = true;
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Handle(HeroDetected message)
        {
            this.WindowFound = true;
            this.Hero = this.GlobalData.Get().Heroes.FirstOrDefault(x => x.Key == message.Hero);
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Handle(OpponentHeroDetected message)
        {
            this.WindowFound = true;
            this.OpponentHero = this.GlobalData.Get().Heroes.FirstOrDefault(x => x.Key == message.Hero);
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Handle(GameEnded message)
        {
            this.WindowFound = true;
            this.Hero = null;
            this.OpponentHero = null;
            this.MyTurn = false;
            this.Turns = 0;
            this.IsInGame = false;
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Handle(NewRound message)
        {
            this.WindowFound = true;
            this.Turns = message.Current;
            this.MyTurn = message.MyTurn;
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Handle(GameStarted message)
        {
            this.WindowFound = true;
            this.IsInGame = true;
        }
    }
}