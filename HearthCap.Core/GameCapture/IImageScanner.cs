// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IImageScanner.cs" company="">
//   
// </copyright>
// <summary>
//   The ImageScanner interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Core.GameCapture
{
    using System.Drawing;

    /// <summary>
    /// The ImageScanner interface.
    /// </summary>
    public interface IImageScanner
    {
        /// <summary>
        /// The run.
        /// </summary>
        /// <param name="img">
        /// The img.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        void Run(Bitmap img, object context);

        /// <summary>
        /// The stop.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        void Stop(object context);
    }
}