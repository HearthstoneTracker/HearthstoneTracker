namespace HearthCap.Features.Status
{
    using System;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Windows.Media;

    using Caliburn.Micro;

    using HearthCap.Core.GameCapture.EngineEvents;
    using HearthCap.Core.GameCapture.HS.Events;
    using HearthCap.Data;
    using HearthCap.Features.Core;
    using HearthCap.Features.Decks;

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
        private readonly IEventAggregator events;

        private readonly IDeckManager deckManager;

        private bool windowMinimized;

        private GameMode gameMode;

        private string deck;

        private Hero hero;

        private Hero opponentHero;

        private decimal height;

        private bool myTurn;

        private int turns;

        private bool isInGame;

        private bool windowLost;

        private bool windowFound;

        [ImportingConstructor]
        public StatusViewModel(IEventAggregator events, IDeckManager deckManager)
        {
            this.events = events;
            this.deckManager = deckManager;
            events.Subscribe(this);
        }

        [Import]
        protected GlobalData GlobalData { get; set; }

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

        public Brush HeroBrush
        {
            get
            {
                return Hero.GetBrush();
            }
        }

        public Brush OpponentHeroBrush
        {
            get
            {
                return OpponentHero.GetBrush();
            }
        }

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
                if (windowMinimized)
                {
                    this.WindowLost = false;
                    this.WindowFound = false;
                } 
                this.NotifyOfPropertyChange(() => this.WindowMinimized);
            }
        }

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
                if (windowFound)
                {
                    this.WindowLost = false;
                    this.WindowMinimized = false;
                } 
                this.NotifyOfPropertyChange(() => this.WindowFound);
                this.NotifyOfPropertyChange(() => this.ShowDeck);                
            }
        }

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
                if (windowLost)
                {
                    this.WindowFound = false;
                    this.WindowMinimized = false;
                }
                this.NotifyOfPropertyChange(() => this.WindowLost);
            }
        }

        public bool IsUnknownGameMode
        {
            get
            {
                return GameMode == GameMode.Unknown;
            }
        }

        public bool ShowDeck
        {
            get
            {
                return GameMode != GameMode.Arena && 
                    GameMode != GameMode.Unknown  &&
                    WindowFound;
            }
        }

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

        public bool IsUnknownDeck
        {
            get
            {
                return String.IsNullOrEmpty(Deck);
            }
        }

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
        /// <param name="message">The message.</param>
        public void Handle(GameModeChanged message)
        {
            this.WindowFound = true;
            GameMode = message.GameMode;
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(DeckDetected message)
        {
            this.WindowFound = true;
            var deck = deckManager.GetOrCreateDeckBySlot(BindableServerCollection.Instance.DefaultName, message.Key);
            if (deck != null)
            {
                Deck = deck.Name;
            }
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(WindowNotFound message)
        {
            this.WindowLost = true;
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(WindowFound message)
        {
            this.WindowFound = true;
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(WindowMinimized message)
        {
            this.WindowMinimized = true;
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(HeroDetected message)
        {
            this.WindowFound = true;
            Hero = GlobalData.Get().Heroes.FirstOrDefault(x => x.Key == message.Hero);
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(OpponentHeroDetected message)
        {
            this.WindowFound = true;
            OpponentHero = GlobalData.Get().Heroes.FirstOrDefault(x => x.Key == message.Hero);
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(GameEnded message)
        {
            this.WindowFound = true;
            Hero = null;
            OpponentHero = null;
            MyTurn = false;
            Turns = 0;
            IsInGame = false;
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(NewRound message)
        {
            this.WindowFound = true;
            Turns = message.Current;
            MyTurn = message.MyTurn;
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(GameStarted message)
        {
            this.WindowFound = true;
            IsInGame = true;
        }
    }
}