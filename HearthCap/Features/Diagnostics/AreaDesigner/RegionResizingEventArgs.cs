using System;

namespace HearthCap.Features.Diagnostics.AreaDesigner
{
    public class RegionResizingEventArgs : EventArgs
    {
        public double Width { get; protected set; }

        public double Height { get; protected set; }

        public bool Cancel { get; set; }

        public RegionResizingEventArgs(double width, double height)
        {
            Width = width;
            Height = height;
        }
    }
}
