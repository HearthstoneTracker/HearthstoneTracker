using System.Drawing;

namespace HearthCap.Core.GameCapture.HS
{
    /// <summary>The scan area.</summary>
    public class ScanArea
    {
        private Rectangle _rectangle;

        public string Key { get; set; }

        public int X
        {
            get { return _rectangle.X; }
            set { _rectangle.X = value; }
        }

        public int Y
        {
            get { return _rectangle.Y; }
            set { _rectangle.Y = value; }
        }

        public int Height
        {
            get { return _rectangle.Height; }
            set { _rectangle.Height = value; }
        }

        public int Width
        {
            get { return _rectangle.Width; }
            set { _rectangle.Width = value; }
        }

        public ulong Hash { get; set; }

        public string Image { get; set; }

        public int BaseResolution { get; set; }

        public string Mostly { get; set; }

        public Rectangle Rectangle
        {
            get { return _rectangle; }
        }
    }
}
