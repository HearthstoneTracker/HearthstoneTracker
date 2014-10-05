// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScanArea.cs" company="">
//   
// </copyright>
// <summary>
//   The scan area.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Core.GameCapture.HS
{
    using System.Drawing;

    /// <summary>The scan area.</summary>
    public class ScanArea
    {
        /// <summary>
        /// The rect.
        /// </summary>
        private Rectangle rect;

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the x.
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Gets or sets the y.
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the hash.
        /// </summary>
        public ulong Hash { get; set; }

        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// Gets or sets the base resolution.
        /// </summary>
        public int BaseResolution { get; set; }

        /// <summary>
        /// Gets or sets the mostly.
        /// </summary>
        public string Mostly { get; set; }

        /// <summary>
        /// Gets the rect.
        /// </summary>
        public Rectangle Rect
        {
            get
            {
                return new Rectangle(this.X, this.Y, this.Width, this.Height);
            }
        }

        // public int MatchThreshold { get; set; }
    }
}