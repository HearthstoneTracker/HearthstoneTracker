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
using LogManager = NLog.LogManager;

namespace HearthCap.Core.GameCapture
{
    [Export(typeof(ICaptureEngine))]
    public class CaptureEngine : ICaptureEngine
    {
        private ICaptureEngine currentCaptureEngine;

        private readonly IAutoCaptureEngine autoCaptureEngine;

        private readonly ILogCaptureEngine logCaptureEngine;

        private CaptureMethod captureMethod;

        public Task StartAsync()
        {
            return currentCaptureEngine.StartAsync();
        }

        public void Stop()
        {
            currentCaptureEngine.Stop();
        }

        public bool IsRunning
        {
            get { return currentCaptureEngine.IsRunning; }
        }

        public bool PublishCapturedWindow
        {
            get { return currentCaptureEngine.PublishCapturedWindow; }
            set { currentCaptureEngine.PublishCapturedWindow = value; }
        }

        public int Speed
        {
            get { return currentCaptureEngine.Speed; }
            set { currentCaptureEngine.Speed = value; }
        }

        public CaptureMethod CaptureMethod
        {
            get { return captureMethod; }
            set
            {
                captureMethod = value;
                SetCurrentEngine(value);
                currentCaptureEngine.CaptureMethod = value;
            }
        }

        private void SetCurrentEngine(CaptureMethod method)
        {
            var oldEngine = currentCaptureEngine;
            switch (method)
            {
                case CaptureMethod.AutoDetect:
                case CaptureMethod.BitBlt:
                case CaptureMethod.Wdm:
                case CaptureMethod.DirectX:
                    currentCaptureEngine = autoCaptureEngine;
                    break;
                case CaptureMethod.Log:
                    currentCaptureEngine = logCaptureEngine;
                    break;
            }

            if (oldEngine != currentCaptureEngine
                && oldEngine.IsRunning)
            {
                oldEngine.Stop();
                currentCaptureEngine.StartAsync();
            }
        }

        public event EventHandler<UnhandledExceptionEventArgs> UnhandledException
        {
            add
            {
                autoCaptureEngine.UnhandledException += value;
                logCaptureEngine.UnhandledException += value;
            }
            remove
            {
                autoCaptureEngine.UnhandledException -= value;
                logCaptureEngine.UnhandledException -= value;
            }
        }

        public event EventHandler<EventArgs> Started
        {
            add
            {
                autoCaptureEngine.Started += value;
                logCaptureEngine.Started += value;
            }
            remove
            {
                autoCaptureEngine.Started -= value;
                logCaptureEngine.Started -= value;
            }
        }

        public event EventHandler<EventArgs> Stopped
        {
            add
            {
                autoCaptureEngine.Stopped += value;
                logCaptureEngine.Stopped += value;
            }
            remove
            {
                autoCaptureEngine.Stopped -= value;
                logCaptureEngine.Stopped -= value;
            }
        }

        [ImportingConstructor]
        public CaptureEngine(IAutoCaptureEngine autoCaptureEngine, ILogCaptureEngine logCaptureEngine)
        {
            this.autoCaptureEngine = autoCaptureEngine;
            this.logCaptureEngine = logCaptureEngine;
            currentCaptureEngine = autoCaptureEngine;
        }
    }

    public interface IAutoCaptureEngine : ICaptureEngine
    {
    }

    [Export(typeof(IAutoCaptureEngine))]
    public unsafe class AutoCaptureEngine : IAutoCaptureEngine, IDisposable
    {
        protected const string HEARTHSTONE_WINDOW_TITLE = "hearthstone";
        protected const string HEARTHSTONE_PROCESS_NAME = "hearthstone";

        private int timerCounter;

        private readonly IEventAggregator events;

        private readonly IEnumerable<IImageScanner> imageScanners;

        private static readonly NLog.Logger Log = LogManager.GetCurrentClassLogger();

        private static readonly TraceLogger TraceLog = new TraceLogger(Log);

        private readonly ScreenCapture screenCapture;

        private readonly CaptureConfig captureConfig;

        private static readonly int delayConstant = 50;
        private int averageSpeed;

