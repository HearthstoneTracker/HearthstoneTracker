// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GameResultModel.cs" company="">
//   
// </copyright>
// <summary>
//   The game result model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Games.Models
{
    using System;

    using Caliburn.Micro;

    using HearthCap.Data;
    using HearthCap.Features.Core;

    /// <summary>
    /// The game result model.
    /// </summary>
    public class GameResultModel : PropertyChangedBase
    {
        /// <summary>
        /// The id.
        /// </summary>
        private Guid id;

        /// <summary>
        /// The hero.
        /// </summary>
        private Hero hero;

        /// <summary>
        /// The opponent hero.
        /// </summary>
        private Hero opponentHero;

        /// <summary>
        /// The victory.
        /// </summary>
        private bool victory;

        /// <summary>
        /// The go first.
        /// </summary>
        private bool goFirst;

        /// <summary>
        /// The started.
        /// </summary>
        private DateTime started;

        /// <summary>
        /// The stopped.
        /// </summary>
        private DateTime stopped;

        /// <summary>
        /// The duration.
        /// </summary>
        private int duration;

        /// <summary>
        /// The deck key.
        /// </summary>
        private string deckKey;

        /// <summary>
        /// The game mode.
        /// </summary>
        private GameMode gameMode;

        /// <summary>
        /// The notes.
        /// </summary>
        private string notes;

        /// <summary>
        /// The arena session id.
        /// </summary>
        private Guid? arenaSessionId;

        /// <summary>
        /// The arena game no.
        /// </summary>
        private int arenaGameNo;

        /// <summary>
        /// The turns.
        /// </summary>
        private int turns;

        /// <summary>
        /// The conceded.
        /// </summary>
        private bool conceded;

        /// <summary>
        /// The arena session.
        /// </summary>
        private ArenaSessionModel arenaSession;

        /// <summary>
        /// The server.
        /// </summary>
        private string server;

        /// <summary>
        /// The deck.
        /// </summary>
        private Deck deck;

        /// <summary>
        /// The created.
        /// </summary>
        private DateTime created;

        /// <summary>
        /// The modified.
        /// </summary>
        private DateTime modified;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameResultModel"/> class.
        /// </summary>
        public GameResultModel()
        {
            this.Id = Guid.NewGuid();
            this.Server = BindableServerCollection.Instance.DefaultName;
            this.Started = DateTime.Now;
            this.Stopped = DateTime.Now;
            this.Created = DateTime.Now;
            this.Modified = this.Created;
        }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public Guid Id
        {
            get
            {
                return this.id;
            }

            set
            {
                if (value.Equals(this.id))
                {
                    return;
                }

                this.id = value;
                this.NotifyOfPropertyChange(() => this.Id);
            }
        }

        /// <summary>
        /// Gets or sets the deck.
        /// </summary>
        public Deck Deck
        {
            get
            {
                return this.deck;
            }

            set
            {
                if (Equals(value, this.deck))
                {
                    return;
                }

                this.deck = value;
                this.NotifyOfPropertyChange(() => this.Deck);
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
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether victory.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether go first.
        /// </summary>
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
        /// Gets or sets the stopped.
        /// </summary>
        public DateTime Stopped
        {
            get
            {
                return this.stopped;
            }

            set
            {
                if (value.Equals(this.stopped))
                {
                    return;
                }

                this.stopped = value;
                this.NotifyOfPropertyChange(() => this.Stopped);
            }
        }

        /// <summary>
        /// Gets or sets the duration.
        /// </summary>
        public int Duration
        {
            get
            {
                return this.duration;
            }

            set
            {
                if (value == this.duration)
                {
                    return;
                }

                this.duration = value;
                this.NotifyOfPropertyChange(() => this.Duration);
            }
        }

        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        public DateTime Created
        {
            get
            {
                return this.created;
            }

            set
            {
                if (value.Equals(this.created))
                {
                    return;
                }

                this.created = value;
                this.NotifyOfPropertyChange(() => this.Created);
            }
        }

        /// <summary>
        /// Gets or sets the modified.
        /// </summary>
        public DateTime Modified
        {
            get
            {
                return this.modified;
            }

            set
            {
                if (value.Equals(this.modified))
                {
                    return;
                }

                this.modified = value;
                this.NotifyOfPropertyChange(() => this.Modified);
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
        /// Gets or sets the arena session id.
        /// </summary>
        public Guid? ArenaSessionId
        {
            get
            {
                return this.arenaSessionId;
            }

            set
            {
                if (value.Equals(this.arenaSessionId))
                {
                    return;
                }

                this.arenaSessionId = value;
                this.NotifyOfPropertyChange(() => this.ArenaSessionId);
            }
        }

        /// <summary>
        /// Gets or sets the arena game no.
        /// </summary>
        public int ArenaGameNo
        {
            get
            {
                return this.arenaGameNo;
            }

            set
            {
                if (value == this.arenaGameNo)
                {
                    return;
                }

                this.arenaGameNo = value;
                this.NotifyOfPropertyChange(() => this.ArenaGameNo);
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
        /// Gets or sets a value indicating whether conceded.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the arena session.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the server.
        /// </summary>
        public string Server
        {
            get
            {
                return this.server;
            }

            set
            {
                if (value == this.server)
                {
                    return;
                }

                this.server = value;
                this.NotifyOfPropertyChange(() => this.Server);
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">
        /// The object to compare with the current object. 
        /// </param>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return this.Equals((GameResultModel)obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="other">
        /// The other.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        protected bool Equals(GameResultModel other)
        {
            return this.Id.Equals(other.Id);
        }
    }
}