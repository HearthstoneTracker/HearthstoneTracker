// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RegionMovingEventArgs.cs" company="">
//   
// </copyright>
// <summary>
//   The region moving event args.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Diagnostics.AreaDesigner
{
    using System;

    /// <summary>
    /// The region moving event args.
    /// </summary>
    public class RegionMovingEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegionMovingEventArgs"/> class. 
        /// Initializes a new instance of the <see cref="T:System.EventArgs"/> class.
        /// </summary>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        public RegionMovingEventArgs(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Gets or sets a value indicating whether cancel.
        /// </summary>
        public bool Cancel { get; set; }

        /// <summary>
        /// Gets or sets the x.
        /// </summary>
        public double X { get; protected set; }

        /// <summary>
        /// Gets or sets the y.
        /// </summary>
        public double Y { get; protected set; }
    }
}