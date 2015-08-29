using System;
using System.Drawing.Imaging;
using AForge.Imaging;
using AForge.Imaging.Filters;
using MathNet.Numerics.LinearAlgebra.Double;

namespace PHash.AForge
{
    public class AForgePerceptualHash : PerceptualHash
    {
        private const int dctsize = 8;

        public static IInPlaceFilter Filter = new Convolution(Kernel.Create(3, 3, 1), 9);

        public static IFilter ExtractChannel = new YCbCrExtractYChannel();

        public static IFilter Resize = new ResizeNearestNeighbor(32, 32);

        public override ulong Create(BitmapData image)
        {
            var data = new DenseMatrix(32, 32);

            using (var unmanaged = new UnmanagedImage(image))
            {
                using (var filtered = ExtractChannel.Apply(unmanaged))
                {
                    Filter.ApplyInPlace(filtered);

                    using (var imgdata = Resize.Apply(filtered))
                    {
                        unsafe
                        {
                            var src = (byte*)imgdata.ImageData.ToPointer();
                            var offset = imgdata.Stride - imgdata.Width;
                            var width = imgdata.Width;
                            var height = imgdata.Height;
                            for (var y = 0; y < height; y++)
                            {
                                for (var x = 0; x < width; x++, src++)
                                {
                                    data.At(y, x, (float)*src / 255);
                                }
                                src += offset;
                            }
                        }
                    }
                }
            }

            var dct = DctMatrix.FastDCT(data);

            var vals = new double[dctsize * dctsize];
            var valscount = 0;
            for (var r = 1; r <= dctsize; r++)
            {
                for (var c = 1; c <= dctsize; c++)
                {
                    vals[valscount] = dct.At(r, c);
                    ++valscount;
                }
            }

            var sorted = new double[dctsize * dctsize];
            Array.Copy(vals, sorted, vals.Length);
            Array.Sort(sorted);

            var mid = dctsize * dctsize / 2;
            var median = (sorted[mid - 1] + sorted[mid]) / 2d;

            ulong index = 1;
            ulong result = 0;
            for (var i = 0; i < dctsize * dctsize; i++)
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
