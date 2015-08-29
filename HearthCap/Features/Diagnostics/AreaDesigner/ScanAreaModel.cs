using System.Drawing;
using Caliburn.Micro;
using HearthCap.Core.GameCapture.HS;

namespace HearthCap.Features.Diagnostics.AreaDesigner
{
    public class ScanAreaModel : PropertyChangedBase
    {
        private ulong hash;

        private int width;

        private int height;

        private int y;

        private string key;

        private int x;

        private Image image;

        private string imageLocation;

        private int baseResolution;

        private string mostly;

        // public static Func<string, Image> GetAreaImage = GetAreaImageDef;

        public ScanAreaModel()
        {
        }

        public ScanAreaModel(ScanArea scanArea)
        {
            Key = scanArea.Key;
            X = scanArea.X;
            Y = scanArea.Y;
            Width = scanArea.Width;
            Height = scanArea.Height;
            Hash = scanArea.Hash;
            ImageLocation = scanArea.Image;
            BaseResolution = scanArea.BaseResolution;
            Mostly = scanArea.Mostly;
            // this.Image = GetAreaImage(scanArea.Key);
        }

        public string Key
        {
            get { return key; }
            set
            {
                if (value == key)
                {
                    return;
                }
                key = value;
                NotifyOfPropertyChange(() => Key);
            }
        }

        public int X
        {
            get { return x; }
            set
            {
                if (value == x)
                {
                    return;
                }
                x = value;
                NotifyOfPropertyChange(() => X);
            }
        }

        public int Y
        {
            get { return y; }
            set
            {
                if (value == y)
                {
                    return;
                }
                y = value;
                NotifyOfPropertyChange(() => Y);
            }
        }

        public int Height
        {
            get { return height; }
            set
            {
                if (value == height)
                {
                    return;
                }
                height = value;
                NotifyOfPropertyChange(() => Height);
            }
        }

        public int Width
        {
            get { return width; }
            set
            {
                if (value == width)
                {
                    return;
                }
                width = value;
                NotifyOfPropertyChange(() => Width);
            }
        }

        public Image Image
        {
            get { return image; }
            set
            {
                if (Equals(value, image))
                {
                    return;
                }
                image = value;
                NotifyOfPropertyChange(() => Image);
            }
        }

        public string ImageLocation
        {
            get { return imageLocation; }
            set
            {
                if (value == imageLocation)
                {
                    return;
                }
                imageLocation = value;
                NotifyOfPropertyChange(() => ImageLocation);
            }
        }

        public ulong Hash
        {
            get { return hash; }
            set
            {
                if (value == hash)
                {
                    return;
                }
                hash = value;
                NotifyOfPropertyChange(() => Hash);
            }
        }

        public int BaseResolution
        {
            get { return baseResolution; }
            set
            {
                if (value == baseResolution)
                {
                    return;
                }
                baseResolution = value;
                NotifyOfPropertyChange(() => BaseResolution);
            }
        }

        public string Mostly
        {
            get { return mostly; }
            set
            {
                if (value == mostly)
                {
                    return;
                }
                mostly = value;
                NotifyOfPropertyChange(() => Mostly);
            }
        }
    }
}
