using System;
using System.Globalization;
using System.Windows.Data;

namespace HearthCap.UI.Converters
{
    public class ValueToBooleanInverseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null
                && parameter != null)
            {
                return !value.Equals(parameter);
            }

            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
