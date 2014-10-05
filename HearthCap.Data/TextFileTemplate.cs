// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextFileTemplate.cs" company="">
//   
// </copyright>
// <summary>
//   The text file template.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Data
{
    using System;

    /// <summary>
    /// The text file template.
    /// </summary>
    public class TextFileTemplate : IEntityWithId<Guid>
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the filename.
        /// </summary>
        public string Filename { get; set; }

        /// <summary>
        /// Gets or sets the template.
        /// </summary>
        public string Template { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextFileTemplate"/> class.
        /// </summary>
        public TextFileTemplate()
        {
            this.Id = Guid.NewGuid();
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
        protected bool Equals(TextFileTemplate other)
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

            return this.Equals((TextFileTemplate)obj);
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