using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace HearthCap.UI.Converters
{
    public class ValueToCollapsedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null
                && parameter != null)
            {
                return value.Equals(parameter) ? Visibility.Collapsed : Visibility.Visible;
            }

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
