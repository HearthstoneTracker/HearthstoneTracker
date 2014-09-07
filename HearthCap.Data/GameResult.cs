namespace HearthCap.Data
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class GameResult : IEntityWithId<Guid>
    {
        public GameResult()
        {
            Id = Guid.NewGuid();
            Created = DateTime.Now;
            Modified = Created;
        }

        public Guid Id { get; protected set; }

        public Hero Hero { get; set; }

        public Hero OpponentHero { get; set; }

        public bool Victory { get; set; }

        public bool GoFirst { get; set; }

        public DateTime Started { get; set; }

        public DateTime Stopped { get; set; }

        public int Duration
        {
            get
            {
                if (Stopped > Started)
                {
                    return (int)Math.Round(Stopped.Subtract(Started).TotalMinutes, 0);
                }
                return 0;
            }
        }

        public Deck Deck { get; set; }

        public string DeckKey { get; set; }

        public GameMode GameMode { get; set; }

        public string Notes { get; set; }

        public Guid? ArenaSessionId { get; set; }

        public int ArenaGameNo { get; set; }

        public int Turns { get; set; }

        public bool Conceded { get; set; }

        public ArenaSession ArenaSession { get; set; }

        public string Server { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; protected set; }

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
            return Equals((GameResult)obj);
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

        protected bool Equals(GameResult other)
        {
            return this.Id.Equals(other.Id);
        }
    }
}