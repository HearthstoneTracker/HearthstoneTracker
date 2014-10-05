// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeckScreenshotTaken.cs" company="">
//   
// </copyright>
// <summary>
//   The deck screenshot taken.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Core.GameCapture.HS.Events
{
    using System.Drawing;

    /// <summary>
    /// The deck screenshot taken.
    /// </summary>
    public class DeckScreenshotTaken
    {
        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        public Bitmap Image { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeckScreenshotTaken"/> class.
        /// </summary>
        /// <param name="image">
        /// The image.
        /// </param>
        public DeckScreenshotTaken(Bitmap image)
        {
            this.Image = image;
        }
    }
}