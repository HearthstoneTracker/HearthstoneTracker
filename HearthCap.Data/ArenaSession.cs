namespace HearthCap.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public class ArenaSession : IEntityWithId<Guid>
    {
        public ArenaSession()
        {
            Id = Guid.NewGuid();
            Games = new List<GameResult>();
            StartDate = DateTime.Now;
            // EndDate = DateTime.Now;
            Created = DateTime.Now;
            Modified = Created;
        }

        public Guid Id { get; protected set; }

        public Hero Hero { get; set; }

        // TODO: drafted cards
        // public IList<Card> Deck { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public IList<GameResult> Games { get; protected set; }

        public int Wins { get; set; }

        public int Losses { get; set; }

        public int RewardGold { get; set; }

        public int RewardDust { get; set; }

        public int RewardPacks { get; set; }

        // cards / epics
        public string RewardOther { get; set; }

        public string Notes { get; set; }

        public bool Retired { get; set; }

        public string Server { get; set; }

        public ArenaDeckImage Image1 { get; set; }

        public ArenaDeckImage Image2 { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; protected set; }

        public bool IsEnded
        {
            get
            {
                return this.Wins == 12 || this.Losses == 3 || this.Retired;
            }
        }

        public bool IncompleteWins
        {
            get
            {
                return Wins != Games.Count(x => x.Victory);
            }
        }

        public bool IncompleteLosses
        {
            get
            {
                return Losses != Games.Count(x => !x.Victory);
            }
        }

        protected bool Equals(ArenaSession other)
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
            return Equals((ArenaSession)obj);
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

        public void OrderGamesBy<T>(Func<GameResult, T> keySelector)
        {
            this.Games = this.Games.OrderBy(keySelector).ToList();
        }

        public void OrderGamesByDescending<T>(Func<GameResult, T> keySelector)
        {
            this.Games = this.Games.OrderByDescending(keySelector).ToList();
        }
    }
}