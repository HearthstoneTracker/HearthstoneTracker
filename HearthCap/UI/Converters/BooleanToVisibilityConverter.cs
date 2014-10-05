// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BooleanToVisibilityConverter.cs" company="">
//   
// </copyright>
// <summary>
//   The boolean to visibility converter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.UI.Converters
{
    using System.Windows;

    /// <summary>
    /// The boolean to visibility converter.
    /// </summary>
    public sealed class BooleanToVisibilityConverter : BooleanConverter<Visibility>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BooleanToVisibilityConverter"/> class.
        /// </summary>
        public BooleanToVisibilityConverter() :
            base(Visibility.Visible, Visibility.Collapsed) { }
    }
}