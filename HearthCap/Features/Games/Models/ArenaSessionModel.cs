namespace HearthCap.Features.Games.Models
{
    using System;
    using System.ComponentModel;
    using System.Windows.Data;

    using Caliburn.Micro;

    using HearthCap.Data;
    using HearthCap.Features.Core;

    public class ArenaSessionModel : PropertyChangedBase
    {
        private Guid id;

        private Hero hero;

        private DateTime startDate;

        private DateTime? endDate;

        private readonly BindableCollection<GameResultModel> games;

        private int wins;

        private int losses;

        private int rewardGold;

        private int rewardDust;

        private int rewardPacks;

        private string rewardOther;

        private bool retired;

        private bool isEnded;

        private static readonly object gamesLock = new object();

        private string notes;

        private string server;

        private ICollectionView _games;

        private DateTime created;

        private DateTime modified;

        private ArenaDeckImage image1;

        private ArenaDeckImage image2;

        public ArenaSessionModel()
        {
            Id = Guid.NewGuid();
            games = new BindableCollection<GameResultModel>();
            StartDate = DateTime.Now;
            RewardPacks = 1;
            Server = BindableServerCollection.Instance.DefaultName;
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

        // TODO: drafted cards
        // public IList<Card> Deck { get; set; }

        public DateTime StartDate
        {
            get
            {
                return this.startDate;
            }
            set
            {
                if (value.Equals(this.startDate))
                {
                    return;
                }
                this.startDate = value;
                this.NotifyOfPropertyChange(() => this.StartDate);
            }
        }

        public DateTime? EndDate
        {
            get
            {
                return this.endDate;
            }
            set
            {
                if (value.Equals(this.endDate))
                {
                    return;
                }
                this.endDate = value;
                this.NotifyOfPropertyChange(() => this.EndDate);
            }
        }

        public BindableCollection<GameResultModel> Games
        {
            get
            {
                return this.games;
            }
            //set
            //{
            //    if (Equals(value, this.games))
            //    {
            //        return;
            //    }
            //    this.games = value;
            //    this.NotifyOfPropertyChange(() => this.Games);
            //}
        }

        public ICollectionView _Games
        {
            get
            {
                if (_games == null)
                {
                    Execute.OnUIThread(
                        () =>
                            {
                                _games = CollectionViewSource.GetDefaultView(this.games);
                                _games.SortDescriptions.Add(new SortDescription("Started", ListSortDirection.Descending));
                                _games.Refresh();
                            });
                    // BindingOperations.EnableCollectionSynchronization(_Games, gamesLock);
                }
                return _games;
            }
        }

        public int Wins
        {
            get
            {
                return this.wins;
            }
            set
            {
                if (value == this.wins)
                {
                    return;
                }
                this.wins = value;
                this.NotifyOfPropertyChange(() => this.Wins);
                this.NotifyOfPropertyChange(() => this.IsEnded);
            }
        }

        public int Losses
        {
            get
            {
                return this.losses;
            }
            set
            {
                if (value == this.losses)
                {
                    return;
                }
                this.losses = value;
                this.NotifyOfPropertyChange(() => this.Losses);
                this.NotifyOfPropertyChange(() => this.IsEnded);
            }
        }

        public int RewardGold
        {
            get
            {
                return this.rewardGold;
            }
            set
            {
                if (value == this.rewardGold)
                {
                    return;
                }
                this.rewardGold = value;
                this.NotifyOfPropertyChange(() => this.RewardGold);
            }
        }

        public int RewardDust
        {
            get
            {
                return this.rewardDust;
            }
            set
            {
                if (value == this.rewardDust)
                {
                    return;
                }
                this.rewardDust = value;
                this.NotifyOfPropertyChange(() => this.RewardDust);
            }
        }

        public int RewardPacks
        {
            get
            {
                return this.rewardPacks;
            }
            set
            {
                if (value == this.rewardPacks)
                {
                    return;
                }
                this.rewardPacks = value;
                this.NotifyOfPropertyChange(() => this.RewardPacks);
            }
        }

        // cards / epics
        public string RewardOther
        {
            get
            {
                return this.rewardOther;
            }
            set
            {
                if (value == this.rewardOther)
                {
                    return;
                }
                this.rewardOther = value;
                this.NotifyOfPropertyChange(() => this.RewardOther);
            }
        }

        public bool Retired
        {
            get
            {
                return this.retired;
            }
            set
            {
                if (value.Equals(this.retired))
                {
                    return;
                }
                this.retired = value;
                this.NotifyOfPropertyChange(() => this.Retired);
                this.NotifyOfPropertyChange(() => this.IsEnded);
            }
        }

        public bool IsEnded
        {
            get
            {
                return this.isEnded;
            }
            set
            {
                if (value.Equals(this.isEnded))
                {
                    return;
                }
                this.isEnded = value;
                this.NotifyOfPropertyChange(() => this.IsEnded);
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

        public ArenaDeckImage Image1
        {
            get
            {
                return this.image1;
            }
            set
            {
                if (Equals(value, this.image1))
                {
                    return;
                }
                this.image1 = value;
                this.NotifyOfPropertyChange(() => this.Image1);
            }
        }

        public ArenaDeckImage Image2
        {
            get
            {
                return this.image2;
            }
            set
            {
                if (Equals(value, this.image2))
                {
                    return;
                }
                this.image2 = value;
                this.NotifyOfPropertyChange(() => this.Image2);
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

        protected bool Equals(ArenaSessionModel other)
        {
            return this.Id.Equals(other.Id);
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
            return this.Equals((ArenaSessionModel)obj);
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

        //public void OrderGamesBy<T>(Func<GameResultModel, T> keySelector)
        //{
        //    this.Games = new BindableCollection<GameResultModel>(this.Games.OrderBy(keySelector));
        //}

        //public void OrderGamesByDescending<T>(Func<GameResultModel, T> keySelector)
        //{
        //    this.Games = new BindableCollection<GameResultModel>(this.Games.OrderByDescending(keySelector));
        //}
    }
}