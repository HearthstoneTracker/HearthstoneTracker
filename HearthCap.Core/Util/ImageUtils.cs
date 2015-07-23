namespace HearthCap.Core.Util
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography;

    public static class ImageUtils
    {
        public unsafe static bool IsAllBlack(this Bitmap img)
        {
            bool allblack = true;
            var data = img.LockBits(new Rectangle(0, 0, 64, 64), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            try
            {
                var bytes = (byte*)data.Scan0;
                for (int i = 0; i < data.Height * data.Stride; i++)
                {
                    allblack = (bytes[i] == 0 || bytes[i] == 255);
                    if (!allblack)
                    {
                        break;
                    }
                }
            }
            finally
            {
                img.UnlockBits(data);
            }

            return allblack;
        }

        [DllImport("msvcrt.dll")]
        private static extern int memcmp(IntPtr b1, IntPtr b2, long count);

        public static bool CompareMemCmp(Bitmap b1, Bitmap b2)
        {
            if ((b1 == null) != (b2 == null)) return false;
            if (b1.Size != b2.Size) return false;

            var bd1 = b1.LockBits(new Rectangle(new Point(0, 0), b1.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            var bd2 = b2.LockBits(new Rectangle(new Point(0, 0), b2.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            try
            {
                IntPtr bd1scan0 = bd1.Scan0;
                IntPtr bd2scan0 = bd2.Scan0;

                int stride = bd1.Stride;
                int len = stride * b1.Height;

                return memcmp(bd1scan0, bd2scan0, len) == 0;
            }
            finally
            {
                b1.UnlockBits(bd1);
                b2.UnlockBits(bd2);
            }
        }

        public unsafe static bool AreEqual(Bitmap imageA, Bitmap imageB)
        {
            if (imageA.Width != imageB.Width) return false;
            if (imageA.Height != imageB.Height) return false;

            var d1 = imageA.LockBits(new Rectangle(0, 0, imageA.Width - 1, imageA.Height - 1), ImageLockMode.ReadOnly, imageA.PixelFormat);
            var d2 = imageB.LockBits(new Rectangle(0, 0, imageB.Width - 1, imageB.Height - 1), ImageLockMode.ReadOnly, imageB.PixelFormat);

            var data1 = (byte*)d1.Scan0;
            var data2 = (byte*)d2.Scan0;
            bool result = true;
            for (int n = 0; n < d1.Height * d1.Stride; n++)
            {
                if (data1[n] != data2[n])
                {
                    result = false;
                    break;
                }
            }

            imageA.UnlockBits(d1);
            imageB.UnlockBits(d2);

            return result;
        }
        private static ImageConverter ic = new ImageConverter();
        private static SHA512Managed sha = new SHA512Managed();

        public static bool AreEqualHash(Bitmap imageA, Bitmap imageB)
        {
            if (imageA.Width != imageB.Width) return false;
            if (imageA.Height != imageB.Height) return false;

            var btImage1 = new byte[1];
            var btImage2 = new byte[1];
            btImage1 = (byte[])ic.ConvertTo(imageA, btImage1.GetType());
            btImage2 = (byte[])ic.ConvertTo(imageB, btImage2.GetType());

            //Compute a hash for each image
            byte[] hash1 = sha.ComputeHash(btImage1);
            byte[] hash2 = sha.ComputeHash(btImage2);

            //Compare the hash values
            for (int i = 0; i < hash1.Length && i < hash2.Length; i++)
            {
                if (hash1[i] != hash2[i])
                {
                    return false;
                }
            }

            return true;
        }

        public static bool AreEqual(byte[] imageA, byte[] imageB)
        {
            if (imageA.Length != imageB.Length) return false;

            //Compare the hash values
            for (int i = 0; i < imageA.Length && i < imageB.Length; i++)
            {
                if (imageA[i] != imageB[i])
                {
                    return false;
                }
            }

            return true;
        }

        public static unsafe bool AreEqual(byte[] imageA, BitmapData imageB)
        {
            //Compare the hash values
            var imageBlen = imageB.Height * imageB.Stride;
            var imageBdata = (byte*)imageB.Scan0;
            for (int i = 0; i < imageA.Length && i < imageBlen; i++)
            {
                if (imageA[i] != imageBdata[i])
                {
                    return false;
                }
            }

            return true;
        }

        public static byte[] GetHash(this Bitmap img)
        {
            var data = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadOnly, img.PixelFormat);
            var bytes = new byte[data.Height * data.Stride];
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);
            img.UnlockBits(data);
            return sha.ComputeHash(bytes);
        }

        public static byte[] GetBytes(this Bitmap img)
        {
            var data = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadOnly, img.PixelFormat);
            var bytes = new byte[data.Height * data.Stride];
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);
            img.UnlockBits(data);
            return bytes;
        }

        public static byte[] GetBytes(this BitmapData bitmapData)
        {
            var bytes = new byte[bitmapData.Height * bitmapData.Stride];
            Marshal.Copy(bitmapData.Scan0, bytes, 0, bytes.Length);
            return bytes;
        }

        public static Color GetDominantColor(this BitmapData bitmap)
        {
            if (bitmap.PixelFormat != PixelFormat.Format32bppArgb && bitmap.PixelFormat != PixelFormat.Format32bppPArgb
                && bitmap.PixelFormat != PixelFormat.Format32bppRgb) throw new ApplicationException("expected 32bit image");
            // var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            var stride = bitmap.Stride;
            var scan0 = bitmap.Scan0;
            var totals = new long[] { 0, 0, 0 };
            var width = bitmap.Width;
            var height = bitmap.Height;

            unsafe
            {
                var p = (byte*)(void*)scan0;
                for (var y = 0; y < height; y++)
                    for (var x = 0; x < width; x++)
                        for (var color = 0; color < 3; color++)
                        {
                            var idx = (y * stride) + x * 4 + color;
                            totals[color] += p[idx];
                        }
            }
            // bitmap.UnlockBits(bitmapData);

            var avgB = (int)(totals[0] / (width * height));
            var avgG = (int)(totals[1] / (width * height));
            var avgR = (int)(totals[2] / (width * height));

            return Color.FromArgb(avgR, avgG, avgB);
        }

        public static Color GetDominantColor(this Bitmap bitmap)
        {
            var data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
            var result = GetDominantColor(data);
            bitmap.UnlockBits(data);
            return result;
        }

        public static bool IsMostly(this Bitmap bitmap, Mostly mostly)
        {
            var data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
            try
            {
                switch (mostly)
                {
                    case Mostly.Red:
                        return IsMostlyRed(data);
                    case Mostly.Blue:
                        return IsMostlyBlue(data);
                    case Mostly.Green:
                        return IsMostlyGreen(data);
                }
                throw new ArgumentException("Unknown color channel: " + mostly, "mostly");
            }
            finally
            {
                bitmap.UnlockBits(data);                
            }
        }

        public static bool IsMostly(this BitmapData bitmap, Mostly mostly)
        {
            switch (mostly)
            {
                case Mostly.Red:
                    return IsMostlyRed(bitmap);
                case Mostly.Blue:
                    return IsMostlyBlue(bitmap);
                case Mostly.Green:
                    return IsMostlyGreen(bitmap);
            }
            throw new ArgumentException("Unknown color channel: " + mostly, "mostly");
        }

        public static bool IsMostlyRed(this BitmapData bitmap)
        {
            var c = bitmap.GetDominantColor();
            return c.R > c.B && c.R > c.G;
        }

        public static bool IsMostlyBlue(this BitmapData bitmap)
        {
            var c = bitmap.GetDominantColor();
            return c.B > c.R && c.B > c.G;
        }

        public static bool IsMostlyGreen(this BitmapData bitmap)
        {
            var c = bitmap.GetDominantColor();
            return c.G > c.R && c.G > c.B;
        }

        public static Color GetDominantColorSlow(this Bitmap bitmap)
        {
            if (bitmap.PixelFormat != PixelFormat.Format32bppArgb && bitmap.PixelFormat != PixelFormat.Format32bppPArgb
                && bitmap.PixelFormat != PixelFormat.Format32bppRgb) throw new ApplicationException("expected 32bit image");

            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
            var scan0 = bitmapData.Scan0;
            var colorDist = new Dictionary<Color, double>();
            var width = bitmap.Width;
            var height = bitmap.Height;
            var stride = bitmapData.Stride;
            const int alphaThershold = 10;
            UInt64 pixelCount = 0;
            UInt64 avgAlpha = 0;

            unsafe
            {
                var pixels = (byte*)(void*)scan0;
                for (var y = 0; y < height; y++)
                    for (var x = 0; x < width; x++)
                    {
                        var index = (y * stride) + x * 4;
                        byte r1 = pixels[index];
                        byte g1 = pixels[index + 1];
                        byte b1 = pixels[index + 2];
                        byte a1 = pixels[index + 3];

                        if (a1 <= alphaThershold)
                            continue; //ignore

                        pixelCount++;
                        avgAlpha += a1;

                        var cl = Color.FromArgb(0, r1, g1, b1);
                        double dist = 0;
                        if (!colorDist.ContainsKey(cl))
                        {
                            colorDist.Add(cl, 0);

                            for (int y2 = 0; y2 < height; y2++)
                            {
                                for (int x2 = 0; x2 < width; x2++)
                                {
                                    int index2 = y2 * stride + x2 * 4;
                                    byte r2 = pixels[index2];
                                    byte g2 = pixels[index2 + 1];
                                    byte b2 = pixels[index2 + 2];
                                    byte a2 = pixels[index2 + 3];

                                    if (a2 <= alphaThershold)
                                        continue; //ignore

                                    dist += Math.Sqrt(Math.Pow(r2 - r1, 2) +
                                                      Math.Pow(g2 - g1, 2) +
                                                      Math.Pow(b2 - b1, 2));
                                }
                            }

                            colorDist[cl] = dist;
                        }
                    }
            }

            //clamp alpha
            avgAlpha = avgAlpha / pixelCount;
            if (avgAlpha >= (255 - alphaThershold))
                avgAlpha = 255;

            //take weighted average of top 2% of colors         
            var clrs =
                (from entry in colorDist orderby entry.Value ascending select new { Color = entry.Key, Dist = 1.0 / Math.Max(1, entry.Value) }).ToList
                    ().Take(Math.Max(1, (int)(colorDist.Count * 0.02))).ToList();

            double sumDist = clrs.Sum(x => x.Dist);
            var result = Color.FromArgb((byte)avgAlpha,
                                          (byte)(clrs.Sum(x => x.Color.R * x.Dist) / sumDist),
                                          (byte)(clrs.Sum(x => x.Color.G * x.Dist) / sumDist),
                                          (byte)(clrs.Sum(x => x.Color.B * x.Dist) / sumDist));

            return result;
        }
    }

    public static class BitmapLockExtensions
    {
        public static BitmapDataReleaser Lock(this Bitmap bitmap, Rectangle rect, PixelFormat pixelFormat, ImageLockMode imageLockMode = ImageLockMode.ReadOnly)
        {
            return new BitmapDataReleaser(bitmap, rect, pixelFormat, imageLockMode);
        }

        public class BitmapDataReleaser : IDisposable
        {
            public Bitmap Bitmap { get; private set; }

            public Rectangle Rectangle { get; private set; }

            public PixelFormat PixelFormat { get; private set; }

            public ImageLockMode ImageLockMode { get; private set; }

            public BitmapData Data { get; private set; }

            public BitmapDataReleaser(Bitmap bitmap, Rectangle rectangle, PixelFormat? pixelFormat = null, ImageLockMode imageLockMode = ImageLockMode.ReadOnly)
            {
                this.Bitmap = bitmap;
                this.Rectangle = rectangle;
                this.PixelFormat = pixelFormat ?? bitmap.PixelFormat;
                this.ImageLockMode = imageLockMode;

                this.Data = bitmap.LockBits(Rectangle, ImageLockMode, PixelFormat);
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                Bitmap.UnlockBits(Data);
            }
        }
    }
}