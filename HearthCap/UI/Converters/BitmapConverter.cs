// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BitmapConverter.cs" company="">
//   
// </copyright>
// <summary>
//   The bitmap converter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.UI.Converters
{
    using System;
    using System.Drawing;
    using System.Globalization;
    using System.Windows.Data;

    using HearthCap.Util;

    /// <summary>
    /// The bitmap converter.
    /// </summary>
    public class BitmapConverter : IValueConverter
    {
        #region IValueConverter Members

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
            var bmp = value as Bitmap;
            if (bmp != null)
            {
                return ImageHelper.Bitmap2BitmapSource(bmp);
            }

            return null;
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

        #endregion
    }

}