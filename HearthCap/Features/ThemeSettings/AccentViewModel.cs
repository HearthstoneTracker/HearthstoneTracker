// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccentViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The accent view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.ThemeSettings
{
    using System.Windows.Media;

    /// <summary>
    /// The accent view model.
    /// </summary>
    public class AccentViewModel
    {
        /// <summary>
        /// The name.
        /// </summary>
        private readonly string name;

        /// <summary>
        /// The color brush.
        /// </summary>
        private readonly Brush colorBrush;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccentViewModel"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="colorBrush">
        /// The color brush.
        /// </param>
        public AccentViewModel(string name, Brush colorBrush)
        {
            this.name = name;
            this.colorBrush = colorBrush;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>
        /// Gets the color brush.
        /// </summary>
        public Brush ColorBrush
        {
            get
            {
                return this.colorBrush;
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

            return this.Equals((AccentViewModel)obj);
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