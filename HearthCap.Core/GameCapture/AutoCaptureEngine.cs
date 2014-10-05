// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutoCaptureEngine.cs" company="">
//   
// </copyright>
// <summary>
//   The capture engine.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Core.GameCapture
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.IO.MemoryMappedFiles;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Security.AccessControl;
    using System.Security.Principal;
    using System.Threading;
    using System.Threading.Tasks;

    using Caliburn.Micro;

    using Capture;
    using Capture.Hook;
    using Capture.Interface;

    using HearthCap.Core.GameCapture.EngineEvents;
    using HearthCap.Core.GameCapture.Logging.LogEvents;
    using HearthCap.Core.Util;

    using NLog;

    using LogManager = NLog.LogManager;

    /// <summary>
    /// The capture engine.
    /// </summary>
    [Export(typeof(ICaptureEngine))]
    public class CaptureEngine : ICaptureEngine
    {
        /// <summary>
        /// The current capture engine.
        /// </summary>
        private ICaptureEngine currentCaptureEngine;

        /// <summary>
        /// The auto capture engine.
        /// </summary>
        private IAutoCaptureEngine autoCaptureEngine;

        /// <summary>
        /// The log capture engine.
        /// </summary>
        private ILogCaptureEngine logCaptureEngine;

        /// <summary>
        /// The capture method.
        /// </summary>
        private CaptureMethod captureMethod;

        /// <summary>
        /// The start async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public Task StartAsync()
        {
            return this.currentCaptureEngine.StartAsync();
        }

        /// <summary>
        /// The stop.
        /// </summary>
        public void Stop()
        {
            this.currentCaptureEngine.Stop();
        }

        /// <summary>
        /// Gets a value indicating whether is running.
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return this.currentCaptureEngine.IsRunning;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether publish captured window.
        /// </summary>
        public bool PublishCapturedWindow
        {
            get
            {
                return this.currentCaptureEngine.PublishCapturedWindow;
            }

            set
            {
                this.currentCaptureEngine.PublishCapturedWindow = value;
            }
        }

        /// <summary>
        /// Gets or sets the speed.
        /// </summary>
        public int Speed
        {
            get
            {
                return this.currentCaptureEngine.Speed;
            }

            set
            {
                this.currentCaptureEngine.Speed = value;
            }
        }

        /// <summary>
        /// Gets or sets the capture method.
        /// </summary>
        public CaptureMethod CaptureMethod
        {
            get
            {
                return this.captureMethod;
            }

            set
            {
                this.captureMethod = value;
                this.SetCurrentEngine(value);
                this.currentCaptureEngine.CaptureMethod = value;
            }
        }

        /// <summary>
        /// The set current engine.
        /// </summary>
        /// <param name="method">
        /// The method.
        /// </param>
        private void SetCurrentEngine(CaptureMethod method)
        {
            var oldEngine = this.currentCaptureEngine;
            switch (method)
            {
                case CaptureMethod.AutoDetect:
                case CaptureMethod.BitBlt:
                case CaptureMethod.Wdm:
                case CaptureMethod.DirectX:
                    this.currentCaptureEngine = this.autoCaptureEngine;
                    break;
                case CaptureMethod.Log:
                    this.currentCaptureEngine = this.logCaptureEngine;
                    break;
            }

            if (oldEngine != this.currentCaptureEngine && oldEngine.IsRunning)
            {
                oldEngine.Stop();
                this.currentCaptureEngine.StartAsync();
            }
        }

        /// <summary>
        /// The unhandled exception.
        /// </summary>
        public event EventHandler<UnhandledExceptionEventArgs> UnhandledException
        {
            add
            {
                this.autoCaptureEngine.UnhandledException += value;
                this.logCaptureEngine.UnhandledException += value;
            }

            remove
            {
                this.autoCaptureEngine.UnhandledException -= value;
                this.logCaptureEngine.UnhandledException -= value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CaptureEngine"/> class.
        /// </summary>
        /// <param name="autoCaptureEngine">
        /// The auto capture engine.
        /// </param>
        /// <param name="logCaptureEngine">
        /// The log capture engine.
        /// </param>
        [ImportingConstructor]
        public CaptureEngine(IAutoCaptureEngine autoCaptureEngine, ILogCaptureEngine logCaptureEngine)
        {
            this.autoCaptureEngine = autoCaptureEngine;
            this.logCaptureEngine = logCaptureEngine;
            this.currentCaptureEngine = autoCaptureEngine;
        }
    }

    /// <summary>
    /// The AutoCaptureEngine interface.
    /// </summary>
    public interface IAutoCaptureEngine : ICaptureEngine
    {
    }

    /// <summary>
    /// The auto capture engine.
    /// </summary>
    [Export(typeof(IAutoCaptureEngine))]
    public unsafe class AutoCaptureEngine : IAutoCaptureEngine, IDisposable
    {
        /// <summary>
        /// The hearthston e_ windo w_ title.
        /// </summary>
        protected const string HEARTHSTONE_WINDOW_TITLE = "hearthstone";

        /// <summary>
        /// The hearthston e_ proces s_ name.
        /// </summary>
        protected const string HEARTHSTONE_PROCESS_NAME = "hearthstone";

        /// <summary>
        /// The timer counter.
        /// </summary>
        private int timerCounter;

        /// <summary>
        /// The events.
        /// </summary>
        private readonly IEventAggregator events;

        /// <summary>
        /// The image scanners.
        /// </summary>
        private readonly IEnumerable<IImageScanner> imageScanners;

        /// <summary>
        /// The log.
        /// </summary>
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The trace log.
        /// </summary>
        private static readonly TraceLogger TraceLog = new TraceLogger(Log);

        /// <summary>
        /// The screen capture.
        /// </summary>
        private ScreenCapture screenCapture;

        /// <summary>
        /// The capture config.
        /// </summary>
        private CaptureConfig captureConfig;

        /// <summary>
        /// The delay constant.
        /// </summary>
        private static readonly int delayConstant = 50;

        /// <summary>
        /// The average speed.
        /// </summary>
        private int averageSpeed;

        /// <summary>
        /// The average count.
        /// </summary>
        private int averageCount;

        /// <summary>
        /// The running.
        /// </summary>
        private bool running;

        /// <summary>
        /// The delay.
        /// </summary>
        private int delay;

        /// <summary>
        /// The extra delay.
        /// </summary>
        private int extraDelay;

        /// <summary>
        /// The last capture method.
        /// </summary>
        private CaptureMethod lastCaptureMethod = CaptureMethod.AutoDetect;

        /// <summary>
        /// The preferred capture method.
        /// </summary>
        private CaptureMethod preferredCaptureMethod = CaptureMethod.AutoDetect;

        /// <summary>
        /// The current capture method.
        /// </summary>
        private CaptureMethod? currentCaptureMethod;

        /// <summary>
        /// The window lost.
        /// </summary>
        private bool windowLost;

        /// <summary>
        /// The window found.
        /// </summary>
        private bool windowFound;

        /// <summary>
        /// The attached.
        /// </summary>
        private bool attached;

        /// <summary>
        /// The capture process.
        /// </summary>
        private CaptureProcess captureProcess;

        /// <summary>
        /// The empty rect.
        /// </summary>
        private static readonly Rectangle emptyRect = new Rectangle(0, 0, 0, 0);

        /// <summary>
        /// The wait for screenshot timeout.
        /// </summary>
        private static readonly TimeSpan waitForScreenshotTimeout = TimeSpan.FromSeconds(1);

        /// <summary>
        /// The window minimized.
        /// </summary>
        private bool windowMinimized;

        /// <summary>
        /// The capture interface.
        /// </summary>
        private CaptureInterface captureInterface;

        /// <summary>
        /// The last image height.
        /// </summary>
        private int lastImageHeight;

        /// <summary>
        /// The direct x error count.
        /// </summary>
        private int directXErrorCount;

        /// <summary>
        /// The dont use direct x.
        /// </summary>
        private bool dontUseDirectX;

        /// <summary>
        /// The directx retry count.
        /// </summary>
        private int directxRetryCount;

        // private Thread scannerThread;

        // private ManualResetEventSlim scannerWaitHandle = new ManualResetEventSlim(false);

        // private ManualResetEventSlim captureWaitHandle = new ManualResetEventSlim(true);

        // private ScreenshotResource currentImage;

        // private int lastCaptured = -1;

        /// <summary>
        /// The shared mem mutexes.
        /// </summary>
        private Mutex[] sharedMemMutexes;

        /// <summary>
        /// The shared textures.
        /// </summary>
        private MemoryMappedFile[] sharedTextures = new MemoryMappedFile[2];

        /// <summary>
        /// The shared textures access.
        /// </summary>
        private MemoryMappedViewAccessor[] sharedTexturesAccess = new MemoryMappedViewAccessor[2];

        /// <summary>
        /// The shared textures ptr.
        /// </summary>
        private byte*[] sharedTexturesPtr = { (byte*)0, (byte*)0 };

        /// <summary>
        /// The copy data mem ptr.
        /// </summary>
        private byte* copyDataMemPtr = (byte*)0;

        /// <summary>
        /// The capture dx wait handle.
        /// </summary>
        private EventWaitHandle captureDxWaitHandle;

        /// <summary>
        /// The hook ready wait handle.
        /// </summary>
        private EventWaitHandle hookReadyWaitHandle;

        /// <summary>
        /// The copy data mem.
        /// </summary>
        private MemoryMappedFile copyDataMem;

        /// <summary>
        /// The copy data mem access.
        /// </summary>
        private MemoryMappedViewAccessor copyDataMemAccess;

        /// <summary>
        /// The last known texture id.
        /// </summary>
        private Guid lastKnownTextureId;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoCaptureEngine"/> class.
        /// </summary>
        /// <param name="events">
        /// The events.
        /// </param>
        /// <param name="imageScanners">
        /// The image scanners.
        /// </param>
        [ImportingConstructor]
        public AutoCaptureEngine(
            IEventAggregator events, 
            [ImportMany]IEnumerable<IImageScanner> imageScanners)
        {
            this.events = events;
            this.imageScanners = imageScanners;
            this.screenCapture = new ScreenCapture();
            var direct3DVersion = Direct3DVersion.Direct3D9SharedMem;
            this.CaptureMethod = CaptureMethod.AutoDetect;
            this.captureConfig = new CaptureConfig {
                Direct3DVersion = direct3DVersion
            };
            var security = new MutexSecurity();
            security.AddAccessRule(new MutexAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), MutexRights.Synchronize | MutexRights.Modify, AccessControlType.Allow));
            bool created;
            this.sharedMemMutexes = new[]
            {
                new Mutex(false, "Global\\DXHookD3D9Shared0", out created, security), 
                new Mutex(false, "Global\\DXHookD3D9Shared1", out created, security)
            };
            var ewsecurity = new EventWaitHandleSecurity();
            ewsecurity.AddAccessRule(new EventWaitHandleAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), EventWaitHandleRights.FullControl, AccessControlType.Allow));
            this.captureDxWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset, "Global\\DXHookD3D9Capture", out created, ewsecurity);
            this.hookReadyWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset, "Global\\DXHookD3D9CaptureReady", out created, ewsecurity);
        }

        /// <summary>
        /// The unhandled exception.
        /// </summary>
        public event EventHandler<UnhandledExceptionEventArgs> UnhandledException;

        /// <summary>
        /// The started.
        /// </summary>
        public event EventHandler<EventArgs> Started;

        /// <summary>
        /// The stopped.
        /// </summary>
        public event EventHandler<EventArgs> Stopped;

        /// <summary>
        /// Gets a value indicating whether is running.
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return this.running;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether publish captured window.
        /// </summary>
        public bool PublishCapturedWindow { get; set; }

        /// <summary>
        /// Gets or sets the speed.
        /// </summary>
        public int Speed { get; set; }

        /// <summary>
        /// The start.
        /// </summary>
        public void Start()
        {
            if (this.running)
            {
                Log.Warn("already running");
                return;
            }

            this.running = true;

            // this.scannerThread = new Thread(ScannerThread) { IsBackground = true };
            // this.scannerThread.Start();
            Log.Debug("Capture method: {0}, Version: {1}", this.CaptureMethod, Assembly.GetEntryAssembly().GetName().Version);
            this.lastCaptureMethod = this.CaptureMethod;
            this.preferredCaptureMethod = this.CaptureMethod;
            while (this.running)
            {
                var start = DateTime.Now.Ticks;

                this.CaptureLoop();

                if (this.running)
                {
                    this.DelayInternal(start);
                }
            }

            foreach (var imageScanner in this.imageScanners)
            {
                imageScanner.Stop(null);

                // imageScanner.Dispose();
            }

            this.windowLost = false;
            this.windowFound = false;
            this.windowMinimized = false;
            this.currentCaptureMethod = null;
            this.averageCount = 0;
            this.averageSpeed = 0;
            this.attached = false;
            this.extraDelay = 0;
            this.lastImageHeight = 0;
            this.directXErrorCount = 0;
            this.dontUseDirectX = false;

            // this.lastCaptured = -1;
            this.OnStopped();
        }

        // private void ScannerThread()
        // {
        // while (running)
        // {
        // scannerWaitHandle.Wait();
        // scannerWaitHandle.Reset();
        // if (!running)
        // {
        // break;
        // }
        // if (currentImage == null)
        // {
        // Log.Warn("signaled without having an image");
        // continue;
        // }

        // var start = DateTime.Now.Ticks;
        // try
        // {
        // // scan areas, publish events;
        // foreach (var scanner in this.imageScanners)
        // {
        // scanner.Run(this.currentImage.Bitmap, null);
        // }
        // }
        // finally
        // {
        // var stop = DateTime.Now.Ticks;
        // var proc = TimeSpan.FromTicks(stop - start).Milliseconds;
        // TraceLog.Log("Scanner speed: {0}", proc);

        // // signal capture thread to pass next capture
        // captureWaitHandle.Set();
        // }
        // }
        // }

        /// <summary>
        /// The start async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public Task StartAsync()
        {
            return Task.Run(() => this.Start()).ContinueWith(t => this.OnUnhandledException(t.Exception), TaskContinuationOptions.OnlyOnFaulted);
        }

        /// <summary>
        /// The capture loop.
        /// </summary>
        private void CaptureLoop()
        {
            IntPtr wnd;
            try
            {
                wnd = HearthstoneHelper.GetHearthstoneWindow();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                this.OnWindowLost();
                return;
            }

            // Verify window exists
            if (wnd == IntPtr.Zero)
            {
                this.OnWindowLost();
                return;
            }

            ScreenshotResource img = null;
            try
            {
                if (this.currentCaptureMethod == null)
                {
                    this.currentCaptureMethod = this.CaptureMethod;
                }

                if (this.preferredCaptureMethod != this.CaptureMethod)
                {
                    // user changed capture method in settings
                    this.currentCaptureMethod = this.CaptureMethod;
                    this.preferredCaptureMethod = this.CaptureMethod;
                }

                if (this.dontUseDirectX && this.currentCaptureMethod.Value == CaptureMethod.DirectX)
                {
                    Log.Info("DirectX gave too much errors, switching to Wdm.");
                    this.currentCaptureMethod = CaptureMethod.Wdm;
                }

                switch (this.currentCaptureMethod)
                {
                    case CaptureMethod.AutoDetect:
                        var detectedMethod = this.DetectCaptureMethod(wnd, out img);
                        if (detectedMethod == null)
                        {
                            this.OnWindowMinimized();
                            this.extraDelay = 200; // small delay, just enough to not be noticable, but less cpu
                            Thread.Sleep(this.extraDelay);
                            return;
                        }

                        this.currentCaptureMethod = detectedMethod.Value;
                        break;
                    case CaptureMethod.DirectX:
                        img = this.CaptureDirectX(wnd);
                        break;
                    case CaptureMethod.Wdm:
                        img = this.CaptureWdm(wnd);
                        break;
                    case CaptureMethod.BitBlt:
                        img = this.CaptureWdm(wnd, false, true);
                        break;
                }

                if (this.lastCaptureMethod != this.currentCaptureMethod.Value)
                {
                    Log.Debug("Capture method changed from {0} to {1}", this.lastCaptureMethod, this.currentCaptureMethod.Value);

                    // Do not detach hook, so we can quickly switch again
                    // if (lastCaptureMethod == CaptureMethod.DirectX)
                    // {
                    // DettachHookFromProcess();
                    // }
                    this.lastCaptureMethod = this.currentCaptureMethod.Value;
                }

                if (img == null || img.Bitmap == null)
                {
                    TraceLog.Log("No image data found.");
                    this.OnWindowMinimized();
                    this.extraDelay = 200; // small delay, just enough to not be noticable, but less cpu
                    Thread.Sleep(this.extraDelay);
                    return;
                }

                if (this.CaptureMethod == CaptureMethod.AutoDetect)
                {
                    if (this.lastImageHeight > 0 && this.lastImageHeight != img.Bitmap.Height)
                    {
                        // reset capture method so we detect again on next run
                        Log.Debug("Auto-detect: Image resolution changed from {0} to {1}, reset auto-detect.", this.lastImageHeight, img.Bitmap.Height);
                        this.currentCaptureMethod = null;
                    }

                    this.lastImageHeight = img.Bitmap.Height;
                }

                this.extraDelay = 0;
                this.OnWindowFound();

                if (this.PublishCapturedWindow)
                {
                    // Log.Diag("Window captured");
                    var bmpcpy = new Bitmap(img.Bitmap);
                    this.Publish(new WindowCaptured(bmpcpy), log: false);
                }

                var start = DateTime.Now.Ticks;

                try
                {
                    // scan areas, publish events;
                    foreach (var scanner in this.imageScanners)
                    {
                        scanner.Run(img.Bitmap, null);
                    }
                }
                finally
                {
                    try
                    {
                        if (img != null)
                        {
                            img.Dispose();
                        }
                    }
                    catch
                    {
                    }

                    var stop = DateTime.Now.Ticks;
                    var proc = TimeSpan.FromTicks(stop - start).Milliseconds;
                    TraceLog.Log("Scanner speed: {0}", proc);
                }

                // wait with passing next capture to scanner and signal scanner thread to process
                // this.captureWaitHandle.Wait();
                // this.captureWaitHandle.Reset();
                // if (this.currentImage != null)
                // {
                // this.currentImage.Dispose();
                // this.currentImage = null;
                // }
                // this.currentImage = img;
                // this.scannerWaitHandle.Set();
            }
            catch (ScreenshotCaptureException ex)
            {
                Log.Debug(ex.ToString());
                this.OnWindowLost();
                this.extraDelay = 2000;
                Thread.Sleep(this.extraDelay);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        /// <summary>
        /// The detect capture method.
        /// </summary>
        /// <param name="wnd">
        /// The wnd.
        /// </param>
        /// <param name="img">
        /// The img.
        /// </param>
        /// <returns>
        /// The <see cref="CaptureMethod?"/>.
        /// </returns>
        private CaptureMethod? DetectCaptureMethod(IntPtr wnd, out ScreenshotResource img)
        {
            if (NativeMethods.IsIconic(wnd))
            {
                img = null;
                return null;
            }

            if (ScreenCapture.IsFullScreen(wnd))
            {
                if (this.dontUseDirectX)
                {
                    Log.Warn("Auto-detect: full-screen and DirectX errors. Capturing will probably not work.");
                }
                else
                {
                    Log.Info("Auto-detect: window is full-screen, use DirectX");
                    try
                    {
                        img = this.CaptureDirectX(wnd);
                        if (img != null && img.Bitmap != null)
                        {
                            Log.Info("Auto-detect: Can use DirectX");
                            return CaptureMethod.DirectX;
                        }
                        else
                        {
                            Log.Warn("Auto-detect: DirectX returned empty image");
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Warn("Auto-detect: DirectX throws errors, investigate: " + ex);
                    }
                }

                img = null;
                return null;
            }

            if (!this.dontUseDirectX)
            {
                try
                {
                    img = this.CaptureDirectX(wnd);
                    if (img != null && img.Bitmap != null)
                    {
                        Log.Info("Auto-detect: Can use DirectX");
                        return CaptureMethod.DirectX;
                    }
                }
                catch (Exception ex)
                {
                    Log.Warn("Auto-detect: DirectX throws errors, investigate: " + ex);
                }

            }

            try
            {
                img = this.CaptureWdm(wnd, true, false);
                if (img != null)
                {
                    if (!img.Bitmap.IsAllBlack())
                    {
                        Log.Info("Auto-detect: Can use Wdm");
                        return CaptureMethod.Wdm;
                    }
                }
            }
            catch (Exception)
            {
            }

            // if (dontUseDirectX)
            // {
            // Log.Info("Auto-detect: DirectX gave too much errors, skipping DirectX.");
            // }
            // else
            // {
            // try
            // {
            // img = CaptureDirectX(wnd);
            // if (img != null)
            // {
            // if (!img.Bitmap.IsAllBlack())
            // {
            // Log.Info("Auto-detect: Can use DirectX");
            // return CaptureMethod.DirectX;
            // }
            // }
            // }
            // catch (Exception)
            // {
            // }
            // }
            try
            {
                img = this.CaptureWdm(wnd, false, true);
                if (img != null && img.Bitmap != null)
                {
                    if (!img.Bitmap.IsAllBlack())
                    {
                        Log.Info("Auto-detect: Can use BitBlt");
                        return CaptureMethod.BitBlt;
                    }
                }
            }
            catch (Exception)
            {
            }

            img = null;
            return null;
        }

        /// <summary>
        /// The capture wdm.
        /// </summary>
        /// <param name="wnd">
        /// The wnd.
        /// </param>
        /// <param name="forcePrintWindow">
        /// The force print window.
        /// </param>
        /// <param name="forceBitBlt">
        /// The force bit blt.
        /// </param>
        /// <returns>
        /// The <see cref="ScreenshotResource"/>.
        /// </returns>
        private ScreenshotResource CaptureWdm(IntPtr wnd, bool forcePrintWindow = false, bool forceBitBlt = false)
        {
            try
            {
                var img = this.screenCapture.GetDesktopBitmapBg(wnd, forcePrintWindow, forceBitBlt) as Bitmap;
                if (img == null)
                {
                    return null;
                }

                return new ScreenshotResource(img);
            }
            catch (Exception ex)
            {
                if (ex.Source.ToLowerInvariant() == "system.drawing")
                {
                    // expected when window is minimized
                    return null;
                }

                Log.Error(ex);
                return null;
            }
        }

        /// <summary>
        /// The capture direct x.
        /// </summary>
        /// <param name="wnd">
        /// The wnd.
        /// </param>
        /// <returns>
        /// The <see cref="ScreenshotResource"/>.
        /// </returns>
        /// <exception cref="ScreenshotCaptureException">
        /// </exception>
        private ScreenshotResource CaptureDirectX(IntPtr wnd)
        {
            if (!this.AttachHookToProcess())
            {
                throw new ScreenshotCaptureException("Error attaching to DirectX");
            }

            ScreenshotResource result = null;

            // Bitmap img = null;
            // captureProcess.BringProcessWindowToFront();
            // Initiate the screenshot of the CaptureInterface, the appropriate event handler within the target process will take care of the rest
            if (this.captureConfig.Direct3DVersion == Direct3DVersion.Direct3D9
                || this.captureConfig.Direct3DVersion == Direct3DVersion.Direct3D9Simple)
            {
                var start = DateTime.Now.Ticks;
                var task =
                    Task<Screenshot>.Factory.FromAsync(
                        (rect, timeout, callback, ctxt) => this.captureProcess.CaptureInterface.BeginGetScreenshot(rect, timeout, callback), 
                        this.captureProcess.CaptureInterface.EndGetScreenshot, 
                        emptyRect, 
                        waitForScreenshotTimeout, 
                        null);
                Screenshot screen = null;
                try
                {
                    task.Wait();
                    screen = task.Result;

                    var stop = DateTime.Now.Ticks;
                    var proc = TimeSpan.FromTicks(stop - start).Milliseconds;
                    TraceLog.Log("DX Capture speed: {0}", proc);

                    if (screen == null && this.directxRetryCount == 0)
                    {
                        Log.Debug("No data received from DirectX hook, retrying once.");
                        this.directxRetryCount++;
                        return this.CaptureDirectX(wnd);
                    }
                    else if (screen == null)
                    {
                        Log.Debug("No data received from DirectX hook.");
                        return null;
                    }

                    this.directxRetryCount = 0;

                    task.Dispose();
                    try
                    {
                        var width = screen.Width;
                        var height = screen.Height;
                        var bitmapData = screen.CapturedBitmap;
                        var img = new Bitmap(width, height, PixelFormat.Format32bppRgb);
                        var bmpData = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.WriteOnly, img.PixelFormat);

                        Marshal.Copy(bitmapData, 0, bmpData.Scan0, bitmapData.Length);
                        img.UnlockBits(bmpData);

                        result = new ScreenshotResource(img);
                    }
                    catch (Exception ex)
                    {
                        Log.Debug("Error decoding DirectX pixels: {0}", ex);
                        return null;
                    }
                }
                finally
                {
                    if (screen != null)
                    {
                        screen.Dispose();
                    }
                }
            }
            else if (this.captureConfig.Direct3DVersion == Direct3DVersion.Direct3D9SharedMem)
            {
                try
                {
                    if (!this.hookReadyWaitHandle.WaitOne(2000))
                    {
                        Log.Debug("Waiting for DirectX hook initialization.");
                        return null;
                    }

                    this.captureDxWaitHandle.Set();
                    if (this.copyDataMem == null)
                    {
                        try
                        {
                            this.copyDataMem = MemoryMappedFile.OpenExisting("CaptureHookSharedMemData", MemoryMappedFileRights.Read);
                            this.copyDataMemAccess = this.copyDataMem.CreateViewAccessor(0, 0, MemoryMappedFileAccess.Read);
                            this.copyDataMemAccess.SafeMemoryMappedViewHandle.AcquirePointer(ref this.copyDataMemPtr);
                        }
                        catch (FileNotFoundException)
                        {
                            // not hooked
                            Log.Debug("Shared memory not found");
                            return null;
                        }
                    }

                    if (this.copyDataMemAccess == null)
                    {
                        // not hooked
                        Log.Debug("Shared memory not opened yet");
                        return null;
                    }

                    int lastRendered;
                    CopyData copyData = *((CopyData*)this.copyDataMemPtr);
                    if (copyData.height <= 0 || copyData.textureId == Guid.Empty)
                    {
                        return null;
                    }

                    lastRendered = copyData.lastRendered;

                    if (lastRendered != -1)
                    {
                        if (!this.sharedMemMutexes[lastRendered].WaitOne(1000))
                        {
                            Log.Warn("Failed acquiring shared texture lock in time (1000).");
                            return null;
                        }

                        if (this.lastKnownTextureId != copyData.textureId)
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                if (this.sharedTexturesAccess[i] != null)
                                {
                                    this.sharedTexturesAccess[i].SafeMemoryMappedViewHandle.ReleasePointer();
                                    this.sharedTexturesAccess[i].Dispose();
                                    this.sharedTexturesAccess[i] = null;
                                }

                                if (this.sharedTextures[i] != null)
                                {
                                    this.sharedTextures[i].Dispose();
                                    this.sharedTextures[i] = null;
                                }
                            }
                        }

                        if (this.sharedTextures[lastRendered] == null)
                        {
                            this.sharedTextures[lastRendered] = MemoryMappedFile.OpenExisting(
                                copyData.textureId.ToString() + lastRendered, 
                                MemoryMappedFileRights.ReadWrite);
                            this.sharedTexturesAccess[lastRendered] = this.sharedTextures[lastRendered].CreateViewAccessor(
                                0, 
                                copyData.height * copyData.pitch, 
                                MemoryMappedFileAccess.ReadWrite);
                            this.sharedTexturesAccess[lastRendered].SafeMemoryMappedViewHandle.AcquirePointer(ref this.sharedTexturesPtr[lastRendered]);
                        }

                        var img = new Bitmap(
                            copyData.width, 
                            copyData.height, 
                            copyData.pitch, 
                            PixelFormat.Format32bppRgb, 
                            new IntPtr(this.sharedTexturesPtr[lastRendered]));
                        this.lastKnownTextureId = copyData.textureId;
                        result = new DxScreenshotResource(img, this.sharedMemMutexes[lastRendered]);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
            }

            return result;
        }

        /// <summary>
        /// The memcpy.
        /// </summary>
        /// <param name="dest">
        /// The dest.
        /// </param>
        /// <param name="src">
        /// The src.
        /// </param>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <returns>
        /// The <see cref="IntPtr"/>.
        /// </returns>
        [DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern IntPtr memcpy(IntPtr dest, IntPtr src, UIntPtr count);

        /// <summary>
        /// The dettach hook from process.
        /// </summary>
        private void DettachHookFromProcess()
        {
            if (this.captureProcess != null)
            {
                this.captureInterface.RemoteMessage -= this.CaptureInterfaceOnRemoteMessage;
                this.captureProcess.Dispose();
                this.captureProcess = null;
                this.attached = false;
            }

            for (int i = 0; i < 2; i++)
            {
                if (this.sharedTexturesAccess[i] != null)
                {
                    this.sharedTexturesAccess[i].SafeMemoryMappedViewHandle.ReleasePointer();
                    this.sharedTexturesAccess[i].Dispose();
                    this.sharedTexturesAccess[i] = null;
                }

                if (this.sharedTextures[i] != null)
                {
                    bool locked = false;
                    try
                    {
                        locked = this.sharedMemMutexes[i].WaitOne(1000);
                    }
                    catch (AbandonedMutexException ex)
                    {
                        locked = true;
                    }
                    finally
                    {
                        this.sharedTextures[i].Dispose();
                        this.sharedTextures[i] = null;
                        if (locked)
                        {
                            this.sharedMemMutexes[i].ReleaseMutex();
                        }
                    }
                }
            }

            if (this.copyDataMemAccess != null)
            {
                this.copyDataMemAccess.SafeMemoryMappedViewHandle.ReleasePointer();
                this.copyDataMemAccess.Dispose();
                this.copyDataMemAccess = null;
            }

            if (this.copyDataMem != null)
            {
                this.copyDataMem.Dispose();
                this.copyDataMem = null;
            }

            // lastCaptured = -1;
        }

        /// <summary>
        /// The attach hook to process.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool AttachHookToProcess()
        {
            if (this.attached) return true;

            Process[] processes = Process.GetProcessesByName(HEARTHSTONE_PROCESS_NAME);

            if (processes.Length <= 0)
            {
                Log.Debug("GetProcessesByName failed.");
                return false;
            }

            var process = processes[0];

            // Check incompatible modules:
            foreach (ProcessModule module in process.Modules)
            {
                if (module.ModuleName.ToLower().StartsWith("rtsshooks"))
                {
                    this.Publish(new IncompatibleHooksFound("RTSSHooks", "MSI Afterburner / Riva Tuner Statistics Server"));
                }
            }

            // if (process.MainWindowHandle == IntPtr.Zero)
            // {
            // Log.Debug("Could not get MainWindowHandle.");
            // return false;
            // }
            if (HookManager.IsHooked(process.Id)) return true;

            if (this.captureProcess != null)
            {
                Log.Warn("Call DettachHookFromProcess first");
                this.DettachHookFromProcess();
                this.extraDelay = 200;
                Thread.Sleep(200);
            }

            this.captureInterface = new CaptureInterface();
            this.captureInterface.RemoteMessage += this.CaptureInterfaceOnRemoteMessage;
            this.captureProcess = new CaptureProcess(process, this.captureConfig, this.captureInterface);
            this.attached = true;
            return true;
        }

        /// <summary>
        /// The on window minimized.
        /// </summary>
        private void OnWindowMinimized()
        {
            if (!this.windowMinimized)
            {
                Log.Debug("Hearthstone window found, but could not capture (minimized?)");
                this.Publish(new WindowMinimized());
            }

            this.windowFound = false;
            this.windowLost = false;
            this.windowMinimized = true;
        }

        /// <summary>
        /// The on window lost.
        /// </summary>
        private void OnWindowLost()
        {
            if (!this.windowLost)
            {
                Log.Debug("Hearthstone window is lost (not running?)");
                this.Publish(new WindowNotFound());
            }

            this.windowMinimized = false;
            this.windowFound = false;
            this.windowLost = true;
            this.attached = false;
            this.currentCaptureMethod = null;
            this.extraDelay = 2000;
            Thread.Sleep(this.extraDelay);
        }

        /// <summary>
        /// The on window found.
        /// </summary>
        private void OnWindowFound()
        {
            if (!this.windowFound)
            {
                Log.Debug("Window found, and capturing.");
                this.Publish(new WindowFound());
            }

            this.windowFound = true;
            this.windowLost = false;
            this.windowMinimized = false;
        }

        /// <summary>
        /// Gets or sets the capture method.
        /// </summary>
        public CaptureMethod CaptureMethod { get; set; }

        /// <summary>
        /// The stop.
        /// </summary>
        public void Stop()
        {
            this.running = false;
        }

        /// <summary>
        /// The on started.
        /// </summary>
        protected virtual void OnStarted()
        {
            var handler = this.Started;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// The on stopped.
        /// </summary>
        protected virtual void OnStopped()
        {
            var handler = this.Stopped;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// The on unhandled exception.
        /// </summary>
        /// <param name="exception">
        /// The exception.
        /// </param>
        protected virtual void OnUnhandledException(Exception exception)
        {
            var e = new UnhandledExceptionEventArgs(exception, false);
            var handler = this.UnhandledException;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// The delay internal.
        /// </summary>
        /// <param name="start">
        /// The start.
        /// </param>
        protected void DelayInternal(long start)
        {
            var stop = DateTime.Now.Ticks;
            var proc = TimeSpan.FromTicks(stop - start).Milliseconds - this.extraDelay;
            this.averageSpeed += proc;
            this.averageCount++;
            var average = this.averageSpeed / this.averageCount;
            TraceLog.Log(string.Format("Desired speed: {0} (+{2}), Actual speed: {1}, Average: {3}", this.Speed, proc, this.delay, average));
            if (this.averageCount == 100)
            {
                this.averageSpeed = average;
                this.averageCount = 1;
            }

            // only adjust once per second
            if (this.timerCounter < Math.Floor(1000f / this.Speed))
            {
                this.timerCounter++;
            }
            else
            {
                this.timerCounter = 0;
                if (proc > (this.Speed + this.delay + delayConstant))
                {
                    Log.Debug("processing taking longer then desired speed: {2}ms, actual: {0}ms. (adding {1}ms. delay)", proc, delayConstant, this.Speed);
                    this.delay += delayConstant;
                }
                else
                {
                    if (this.delay > 0)
                    {
                        if (proc + this.delay < (this.Speed + delayConstant))
                        {
                            this.delay -= delayConstant;
                            Log.Debug("Removing {1}ms. delay. Delay is now: {0}", this.delay, delayConstant);
                        }
                    }
                }
            }

            var wait = this.Speed - proc + this.delay;
            Thread.Sleep(wait < 0 ? 0 : wait);
        }

        /// <summary>
        /// The publish.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="log">
        /// The log.
        /// </param>
        protected void Publish(EngineEvent message, bool log = true)
        {
            if (log)
            {
                Log.Info("Event ({0}): {1}", message.GetType().Name, message.Message);
            }

            this.events.PublishOnBackgroundThread(message);
        }

        /// <summary>
        /// The capture interface on remote message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        private void CaptureInterfaceOnRemoteMessage(MessageReceivedEventArgs message)
        {
            switch (message.MessageType)
            {
                case MessageType.Debug:
                    Log.Debug(message.Message);
                    break;
                case MessageType.Error:
                    this.directXErrorCount++;
                    Log.Error(message.Message);
                    if (this.directXErrorCount >= 10)
                    {
                        this.dontUseDirectX = true;
                        this.DettachHookFromProcess();
                    }

                    break;
                case MessageType.Warning:
                    Log.Warn(message.Message);
                    break;
                case MessageType.Information:
                    Log.Info(message.Message);
                    break;
                case MessageType.Trace:
                    TraceLog.Log(message.Message);
                    break;
            }

            message.Dispose();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (this.captureProcess != null)
            {
                this.captureProcess.Dispose();
                this.captureProcess = null;
            }

            for (int i = 0; i < 2; i++)
            {
                if (this.sharedTexturesAccess[i] != null)
                {
                    this.sharedTexturesAccess[i].SafeMemoryMappedViewHandle.ReleasePointer();
                    this.sharedTexturesAccess[i].Dispose();
                    this.sharedTexturesAccess[i] = null;
                }

                if (this.sharedTextures[i] != null)
                {
                    this.sharedTextures[i].Dispose();
                    this.sharedTextures[i] = null;
                }
            }

            if (this.copyDataMemAccess != null)
            {
                this.copyDataMemAccess.SafeMemoryMappedViewHandle.ReleasePointer();
                this.copyDataMemAccess.Dispose();
                this.copyDataMemAccess = null;
            }

            if (this.copyDataMem != null)
            {
                this.copyDataMem.Dispose();
                this.copyDataMem = null;
            }
        }
    }

    /// <summary>
    /// The dx screenshot resource.
    /// </summary>
    internal class DxScreenshotResource : ScreenshotResource
    {
        /// <summary>
        /// The access.
        /// </summary>
        private readonly MemoryMappedViewAccessor access;

        /// <summary>
        /// The disposed.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="DxScreenshotResource"/> class.
        /// </summary>
        /// <param name="bitmap">
        /// The bitmap.
        /// </param>
        /// <param name="mutex">
        /// The mutex.
        /// </param>
        /// <param name="access">
        /// The access.
        /// </param>
        public DxScreenshotResource(Bitmap bitmap, Mutex mutex, MemoryMappedViewAccessor access)
            : base(bitmap)
        {
            this.access = access;
            this.Mutex = mutex;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DxScreenshotResource"/> class.
        /// </summary>
        /// <param name="bitmap">
        /// The bitmap.
        /// </param>
        /// <param name="mutex">
        /// The mutex.
        /// </param>
        public DxScreenshotResource(Bitmap bitmap, Mutex mutex)
            : this(bitmap, mutex, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DxScreenshotResource"/> class.
        /// </summary>
        /// <param name="bitmap">
        /// The bitmap.
        /// </param>
        public DxScreenshotResource(Bitmap bitmap)
            : this(bitmap, null, null)
        {
        }

        /// <summary>
        /// Gets the mutex.
        /// </summary>
        protected Mutex Mutex { get; private set; }

        /// <summary>
        /// The dispose.
        /// </summary>
        /// <param name="disposing">
        /// The disposing.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (this.access != null)
                    {
                        this.access.SafeMemoryMappedViewHandle.ReleasePointer();
                        this.access.Dispose();
                    }

                    if (this.Mutex != null)
                    {
                        this.Mutex.ReleaseMutex();
                        this.Mutex = null;
                    }
                }

                this.disposed = true;
            }

            base.Dispose(disposing);
        }
    }

    /// <summary>
    /// The screenshot resource.
    /// </summary>
    internal class ScreenshotResource : IDisposable
    {
        /// <summary>
        /// The disposed.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScreenshotResource"/> class.
        /// </summary>
        /// <param name="bitmap">
        /// The bitmap.
        /// </param>
        public ScreenshotResource(Bitmap bitmap)
        {
            this.Bitmap = bitmap;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="ScreenshotResource"/> class. 
        /// </summary>
        ~ScreenshotResource()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Gets the bitmap.
        /// </summary>
        public Bitmap Bitmap { get; private set; }

        /// <summary>
        /// The dispose.
        /// </summary>
        /// <param name="disposing">
        /// The disposing.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.Bitmap.Dispose();
                    this.Bitmap = null;

                    // ...
                }

                this.disposed = true;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}