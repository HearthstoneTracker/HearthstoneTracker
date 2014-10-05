// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BooleanConverter.cs" company="">
//   
// </copyright>
// <summary>
//   The boolean converter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.UI.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>
    /// The boolean converter.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public class BooleanConverter<T> : IValueConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BooleanConverter{T}"/> class.
        /// </summary>
        /// <param name="trueValue">
        /// The true value.
        /// </param>
        /// <param name="falseValue">
        /// The false value.
        /// </param>
        public BooleanConverter(T trueValue, T falseValue)
        {
            this.True = trueValue;
            this.False = falseValue;
        }

        /// <summary>
        /// Gets or sets the true.
        /// </summary>
        public T True { get; set; }

        /// <summary>
        /// Gets or sets the false.
        /// </summary>
        public T False { get; set; }

        /// <summary>
        /// The convert.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="targetType">
        /// The target type.
        /// </param>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <param name="culture">
        /// The culture.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool && ((bool)value) ? this.True : this.False;
        }

        /// <summary>
        /// The convert back.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="targetType">
        /// The target type.
        /// </param>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <param name="culture">
        /// The culture.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is T && EqualityComparer<T>.Default.Equals((T)value, this.True);
        }
    }
}