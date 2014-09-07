namespace HearthCap.UI.Converters
{
    using System;
    using System.Drawing;
    using System.Windows.Data;

    using HearthCap.Util;

    public class BitmapConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var bmp = value as Bitmap;
            if (bmp != null)
            {
                return ImageHelper.Bitmap2BitmapSource(bmp);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }

}