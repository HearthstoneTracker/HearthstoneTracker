// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RegionResizingEventArgs.cs" company="">
//   
// </copyright>
// <summary>
//   The region resizing event args.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Diagnostics.AreaDesigner
{
    using System;

    /// <summary>
    /// The region resizing event args.
    /// </summary>
    public class RegionResizingEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        public double Width { get; protected set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        public double Height { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether cancel.
        /// </summary>
        public bool Cancel { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegionResizingEventArgs"/> class.
        /// </summary>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        public RegionResizingEventArgs(double width, double height)
        {
            this.Width = width;
            this.Height = height;
        }
    }
}