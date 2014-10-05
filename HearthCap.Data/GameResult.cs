// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GameResult.cs" company="">
//   
// </copyright>
// <summary>
//   The game result.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Data
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// The game result.
    /// </summary>
    public class GameResult : IEntityWithId<Guid>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GameResult"/> class.
        /// </summary>
        public GameResult()
        {
            this.Id = Guid.NewGuid();
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

        /// <summary>
        /// Gets or sets the opponent hero.
        /// </summary>
        public Hero OpponentHero { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether victory.
        /// </summary>
        public bool Victory { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether go first.
        /// </summary>
        public bool GoFirst { get; set; }

        /// <summary>
        /// Gets or sets the started.
        /// </summary>
        public DateTime Started { get; set; }

        /// <summary>
        /// Gets or sets the stopped.
        /// </summary>
        public DateTime Stopped { get; set; }

        /// <summary>
        /// Gets the duration.
        /// </summary>
        public int Duration
        {
            get
            {
                if (this.Stopped > this.Started)
                {
                    return (int)Math.Round(this.Stopped.Subtract(this.Started).TotalMinutes, 0);
                }

                return 0;
            }
        }

        /// <summary>
        /// Gets or sets the deck.
        /// </summary>
        public Deck Deck { get; set; }

        /// <summary>
        /// Gets or sets the deck key.
        /// </summary>
        public string DeckKey { get; set; }

        /// <summary>
        /// Gets or sets the game mode.
        /// </summary>
        public GameMode GameMode { get; set; }

        /// <summary>
        /// Gets or sets the notes.
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// Gets or sets the arena session id.
        /// </summary>
        public Guid? ArenaSessionId { get; set; }

        /// <summary>
        /// Gets or sets the arena game no.
        /// </summary>
        public int ArenaGameNo { get; set; }

        /// <summary>
        /// Gets or sets the turns.
        /// </summary>
        public int Turns { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether conceded.
        /// </summary>
        public bool Conceded { get; set; }

        /// <summary>
        /// Gets or sets the arena session.
        /// </summary>
        public ArenaSession ArenaSession { get; set; }

        /// <summary>
        /// Gets or sets the server.
        /// </summary>
        public string Server { get; set; }

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

            return this.Equals((GameResult)obj);
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
        protected bool Equals(GameResult other)
        {
            return this.Id.Equals(other.Id);
        }
    }
}