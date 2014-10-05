// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CornerDetector.cs" company="">
//   
// </copyright>
// <summary>
//   The corner detector.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PHash.AForge
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    using global::AForge.Imaging;

    using Point = System.Drawing.Point;

    /// <summary>
    /// The corner detector.
    /// </summary>
    public class CornerDetector : ICornerDetector
    {
        /// <summary>
        /// The get corners.
        /// </summary>
        /// <param name="bitmap">
        /// The bitmap.
        /// </param>
        /// <returns>
        /// The <see cref="IList"/>.
        /// </returns>
        public IList<Point> GetCorners(Bitmap bitmap)
        {
            var mcd = new MoravecCornersDetector();
            var corners = mcd.ProcessImage(bitmap);

            return new List<Point>(corners.Select(x=>new Point(x.X, x.Y)));
        }
    }
}