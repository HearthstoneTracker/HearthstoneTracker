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

namespace HearthCap.Features.Status
{
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
            get { return hero; }
            set
            {
                if (Equals(value, hero))
                {
                    return;
                }
                hero = value;
                NotifyOfPropertyChange(() => Hero);
                NotifyOfPropertyChange(() => HeroBrush);
            }
        }

        public Hero OpponentHero
        {
            get { return opponentHero; }
            set
            {
                if (Equals(value, opponentHero))
                {
                    return;
                }
                opponentHero = value;
                NotifyOfPropertyChange(() => OpponentHero);
                NotifyOfPropertyChange(() => OpponentHeroBrush);
            }
        }

        public Brush HeroBrush
        {
            get { return Hero.GetBrush(); }
        }

        public Brush OpponentHeroBrush
        {
            get { return OpponentHero.GetBrush(); }
        }

        public bool WindowMinimized
        {
            get { return windowMinimized; }
            set
            {
                if (value.Equals(windowMinimized))
                {
                    return;
                }
                windowMinimized = value;
                if (windowMinimized)
                {
                    WindowLost = false;
                    WindowFound = false;
                }
                NotifyOfPropertyChange(() => WindowMinimized);
            }
        }

        public bool WindowFound
        {
            get { return windowFound; }
            set
            {
                if (value.Equals(windowFound))
                {
                    return;
                }
                windowFound = value;
                if (windowFound)
                {
                    WindowLost = false;
                    WindowMinimized = false;
                }
                NotifyOfPropertyChange(() => WindowFound);
                NotifyOfPropertyChange(() => ShowDeck);
            }
        }

        public bool WindowLost
        {
            get { return windowLost; }
            set
            {
                if (value.Equals(windowLost))
                {
                    return;
                }
                windowLost = value;
                if (windowLost)
                {
                    WindowFound = false;
                    WindowMinimized = false;
                }
                NotifyOfPropertyChange(() => WindowLost);
            }
        }

        public bool IsUnknownGameMode
        {
            get { return GameMode == GameMode.Unknown; }
        }

        public bool ShowDeck
        {
            get
            {
                return WindowFound &&
                       (GameMode == GameMode.Casual ||
                        GameMode == GameMode.Challenge ||
                        GameMode == GameMode.Practice ||
                        GameMode == GameMode.Ranked);
            }
        }

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
                NotifyOfPropertyChange(() => IsUnknownGameMode);
                NotifyOfPropertyChange(() => ShowDeck);
            }
        }

        public string Deck
        {
            get { return deck; }
            set
            {
                if (value == deck)
                {
                    return;
                }
                deck = value;
                NotifyOfPropertyChange(() => Deck);
                NotifyOfPropertyChange(() => IsUnknownDeck);
            }
        }

        public bool IsUnknownDeck
        {
            get { return String.IsNullOrEmpty(Deck); }
        }

        public decimal Height
        {
            get { return height; }
            set
            {
                if (value == height)
                {
                    return;
                }
                height = value;
                NotifyOfPropertyChange(() => Height);
            }
        }

        public bool MyTurn
        {
            get { return myTurn; }
            set
            {
                if (value.Equals(myTurn))
                {
                    return;
                }
                myTurn = value;
                NotifyOfPropertyChange(() => MyTurn);
            }
        }

        public int Turns
        {
            get { return turns; }
            set
            {
                if (value == turns)
                {
                    return;
                }
                turns = value;
                NotifyOfPropertyChange(() => Turns);
            }
        }

        public bool IsInGame
        {
            get { return isInGame; }
            set
            {
                if (value.Equals(isInGame))
                {
                    return;
                }
                isInGame = value;
                NotifyOfPropertyChange(() => IsInGame);
            }
        }

        /// <summary>
        ///     Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(GameModeChanged message)
        {
            WindowFound = true;
            GameMode = message.GameMode;
        }

        /// <summary>
        ///     Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(DeckDetected message)
        {
            WindowFound = true;
            var deck = deckManager.GetOrCreateDeckBySlot(BindableServerCollection.Instance.DefaultName, message.Key);
            if (deck != null)
            {
                Deck = deck.Name;
            }
        }

        /// <summary>
        ///     Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(WindowNotFound message)
        {
            WindowLost = true;
        }

        /// <summary>
        ///     Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(WindowFound message)
        {
            WindowFound = true;
        }

        /// <summary>
        ///     Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(WindowMinimized message)
        {
            WindowMinimized = true;
        }

        /// <summary>
        ///     Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(HeroDetected message)
        {
            WindowFound = true;
            Hero = GlobalData.Get().Heroes.FirstOrDefault(x => x.Key == message.Hero);
        }

        /// <summary>
        ///     Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(OpponentHeroDetected message)
        {
            WindowFound = true;
            OpponentHero = GlobalData.Get().Heroes.FirstOrDefault(x => x.Key == message.Hero);
        }

        /// <summary>
        ///     Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(GameEnded message)
        {
            WindowFound = true;
            Hero = null;
            OpponentHero = null;
            MyTurn = false;
            Turns = 0;
            IsInGame = false;
        }

        /// <summary>
        ///     Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(NewRound message)
        {
            WindowFound = true;
            Turns = message.Current;
            MyTurn = message.MyTurn;
        }

        /// <summary>
        ///     Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(GameStarted message)
        {
            WindowFound = true;
            IsInGame = true;
        }
    }
}
