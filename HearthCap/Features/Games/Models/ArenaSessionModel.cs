// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArenaSessionModel.cs" company="">
//   
// </copyright>
// <summary>
//   The arena session model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Games.Models
{
    using System;
    using System.ComponentModel;
    using System.Windows.Data;

    using Caliburn.Micro;

    using HearthCap.Data;
    using HearthCap.Features.Core;

    /// <summary>
    /// The arena session model.
    /// </summary>
    public class ArenaSessionModel : PropertyChangedBase
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
        /// The start date.
        /// </summary>
        private DateTime startDate;

        /// <summary>
        /// The end date.
        /// </summary>
        private DateTime? endDate;

        /// <summary>
        /// The games.
        /// </summary>
        private readonly BindableCollection<GameResultModel> games;

        /// <summary>
        /// The wins.
        /// </summary>
        private int wins;

        /// <summary>
        /// The losses.
        /// </summary>
        private int losses;

        /// <summary>
        /// The reward gold.
        /// </summary>
        private int rewardGold;

        /// <summary>
        /// The reward dust.
        /// </summary>
        private int rewardDust;

        /// <summary>
        /// The reward packs.
        /// </summary>
        private int rewardPacks;

        /// <summary>
        /// The reward other.
        /// </summary>
        private string rewardOther;

        /// <summary>
        /// The retired.
        /// </summary>
        private bool retired;

        /// <summary>
        /// The is ended.
        /// </summary>
        private bool isEnded;

        /// <summary>
        /// The games lock.
        /// </summary>
        private static readonly object gamesLock = new object();

        /// <summary>
        /// The notes.
        /// </summary>
        private string notes;

        /// <summary>
        /// The server.
        /// </summary>
        private string server;

        /// <summary>
        /// The _games.
        /// </summary>
        private ICollectionView _games;

        /// <summary>
        /// The created.
        /// </summary>
        private DateTime created;

        /// <summary>
        /// The modified.
        /// </summary>
        private DateTime modified;

        /// <summary>
        /// The image 1.
        /// </summary>
        private ArenaDeckImage image1;

        /// <summary>
        /// The image 2.
        /// </summary>
        private ArenaDeckImage image2;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArenaSessionModel"/> class.
        /// </summary>
        public ArenaSessionModel()
        {
            this.Id = Guid.NewGuid();
            this.games = new BindableCollection<GameResultModel>();
            this.StartDate = DateTime.Now;
            this.RewardPacks = 1;
            this.Server = BindableServerCollection.Instance.DefaultName;
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

        // TODO: drafted cards
        // public IList<Card> Deck { get; set; }

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
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

        /// <summary>
        /// Gets the games.
        /// </summary>
        public BindableCollection<GameResultModel> Games
        {
            get
            {
                return this.games;
            }

            // set
            // {
            // if (Equals(value, this.games))
            // {
            // return;
            // }
            // this.games = value;
            // this.NotifyOfPropertyChange(() => this.Games);
            // }
        }

        /// <summary>
        /// Gets the _ games.
        /// </summary>
        public ICollectionView _Games
        {
            get
            {
                if (this._games == null)
                {
                    Execute.OnUIThread(
                        () =>
                            {
                                this._games = CollectionViewSource.GetDefaultView(this.games);
                                this._games.SortDescriptions.Add(new SortDescription("Started", ListSortDirection.Descending));
                                this._games.Refresh();
                            });

                    // BindingOperations.EnableCollectionSynchronization(_Games, gamesLock);
                }

                return this._games;
            }
        }

        /// <summary>
        /// Gets or sets the wins.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the losses.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the reward gold.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the reward dust.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the reward packs.
        /// </summary>
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
        /// <summary>
        /// Gets or sets the reward other.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether retired.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether is ended.
        /// </summary>
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
        /// Gets or sets the image 1.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the image 2.
        /// </summary>
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
        /// The equals.
        /// </summary>
        /// <param name="other">
        /// The other.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
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

        // public void OrderGamesBy<T>(Func<GameResultModel, T> keySelector)
        // {
        // this.Games = new BindableCollection<GameResultModel>(this.Games.OrderBy(keySelector));
        // }

        // public void OrderGamesByDescending<T>(Func<GameResultModel, T> keySelector)
        // {
        // this.Games = new BindableCollection<GameResultModel>(this.Games.OrderByDescending(keySelector));
        // }
    }
}