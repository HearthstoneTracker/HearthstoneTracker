// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPerceptualHash.cs" company="">
//   
// </copyright>
// <summary>
//   The PerceptualHash interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PHash
{
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// The PerceptualHash interface.
    /// </summary>
    public interface IPerceptualHash
    {
        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="image">
        /// The image.
        /// </param>
        /// <returns>
        /// The <see cref="ulong"/>.
        /// </returns>
        ulong Create(Bitmap image);

        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="image">
        /// The image.
        /// </param>
        /// <returns>
        /// The <see cref="ulong"/>.
        /// </returns>
        ulong Create(BitmapData image);
    }
}