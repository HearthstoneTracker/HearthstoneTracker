// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CanvasToImageSourceConverter.cs" company="">
//   
// </copyright>
// <summary>
//   The canvas to image source parameters.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.UI.Converters
{
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

    /// <summary>
    /// The canvas to image source parameters.
    /// </summary>
    public class CanvasToImageSourceParameters
    {
        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        public double Width { get; set; }
    }

    /// <summary>
    /// The canvas to image source converter.
    /// </summary>
    public class CanvasToImageSourceConverter : IValueConverter
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
            // Converts a Canvas to an image source
            var canvas = value as Canvas;
            if (canvas == null)
            {
                return null;
            }

            var param = parameter as CanvasToImageSourceParameters;
            double width = canvas.Width;
            double height = canvas.Height;
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
            var bmp = this.GetBitmap(rtb);
            return rtb;
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

        /// <summary>
        /// The get bitmap.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="Bitmap"/>.
        /// </returns>
        Bitmap GetBitmap(BitmapSource source)
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