// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScanAreaModel.cs" company="">
//   
// </copyright>
// <summary>
//   The scan area model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Diagnostics.AreaDesigner
{
    using System;
    using System.Drawing;
    using System.IO;

    using Caliburn.Micro;

    using HearthCap.Core.GameCapture.HS;

    /// <summary>
    /// The scan area model.
    /// </summary>
    public class ScanAreaModel : PropertyChangedBase
    {
        /// <summary>
        /// The hash.
        /// </summary>
        private ulong hash;

        /// <summary>
        /// The width.
        /// </summary>
        private int width;

        /// <summary>
        /// The height.
        /// </summary>
        private int height;

        /// <summary>
        /// The y.
        /// </summary>
        private int y;

        /// <summary>
        /// The key.
        /// </summary>
        private string key;

        /// <summary>
        /// The x.
        /// </summary>
        private int x;

        /// <summary>
        /// The image.
        /// </summary>
        private Image image;

        /// <summary>
        /// The image location.
        /// </summary>
        private string imageLocation;

        /// <summary>
        /// The base resolution.
        /// </summary>
        private int baseResolution;

        /// <summary>
        /// The mostly.
        /// </summary>
        private string mostly;

        // public static Func<string, Image> GetAreaImage = GetAreaImageDef;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScanAreaModel"/> class.
        /// </summary>
        public ScanAreaModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScanAreaModel"/> class.
        /// </summary>
        /// <param name="scanArea">
        /// The scan area.
        /// </param>
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

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the x.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the y.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the image.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the image location.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the hash.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the base resolution.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the mostly.
        /// </summary>
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

        /// <summary>
        /// The get area image def.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="Image"/>.
        /// </returns>
        private static Image GetAreaImageDef(string key)
        {
            string baseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
            baseDir = Path.Combine(baseDir, "images");
            var filename = key + ".png";
            filename = Path.Combine(baseDir, filename);
            Image result = null;
            try
            {
                result = Image.FromFile(filename);
            }
            catch (Exception ex)
            {
                
            }

            return result;
        }
    }
}