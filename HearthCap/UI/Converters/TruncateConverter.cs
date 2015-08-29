using System;
using System.Globalization;
using System.Windows.Data;

namespace HearthCap.UI.Converters
{
    public class TruncateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return string.Empty;
            }

            if (parameter == null)
            {
                return value;
            }

            int maxLength;
            if (!int.TryParse(parameter.ToString(), out maxLength))
            {
                return value;
            }
            var str = value.ToString();
            if (str.Length > maxLength)
            {
                str = str.Substring(0, maxLength) + "...";
            }
            return str;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
