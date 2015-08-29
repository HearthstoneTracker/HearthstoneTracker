using System;
using System.ComponentModel;
using System.Windows.Data;
using Caliburn.Micro;
using HearthCap.Data;
using HearthCap.Features.Core;

namespace HearthCap.Features.Games.Models
{
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

        // TODO: drafted cards
        // public IList<Card> Deck { get; set; }

        public DateTime StartDate
        {
            get { return startDate; }
            set
            {
                if (value.Equals(startDate))
                {
                    return;
                }
                startDate = value;
                NotifyOfPropertyChange(() => StartDate);
            }
        }

        public DateTime? EndDate
        {
            get { return endDate; }
            set
            {
                if (value.Equals(endDate))
                {
                    return;
                }
                endDate = value;
                NotifyOfPropertyChange(() => EndDate);
            }
        }

        public BindableCollection<GameResultModel> Games
        {
            get { return games; }
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
                                _games = CollectionViewSource.GetDefaultView(games);
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
            get { return wins; }
            set
            {
                if (value == wins)
                {
                    return;
                }
                wins = value;
                NotifyOfPropertyChange(() => Wins);
                NotifyOfPropertyChange(() => IsEnded);
            }
        }

        public int Losses
        {
            get { return losses; }
            set
            {
                if (value == losses)
                {
                    return;
                }
                losses = value;
                NotifyOfPropertyChange(() => Losses);
                NotifyOfPropertyChange(() => IsEnded);
            }
        }

        public int RewardGold
        {
            get { return rewardGold; }
            set
            {
                if (value == rewardGold)
                {
                    return;
                }
                rewardGold = value;
                NotifyOfPropertyChange(() => RewardGold);
            }
        }

        public int RewardDust
        {
            get { return rewardDust; }
            set
            {
                if (value == rewardDust)
                {
                    return;
                }
                rewardDust = value;
                NotifyOfPropertyChange(() => RewardDust);
            }
        }

        public int RewardPacks
        {
            get { return rewardPacks; }
            set
            {
                if (value == rewardPacks)
                {
                    return;
                }
                rewardPacks = value;
                NotifyOfPropertyChange(() => RewardPacks);
            }
        }

        // cards / epics
        public string RewardOther
        {
            get { return rewardOther; }
            set
            {
                if (value == rewardOther)
                {
                    return;
                }
                rewardOther = value;
                NotifyOfPropertyChange(() => RewardOther);
            }
        }

        public bool Retired
        {
            get { return retired; }
            set
            {
                if (value.Equals(retired))
                {
                    return;
                }
                retired = value;
                NotifyOfPropertyChange(() => Retired);
                NotifyOfPropertyChange(() => IsEnded);
            }
        }

        public bool IsEnded
        {
            get { return isEnded; }
            set
            {
                if (value.Equals(isEnded))
                {
                    return;
                }
                isEnded = value;
                NotifyOfPropertyChange(() => IsEnded);
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

        public ArenaDeckImage Image1
        {
            get { return image1; }
            set
            {
                if (Equals(value, image1))
                {
                    return;
                }
                image1 = value;
                NotifyOfPropertyChange(() => Image1);
            }
        }

        public ArenaDeckImage Image2
        {
            get { return image2; }
            set
            {
                if (Equals(value, image2))
                {
                    return;
                }
                image2 = value;
                NotifyOfPropertyChange(() => Image2);
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

        protected bool Equals(ArenaSessionModel other)
        {
            return Id.Equals(other.Id);
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
            return Equals((ArenaSessionModel)obj);
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
