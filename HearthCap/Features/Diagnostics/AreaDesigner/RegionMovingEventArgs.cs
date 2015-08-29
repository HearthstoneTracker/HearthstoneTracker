using System;

namespace HearthCap.Features.Diagnostics.AreaDesigner
{
    public class RegionMovingEventArgs : EventArgs
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:System.EventArgs" /> class.
        /// </summary>
        public RegionMovingEventArgs(double x, double y)
        {
            X = x;
            Y = y;
        }

        public bool Cancel { get; set; }

        public double X { get; protected set; }

        public double Y { get; protected set; }
    }
}
