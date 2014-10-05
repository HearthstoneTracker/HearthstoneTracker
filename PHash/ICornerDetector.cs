// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICornerDetector.cs" company="">
//   
// </copyright>
// <summary>
//   The CornerDetector interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PHash
{
    using System.Collections.Generic;
    using System.Drawing;

    /// <summary>
    /// The CornerDetector interface.
    /// </summary>
    public interface ICornerDetector
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
        IList<Point> GetCorners(Bitmap bitmap);
    }
}