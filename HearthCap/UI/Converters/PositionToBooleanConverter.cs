using System;
using System.Globalization;
using System.Windows.Data;
using MahApps.Metro.Controls;

namespace HearthCap.UI.Converters
{
    public class PositionToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null
                && parameter != null)
            {
                return value.Equals(parameter);
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var position = (Position)parameter;
            if (value != null
                && (bool)value)
            {
                return position;
            }
            return position == Position.Left ? Position.Right : Position.Left;
        }
    }
}
