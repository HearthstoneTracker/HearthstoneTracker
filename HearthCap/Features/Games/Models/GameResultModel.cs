namespace HearthCap.Features.Games.Models
{
    using System;

    using Caliburn.Micro;

    using HearthCap.Data;
    using HearthCap.Features.Core;

    public class GameResultModel : PropertyChangedBase
    {
        private Guid id;

        private Hero hero;

        private Hero opponentHero;

        private bool victory;

        private bool goFirst;

        private DateTime started;

        private DateTime stopped;

        private int duration;

        private string deckKey;

        private GameMode gameMode;

        private string notes;

        private Guid? arenaSessionId;

        private int arenaGameNo;

        private int turns;

        private bool conceded;

        private ArenaSessionModel arenaSession;

        private string server;

        private Deck deck;

        private DateTime created;

        private DateTime modified;

        public GameResultModel()
        {
            Id = Guid.NewGuid();
            Server = BindableServerCollection.Instance.DefaultName;
            Started = DateTime.Now;
            Stopped = DateTime.Now;
            Created = DateTime.Now;
            Modified = Created;
        }

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
        /// <param name="obj">The object to compare with the current object. </param>
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

        protected bool Equals(GameResultModel other)
        {
            return this.Id.Equals(other.Id);
        }
    }
}