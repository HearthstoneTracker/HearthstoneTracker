namespace HearthCap.Features.Diagnostics.AreaDesigner
{
    using System;

    public class RegionResizingEventArgs : EventArgs
    {
        public double Width { get; protected set; }

        public double Height { get; protected set; }

        public bool Cancel { get; set; }

        public RegionResizingEventArgs(double width, double height)
        {
            this.Width = width;
            this.Height = height;
        }
    }
}