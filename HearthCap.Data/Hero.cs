namespace HearthCap.Data
{
    using System;

    public class Hero : IEntityWithId<Guid>
    {
        protected Hero() { }

        public Hero(string key)
        {
            Id = Guid.NewGuid();
            Key = key;
        }

        public Guid Id { get; protected set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Icon { get; set; }

        public string ClassName { get; set; }

        public string Key { get; set; }

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
            return Equals((Hero)obj);
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

        protected bool Equals(Hero other)
        {
            return this.Id.Equals(other.Id);
        }
    }
}