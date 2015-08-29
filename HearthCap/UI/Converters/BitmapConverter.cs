using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Data;
using HearthCap.Util;

namespace HearthCap.UI.Converters
{
    public class BitmapConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var bmp = value as Bitmap;
            if (bmp != null)
            {
                return ImageHelper.Bitmap2BitmapSource(bmp);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
