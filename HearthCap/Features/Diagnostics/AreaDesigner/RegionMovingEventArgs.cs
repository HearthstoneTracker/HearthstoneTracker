namespace HearthCap.Features.Diagnostics.AreaDesigner
{
    using System;

    public class RegionMovingEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.EventArgs"/> class.
        /// </summary>
        public RegionMovingEventArgs(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        public bool Cancel { get; set; }

        public double X { get; protected set; }

        public double Y { get; protected set; }
    }
}