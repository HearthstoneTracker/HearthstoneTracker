// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringToBrushConverter.cs" company="">
//   
// </copyright>
// <summary>
//   The string to brush converter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#if NETFX_CORE
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI;
using Windows.UI.Xaml.Media;

#else

#endif

namespace HearthCap.UI.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;

    /// <summary>
    /// The string to brush converter.
    /// </summary>
    public class StringToBrushConverter : IValueConverter
    {
#if NETFX_CORE

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return InternalConvert(value, targetType, parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

#else

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
            return this.InternalConvert(value, targetType, parameter);
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
        /// <exception cref="NotImplementedException">
        /// </exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

#endif

        /// <summary>
        /// The internal convert.
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
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object InternalConvert(object value, Type targetType, object parameter)
        {
            if (value == null)
            {
                return null;
            }

            string colorName = value.ToString();
            SolidColorBrush scb = new SolidColorBrush();
            switch (colorName)
            {
                case "Magenta":
                    scb.Color = Colors.Magenta;
                    return scb;
                case "Purple":
                    scb.Color = Colors.Purple;
                    return scb;
                case "Brown":
                    scb.Color = Colors.Brown;
                    return scb;
                case "Orange":
                    scb.Color = Colors.Orange;
                    return scb;
                case "Blue":
                    scb.Color = Colors.Blue;
                    return scb;
                case "Red":
                    scb.Color = Colors.Red;
                    return scb;
                case "Yellow":
                    scb.Color = Colors.Yellow;
                    return scb;
                case "Green":
                    scb.Color = Colors.Green;
                    return scb;
                default:
                    return null;
            }
        }
    }
}
