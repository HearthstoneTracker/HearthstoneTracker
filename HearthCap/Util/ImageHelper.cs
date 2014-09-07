namespace HearthCap.Util
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Windows;
    using System.Windows.Interop;
    using System.Windows.Media.Imaging;

    public static class ImageHelper
    {
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