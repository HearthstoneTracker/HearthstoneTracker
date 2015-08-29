using System;
using Caliburn.Micro;
using HearthCap.Data;
using HearthCap.Features.Core;

namespace HearthCap.Features.Games.Models
{
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
            get { return id; }
            set
            {
                if (value.Equals(id))
                {
                    return;
                }
                id = value;
                NotifyOfPropertyChange(() => Id);
            }
        }

        public Deck Deck
        {
            get { return deck; }
            set
            {
                if (Equals(value, deck))
                {
                    return;
                }
                deck = value;
                NotifyOfPropertyChange(() => Deck);
            }
        }

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
            }
        }

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

        public DateTime Started
        {
            get { return started; }
            set
            {
                if (value.Equals(started))
                {
                    return;
                }
                started = value;
                NotifyOfPropertyChange(() => Started);
            }
        }

        public DateTime Stopped
        {
            get { return stopped; }
            set
            {
                if (value.Equals(stopped))
                {
                    return;
                }
                stopped = value;
                NotifyOfPropertyChange(() => Stopped);
            }
        }

        public int Duration
        {
            get { return duration; }
            set
            {
                if (value == duration)
                {
                    return;
                }
                duration = value;
                NotifyOfPropertyChange(() => Duration);
            }
        }

        public DateTime Created
        {
            get { return created; }
            set
            {
                if (value.Equals(created))
                {
                    return;
                }
                created = value;
                NotifyOfPropertyChange(() => Created);
            }
        }

        public DateTime Modified
        {
            get { return modified; }
            set
            {
                if (value.Equals(modified))
                {
                    return;
                }
                modified = value;
                NotifyOfPropertyChange(() => Modified);
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
            }
        }

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

        public Guid? ArenaSessionId
        {
            get { return arenaSessionId; }
            set
            {
                if (value.Equals(arenaSessionId))
                {
                    return;
                }
                arenaSessionId = value;
                NotifyOfPropertyChange(() => ArenaSessionId);
            }
        }

        public int ArenaGameNo
        {
            get { return arenaGameNo; }
            set
            {
                if (value == arenaGameNo)
                {
                    return;
                }
                arenaGameNo = value;
                NotifyOfPropertyChange(() => ArenaGameNo);
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

        public string Server
        {
            get { return server; }
            set
            {
                if (value == server)
                {
                    return;
                }
                server = value;
                NotifyOfPropertyChange(() => Server);
            }
        }

        /// <summary>
        ///     Determines whether the specified <see cref="T:System.Object" /> is equal to the current
        ///     <see cref="T:System.Object" />.
        /// </summary>
        /// <returns>
        ///     true if the specified object  is equal to the current object; otherwise, false.
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
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((GameResultModel)obj);
        }

        /// <summary>
        ///     Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        ///     A hash code for the current <see cref="T:System.Object" />.
        /// </returns>
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        protected bool Equals(GameResultModel other)
        {
            return Id.Equals(other.Id);
        }
    }
}
