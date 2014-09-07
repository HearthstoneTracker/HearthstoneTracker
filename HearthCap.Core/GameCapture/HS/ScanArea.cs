namespace HearthCap.Core.GameCapture.HS
{
    using System.Drawing;

    using HearthCap.Core.Util;

    /// <summary>The scan area.</summary>
    public class ScanArea
    {
        private Rectangle rect;

        public string Key { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public int Height { get; set; }

        public int Width { get; set; }

        public ulong Hash { get; set; }

        public string Image { get; set; }

        public int BaseResolution { get; set; }

        public string Mostly { get; set; }

        public Rectangle Rect
        {
            get
            {
                return new Rectangle(X, Y, Width, Height);
            }
        }
        // public int MatchThreshold { get; set; }
    }
}