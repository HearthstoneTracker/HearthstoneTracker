using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using Point = System.Drawing.Point;
using Size = System.Windows.Size;

namespace HearthCap.UI.Converters
{
    public class CanvasToImageSourceParameters
    {
        public double Height { get; set; }

        public double Width { get; set; }
    }

    public class CanvasToImageSourceConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Converts a Canvas to an image source
            var canvas = value as Canvas;
            if (canvas == null)
            {
                return null;
            }
            var param = parameter as CanvasToImageSourceParameters;
            var width = canvas.Width;
            var height = canvas.Height;
            if (param != null)
            {
                width = param.Width;
                height = param.Height;
            }

            if (!canvas.IsMeasureValid)
            {
                canvas.Measure(new Size(width, height));
                canvas.Arrange(new Rect(0, 0, width, height));
                canvas.UpdateLayout();
            }

            var rtb = new RenderTargetBitmap((int)width, (int)height, 96d, 96d, PixelFormats.Pbgra32);
            rtb.Render(canvas);
            var bmp = GetBitmap(rtb);
            return rtb;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private Bitmap GetBitmap(BitmapSource source)
        {
            var bmp = new Bitmap(source.PixelWidth, source.PixelHeight, PixelFormat.Format32bppPArgb);
            var data = bmp.LockBits(new Rectangle(Point.Empty, bmp.Size), ImageLockMode.WriteOnly, PixelFormat.Format32bppPArgb);
            source.CopyPixels(Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride);
            bmp.UnlockBits(data);
            return bmp;
        }

        #endregion
    }
}
