// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AreaDesignerViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The area designer view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Diagnostics.AreaDesigner
{
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

    using global::PHash;

    using Size = System.Drawing.Size;

#if DEBUG

    /// <summary>
    /// The area designer view model.
    /// </summary>
    [Export(typeof(ITab))]
#endif
    public class AreaDesignerViewModel : 
        TabViewModel, 
        IHandle<WindowCaptured>
    {
        /// <summary>
        /// The events.
        /// </summary>
        private readonly IEventAggregator events;

        /// <summary>
        /// The capture engine.
        /// </summary>
        private readonly ICaptureEngine captureEngine;

        /// <summary>
        /// The regions.
        /// </summary>
        private BindableCollection<RegionModel> regions;

        /// <summary>
        /// The screenshot.
        /// </summary>
        private BitmapImage screenshot;

        /// <summary>
        /// The region.
        /// </summary>
        private RegionModel region;

        /// <summary>
        /// The overlay.
        /// </summary>
        private OverlayRegionModel overlay;

        /// <summary>
        /// The capturing.
        /// </summary>
        private bool capturing = true;

        /// <summary>
        /// The scan areas model.
        /// </summary>
        private ScanAreasModel scanAreasModel;

        /// <summary>
        /// The selected scan area.
        /// </summary>
        private ScanAreaModel selectedScanArea;

        /// <summary>
        /// The hasher.
        /// </summary>
        private IPerceptualHash hasher;

        /// <summary>
        /// The corner detecter.
        /// </summary>
        private readonly ICornerDetector cornerDetecter;

        /// <summary>
        /// The region key.
        /// </summary>
        private string regionKey;

        /// <summary>
        /// The manual hash.
        /// </summary>
        private ulong manualHash;

        /// <summary>
        /// The compare hash.
        /// </summary>
        private string compareHash;

        /// <summary>
        /// The compare hash result.
        /// </summary>
        private int compareHashResult;

        /// <summary>
        /// The mostly.
        /// </summary>
        private string mostly;

        /// <summary>
        /// The right most corner.
        /// </summary>
        private int rightMostCorner;

        /// <summary>
        /// Initializes a new instance of the <see cref="AreaDesignerViewModel"/> class.
        /// </summary>
        /// <param name="events">
        /// The events.
        /// </param>
        /// <param name="scanAreaProvider">
        /// The scan area provider.
        /// </param>
        /// <param name="captureEngine">
        /// The capture engine.
        /// </param>
        /// <param name="hasher">
        /// The hasher.
        /// </param>
        /// <param name="cornerDetecter">
        /// The corner detecter.
        /// </param>
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
            this.DisplayName = "Area Designer";
            this.events = events;
            this.captureEngine = captureEngine;
            this.Order = 2000;
            this.regions = new BindableCollection<RegionModel>();
            this.scanAreasModel = new ScanAreasModel(scanAreaProvider);
            this.events = events;
            events.Subscribe(this);
        }

        /// <summary>
        /// Gets or sets the compare hash result.
        /// </summary>
        public int CompareHashResult
        {
            get
            {
                return this.compareHashResult;
            }

            set
            {
                if (value == this.compareHashResult)
                {
                    return;
                }

                this.compareHashResult = value;
                this.NotifyOfPropertyChange(() => this.CompareHashResult);
            }
        }

        /// <summary>
        /// Gets or sets the compare hash.
        /// </summary>
        public string CompareHash
        {
            get
            {
                return this.compareHash;
            }

            set
            {
                if (value == this.compareHash)
                {
                    return;
                }

                this.compareHash = value;
                this.NotifyOfPropertyChange(() => this.CompareHash);
            }
        }

        /// <summary>
        /// Gets the board x.
        /// </summary>
        public int BoardX
        {
            get
            {
                if (this.Screenshot == null)
                {
                    return 0;
                }

                var res = new Size(this.Screenshot.PixelWidth, this.Screenshot.PixelHeight);
                return (int)ResolutionHelper.GetBoardX(res);

                // return 0;
            }
        }

        /// <summary>
        /// Gets the board width.
        /// </summary>
        public double BoardWidth
        {
            get
            {
                if (this.Screenshot == null)
                {
                    return 0;
                }

                var res = new Size(this.Screenshot.PixelWidth, this.Screenshot.PixelHeight);
                return (int)ResolutionHelper.GetBoardWidth(res);                
                // return res.Width;
            }
        }

        /// <summary>
        /// Gets the board height.
        /// </summary>
        public int BoardHeight
        {
            get
            {
                if (this.Screenshot == null)
                {
                    return 0;
                }

                return this.Screenshot.PixelHeight;
            }
        }

        /// <summary>
        /// Gets a value indicating whether can create region.
        /// </summary>
        public bool CanCreateRegion
        {
            get
            {
                return this.Screenshot != null && !string.IsNullOrWhiteSpace(this.RegionKey);
            }
        }

        /// <summary>
        /// Gets or sets the region key.
        /// </summary>
        public string RegionKey
        {
            get
            {
                return this.regionKey;
            }

            set
            {
                if (value == this.regionKey)
                {
                    return;
                }

                this.regionKey = value;
                this.NotifyOfPropertyChange(() => this.RegionKey);
            }
        }

        /// <summary>
        /// Gets or sets the screenshot.
        /// </summary>
        public BitmapImage Screenshot
        {
            get { return this.screenshot; }
            set
            {
                var old = this.screenshot;
                this.screenshot = value;
                this.NotifyOfPropertyChange(() => this.Screenshot);
                if ((value != null) == this.HasScreenshot)
                {
                    this.NotifyOfPropertyChange(() => this.HasScreenshot);
                    this.NotifyOfPropertyChange(() => this.BoardWidth);
                    this.NotifyOfPropertyChange(() => this.BoardX);
                    this.NotifyOfPropertyChange(() => this.BoardHeight);
                }

                if (old != null && value != null)
                {
                    if (old.PixelWidth != value.PixelWidth)
                    {
                        this.NotifyOfPropertyChange(() => this.BoardWidth);
                        this.NotifyOfPropertyChange(() => this.BoardX);                        
                    }

                    if (old.PixelHeight != value.PixelHeight)
                    {
                        this.NotifyOfPropertyChange(() => this.BoardHeight);
                    }
                }

                if (value != null)
                {
                    this.scanAreasModel.BaseResolution = value.PixelHeight;
                }
            }
        }

        /// <summary>
        /// Gets or sets the region.
        /// </summary>
        public RegionModel Region
        {
            get
            {
                return this.region;
            }

            set
            {
                if (Equals(value, this.region))
                {
                    return;
                }

                this.region = value;
                this.NotifyOfPropertyChange(() => this.Region);
            }
        }

        /// <summary>
        /// Gets the regions.
        /// </summary>
        public IObservableCollection<RegionModel> Regions
        {
            get
            {
                return this.regions;
            }
        }

        /// <summary>
        /// Gets the scan areas model.
        /// </summary>
        public ScanAreasModel ScanAreasModel
        {
            get
            {
                return this.scanAreasModel;
            }
        }

        /// <summary>
        /// Gets or sets the selected scan area.
        /// </summary>
        public ScanAreaModel SelectedScanArea
        {
            get
            {
                return this.selectedScanArea;
            }

            set
            {
                if (Equals(value, this.selectedScanArea))
                {
                    return;
                }

                this.selectedScanArea = value;
                this.UpdateScanArea();
                this.NotifyOfPropertyChange(() => this.SelectedScanArea);
            }
        }

        /// <summary>
        /// Gets a value indicating whether has screenshot.
        /// </summary>
        public bool HasScreenshot
        {
            get
            {
                return this.Screenshot != null;
            }
        }

        /// <summary>
        /// Gets or sets the manual hash.
        /// </summary>
        public ulong ManualHash
        {
            get
            {
                return this.manualHash;
            }

            set
            {
                if (value == this.manualHash)
                {
                    return;
                }

                this.manualHash = value;
                this.NotifyOfPropertyChange(() => this.ManualHash);
            }
        }

        /// <summary>
        /// The save areas.
        /// </summary>
        public async void SaveAreas()
        {
            await this.ScanAreasModel.Save();
        }

        /// <summary>
        /// The create region.
        /// </summary>
        [Dependencies("Screenshot", "RegionKey")]
        public void CreateRegion()
        {
            if (this.Screenshot == null) return;
            if (string.IsNullOrWhiteSpace(this.RegionKey)) return;
            
            // TODO: show error
            if (this.scanAreasModel.Areas.Any(x => x.Key == this.RegionKey)) return;

            var model = this.scanAreasModel.AddArea(this.RegionKey);
            if (model != null)
            {
                this.SelectedScanArea = model;
                this.RegionKey = string.Empty;
            }
        }

        /// <summary>
        /// The update hash.
        /// </summary>
        public void UpdateHash()
        {
            if (this.SelectedScanArea == null || this.Screenshot == null)
            {
                return;
            }

            var model = this.SelectedScanArea;
            var image = ImageHelper.BitmapImage2Bitmap(this.Screenshot);
            var roi = image.Clone(new Rectangle(this.Region.XPos + this.BoardX, this.Region.YPos, this.Region.Width, this.Region.Height), image.PixelFormat);
            var hash = this.hasher.Create(roi);

            var filename = AppDomain.CurrentDomain.BaseDirectory + "\\data\\images\\" + model.Key + ".png";
            roi.Save(filename, ImageFormat.Png);

            image.Dispose();
            model.Hash = hash;
            model.Image = roi;

            if (!string.IsNullOrWhiteSpace(this.CompareHash))
            {
                this.CompareHashResult = PerceptualHash.HammingDistance(model.Hash, Convert.ToUInt64(this.CompareHash));
            }

            var color = roi.GetDominantColor();
            this.Mostly = string.Format("R: {0} G: {1} B: {2}", color.R, color.G, color.B);

            var corners = this.cornerDetecter.GetCorners(roi);
            var right = 0;
            foreach (var c in corners)
            {
                if (c.X > right)
                {
                    right = c.X;
                }
            }

            this.RightMostCorner = right;
        }

        /// <summary>
        /// The resize to right most corner.
        /// </summary>
        public void ResizeToRightMostCorner()
        {
            var image = ImageHelper.BitmapImage2Bitmap(this.Screenshot);
            var roi = image.Clone(new Rectangle(this.Region.XPos + this.BoardX, this.Region.YPos, this.Region.Width, this.Region.Height), image.PixelFormat);
            var corners = this.cornerDetecter.GetCorners(roi);
            var right = 0;
            foreach (var c in corners)
            {
                if (c.X > right)
                {
                    right = c.X;
                }
            }

            this.RightMostCorner = right;
            this.Region.Width = this.Region.XPos + right;
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
        /// Gets or sets the right most corner.
        /// </summary>
        public int RightMostCorner
        {
            get
            {
                return this.rightMostCorner;
            }

            set
            {
                if (value == this.rightMostCorner)
                {
                    return;
                }

                this.rightMostCorner = value;
                this.NotifyOfPropertyChange(() => this.RightMostCorner);
            }
        }

        /// <summary>
        /// The open image.
        /// </summary>
        public void OpenImage()
        {
            var dlg = new SaveFileDialog
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
                var hash = this.hasher.Create(img);
                this.ManualHash = hash;
            }
        }

        /// <summary>
        /// The update manual hash.
        /// </summary>
        public void UpdateManualHash()
        {
            if (this.SelectedScanArea == null)
            {
                return;
            }

            var model = this.SelectedScanArea;
            var hash = this.ManualHash;
            model.Hash = hash;
        }

        /// <summary>
        /// The save region as.
        /// </summary>
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

                var image = ImageHelper.BitmapImage2Bitmap(this.Screenshot);
                var roi = image.Clone(new Rectangle(this.Region.XPos + this.BoardX, this.Region.YPos, this.Region.Width, this.Region.Height), image.PixelFormat);
                roi.Save(filename, ImageFormat.Png);

                // var cropped = new CroppedBitmap(this.Screenshot, new Int32Rect((int)this.Region.XPos, (int)this.Region.YPos, (int)this.Region.Width, (int)this.Region.Height));
                // var encoder = new PngBitmapEncoder();
                // using (var fs = new FileStream(filename, FileMode.Create, FileAccess.Write))
                // {
                // encoder.Frames.Add(BitmapFrame.Create(cropped));
                // encoder.Save(fs);
                // fs.Flush();
                // fs.Close();
                // }
            }
        }

        /// <summary>
        /// The toggle capture.
        /// </summary>
        public void ToggleCapture()
        {
            this.capturing = !this.capturing;
        }

        /// <summary>
        /// The update scan area.
        /// </summary>
        private void UpdateScanArea()
        {
            if (this.SelectedScanArea == null || this.Screenshot == null)
            {
                return;
            }
            
            var area = this.SelectedScanArea;
            this.Regions.Clear();

            var region = new RegionModel {
                XPos = area.X, 
                YPos = area.Y, 
                Height = area.Height, 
                Width = area.Width, 
                MinHeight = 8, 
                MinWidth = 8, 
            };
            region.Moving += (sender, args) =>
            {
                if (args.X + region.Width > this.BoardWidth)
                {
                    args.Cancel = true;
                    return;
                }

                if (args.Y + region.Height > this.BoardHeight)
                {
                    args.Cancel = true;
                    return;
                }

                this.SelectedScanArea.X = (int)args.X;
                this.SelectedScanArea.Y = (int)args.Y;
            };
            region.Resizing += (sender, args) =>
            {
                if (region.XPos + args.Width > this.BoardWidth)
                {
                    args.Cancel = true; 
                    return;
                }

                if (region.YPos + args.Height > this.BoardHeight)
                {
                    args.Cancel = true;
                    return;
                }

                this.SelectedScanArea.Width = (int)args.Width;
                this.SelectedScanArea.Height = (int)args.Height;
            };
            this.Region = region;
            this.Regions.Add(region);
        }

        /// <summary>
        /// Called when activating.
        /// </summary>
        protected override void OnActivate()
        {
            this.captureEngine.PublishCapturedWindow = true;
        }

        /// <summary>
        /// Called when deactivating.
        /// </summary>
        /// <param name="close">
        /// Inidicates whether this instance will be closed.
        /// </param>
        protected override void OnDeactivate(bool close)
        {
            this.captureEngine.PublishCapturedWindow = false;
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Handle(WindowCaptured message)
        {
            Execute.OnUIThread(
                () =>
                {
                    if (!this.capturing)
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
                    this.Screenshot = bi;
                    ms.Dispose();
                    message.Data.Dispose();
                });
        }
    }
}