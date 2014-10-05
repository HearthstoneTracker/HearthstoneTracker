// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageHelper.cs" company="">
//   
// </copyright>
// <summary>
//   The image helper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Util
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Windows;
    using System.Windows.Interop;
    using System.Windows.Media.Imaging;

    /// <summary>
    /// The image helper.
    /// </summary>
    public static class ImageHelper
    {
        /// <summary>
        /// The bitmap image 2 bitmap.
        /// </summary>
        /// <param name="bitmapImage">
        /// The bitmap image.
        /// </param>
        /// <returns>
        /// The <see cref="Bitmap"/>.
        /// </returns>
        public static Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            using (var outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                var bitmap = new Bitmap(outStream);
                return new Bitmap(bitmap);
            }
        }

        /// <summary>
        /// The bitmap 2 bitmap source.
        /// </summary>
        /// <param name="bmp">
        /// The bmp.
        /// </param>
        /// <returns>
        /// The <see cref="BitmapSource"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        public static BitmapSource Bitmap2BitmapSource(Bitmap bmp)
        {
            if (bmp == null)
            {
                throw new ArgumentNullException("bmp");
            }
            
            var bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
                bmp.GetHbitmap(), 
                IntPtr.Zero, 
                Int32Rect.Empty, 
                BitmapSizeOptions.FromEmptyOptions());
            return bitmapSource;
        }
    }
}