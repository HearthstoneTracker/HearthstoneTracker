// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeletedArenaSession.cs" company="">
//   
// </copyright>
// <summary>
//   The deleted arena session.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    /// <summary>
    /// The deleted arena session.
    /// </summary>
    public class DeletedArenaSession : IEntityWithId<Guid>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeletedArenaSession"/> class.
        /// </summary>
        public DeletedArenaSession()
        {
            this.Games = new List<DeletedGameResult>();
        }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the hero.
        /// </summary>
        public Hero Hero { get; set; }

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
        public IList<DeletedGameResult> Games { get; protected set; }

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
        /// Gets or sets the deleted date.
        /// </summary>
        public DateTime DeletedDate { get; set; }

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
        protected bool Equals(DeletedArenaSession other)
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

            return this.Equals((DeletedArenaSession)obj);
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
    }
}