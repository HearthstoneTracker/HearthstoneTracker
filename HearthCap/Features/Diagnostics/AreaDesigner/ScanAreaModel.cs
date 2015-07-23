namespace HearthCap.Features.Diagnostics.AreaDesigner
{
    using System;
    using System.Drawing;
    using System.IO;

    using Caliburn.Micro;

    using HearthCap.Core.GameCapture.HS;

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
            this.Key = scanArea.Key;
            this.X = scanArea.X;
            this.Y = scanArea.Y;
            this.Width = scanArea.Width;
            this.Height = scanArea.Height;
            this.Hash = scanArea.Hash;
            this.ImageLocation = scanArea.Image;
            this.BaseResolution = scanArea.BaseResolution;
            this.Mostly = scanArea.Mostly;
            // this.Image = GetAreaImage(scanArea.Key);
        }

        public string Key
        {
            get
            {
                return this.key;
            }
            set
            {
                if (value == this.key)
                {
                    return;
                }
                this.key = value;
                this.NotifyOfPropertyChange(() => this.Key);
            }
        }

        public int X
        {
            get
            {
                return this.x;
            }
            set
            {
                if (value == this.x)
                {
                    return;
                }
                this.x = value;
                this.NotifyOfPropertyChange(() => this.X);
            }
        }

        public int Y
        {
            get
            {
                return this.y;
            }
            set
            {
                if (value == this.y)
                {
                    return;
                }
                this.y = value;
                this.NotifyOfPropertyChange(() => this.Y);
            }
        }

        public int Height
        {
            get
            {
                return this.height;
            }
            set
            {
                if (value == this.height)
                {
                    return;
                }
                this.height = value;
                this.NotifyOfPropertyChange(() => this.Height);
            }
        }

        public int Width
        {
            get
            {
                return this.width;
            }
            set
            {
                if (value == this.width)
                {
                    return;
                }
                this.width = value;
                this.NotifyOfPropertyChange(() => this.Width);
            }
        }

        public Image Image
        {
            get
            {
                return this.image;
            }
            set
            {
                if (Equals(value, this.image))
                {
                    return;
                }
                this.image = value;
                this.NotifyOfPropertyChange(() => this.Image);
            }
        }

        public string ImageLocation
        {
            get
            {
                return this.imageLocation;
            }
            set
            {
                if (value == this.imageLocation)
                {
                    return;
                }
                this.imageLocation = value;
                this.NotifyOfPropertyChange(() => this.ImageLocation);
            }
        }

        public ulong Hash
        {
            get
            {
                return this.hash;
            }
            set
            {
                if (value == this.hash)
                {
                    return;
                }
                this.hash = value;
                this.NotifyOfPropertyChange(() => this.Hash);
            }
        }

        public int BaseResolution
        {
            get
            {
                return this.baseResolution;
            }
            set
            {
                if (value == this.baseResolution)
                {
                    return;
                }
                this.baseResolution = value;
                this.NotifyOfPropertyChange(() => this.BaseResolution);
            }
        }

        public string Mostly
        {
            get
            {
                return this.mostly;
            }
            set
            {
                if (value == this.mostly)
                {
                    return;
                }
                this.mostly = value;
                this.NotifyOfPropertyChange(() => this.Mostly);
            }
        }
    }
}