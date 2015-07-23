namespace HearthCap.Features.ThemeSettings
{
    using System.Windows.Media;

    using Caliburn.Micro;

    public class AccentViewModel
    {
        private readonly string name;

        private readonly Brush colorBrush;

        public AccentViewModel(string name, Brush colorBrush)
        {
            this.name = name;
            this.colorBrush = colorBrush;
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }

        public Brush ColorBrush
        {
            get
            {
                return this.colorBrush;
            }
        }

        protected bool Equals(AccentViewModel other)
        {
            return string.Equals(this.name, other.name);
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
            return Equals((AccentViewModel)obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return this.name.GetHashCode();
        }
    }
}