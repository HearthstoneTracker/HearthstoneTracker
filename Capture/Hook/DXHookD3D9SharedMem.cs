namespace Capture.Hook
{
    using System;
    using System.Collections.Generic;
    using System.IO.MemoryMappedFiles;
    using System.Runtime.InteropServices;
    using System.Security.AccessControl;
    using System.Security.Principal;
    using System.Threading;

    using Capture.Interface;

    using SharpDX;
    using SharpDX.Direct3D9;

    internal unsafe class DXHookD3D9SharedMem : BaseDXHook
    {
        #region Constants

        private const int D3D9Ex_DEVICE_METHOD_COUNT = 15;

        private const int D3D9_DEVICE_METHOD_COUNT = 119;

        #endregion

        #region Fields
        private CopyData copyData;

        private readonly int copyDataSize = Marshal.SizeOf(typeof(CopyData));
        private MemoryMappedFile[] sharedTextures = new MemoryMappedFile[2];
        private MemoryMappedViewAccessor[] sharedTexturesAccess = new MemoryMappedViewAccessor[2];
        private byte*[] sharedTexturesPtr = { (byte*)0, (byte*)0 };

        private MemoryMappedFile copyDataMem;
        private MemoryMappedViewAccessor copyDataMemAccess;
        private byte* copyDataMemPtr = (byte*)0;

        private readonly ManualResetEventSlim copyReadySignal = new ManualResetEventSlim(false);

        private readonly object[] surfaceLocks = new object[BUFFERS];

        private HookData<Direct3D9DeviceEx_PresentExDelegate> Direct3DDeviceEx_PresentExHook = null;

        private HookData<Direct3D9DeviceEx_ResetExDelegate> Direct3DDeviceEx_ResetExHook = null;

        private HookData<Direct3D9Device_EndSceneDelegate> Direct3DDevice_EndSceneHook = null;

        private HookData<Direct3D9Device_PresentDelegate> Direct3DDevice_PresentHook = null;

        private HookData<Direct3D9Device_ResetDelegate> Direct3DDevice_ResetHook = null;

        private IntPtr currentDevice;

        private bool hooksStarted;

        private List<IntPtr> id3dDeviceFunctionAddresses = new List<IntPtr>();

        private bool killThread;

        private Query[] queries = new Query[BUFFERS];

        private bool[] issuedQueries = new bool[BUFFERS];

        private Surface[] copySurfaces = new Surface[BUFFERS];

        private Thread retrieveThread;

        private bool supportsDirect3DEx = false;

        private Surface[] surfaces = new Surface[BUFFERS];

        private IntPtr surfaceDataPointer;

        private bool[] surfaceLocked = new bool[BUFFERS];

        private Mutex[] sharedMemMutexes;

        private bool surfacesSetup;

        private int presentHookRecurse;

        private int curCapture;

        private int copyWait;

        private int currentSurface;

        private const int BUFFERS = 3;

        private EventWaitHandle captureWaitHandle;
        private EventWaitHandle hookReadyWaitHandle;

        #endregion

        #region Constructors and Destructors

        public DXHookD3D9SharedMem(CaptureInterface ssInterface)
            : base(ssInterface)
        {
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
            captureWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset, "Global\\DXHookD3D9Capture", out created, ewsecurity);
            hookReadyWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset, "Global\\DXHookD3D9CaptureReady", out created, ewsecurity);
        }

        #endregion

        [DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern IntPtr memcpy(IntPtr dest, IntPtr src, UIntPtr count);

        #region Delegates

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        private unsafe delegate int Direct3D9DeviceEx_PresentExDelegate(
            IntPtr devicePtr,
            Rectangle* pSourceRect,
            Rectangle* pDestRect,
            IntPtr hDestWindowOverride,
            IntPtr pDirtyRegion,
            Present dwFlags);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate int Direct3D9DeviceEx_ResetExDelegate(IntPtr devicePtr, ref PresentParameters presentParameters, DisplayModeEx displayModeEx);

        /// <summary>
        /// The IDirect3DDevice9.EndScene function definition
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate int Direct3D9Device_EndSceneDelegate(IntPtr device);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        private unsafe delegate int Direct3D9Device_PresentDelegate(
            IntPtr devicePtr,
            Rectangle* pSourceRect,
            Rectangle* pDestRect,
            IntPtr hDestWindowOverride,
            IntPtr pDirtyRegion);

        /// <summary>
        /// The IDirect3DDevice9.Reset function definition
        /// </summary>
        /// <param name="device"></param>
        /// <param name="presentParameters"></param>
        /// <returns></returns>
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate int Direct3D9Device_ResetDelegate(IntPtr device, ref PresentParameters presentParameters);

        #endregion

        #region Properties

        protected override string HookName
        {
            get
            {
                return "DXHookD3D9";
            }
        }

        #endregion

        #region Public Methods and Operators

        public override void Cleanup()
        {
            // ClearData();
        }

        public override unsafe void Hook()
        {
            this.DebugMessage("Hook: Begin");

            this.DebugMessage("Hook: Before device creation");
            using (var d3d = new Direct3D())
            {
                this.DebugMessage("Hook: Direct3D created");
                using (
                    var device = new Device(
                        d3d,
                        0,
                        DeviceType.NullReference,
                        IntPtr.Zero,
                        CreateFlags.HardwareVertexProcessing,
                        new PresentParameters() { BackBufferWidth = 1, BackBufferHeight = 1 }))
                {
                    this.id3dDeviceFunctionAddresses.AddRange(this.GetVTblAddresses(device.NativePointer, D3D9_DEVICE_METHOD_COUNT));
                }
            }

            try
            {
                using (var d3dEx = new Direct3DEx())
                {
                    this.DebugMessage("Hook: Try Direct3DEx...");
                    using (
                        var deviceEx = new DeviceEx(
                            d3dEx,
                            0,
                            DeviceType.NullReference,
                            IntPtr.Zero,
                            CreateFlags.HardwareVertexProcessing,
                            new PresentParameters() { BackBufferWidth = 1, BackBufferHeight = 1 },
                            new DisplayModeEx() { Width = 800, Height = 600 }))
                    {
                        this.id3dDeviceFunctionAddresses.AddRange(
                            this.GetVTblAddresses(deviceEx.NativePointer, D3D9_DEVICE_METHOD_COUNT, D3D9Ex_DEVICE_METHOD_COUNT));
                        this.supportsDirect3DEx = true;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            this.DebugMessage("Setting up Direct3D hooks...");
            this.Direct3DDevice_EndSceneHook =
                new HookData<Direct3D9Device_EndSceneDelegate>(
                    this.id3dDeviceFunctionAddresses[(int)Direct3DDevice9FunctionOrdinals.EndScene],
                    new Direct3D9Device_EndSceneDelegate(this.EndSceneHook),
                    this);

            this.Direct3DDevice_EndSceneHook.ReHook();
            this.Hooks.Add(this.Direct3DDevice_EndSceneHook.Hook);

            this.Direct3DDevice_PresentHook =
                new HookData<Direct3D9Device_PresentDelegate>(
                    this.id3dDeviceFunctionAddresses[(int)Direct3DDevice9FunctionOrdinals.Present],
                    new Direct3D9Device_PresentDelegate(this.PresentHook),
                    this);

            this.Direct3DDevice_ResetHook =
                new HookData<Direct3D9Device_ResetDelegate>(
                    this.id3dDeviceFunctionAddresses[(int)Direct3DDevice9FunctionOrdinals.Reset],
                    new Direct3D9Device_ResetDelegate(this.ResetHook),
                    this);

            if (this.supportsDirect3DEx)
            {
                this.DebugMessage("Setting up Direct3DEx hooks...");
                this.Direct3DDeviceEx_PresentExHook =
                    new HookData<Direct3D9DeviceEx_PresentExDelegate>(
                        this.id3dDeviceFunctionAddresses[(int)Direct3DDevice9ExFunctionOrdinals.PresentEx],
                        new Direct3D9DeviceEx_PresentExDelegate(this.PresentExHook),
                        this);

                this.Direct3DDeviceEx_ResetExHook =
                    new HookData<Direct3D9DeviceEx_ResetExDelegate>(
                        this.id3dDeviceFunctionAddresses[(int)Direct3DDevice9ExFunctionOrdinals.ResetEx],
                        new Direct3D9DeviceEx_ResetExDelegate(this.ResetExHook),
                        this);
            }

            this.Direct3DDevice_ResetHook.ReHook();
            this.Hooks.Add(this.Direct3DDevice_ResetHook.Hook);

            this.Direct3DDevice_PresentHook.ReHook();
            this.Hooks.Add(this.Direct3DDevice_PresentHook.Hook);

            if (this.supportsDirect3DEx)
            {
                this.Direct3DDeviceEx_PresentExHook.ReHook();
                this.Hooks.Add(this.Direct3DDeviceEx_PresentExHook.Hook);

                this.Direct3DDeviceEx_ResetExHook.ReHook();
                this.Hooks.Add(this.Direct3DDeviceEx_ResetExHook.Hook);
            }

            this.DebugMessage("Hook: End");
        }

        #endregion

        #region Methods

        protected override void Dispose(bool disposing)
        {
            if (true)
            {
                try
                {
                    this.ClearData();
                }
                catch
                {
                }
            }
            base.Dispose(disposing);
        }

        private void ClearData()
        {
            this.DebugMessage("ClearData called");
            hookReadyWaitHandle.Reset();

            if (this.retrieveThread != null)
            {
                this.killThread = true;
                this.copyReadySignal.Set();

                if (!this.retrieveThread.Join(500))
                {
                    this.retrieveThread.Abort();
                }

                this.copyReadySignal.Reset();
                this.retrieveThread = null;
            }

            // currentDevice = null;
            if (this.Request != null)
            {
                this.Request.Dispose();
                this.Request = null;
            }

            for (int i = 0; i < BUFFERS; i++)
            {
                if (this.surfaceLocked[i])
                {
                    lock (this.surfaceLocks[i])
                    {
                        this.surfaces[i].UnlockRectangle();
                        this.surfaceLocked[i] = false;
                    }
                }
                this.issuedQueries[i] = false;
                if (this.surfaces[i] != null)
                {
                    this.surfaces[i].Dispose();
                    this.surfaces[i] = null;
                }
                if (this.copySurfaces[i] != null)
                {
                    this.copySurfaces[i].Dispose();
                    this.copySurfaces[i] = null;
                }
                if (this.queries[i] != null)
                {
                    this.queries[i].Dispose();
                    this.queries[i] = null;
                }
                lock (this.surfaceLocks[i])
                {
                    this.surfaces[i] = null;
                }
                //if (Monitor.TryEnter(this.surfaceLocks[i]))
                //{
                //    Monitor.Exit(this.surfaceLocks[i]);
                //    this.surfaceLocks[i] = null;
                //}
            }

            for (int i = 0; i < 2; i++)
            {
                if (this.sharedTexturesAccess[i] != null)
                {
                    this.sharedTexturesAccess[i].SafeMemoryMappedViewHandle.ReleasePointer();
                    this.sharedTexturesPtr[i] = (byte*)0;
                    this.sharedTexturesAccess[i].Dispose();
                    this.sharedTexturesAccess[i] = null;
                }

                if (this.sharedTextures[i] != null)
                {
                    this.sharedTextures[i].Dispose();
                    this.sharedTextures[i] = null;
                }
            }

            this.copyData.width = 0;
            this.copyData.height = 0;
            this.copyData.pitch = 0;
            this.copyData.lastRendered = 0;
            this.copyData.textureId = Guid.Empty;

            if (copyDataMemAccess != null)
            {
                copyDataMemAccess.Write(0, ref copyData);
                copyDataMemAccess.Flush();

                copyDataMemAccess.SafeMemoryMappedViewHandle.ReleasePointer();
                copyDataMemPtr = (byte*)0;
                copyDataMemAccess.Dispose();
                copyDataMemAccess = null;
                // copyDataMemPtr = (byte*)0;
            }

            if (copyDataMem != null)
            {
                copyDataMem.Dispose();
                copyDataMem = null;
            }

            this.captureWaitHandle.Reset();
            this.hooksStarted = false;
            this.surfacesSetup = false;
            this.copyWait = 0;
            this.curCapture = 0;
            this.currentSurface = 0;
            surfaceDataPointer = IntPtr.Zero;
        }

        /// <summary>
        /// Implementation of capturing from the render target of the Direct3D9 Device (or DeviceEx)
        /// </summary>
        /// <param name="device"></param>
        private void DoCaptureRenderTarget(Device device, string hook)
        {
            try
            {
                if (!this.surfacesSetup)
                {
                    using (Surface backbuffer = device.GetRenderTarget(0))
                    {
                        copyData.format = (int)backbuffer.Description.Format;
                        copyData.width = backbuffer.Description.Width;
                        copyData.height = backbuffer.Description.Height;
                    }

                    this.SetupSurfaces(device);
                }

                if (!this.surfacesSetup)
                {
                    return;
                }

                this.HandleCaptureRequest(device);
            }
            catch (Exception e)
            {
                this.DebugMessage(e.ToString());
            }
        }

        /// <summary>
        /// Hook for IDirect3DDevice9.EndScene
        /// </summary>
        /// <param name="devicePtr">Pointer to the IDirect3DDevice9 instance. Note: object member functions always pass "this" as the first parameter.</param>
        /// <returns>The HRESULT of the original EndScene</returns>
        /// <remarks>Remember that this is called many times a second by the Direct3D application - be mindful of memory and performance!</remarks>
        private int EndSceneHook(IntPtr devicePtr)
        {
            int hresult = Result.Ok.Code;
            var device = (Device)devicePtr;
            try
            {
                if (!this.hooksStarted)
                {
                    this.DebugMessage("EndSceneHook: hooks not started");
                    this.SetupData(device);
                }
            }
            catch (Exception ex)
            {
                this.DebugMessage(ex.ToString());
            }
            hresult = this.Direct3DDevice_EndSceneHook.Original(devicePtr);
            return hresult;
        }

        private void HandleCaptureRequest(Device device)
        {
            try
            {
                if (killThread)
                    return;

                for (int i = 0; i < BUFFERS; i++)
                {
                    bool tmp;
                    if (this.issuedQueries[i] && this.queries[i].GetData(out tmp, false))
                    {
                        this.issuedQueries[i] = false;
                        var lockedRect = this.surfaces[i].LockRectangle(LockFlags.ReadOnly);
                        this.surfaceDataPointer = lockedRect.DataPointer;
                        this.currentSurface = i;
                        this.surfaceLocked[i] = true;

                        this.copyReadySignal.Set();
                    }
                }

                if (captureWaitHandle.WaitOne(0) || copyWait < BUFFERS - 1)
                {
                    int nextCapture = (curCapture == BUFFERS - 1) ? 0 : (curCapture + 1);

                    try
                    {
                        using (var backbuffer = device.GetBackBuffer(0, 0))
                        {
                            var sourceTexture = copySurfaces[curCapture];
                            device.StretchRectangle(backbuffer, sourceTexture, TextureFilter.None);
                        }

                        if (copyWait < BUFFERS - 1)
                        {
                            copyWait++;
                        }
                        else
                        {
                            var prevSourceTexture = copySurfaces[nextCapture];
                            var targetTexture = surfaces[nextCapture];

                            if (this.surfaceLocked[nextCapture])
                            {
                                lock (this.surfaceLocks[nextCapture])
                                {
                                    this.surfaces[nextCapture].UnlockRectangle();
                                    this.surfaceLocked[nextCapture] = false;
                                }
                            }

                            try
                            {
                                device.GetRenderTargetData(prevSourceTexture, targetTexture);
                            }
                            catch (Exception ex)
                            {
                                DebugMessage(ex.ToString());
                            }

                            this.queries[nextCapture].Issue(Issue.End);
                            this.issuedQueries[nextCapture] = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        this.DebugMessage(ex.ToString());
                    }
                    finally
                    {
                        captureWaitHandle.Reset();
                        curCapture = nextCapture;                        
                    }
                }

            }
            catch (Exception e)
            {
                this.DebugMessage(e.ToString());
            }
        }

        protected override void InterfaceEventProxy_ScreenshotRequested(ScreenshotRequest request)
        {
            request.Dispose();
        }

        private unsafe void RetrieveImageDataThread()
        {
            int sharedMemId = 0;
            while (true)
            {
                this.copyReadySignal.Wait();
                this.copyReadySignal.Reset();

                if (killThread)
                    break;

                if (this.surfaceDataPointer == IntPtr.Zero)
                {
                    continue;
                }

                int nextSharedMemId = sharedMemId == 0 ? 1 : 0;
                try
                {
                    lock (this.surfaceLocks[currentSurface])
                    {
                        int lastRendered = -1;
                        int lastKnown = -1;
                        try
                        {
                            lastKnown = sharedMemId;
                            if (this.sharedMemMutexes[sharedMemId].WaitOne(0))
                            {
                                lastRendered = sharedMemId;
                            }
                            else
                            {
                                lastKnown = nextSharedMemId;
                                if (this.sharedMemMutexes[nextSharedMemId].WaitOne(0))
                                {
                                    lastRendered = nextSharedMemId;
                                }
                            }
                        }
                        catch (AbandonedMutexException)
                        {
                            sharedMemMutexes[lastKnown].ReleaseMutex();
                            continue;
                        }

                        if (lastRendered != -1)
                        {
                            var src = this.surfaceDataPointer;

                            if (src == IntPtr.Zero)
                            {
                                continue;
                            }

                            if (this.killThread)
                                break;

                            var size = this.copyData.height * this.copyData.pitch;
                            memcpy(new IntPtr(sharedTexturesPtr[lastRendered]), src, new UIntPtr((uint)size));

                            this.copyData.lastRendered = lastRendered;
                            
                            Marshal.StructureToPtr(this.copyData, new IntPtr(copyDataMemPtr), false);
                            this.sharedMemMutexes[lastRendered].ReleaseMutex();
                            sharedMemId = nextSharedMemId;
                        }
                    }
                }
                catch (Exception ex)
                {
                    DebugMessage(ex.ToString());
                }
            }
        }

        private unsafe int PresentExHook(
            IntPtr devicePtr,
            Rectangle* pSourceRect,
            Rectangle* pDestRect,
            IntPtr hDestWindowOverride,
            IntPtr pDirtyRegion,
            Present dwFlags)
        {
            int hresult = Result.Ok.Code;
            var device = (DeviceEx)devicePtr;
            if (!this.hooksStarted)
            {
                hresult = this.Direct3DDeviceEx_PresentExHook.Original(devicePtr, pSourceRect, pDestRect, hDestWindowOverride, pDirtyRegion, dwFlags);
                return hresult;
            }

            try
            {
                if (presentHookRecurse == 0)
                {
                    this.DoCaptureRenderTarget(device, "PresentEx");
                }
            }
            catch (Exception ex)
            {
                this.DebugMessage(ex.ToString());
            }
            finally
            {
                this.presentHookRecurse++;
                hresult = this.Direct3DDeviceEx_PresentExHook.Original(devicePtr, pSourceRect, pDestRect, hDestWindowOverride, pDirtyRegion, dwFlags);
                presentHookRecurse--;
            }
            return hresult;
        }

        private unsafe int PresentHook(
            IntPtr devicePtr,
            Rectangle* pSourceRect,
            Rectangle* pDestRect,
            IntPtr hDestWindowOverride,
            IntPtr pDirtyRegion)
        {
            int hresult;
            var device = (Device)devicePtr;
            if (!this.hooksStarted)
            {
                hresult = this.Direct3DDevice_PresentHook.Original(devicePtr, pSourceRect, pDestRect, hDestWindowOverride, pDirtyRegion);
                return hresult;
            }
            try
            {
                if (presentHookRecurse == 0)
                {
                    this.DoCaptureRenderTarget(device, "PresentHook");
                }
            }
            catch (Exception ex)
            {
                this.DebugMessage(ex.ToString());
            }
            finally
            {
                this.presentHookRecurse++;
                hresult = this.Direct3DDevice_PresentHook.Original(devicePtr, pSourceRect, pDestRect, hDestWindowOverride, pDirtyRegion);
                this.presentHookRecurse--;
            }
            return hresult;
        }

        // private bool IsDeviceReady(Device device)
        // {
        // var cooplevel = device.TestCooperativeLevel();
        // if (cooplevel.Code != ResultCode.Success.Code)
        // {
        // return false;
        // }
        // return true;
        // }

        private int ResetExHook(IntPtr devicePtr, ref PresentParameters presentparameters, DisplayModeEx displayModeEx)
        {
            int hresult = Result.Ok.Code;
            DeviceEx device = (DeviceEx)devicePtr;
            try
            {
                if (!this.hooksStarted)
                {
                    hresult = this.Direct3DDeviceEx_ResetExHook.Original(devicePtr, ref presentparameters, displayModeEx);
                    return hresult;
                }

                this.ClearData();

                hresult = this.Direct3DDeviceEx_ResetExHook.Original(devicePtr, ref presentparameters, displayModeEx);

                if (this.currentDevice != devicePtr)
                {
                    this.hooksStarted = false;
                    this.currentDevice = devicePtr;
                }
            }
            catch (Exception ex)
            {
                this.DebugMessage(ex.ToString());
            }
            return hresult;
        }

        /// <summary>
        /// Reset the _renderTarget so that we are sure it will have the correct presentation parameters (required to support working across changes to windowed/fullscreen or resolution changes)
        /// </summary>
        /// <param name="devicePtr"></param>
        /// <param name="presentParameters"></param>
        /// <returns></returns>
        private int ResetHook(IntPtr devicePtr, ref PresentParameters presentParameters)
        {
            int hresult = Result.Ok.Code;
            Device device = (Device)devicePtr;
            try
            {
                if (!this.hooksStarted)
                {
                    hresult = this.Direct3DDevice_ResetHook.Original(devicePtr, ref presentParameters);
                    return hresult;
                }

                this.ClearData();

                hresult = this.Direct3DDevice_ResetHook.Original(devicePtr, ref presentParameters);

                if (this.currentDevice != devicePtr)
                {
                    this.hooksStarted = false;
                    this.currentDevice = devicePtr;
                }
            }
            catch (Exception ex)
            {
                this.DebugMessage(ex.ToString());
            }
            return hresult;
        }

        private void SetupData(Device device)
        {
            this.DebugMessage("SetupData called");

            using (SwapChain swapChain = device.GetSwapChain(0))
            {
                PresentParameters pp = swapChain.PresentParameters;
                this.copyData.width = pp.BackBufferWidth;
                this.copyData.height = pp.BackBufferHeight;
                this.copyData.format = (int)pp.BackBufferFormat;

                this.DebugMessage(string.Format("D3D9 Setup: w: {0} h: {1} f: {2}", this.copyData.width, this.copyData.height, pp.BackBufferFormat));
            }

            this.hooksStarted = true;
        }

        private void SetupSurfaces(Device device)
        {
            try
            {
                for (int i = 0; i < BUFFERS; i++)
                {
                    this.surfaces[i] = Surface.CreateOffscreenPlain(device, this.copyData.width, this.copyData.height, (Format)this.copyData.format, Pool.SystemMemory);
                    var lockedRect = this.surfaces[i].LockRectangle(LockFlags.ReadOnly);
                    this.copyData.pitch = lockedRect.Pitch;
                    this.surfaces[i].UnlockRectangle();

                    this.copySurfaces[i] = Surface.CreateRenderTarget(device, this.copyData.width, this.copyData.height, (Format)this.copyData.format, MultisampleType.None, 0, false);
                    this.queries[i] = new Query(device, QueryType.Event);

                    this.surfaceLocks[i] = new object();
                }

                this.copyDataMem = MemoryMappedFile.CreateOrOpen("CaptureHookSharedMemData", Marshal.SizeOf(typeof(CopyData)));
                this.copyDataMemAccess = this.copyDataMem.CreateViewAccessor(0, Marshal.SizeOf(typeof(CopyData)));
                this.copyDataMemAccess.SafeMemoryMappedViewHandle.AcquirePointer(ref copyDataMemPtr);

                copyData.textureId = Guid.NewGuid();

                for (int i = 0; i < 2; i++)
                {
                    bool locked = false;
                    try
                    {
                        try
                        {
                            locked = this.sharedMemMutexes[i].WaitOne(0);
                        }
                        catch (AbandonedMutexException)
                        {
                            locked = true;
                        }
                        if (!locked)
                        {
                            DebugMessage("shared mem mutex still locked");
                        }

                        this.sharedTextures[i] = MemoryMappedFile.CreateOrOpen(
                            copyData.textureId.ToString() + i,
                            copyData.pitch * copyData.height,
                            MemoryMappedFileAccess.ReadWrite);
                        this.sharedTexturesAccess[i] = this.sharedTextures[i].CreateViewAccessor(0, copyData.pitch * copyData.height, MemoryMappedFileAccess.ReadWrite);
                        this.sharedTexturesAccess[i].SafeMemoryMappedViewHandle.AcquirePointer(ref this.sharedTexturesPtr[i]);
                    }
                    finally
                    {
                        if (locked)
                        {
                            this.sharedMemMutexes[i].ReleaseMutex();
                        }
                    }
                }

                killThread = false;
                this.surfacesSetup = true;

                Marshal.StructureToPtr(this.copyData, new IntPtr(copyDataMemPtr), false);

                this.retrieveThread = new Thread(this.RetrieveImageDataThread);
                retrieveThread.IsBackground = true;
                this.retrieveThread.Start();
                hookReadyWaitHandle.Set();
            }
            catch (Exception ex)
            {
                this.DebugMessage(ex.ToString());
                ClearData();
            }
        }

        #endregion
    }
}