        private int averageCount;

        private int delay;

        private int extraDelay;

        private CaptureMethod lastCaptureMethod = CaptureMethod.AutoDetect;
        private CaptureMethod preferredCaptureMethod = CaptureMethod.AutoDetect;

        private CaptureMethod? currentCaptureMethod;

        private bool windowLost;

        private bool windowFound;

        private bool attached;

        private CaptureProcess captureProcess;

        private static readonly Rectangle emptyRect = new Rectangle(0, 0, 0, 0);

        private static readonly TimeSpan waitForScreenshotTimeout = TimeSpan.FromSeconds(1);

        private bool windowMinimized;

        private CaptureInterface captureInterface;

        private int lastImageHeight;

        private int directXErrorCount;

        private bool dontUseDirectX;

        private int directxRetryCount;

        // private Thread scannerThread;

        // private ManualResetEventSlim scannerWaitHandle = new ManualResetEventSlim(false);

        // private ManualResetEventSlim captureWaitHandle = new ManualResetEventSlim(true);

        // private ScreenshotResource currentImage;

        // private int lastCaptured = -1;

        private readonly Mutex[] sharedMemMutexes;

        private readonly MemoryMappedFile[] sharedTextures = new MemoryMappedFile[2];
        private readonly MemoryMappedViewAccessor[] sharedTexturesAccess = new MemoryMappedViewAccessor[2];
        private readonly byte*[] sharedTexturesPtr = { (byte*)0, (byte*)0 };
        private byte* copyDataMemPtr = (byte*)0;

        private readonly EventWaitHandle captureDxWaitHandle;
        private readonly EventWaitHandle hookReadyWaitHandle;

        private MemoryMappedFile copyDataMem;

        private MemoryMappedViewAccessor copyDataMemAccess;

        private Guid lastKnownTextureId;

