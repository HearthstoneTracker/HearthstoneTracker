namespace HearthCap.UI.Converters
{
    using System;
    using System.Globalization;
    using System.Reflection;
    using System.Windows.Data;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    public class HeroKeyToImageConverter : IValueConverter
    {
        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value produced by the binding source.</param><param name="targetType">The type of the binding target property.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            var key = value.ToString();
            if (String.IsNullOrEmpty(key))
            {
                return null;
            }

            var img = new BitmapImage();
            img.BeginInit();
            img.CacheOption = BitmapCacheOption.OnLoad;
            string strUri = String.Format(@"pack://application:,,,/HearthCap;component/resources/heroes/{0}.png", key);
            // string strUri = String.Format(@"/Resources/Heroes/{0}.png", key);
            img.UriSource = new Uri(strUri, UriKind.Absolute);
            img.EndInit();
            return img;
        }

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value that is produced by the binding target.</param><param name="targetType">The type to convert to.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}