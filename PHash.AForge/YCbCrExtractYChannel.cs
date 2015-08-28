namespace PHash.AForge
{
    using System;
    using System.Collections.Generic;
    using System.Drawing.Imaging;

    using global::AForge.Imaging;
    using global::AForge.Imaging.Filters;

    public class YCbCrExtractYChannel : BaseFilter
    {
        // private format translation dictionary
        private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

        /// <summary>
        /// Format translations dictionary.
        /// </summary>
        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get { return formatTranslations; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="YCbCrExtractYChannel"/> class.
        /// </summary>
        public YCbCrExtractYChannel()
        {
            // initialize format translation dictionary
            formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format8bppIndexed;
            formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format8bppIndexed;
            formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format8bppIndexed;
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="sourceData">Source image data.</param>
        /// <param name="destinationData">Destination image data.</param>
        protected override unsafe void ProcessFilter(UnmanagedImage sourceData, UnmanagedImage destinationData)
        {
            int pixelSize = System.Drawing.Image.GetPixelFormatSize(sourceData.PixelFormat) / 8;

            // get width and height
            int width = sourceData.Width;
            int height = sourceData.Height;

            int srcOffset = sourceData.Stride - width * pixelSize;
            int dstOffset = destinationData.Stride - width;

            // do the job
            byte* src = (byte*)sourceData.ImageData.ToPointer();
            byte* dst = (byte*)destinationData.ImageData.ToPointer();

            // for each row
            for (int y = 0; y < height; y++)
            {
                // for each pixel
                for (int x = 0; x < width; x++, src += pixelSize, dst++)
                {
                    float r = (float)src[RGB.R] / 255;
                    float g = (float)src[RGB.G] / 255;
                    float b = (float)src[RGB.B] / 255;

                    var yindex = (float)(0.2989 * r + 0.5866 * g + 0.1145 * b);

                    *dst = (byte)(yindex * 255);
                }

                src += srcOffset;
                dst += dstOffset;
            }
        }
    }
}