        [ImportingConstructor]
        public AutoCaptureEngine(
            IEventAggregator events,
            [ImportMany] IEnumerable<IImageScanner> imageScanners)
        {
            this.events = events;
            this.imageScanners = imageScanners;
            screenCapture = new ScreenCapture();
            var direct3DVersion = Direct3DVersion.Direct3D9SharedMem;
            CaptureMethod = CaptureMethod.AutoDetect;
            captureConfig = new CaptureConfig
                {
                    Direct3DVersion = direct3DVersion
                };
            var security = new MutexSecurity();
            security.AddAccessRule(new MutexAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), MutexRights.Synchronize | MutexRights.Modify, AccessControlType.Allow));
            bool created;
            sharedMemMutexes = new[]
                {
                    new Mutex(false, "Global\\DXHookD3D9Shared0", out created, security),
                    new Mutex(false, "Global\\DXHookD3D9Shared1", out created, security)
                };
            var ewsecurity = new EventWaitHandleSecurity();
            ewsecurity.AddAccessRule(new EventWaitHandleAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), EventWaitHandleRights.FullControl, AccessControlType.Allow));
            captureDxWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset, "Global\\DXHookD3D9Capture", out created, ewsecurity);
            hookReadyWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset, "Global\\DXHookD3D9CaptureReady", out created, ewsecurity);
        }

        public event EventHandler<UnhandledExceptionEventArgs> UnhandledException;

        public event EventHandler<EventArgs> Started;

        public event EventHandler<EventArgs> Stopped;

        public bool IsRunning { get; private set; }

        public bool PublishCapturedWindow { get; set; }

        public int Speed { get; set; }

        public void Start()
        {
            if (IsRunning)
            {
                Log.Warn("already running");
                return;
            }

            IsRunning = true;
            try
            {
                Log.Debug("Capture method: {0}, Version: {1}", CaptureMethod, Assembly.GetEntryAssembly().GetName().Version);
                lastCaptureMethod = CaptureMethod;
                preferredCaptureMethod = CaptureMethod;
                OnStarted();

                while (IsRunning)
                {
                    var start = DateTime.Now.Ticks;

                    CaptureLoop();

                    if (IsRunning)
                    {
                        DelayInternal(start);
                    }
                }
            }
            finally
            {
                foreach (var imageScanner in imageScanners)
                {
                    imageScanner.Stop(null);
                    // imageScanner.Dispose();
                }

                windowLost = false;
                windowFound = false;
                windowMinimized = false;
                currentCaptureMethod = null;
                averageCount = 0;
                averageSpeed = 0;
                attached = false;
                extraDelay = 0;
                lastImageHeight = 0;
                directXErrorCount = 0;
                dontUseDirectX = false;
                // this.lastCaptured = -1;

                OnStopped();
            }
        }

        //private void ScannerThread()
        //{
        //    while (running)
        //    {
        //        scannerWaitHandle.Wait();
        //        scannerWaitHandle.Reset();
        //        if (!running)
        //        {
        //            break;
        //        }
        //        if (currentImage == null)
        //        {
        //            Log.Warn("signaled without having an image");
        //            continue;
        //        }

        //        var start = DateTime.Now.Ticks;
        //        try
        //        {
        //            // scan areas, publish events;
        //            foreach (var scanner in this.imageScanners)
        //            {
        //                scanner.Run(this.currentImage.Bitmap, null);
        //            }
        //        }
        //        finally
        //        {
        //            var stop = DateTime.Now.Ticks;
        //            var proc = TimeSpan.FromTicks(stop - start).Milliseconds;
        //            TraceLog.Log("Scanner speed: {0}", proc);

        //            // signal capture thread to pass next capture
        //            captureWaitHandle.Set();
        //        }
        //    }
        //}

        public Task StartAsync()
        {
            return Task.Run(() => Start()).ContinueWith(t => OnUnhandledException(t.Exception), TaskContinuationOptions.OnlyOnFaulted);
        }

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
                OnWindowLost();
                return;
            }

            // Verify window exists
            if (wnd == IntPtr.Zero)
            {
                OnWindowLost();
                return;
            }

            ScreenshotResource img = null;
            try
            {
                if (currentCaptureMethod == null)
                {
                    currentCaptureMethod = CaptureMethod;
                }

                if (preferredCaptureMethod != CaptureMethod)
                {
                    // user changed capture method in settings
                    currentCaptureMethod = CaptureMethod;
                    preferredCaptureMethod = CaptureMethod;
                }

                if (dontUseDirectX && currentCaptureMethod.Value == CaptureMethod.DirectX)
                {
                    Log.Info("DirectX gave too much errors, switching to Wdm.");
                    currentCaptureMethod = CaptureMethod.Wdm;
                }

                switch (currentCaptureMethod)
                {
                    case CaptureMethod.AutoDetect:
                        var detectedMethod = DetectCaptureMethod(wnd, out img);
                        if (detectedMethod == null)
                        {
                            OnWindowMinimized();
                            extraDelay = 200; // small delay, just enough to not be noticable, but less cpu
                            Thread.Sleep(extraDelay);
                            return;
                        }
                        currentCaptureMethod = detectedMethod.Value;
                        break;
                    case CaptureMethod.DirectX:
                        img = CaptureDirectX(wnd);
                        break;
                    case CaptureMethod.Wdm:
                        img = CaptureWdm(wnd);
                        break;
                    case CaptureMethod.BitBlt:
                        img = CaptureWdm(wnd, false, true);
                        break;
                }

                if (lastCaptureMethod != currentCaptureMethod.Value)
                {
                    Log.Debug("Capture method changed from {0} to {1}", lastCaptureMethod, currentCaptureMethod.Value);
                    // Do not detach hook, so we can quickly switch again
                    //if (lastCaptureMethod == CaptureMethod.DirectX)
                    //{
                    //    DettachHookFromProcess();
                    //}
                    lastCaptureMethod = currentCaptureMethod.Value;
                }

                if (img == null
                    || img.Bitmap == null)
                {
                    TraceLog.Log("No image data found.");
                    OnWindowMinimized();
                    extraDelay = 200; // small delay, just enough to not be noticable, but less cpu
                    Thread.Sleep(extraDelay);
                    return;
                }

                if (CaptureMethod == CaptureMethod.AutoDetect)
                {
                    if (lastImageHeight > 0
                        && lastImageHeight != img.Bitmap.Height)
                    {
                        // reset capture method so we detect again on next run
                        Log.Debug("Auto-detect: Image resolution changed from {0} to {1}, reset auto-detect.", lastImageHeight, img.Bitmap.Height);
                        currentCaptureMethod = null;
                    }

                    lastImageHeight = img.Bitmap.Height;
                }

                extraDelay = 0;
                OnWindowFound();

                if (PublishCapturedWindow)
                {
                    // Log.Diag("Window captured");
                    var bmpcpy = new Bitmap(img.Bitmap);
                    Publish(new WindowCaptured(bmpcpy), log: false);
                }

                var start = DateTime.Now.Ticks;

                try
                {
                    // scan areas, publish events;
                    foreach (var scanner in imageScanners)
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
                //this.captureWaitHandle.Wait();
                //this.captureWaitHandle.Reset();
                //if (this.currentImage != null)
                //{
                //    this.currentImage.Dispose();
                //    this.currentImage = null;
                //}
                //this.currentImage = img;
                //this.scannerWaitHandle.Set();
            }
            catch (ScreenshotCaptureException ex)
            {
                Log.Debug(ex.ToString());
                OnWindowLost();
                extraDelay = 2000;
                Thread.Sleep(extraDelay);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        private CaptureMethod? DetectCaptureMethod(IntPtr wnd, out ScreenshotResource img)
        {
            if (NativeMethods.IsIconic(wnd))
            {
                img = null;
                return null;
            }

            if (ScreenCapture.IsFullScreen(wnd))
            {
                if (dontUseDirectX)
                {
                    Log.Warn("Auto-detect: full-screen and DirectX errors. Capturing will probably not work.");
                }
                else
                {
                    Log.Info("Auto-detect: window is full-screen, use DirectX");
                    try
                    {
                        img = CaptureDirectX(wnd);
                        if (img != null
                            && img.Bitmap != null)
                        {
                            Log.Info("Auto-detect: Can use DirectX");
                            return CaptureMethod.DirectX;
                        }
                        Log.Warn("Auto-detect: DirectX returned empty image");
                    }
                    catch (Exception ex)
                    {
                        Log.Warn("Auto-detect: DirectX throws errors, investigate: " + ex);
                    }
                }

                img = null;
                return null;
            }

            if (!dontUseDirectX)
            {
                try
                {
                    img = CaptureDirectX(wnd);
                    if (img != null
                        && img.Bitmap != null)
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
                img = CaptureWdm(wnd, true, false);
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

            //if (dontUseDirectX)
            //{
            //    Log.Info("Auto-detect: DirectX gave too much errors, skipping DirectX.");
            //}
            //else
            //{
            //    try
            //    {
            //        img = CaptureDirectX(wnd);
            //        if (img != null)
            //        {
            //            if (!img.Bitmap.IsAllBlack())
            //            {
            //                Log.Info("Auto-detect: Can use DirectX");
            //                return CaptureMethod.DirectX;
            //            }
            //        }
            //    }
            //    catch (Exception)
            //    {
            //    }
            //}

            try
            {
                img = CaptureWdm(wnd, false, true);
                if (img != null
                    && img.Bitmap != null)
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

        private ScreenshotResource CaptureWdm(IntPtr wnd, bool forcePrintWindow = false, bool forceBitBlt = false)
        {
            try
            {
                var img = screenCapture.GetDesktopBitmapBg(wnd, forcePrintWindow, forceBitBlt) as Bitmap;
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

        private ScreenshotResource CaptureDirectX(IntPtr wnd)
        {
            if (!AttachHookToProcess())
            {
                throw new ScreenshotCaptureException("Error attaching to DirectX");
            }
            ScreenshotResource result = null;
            // Bitmap img = null;
            // captureProcess.BringProcessWindowToFront();
            // Initiate the screenshot of the CaptureInterface, the appropriate event handler within the target process will take care of the rest

            if (captureConfig.Direct3DVersion == Direct3DVersion.Direct3D9
                ||
                captureConfig.Direct3DVersion == Direct3DVersion.Direct3D9Simple)
            {
                var start = DateTime.Now.Ticks;
                var task = Task<Screenshot>.Factory.FromAsync(
                    (rect, timeout, callback, ctxt) => captureProcess.CaptureInterface.BeginGetScreenshot(rect, timeout, callback),
                    captureProcess.CaptureInterface.EndGetScreenshot,
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

                    if (screen == null
                        && directxRetryCount == 0)
                    {
                        Log.Debug("No data received from DirectX hook, retrying once.");
                        directxRetryCount++;
                        return CaptureDirectX(wnd);
                    }
                    else if (screen == null)
                    {
                        Log.Debug("No data received from DirectX hook.");
                        return null;
                    }
                    directxRetryCount = 0;

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
                        Log.Debug(ex, "Error decoding DirectX pixels: {0}");
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
            else if (captureConfig.Direct3DVersion == Direct3DVersion.Direct3D9SharedMem)
            {
                try
                {
                    if (!hookReadyWaitHandle.WaitOne(2000))
                    {
                        Log.Debug("Waiting for DirectX hook initialization.");
                        return null;
                    }

                    captureDxWaitHandle.Set();
                    if (copyDataMem == null)
                    {
                        try
                        {
                            copyDataMem = MemoryMappedFile.OpenExisting("CaptureHookSharedMemData", MemoryMappedFileRights.Read);
                            copyDataMemAccess = copyDataMem.CreateViewAccessor(0, 0, MemoryMappedFileAccess.Read);
                            copyDataMemAccess.SafeMemoryMappedViewHandle.AcquirePointer(ref copyDataMemPtr);
                        }
                        catch (FileNotFoundException)
                        {
                            // not hooked
                            Log.Debug("Shared memory not found");
                            return null;
                        }
                    }
                    if (copyDataMemAccess == null)
                    {
                        // not hooked
                        Log.Debug("Shared memory not opened yet");
                        return null;
                    }

                    int lastRendered;
                    var copyData = (*((CopyData*)copyDataMemPtr));
                    if (copyData.height <= 0
                        || copyData.textureId == Guid.Empty)
                    {
                        return null;
                    }
                    lastRendered = copyData.lastRendered;

                    if (lastRendered != -1)
                    {
                        if (!sharedMemMutexes[lastRendered].WaitOne(1000))
                        {
                            Log.Warn("Failed acquiring shared texture lock in time (1000).");
                            return null;
                        }
                        if (lastKnownTextureId != copyData.textureId)
                        {
                            for (var i = 0; i < 2; i++)
                            {
                                if (sharedTexturesAccess[i] != null)
                                {
                                    sharedTexturesAccess[i].SafeMemoryMappedViewHandle.ReleasePointer();
                                    sharedTexturesAccess[i].Dispose();
                                    sharedTexturesAccess[i] = null;
                                }
                                if (sharedTextures[i] != null)
                                {
                                    sharedTextures[i].Dispose();
                                    sharedTextures[i] = null;
                                }
                            }
                        }

                        if (sharedTextures[lastRendered] == null)
                        {
                            sharedTextures[lastRendered] = MemoryMappedFile.OpenExisting(copyData.textureId.ToString() + lastRendered, MemoryMappedFileRights.ReadWrite);
                            sharedTexturesAccess[lastRendered] = sharedTextures[lastRendered].CreateViewAccessor(
                                0,
                                copyData.height * copyData.pitch,
                                MemoryMappedFileAccess.ReadWrite);
                            sharedTexturesAccess[lastRendered].SafeMemoryMappedViewHandle.AcquirePointer(ref sharedTexturesPtr[lastRendered]);
                        }

                        var img = new Bitmap(
                            copyData.width,
                            copyData.height,
                            copyData.pitch,
                            PixelFormat.Format32bppRgb,
                            new IntPtr(sharedTexturesPtr[lastRendered]));
                        lastKnownTextureId = copyData.textureId;
                        result = new DxScreenshotResource(img, sharedMemMutexes[lastRendered]);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
            }

            return result;
        }

        [DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern IntPtr memcpy(IntPtr dest, IntPtr src, UIntPtr count);

        private void DettachHookFromProcess()
        {
            if (captureProcess != null)
            {
                captureInterface.RemoteMessage -= CaptureInterfaceOnRemoteMessage;
                captureProcess.Dispose();
                captureProcess = null;
                attached = false;
            }

            for (var i = 0; i < 2; i++)
            {
                if (sharedTexturesAccess[i] != null)
                {
                    sharedTexturesAccess[i].SafeMemoryMappedViewHandle.ReleasePointer();
                    sharedTexturesAccess[i].Dispose();
                    sharedTexturesAccess[i] = null;
                }

                if (sharedTextures[i] != null)
                {
                    var locked = false;
                    try
                    {
                        locked = sharedMemMutexes[i].WaitOne(1000);
                    }
                    catch (AbandonedMutexException)
                    {
                        locked = true;
                    }
                    finally
                    {
                        sharedTextures[i].Dispose();
                        sharedTextures[i] = null;
                        if (locked)
                        {
                            sharedMemMutexes[i].ReleaseMutex();
                        }
                    }
                }
            }

            if (copyDataMemAccess != null)
            {
                copyDataMemAccess.SafeMemoryMappedViewHandle.ReleasePointer();
                copyDataMemAccess.Dispose();
                copyDataMemAccess = null;
            }
            if (copyDataMem != null)
            {
                copyDataMem.Dispose();
                copyDataMem = null;
            }
            // lastCaptured = -1;
        }

        private bool AttachHookToProcess()
        {
            if (attached)
            {
                return true;
            }

            var processes = Process.GetProcessesByName(HEARTHSTONE_PROCESS_NAME);

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
                    Publish(new IncompatibleHooksFound("RTSSHooks", "MSI Afterburner / Riva Tuner Statistics Server"));
                }
            }

            //if (process.MainWindowHandle == IntPtr.Zero)
            //{
            //    Log.Debug("Could not get MainWindowHandle.");
            //    return false;
            //}

            if (HookManager.IsHooked(process.Id))
            {
                return true;
            }

            if (captureProcess != null)
            {
                Log.Warn("Call DettachHookFromProcess first");
                DettachHookFromProcess();
                extraDelay = 200;
                Thread.Sleep(200);
            }

            captureInterface = new CaptureInterface();
            captureInterface.RemoteMessage += CaptureInterfaceOnRemoteMessage;
            captureProcess = new CaptureProcess(process, captureConfig, captureInterface);
            attached = true;
            return true;
        }

        private void OnWindowMinimized()
        {
            if (!windowMinimized)
            {
                Log.Debug("Hearthstone window found, but could not capture (minimized?)");
                Publish(new WindowMinimized());
            }
            windowFound = false;
            windowLost = false;
            windowMinimized = true;
        }

        private void OnWindowLost()
        {
            if (!windowLost)
            {
                Log.Debug("Hearthstone window is lost (not running?)");
                Publish(new WindowNotFound());
            }
            windowMinimized = false;
            windowFound = false;
            windowLost = true;
            attached = false;
            currentCaptureMethod = null;
            extraDelay = 2000;
            Thread.Sleep(extraDelay);
        }

        private void OnWindowFound()
        {
            if (!windowFound)
            {
                Log.Debug("Window found, and capturing.");
                Publish(new WindowFound());
            }
            windowFound = true;
            windowLost = false;
            windowMinimized = false;
        }

        public CaptureMethod CaptureMethod { get; set; }

        public void Stop()
        {
            IsRunning = false;
        }

        protected virtual void OnStarted()
        {
            var handler = Started;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        protected virtual void OnStopped()
        {
            var handler = Stopped;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        protected virtual void OnUnhandledException(Exception exception)
        {
            var e = new UnhandledExceptionEventArgs(exception, false);
            var handler = UnhandledException;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected void DelayInternal(long start)
        {
            var stop = DateTime.Now.Ticks;
            var proc = TimeSpan.FromTicks(stop - start).Milliseconds - extraDelay;
            averageSpeed += proc;
            averageCount++;
            var average = averageSpeed / averageCount;
            TraceLog.Log(string.Format("Desired speed: {0} (+{2}), Actual speed: {1}, Average: {3}", Speed, proc, delay, average));
            if (averageCount == 100)
            {
                averageSpeed = average;
                averageCount = 1;
            }

            // only adjust once per second
            if (timerCounter < Math.Floor(1000f / Speed))
            {
                timerCounter++;
            }
            else
            {
                timerCounter = 0;
                if (proc > (Speed + delay + delayConstant))
                {
                    Log.Debug("processing taking longer then desired speed: {2}ms, actual: {0}ms. (adding {1}ms. delay)", proc, delayConstant, Speed);
                    delay += delayConstant;
                }
                else
                {
                    if (delay > 0)
                    {
                        if (proc + delay < (Speed + delayConstant))
                        {
                            delay -= delayConstant;
                            Log.Debug("Removing {1}ms. delay. Delay is now: {0}", delay, delayConstant);
                        }
                    }
                }
            }
            var wait = Speed - proc + delay;
            Thread.Sleep(wait < 0 ? 0 : wait);
        }

        protected void Publish(EngineEvent message, bool log = true)
        {
            if (log)
            {
                Log.Info("Event ({0}): {1}", message.GetType().Name, message.Message);
            }
            events.PublishOnBackgroundThread(message);
        }

        private void CaptureInterfaceOnRemoteMessage(MessageReceivedEventArgs message)
        {
            switch (message.MessageType)
            {
                case MessageType.Debug:
                    Log.Debug(message.Message);
                    break;
                case MessageType.Error:
                    directXErrorCount++;
                    Log.Error(message.Message);
                    if (directXErrorCount >= 10)
                    {
                        dontUseDirectX = true;
                        DettachHookFromProcess();
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
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (captureProcess != null)
            {
                captureProcess.Dispose();
                captureProcess = null;
            }

            for (var i = 0; i < 2; i++)
            {
                if (sharedTexturesAccess[i] != null)
                {
                    sharedTexturesAccess[i].SafeMemoryMappedViewHandle.ReleasePointer();
                    sharedTexturesAccess[i].Dispose();
                    sharedTexturesAccess[i] = null;
                }

                if (sharedTextures[i] != null)
                {
                    sharedTextures[i].Dispose();
                    sharedTextures[i] = null;
                }
            }

            if (copyDataMemAccess != null)
            {
                copyDataMemAccess.SafeMemoryMappedViewHandle.ReleasePointer();
                copyDataMemAccess.Dispose();
                copyDataMemAccess = null;
            }

            if (copyDataMem != null)
            {
                copyDataMem.Dispose();
                copyDataMem = null;
            }
        }
    }

    internal class DxScreenshotResource : ScreenshotResource
    {
        private readonly MemoryMappedViewAccessor access;

        private bool disposed;

        public DxScreenshotResource(Bitmap bitmap, Mutex mutex, MemoryMappedViewAccessor access)
            : base(bitmap)
        {
            this.access = access;
            Mutex = mutex;
        }

        public DxScreenshotResource(Bitmap bitmap, Mutex mutex)
            : this(bitmap, mutex, null)
        {
        }

        public DxScreenshotResource(Bitmap bitmap)
            : this(bitmap, null, null)
        {
        }

        protected Mutex Mutex { get; private set; }

        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (access != null)
                    {
                        access.SafeMemoryMappedViewHandle.ReleasePointer();
                        access.Dispose();
                    }
                    if (Mutex != null)
                    {
                        Mutex.ReleaseMutex();
                        Mutex = null;
                    }
                }
                disposed = true;
            }

            base.Dispose(disposing);
        }
    }

    internal class ScreenshotResource : IDisposable
    {
        private bool disposed;

        public ScreenshotResource(Bitmap bitmap)
        {
            Bitmap = bitmap;
        }

        ~ScreenshotResource()
        {
            Dispose(false);
        }

        public Bitmap Bitmap { get; private set; }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    Bitmap.Dispose();
                    Bitmap = null;
                    // ...
                }
                disposed = true;
            }
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
