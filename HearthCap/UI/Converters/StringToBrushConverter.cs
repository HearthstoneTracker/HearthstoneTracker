using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

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
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return InternalConvert(value, targetType, parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

#endif

        public object InternalConvert(object value, Type targetType, object parameter)
        {
            if (value == null)
            {
                return null;
            }

            var colorName = value.ToString();
            var scb = new SolidColorBrush();
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
