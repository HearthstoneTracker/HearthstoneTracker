// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArenaSession.cs" company="">
//   
// </copyright>
// <summary>
//   The arena session.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    /// <summary>
    /// The arena session.
    /// </summary>
    public class ArenaSession : IEntityWithId<Guid>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArenaSession"/> class.
        /// </summary>
        public ArenaSession()
        {
            this.Id = Guid.NewGuid();
            this.Games = new List<GameResult>();
            this.StartDate = DateTime.Now;

            // EndDate = DateTime.Now;
            this.Created = DateTime.Now;
            this.Modified = this.Created;
        }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public Guid Id { get; protected set; }

        /// <summary>
        /// Gets or sets the hero.
        /// </summary>
        public Hero Hero { get; set; }

        // TODO: drafted cards
        // public IList<Card> Deck { get; set; }

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Gets or sets the games.
        /// </summary>
        public IList<GameResult> Games { get; protected set; }

        /// <summary>
        /// Gets or sets the wins.
        /// </summary>
        public int Wins { get; set; }

        /// <summary>
        /// Gets or sets the losses.
        /// </summary>
        public int Losses { get; set; }

        /// <summary>
        /// Gets or sets the reward gold.
        /// </summary>
        public int RewardGold { get; set; }

        /// <summary>
        /// Gets or sets the reward dust.
        /// </summary>
        public int RewardDust { get; set; }

        /// <summary>
        /// Gets or sets the reward packs.
        /// </summary>
        public int RewardPacks { get; set; }

        // cards / epics
        /// <summary>
        /// Gets or sets the reward other.
        /// </summary>
        public string RewardOther { get; set; }

        /// <summary>
        /// Gets or sets the notes.
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether retired.
        /// </summary>
        public bool Retired { get; set; }

        /// <summary>
        /// Gets or sets the server.
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// Gets or sets the image 1.
        /// </summary>
        public ArenaDeckImage Image1 { get; set; }

        /// <summary>
        /// Gets or sets the image 2.
        /// </summary>
        public ArenaDeckImage Image2 { get; set; }

        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the modified.
        /// </summary>
        public DateTime Modified { get; set; }

        /// <summary>
        /// Gets or sets the timestamp.
        /// </summary>
        [Timestamp]
        public byte[] Timestamp { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether is ended.
        /// </summary>
        public bool IsEnded
        {
            get
            {
                return this.Wins == 12 || this.Losses == 3 || this.Retired;
            }
        }

        /// <summary>
        /// Gets a value indicating whether incomplete wins.
        /// </summary>
        public bool IncompleteWins
        {
            get
            {
                return this.Wins != this.Games.Count(x => x.Victory);
            }
        }

        /// <summary>
        /// Gets a value indicating whether incomplete losses.
        /// </summary>
        public bool IncompleteLosses
        {
            get
            {
                return this.Losses != this.Games.Count(x => !x.Victory);
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

            return this.Equals((ArenaSession)obj);
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
        /// The order games by.
        /// </summary>
        /// <param name="keySelector">
        /// The key selector.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        public void OrderGamesBy<T>(Func<GameResult, T> keySelector)
        {
            this.Games = this.Games.OrderBy(keySelector).ToList();
        }

        /// <summary>
        /// The order games by descending.
        /// </summary>
        /// <param name="keySelector">
        /// The key selector.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        public void OrderGamesByDescending<T>(Func<GameResult, T> keySelector)
        {
            this.Games = this.Games.OrderByDescending(keySelector).ToList();
        }
    }
}