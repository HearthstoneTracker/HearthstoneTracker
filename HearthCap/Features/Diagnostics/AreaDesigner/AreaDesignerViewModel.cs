using System;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using Caliburn.Micro.Recipes.Filters;
using HearthCap.Core.GameCapture;
using HearthCap.Core.GameCapture.HS;
using HearthCap.Core.GameCapture.Logging.LogEvents;
using HearthCap.Core.Util;
using HearthCap.Shell.Tabs;
using HearthCap.Util;
using Microsoft.Win32;
using PHash;

namespace HearthCap.Features.Diagnostics.AreaDesigner
{
#if DEBUG
    [Export(typeof(ITab))]
#endif
    public class AreaDesignerViewModel :
        TabViewModel,
        IHandle<WindowCaptured>
    {
        private readonly IEventAggregator events;

        private readonly ICaptureEngine captureEngine;

        private readonly BindableCollection<RegionModel> regions;

        private BitmapImage screenshot;

        private RegionModel region;

        private bool capturing = true;

        private ScanAreaModel selectedScanArea;

        private readonly IPerceptualHash hasher;

        private readonly ICornerDetector cornerDetecter;

        private string regionKey;

        private ulong manualHash;

        private string compareHash;

        private int compareHashResult;

        private string mostly;

        private int rightMostCorner;

        [ImportingConstructor]
        public AreaDesignerViewModel(IEventAggregator events,
            IScanAreaProvider scanAreaProvider,
            ICaptureEngine captureEngine,
            IPerceptualHash hasher,
            ICornerDetector cornerDetecter)
        {
            // TODO: use MEF
            this.hasher = hasher;
            this.cornerDetecter = cornerDetecter;
            DisplayName = "Area Designer";
            this.events = events;
            this.captureEngine = captureEngine;
            Order = 2000;
            regions = new BindableCollection<RegionModel>();
            ScanAreasModel = new ScanAreasModel(scanAreaProvider);
            this.events = events;
            events.Subscribe(this);
        }

        public int CompareHashResult
        {
            get { return compareHashResult; }
            set
            {
                if (value == compareHashResult)
                {
                    return;
                }
                compareHashResult = value;
                NotifyOfPropertyChange(() => CompareHashResult);
            }
        }

        public string CompareHash
        {
            get { return compareHash; }
            set
            {
                if (value == compareHash)
                {
                    return;
                }
                compareHash = value;
                NotifyOfPropertyChange(() => CompareHash);
            }
        }

        public int BoardX
        {
            get
            {
                if (Screenshot == null)
                {
                    return 0;
                }

                var res = new Size(Screenshot.PixelWidth, Screenshot.PixelHeight);
                return (int)ResolutionHelper.GetBoardX(res);
                // return 0;
            }
        }

        public double BoardWidth
        {
            get
            {
                if (Screenshot == null)
                {
                    return 0;
                }

                var res = new Size(Screenshot.PixelWidth, Screenshot.PixelHeight);
                return (int)ResolutionHelper.GetBoardWidth(res);
                // return res.Width;
            }
        }

        public int BoardHeight
        {
            get
            {
                if (Screenshot == null)
                {
                    return 0;
                }

                return Screenshot.PixelHeight;
            }
        }

        public bool CanCreateRegion
        {
            get { return Screenshot != null && !String.IsNullOrWhiteSpace(RegionKey); }
        }

        public string RegionKey
        {
            get { return regionKey; }
            set
            {
                if (value == regionKey)
                {
                    return;
                }
                regionKey = value;
                NotifyOfPropertyChange(() => RegionKey);
            }
        }

        public BitmapImage Screenshot
        {
            get { return screenshot; }
            set
            {
                var old = screenshot;
                screenshot = value;
                NotifyOfPropertyChange(() => Screenshot);
                if ((value != null) == HasScreenshot)
                {
                    NotifyOfPropertyChange(() => HasScreenshot);
                    NotifyOfPropertyChange(() => BoardWidth);
                    NotifyOfPropertyChange(() => BoardX);
                    NotifyOfPropertyChange(() => BoardHeight);
                }
                if (old != null
                    && value != null)
                {
                    if (old.PixelWidth != value.PixelWidth)
                    {
                        NotifyOfPropertyChange(() => BoardWidth);
                        NotifyOfPropertyChange(() => BoardX);
                    }
                    if (old.PixelHeight != value.PixelHeight)
                    {
                        NotifyOfPropertyChange(() => BoardHeight);
                    }
                }
                if (value != null
                    && ScanAreasModel != null
                    && ScanAreasModel.BaseResolution != value.PixelHeight)
                {
                    ScanAreasModel.BaseResolution = value.PixelHeight;
                }
            }
        }

        public RegionModel Region
        {
            get { return region; }
            set
            {
                if (Equals(value, region))
                {
                    return;
                }
                region = value;
                NotifyOfPropertyChange(() => Region);
            }
        }

        public IObservableCollection<RegionModel> Regions
        {
            get { return regions; }
        }

        public ScanAreasModel ScanAreasModel { get; private set; }

        public ScanAreaModel SelectedScanArea
        {
            get { return selectedScanArea; }
            set
            {
                if (Equals(value, selectedScanArea))
                {
                    return;
                }
                selectedScanArea = value;
                UpdateScanArea();
                NotifyOfPropertyChange(() => SelectedScanArea);
            }
        }

        public bool HasScreenshot
        {
            get { return Screenshot != null; }
        }

        public ulong ManualHash
        {
            get { return manualHash; }
            set
            {
                if (value == manualHash)
                {
                    return;
                }
                manualHash = value;
                NotifyOfPropertyChange(() => ManualHash);
            }
        }

        public async void SaveAreas()
        {
            await ScanAreasModel.Save();
        }

        [Dependencies("Screenshot", "RegionKey")]
        public void CreateRegion()
        {
            if (Screenshot == null)
            {
                return;
            }
            if (String.IsNullOrWhiteSpace(RegionKey))
            {
                return;
            }

            // TODO: show error
            if (ScanAreasModel.Areas.Any(x => x.Key == RegionKey))
            {
                return;
            }

            var model = ScanAreasModel.AddArea(RegionKey);
            if (model != null)
            {
                SelectedScanArea = model;
                RegionKey = String.Empty;
            }
        }

        public void UpdateHash()
        {
            if (SelectedScanArea == null
                || Screenshot == null)
            {
                return;
            }
            var model = SelectedScanArea;
            var image = ImageHelper.BitmapImage2Bitmap(Screenshot);
            var roi = image.Clone(new Rectangle(Region.XPos + BoardX, Region.YPos, Region.Width, Region.Height), image.PixelFormat);
            var hash = hasher.Create(roi);

            // var filename = AppDomain.CurrentDomain.BaseDirectory + "\\data\\images\\" + model.Key + ".png";
            // roi.Save(filename, ImageFormat.Png);

            image.Dispose();
            model.Hash = hash;
            model.Image = roi;

            if (!String.IsNullOrWhiteSpace(CompareHash))
            {
                CompareHashResult = PerceptualHash.HammingDistance(model.Hash, Convert.ToUInt64(CompareHash));
            }

            var color = roi.GetDominantColor();
            Mostly = String.Format("R: {0} G: {1} B: {2}", color.R, color.G, color.B);

            var corners = cornerDetecter.GetCorners(roi);
            var right = 0;
            foreach (var c in corners)
            {
                if (c.X > right)
                {
                    right = c.X;
                }
            }
            RightMostCorner = right;
        }

        public void ResizeToRightMostCorner()
        {
            var image = ImageHelper.BitmapImage2Bitmap(Screenshot);
            var roi = image.Clone(new Rectangle(Region.XPos + BoardX, Region.YPos, Region.Width, Region.Height), image.PixelFormat);
            var corners = cornerDetecter.GetCorners(roi);
            var right = 0;
            foreach (var c in corners)
            {
                if (c.X > right)
                {
                    right = c.X;
                }
            }
            RightMostCorner = right;
            Region.Width = (Region.XPos + right);
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

        public int RightMostCorner
        {
            get { return rightMostCorner; }
            set
            {
                if (value == rightMostCorner)
                {
                    return;
                }
                rightMostCorner = value;
                NotifyOfPropertyChange(() => RightMostCorner);
            }
        }

        public void OpenImage()
        {
            var dlg = new OpenFileDialog
                {
                    FileName = "area",
                    DefaultExt = ".png",
                    Filter = "PNG Images|*.png"
                };

            var result = dlg.ShowDialog();
            if (result == true)
            {
                var filename = dlg.FileName;
                var img = new Bitmap(Image.FromFile(filename));
                var hash = hasher.Create(img);
                ManualHash = hash;
            }
        }

        public void UpdateManualHash()
        {
            if (SelectedScanArea == null)
            {
                return;
            }
            var model = SelectedScanArea;
            var hash = ManualHash;
            model.Hash = hash;
        }

        public void SaveRegionAs()
        {
            var dlg = new OpenFileDialog
                {
                    FileName = "area",
                    DefaultExt = ".png",
                    Filter = "PNG Images|*.png"
                };

            var result = dlg.ShowDialog();
            if (result == true)
            {
                var filename = dlg.FileName;

                var image = ImageHelper.BitmapImage2Bitmap(Screenshot);
                var roi = image.Clone(new Rectangle(Region.XPos + BoardX, Region.YPos, Region.Width, Region.Height), image.PixelFormat);
                roi.Save(filename, ImageFormat.Png);

                //var cropped = new CroppedBitmap(this.Screenshot, new Int32Rect((int)this.Region.XPos, (int)this.Region.YPos, (int)this.Region.Width, (int)this.Region.Height));
                //var encoder = new PngBitmapEncoder();
                //using (var fs = new FileStream(filename, FileMode.Create, FileAccess.Write))
                //{
                //    encoder.Frames.Add(BitmapFrame.Create(cropped));
                //    encoder.Save(fs);
                //    fs.Flush();
                //    fs.Close();
                //}
            }
        }

        public void ToggleCapture()
        {
            capturing = !capturing;
        }

        private void UpdateScanArea()
        {
            if (SelectedScanArea == null
                || Screenshot == null)
            {
                return;
            }

            var area = SelectedScanArea;
            Regions.Clear();

            var region = new RegionModel
                {
                    XPos = area.X,
                    YPos = area.Y,
                    Height = area.Height,
                    Width = area.Width,
                    MinHeight = 8,
                    MinWidth = 8
                };
            region.Moving += (sender, args) =>
                {
                    if (args.X + region.Width > BoardWidth)
                    {
                        args.Cancel = true;
                        return;
                    }
                    if (args.Y + region.Height > BoardHeight)
                    {
                        args.Cancel = true;
                        return;
                    }
                    SelectedScanArea.X = (int)args.X;
                    SelectedScanArea.Y = (int)args.Y;
                };
            region.Resizing += (sender, args) =>
                {
                    if (region.XPos + args.Width > BoardWidth)
                    {
                        args.Cancel = true;
                        return;
                    }
                    if (region.YPos + args.Height > BoardHeight)
                    {
                        args.Cancel = true;
                        return;
                    }
                    SelectedScanArea.Width = (int)args.Width;
                    SelectedScanArea.Height = (int)args.Height;
                };
            Region = region;
            Regions.Add(region);
        }

        /// <summary>
        ///     Called when activating.
        /// </summary>
        protected override void OnActivate()
        {
            captureEngine.PublishCapturedWindow = true;
        }

        /// <summary>
        ///     Called when deactivating.
        /// </summary>
        /// <param name="close">Inidicates whether this instance will be closed.</param>
        protected override void OnDeactivate(bool close)
        {
            captureEngine.PublishCapturedWindow = false;
        }

        /// <summary>
        ///     Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(WindowCaptured message)
        {
            Execute.OnUIThread(
                () =>
                    {
                        if (!capturing)
                        {
                            return;
                        }

                        // this.Screenshot = ImageHelper.Bitmap2BitmapSource(message.Data)
                        var ms = new MemoryStream();
                        var bi = new BitmapImage();
                        bi.BeginInit();
                        bi.CacheOption = BitmapCacheOption.OnLoad;
                        message.Data.Save(ms, ImageFormat.Bmp);
                        ms.Seek(0, SeekOrigin.Begin);
                        bi.StreamSource = ms;
                        bi.EndInit();
                        Screenshot = bi;
                        ms.Dispose();
                        message.Data.Dispose();
                    });
        }
    }
}
