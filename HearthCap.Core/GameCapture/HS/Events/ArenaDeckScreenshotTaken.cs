// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArenaDeckScreenshotTaken.cs" company="">
//   
// </copyright>
// <summary>
//   The arena deck screenshot taken.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Core.GameCapture.HS.Events
{
    using System.Drawing;

    /// <summary>
    /// The arena deck screenshot taken.
    /// </summary>
    public class ArenaDeckScreenshotTaken
    {
        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        public Bitmap Image { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArenaDeckScreenshotTaken"/> class.
        /// </summary>
        /// <param name="image">
        /// The image.
        /// </param>
        public ArenaDeckScreenshotTaken(Bitmap image)
        {
            this.Image = image;
        }
    }
}