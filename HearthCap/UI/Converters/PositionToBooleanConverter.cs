// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PositionToBooleanConverter.cs" company="">
//   
// </copyright>
// <summary>
//   The position to boolean converter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.UI.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    using MahApps.Metro.Controls;

    /// <summary>
    /// The position to boolean converter.
    /// </summary>
    public class PositionToBooleanConverter : IValueConverter
    {
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
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && parameter != null)
            {
                return value.Equals(parameter);
            }

            return false;
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
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var position = (Position)parameter;
            if (value != null && (bool)value)
            {
                return position;
            }
            else
            {
                return position == Position.Left ? Position.Right : Position.Left;
            }
        }
    }
}