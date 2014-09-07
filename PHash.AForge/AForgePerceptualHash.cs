namespace PHash.AForge
{
    using System.Collections.Generic;
    using System.Drawing.Imaging;

    using global::AForge.Imaging;
    using global::AForge.Imaging.Filters;

    using MathNet.Numerics.LinearAlgebra.Double;

    public class AForgePerceptualHash : PerceptualHash
    {
        public static int[,] Filter = Kernel.Create(3, 3, 1);
        public static YCbCrExtractChannel ExtractChannel = new YCbCrExtractChannel(YCbCr.YIndex);

        public override ulong Create(BitmapData image)
        {
            var data = new DenseMatrix(32, 32);
            using (var unmanaged = UnmanagedImage.FromManagedImage(image))
            using (var filtered = ExtractChannel.Apply(unmanaged))
            {
                new Convolution(Filter, 9).ApplyInPlace(filtered);
                using (var imgdata = new ResizeNearestNeighbor(32, 32).Apply(filtered))
                {
                    unsafe
                    {
                        byte* src = (byte*)imgdata.ImageData.ToPointer();
                        int offset = imgdata.Stride - imgdata.Width;
                        int width = imgdata.Width;
                        int height = imgdata.Height;
                        for (int y = 0; y < height; y++)
                        {
                            for (int x = 0; x < width; x++, src++)
                            {
                                data.At(y, x, (float)*src / 255);
                            }
                            src += offset;
                        }
                    }
                }
            }

            var dct = DctMatrix.FastDCT(data);

            int dctsize = 8;
            var vals = new List<double>();
            for (int r = 1; r <= dctsize; r++)
            {
                for (int c = 1; c <= dctsize; c++)
                {
                    vals.Add(dct[r, c]);
                }
            }

            var sorted = new List<double>(vals);
            sorted.Sort();
            var mid = dctsize * dctsize / 2;
            double median = (sorted[mid - 1] + sorted[mid]) / 2d;

            ulong index = 1;
            ulong result = 0;
            for (int i = 0; i < dctsize * dctsize; i++)
            {
                if (vals[i] > median)
                {
                    result |= index;
                }

                index = index << 1;
            }

            return result;
        }
    }
}