// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResolutionHelper.cs" company="">
//   
// </copyright>
// <summary>
//   The resolution helper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Core.GameCapture.HS
{
    using System;
    using System.Drawing;

    /// <summary>
    /// The resolution helper.
    /// </summary>
    public static class ResolutionHelper
    {
        /// <summary>
        /// The up.
        /// </summary>
        static Func<decimal, decimal> up = p => Math.Round(p, 0);

        /// <summary>
        /// The down.
        /// </summary>
        static Func<decimal, decimal> down = p => Math.Round(p, 0);

        /// <summary>
        /// The correct rectangle.
        /// </summary>
        /// <param name="resolution">
        /// The resolution.
        /// </param>
        /// <param name="rect">
        /// The rect.
        /// </param>
        /// <param name="baseResolution">
        /// The base resolution.
        /// </param>
        /// <returns>
        /// The <see cref="Rectangle"/>.
        /// </returns>
        public static Rectangle CorrectRectangle(Size resolution, Rectangle rect, int baseResolution)
        {
            var scaling = GetScaleFactor(resolution, baseResolution);

            // if (scaling < 1)
            // {
            // up = (p) => Math.Ceiling(p);
            // down = (p) => Math.Floor(p);
            // }
            // else if (scaling > 1)
            // {
            // up = (p) => Math.Floor(p);
            // down = (p) => Math.Ceiling(p);
            // }
            var x = (int)up(scaling * rect.X + GetBoardX(resolution));
            var y = (int)up(scaling * rect.Y);
            var width = (int)down(rect.Width * scaling);
            var height = (int)down(rect.Height * scaling);

            return new Rectangle(x, y, width, height);
        }

        /// <summary>
        /// The get scale factor.
        /// </summary>
        /// <param name="resolution">
        /// The resolution.
        /// </param>
        /// <param name="baseResolution">
        /// The base resolution.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        public static decimal GetScaleFactor(Size resolution, int baseResolution)
        {
            if (resolution.Height == baseResolution)
            {
                return 1;
            }

            return resolution.Height / (decimal)baseResolution;
        }

        /// <summary>
        /// The get board width.
        /// </summary>
        /// <param name="resolution">
        /// The resolution.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        public static decimal GetBoardWidth(Size resolution)
        {
            return resolution.Height / (decimal)3 * 4;
        }

        /// <summary>
        /// The get board x.
        /// </summary>
        /// <param name="resolution">
        /// The resolution.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        public static decimal GetBoardX(Size resolution)
        {
            decimal x = (resolution.Width - GetBoardWidth(resolution)) / 2;
            return x;
        }

        /// <summary>
        /// The correct size.
        /// </summary>
        /// <param name="resolution">
        /// The resolution.
        /// </param>
        /// <param name="rect">
        /// The rect.
        /// </param>
        /// <param name="baseResolution">
        /// The base resolution.
        /// </param>
        /// <returns>
        /// The <see cref="Size"/>.
        /// </returns>
        public static Size CorrectSize(Size resolution, Size rect, int baseResolution)
        {
            var scaling = GetScaleFactor(resolution, baseResolution);

            // if (scaling < 1)
            // {
            // up = (p) => Math.Ceiling(p);
            // down = (p) => Math.Floor(p);
            // }
            // else if (scaling > 1)
            // {
            // up = (p) => Math.Floor(p);
            // down = (p) => Math.Ceiling(p);
            // }
            var width = (int)down(rect.Width * scaling);
            var height = (int)down(rect.Height * scaling);
            return new Size(width, height);
        }

        /// <summary>
        /// The correct points.
        /// </summary>
        /// <param name="resolution">
        /// The resolution.
        /// </param>
        /// <param name="rect">
        /// The rect.
        /// </param>
        /// <param name="baseResolution">
        /// The base resolution.
        /// </param>
        /// <returns>
        /// The <see cref="Rectangle"/>.
        /// </returns>
        public static Rectangle CorrectPoints(Size resolution, Rectangle rect, int baseResolution)
        {
            var scaling = GetScaleFactor(resolution, baseResolution);

            // if (scaling < 1)
            // {
            // up = (p) => Math.Ceiling(p);
            // down = (p) => Math.Floor(p);
            // }
            // else if (scaling > 1)
            // {
            // up = (p) => Math.Floor(p);
            // down = (p) => Math.Ceiling(p);
            // }
            var x = (int)(up(scaling * rect.X) + GetBoardX(resolution));
            var y = (int)up(scaling * rect.Y);
            return new Rectangle(x, y, rect.Width, rect.Height);
        }
    }
}