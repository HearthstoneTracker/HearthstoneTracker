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
        private CopyData _copyData;

        private readonly MemoryMappedFile[] _sharedTextures = new MemoryMappedFile[2];
        private readonly MemoryMappedViewAccessor[] _sharedTexturesAccess = new MemoryMappedViewAccessor[2];
        private readonly byte*[] _sharedTexturesPtr = { (byte*)0, (byte*)0 };

        private MemoryMappedFile _copyDataMem;
        private MemoryMappedViewAccessor _copyDataMemAccess;
        private byte* _copyDataMemPtr = (byte*)0;

        private readonly ManualResetEventSlim _copyReadySignal = new ManualResetEventSlim(false);

        private readonly object[] _surfaceLocks = new object[BUFFERS];

        private HookData<Direct3D9DeviceEx_PresentExDelegate> _direct3DDeviceExPresentExHook;

        private HookData<Direct3D9DeviceEx_ResetExDelegate> _direct3DDeviceExResetExHook;

        private HookData<Direct3D9Device_EndSceneDelegate> _direct3DDeviceEndSceneHook;

        private HookData<Direct3D9Device_PresentDelegate> _direct3DDevicePresentHook;

        private HookData<Direct3D9Device_ResetDelegate> _direct3DDeviceResetHook;

        private IntPtr _currentDevice;

        private bool _hooksStarted;

        private readonly List<IntPtr> _id3DDeviceFunctionAddresses = new List<IntPtr>();

        private bool _killThread;

        private readonly Query[] _queries = new Query[BUFFERS];

        private readonly bool[] _issuedQueries = new bool[BUFFERS];

        private readonly Surface[] _copySurfaces = new Surface[BUFFERS];

        private Thread _retrieveThread;

        private bool _supportsDirect3DEx;

        private readonly Surface[] _surfaces = new Surface[BUFFERS];

        private IntPtr _surfaceDataPointer;

        private readonly bool[] _surfaceLocked = new bool[BUFFERS];

        private readonly Mutex[] _sharedMemMutexes;

        private bool _surfacesSetup;

        private int _presentHookRecurse;

        private int _curCapture;

        private int _copyWait;

        private int _currentSurface;

        private const int BUFFERS = 3;

        private readonly EventWaitHandle _captureWaitHandle;
        private readonly EventWaitHandle _hookReadyWaitHandle;

        #endregion

        #region Constructors and Destructors

        public DXHookD3D9SharedMem(CaptureInterface ssInterface)
            : base(ssInterface)
        {
            var security = new MutexSecurity();
            security.AddAccessRule(new MutexAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), MutexRights.Synchronize | MutexRights.Modify, AccessControlType.Allow));
            bool created;
            _sharedMemMutexes = new[]
            {
                new Mutex(false, "Global\\DXHookD3D9Shared0", out created, security),
                new Mutex(false, "Global\\DXHookD3D9Shared1", out created, security)
            };
            var ewsecurity = new EventWaitHandleSecurity();
            ewsecurity.AddAccessRule(new EventWaitHandleAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), EventWaitHandleRights.FullControl, AccessControlType.Allow));
            _captureWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset, "Global\\DXHookD3D9Capture", out created, ewsecurity);
            _hookReadyWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset, "Global\\DXHookD3D9CaptureReady", out created, ewsecurity);
        }

        #endregion

        [DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern IntPtr memcpy(IntPtr dest, IntPtr src, UIntPtr count);

        #region Delegates

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate int Direct3D9DeviceEx_PresentExDelegate(
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
        private delegate int Direct3D9Device_PresentDelegate(
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

        public override void Hook()
        {
            DebugMessage("Hook: Begin");

            DebugMessage("Hook: Before device creation");
            using (var d3D = new Direct3D())
            {
                DebugMessage("Hook: Direct3D created");
                using (
                    var device = new Device(
                        d3D,
                        0,
                        DeviceType.NullReference,
                        IntPtr.Zero,
                        CreateFlags.HardwareVertexProcessing,
                        new PresentParameters() { BackBufferWidth = 1, BackBufferHeight = 1 }))
                {
                    _id3DDeviceFunctionAddresses.AddRange(GetVTblAddresses(device.NativePointer, D3D9_DEVICE_METHOD_COUNT));
                }
            }

            using (var d3DEx = new Direct3DEx())
            {
                DebugMessage("Hook: Try Direct3DEx...");
                using (
                    var deviceEx = new DeviceEx(
                        d3DEx,
                        0,
                        DeviceType.NullReference,
                        IntPtr.Zero,
                        CreateFlags.HardwareVertexProcessing,
                        new PresentParameters() { BackBufferWidth = 1, BackBufferHeight = 1 },
                        new DisplayModeEx() { Width = 800, Height = 600 }))
                {
                    _id3DDeviceFunctionAddresses.AddRange(
                        GetVTblAddresses(deviceEx.NativePointer, D3D9_DEVICE_METHOD_COUNT, D3D9Ex_DEVICE_METHOD_COUNT));
                    _supportsDirect3DEx = true;
                }
            }

            DebugMessage("Setting up Direct3D hooks...");
            _direct3DDeviceEndSceneHook =
                new HookData<Direct3D9Device_EndSceneDelegate>(
                    _id3DDeviceFunctionAddresses[(int)Direct3DDevice9FunctionOrdinals.EndScene],
                    new Direct3D9Device_EndSceneDelegate(EndSceneHook),
                    this);

            _direct3DDeviceEndSceneHook.ReHook();
            Hooks.Add(_direct3DDeviceEndSceneHook.Hook);

            _direct3DDevicePresentHook =
                new HookData<Direct3D9Device_PresentDelegate>(
                    _id3DDeviceFunctionAddresses[(int)Direct3DDevice9FunctionOrdinals.Present],
                    new Direct3D9Device_PresentDelegate(PresentHook),
                    this);

            _direct3DDeviceResetHook =
                new HookData<Direct3D9Device_ResetDelegate>(
                    _id3DDeviceFunctionAddresses[(int)Direct3DDevice9FunctionOrdinals.Reset],
                    new Direct3D9Device_ResetDelegate(ResetHook),
                    this);

            if (_supportsDirect3DEx)
            {
                DebugMessage("Setting up Direct3DEx hooks...");
                _direct3DDeviceExPresentExHook =
                    new HookData<Direct3D9DeviceEx_PresentExDelegate>(
                        _id3DDeviceFunctionAddresses[(int)Direct3DDevice9ExFunctionOrdinals.PresentEx],
                        new Direct3D9DeviceEx_PresentExDelegate(PresentExHook),
                        this);

                _direct3DDeviceExResetExHook =
                    new HookData<Direct3D9DeviceEx_ResetExDelegate>(
                        _id3DDeviceFunctionAddresses[(int)Direct3DDevice9ExFunctionOrdinals.ResetEx],
                        new Direct3D9DeviceEx_ResetExDelegate(ResetExHook),
                        this);
            }

            _direct3DDeviceResetHook.ReHook();
            Hooks.Add(_direct3DDeviceResetHook.Hook);

            _direct3DDevicePresentHook.ReHook();
            Hooks.Add(_direct3DDevicePresentHook.Hook);

            if (_supportsDirect3DEx)
            {
                _direct3DDeviceExPresentExHook.ReHook();
                Hooks.Add(_direct3DDeviceExPresentExHook.Hook);

                _direct3DDeviceExResetExHook.ReHook();
                Hooks.Add(_direct3DDeviceExResetExHook.Hook);
            }

            DebugMessage("Hook: End");
        }

        #endregion

        #region Methods

        protected override void Dispose(bool disposing)
        {
            if (true)
            {
                try
                {
                    ClearData();
                }
                // ReSharper disable once CatchAllClause
                catch
                {
                    // We don't care
                }
            }
            base.Dispose(disposing);
        }

        private void ClearData()
        {
            DebugMessage("ClearData called");
            _hookReadyWaitHandle.Reset();

            if (_retrieveThread != null)
            {
                _killThread = true;
                _copyReadySignal.Set();

                if (!_retrieveThread.Join(500))
                {
                    _retrieveThread.Abort();
                }

                _copyReadySignal.Reset();
                _retrieveThread = null;
            }

            // currentDevice = null;
            if (Request != null)
            {
                Request.Dispose();
                Request = null;
            }

            for (int i = 0; i < BUFFERS; i++)
            {
                if (_surfaceLocked[i])
                {
                    lock (_surfaceLocks[i])
                    {
                        _surfaces[i].UnlockRectangle();
                        _surfaceLocked[i] = false;
                    }
                }
                _issuedQueries[i] = false;
                if (_surfaces[i] != null)
                {
                    _surfaces[i].Dispose();
                    _surfaces[i] = null;
                }
                if (_copySurfaces[i] != null)
                {
                    _copySurfaces[i].Dispose();
                    _copySurfaces[i] = null;
                }
                if (_queries[i] != null)
                {
                    _queries[i].Dispose();
                    _queries[i] = null;
                }
                lock (_surfaceLocks[i])
                {
                    _surfaces[i] = null;
                }
                //if (Monitor.TryEnter(this.surfaceLocks[i]))
                //{
                //    Monitor.Exit(this.surfaceLocks[i]);
                //    this.surfaceLocks[i] = null;
                //}
            }

            for (int i = 0; i < 2; i++)
            {
                if (_sharedTexturesAccess[i] != null)
                {
                    _sharedTexturesAccess[i].SafeMemoryMappedViewHandle.ReleasePointer();
                    _sharedTexturesPtr[i] = (byte*)0;
                    _sharedTexturesAccess[i].Dispose();
                    _sharedTexturesAccess[i] = null;
                }

                if (_sharedTextures[i] != null)
                {
                    _sharedTextures[i].Dispose();
                    _sharedTextures[i] = null;
                }
            }

            _copyData.width = 0;
            _copyData.height = 0;
            _copyData.pitch = 0;
            _copyData.lastRendered = 0;
            _copyData.textureId = Guid.Empty;

            if (_copyDataMemAccess != null)
            {
                _copyDataMemAccess.Write(0, ref _copyData);
                _copyDataMemAccess.Flush();

                _copyDataMemAccess.SafeMemoryMappedViewHandle.ReleasePointer();
                _copyDataMemPtr = (byte*)0;
                _copyDataMemAccess.Dispose();
                _copyDataMemAccess = null;
                // copyDataMemPtr = (byte*)0;
            }

            if (_copyDataMem != null)
            {
                _copyDataMem.Dispose();
                _copyDataMem = null;
            }

            _captureWaitHandle.Reset();
            _hooksStarted = false;
            _surfacesSetup = false;
            _copyWait = 0;
            _curCapture = 0;
            _currentSurface = 0;
            _surfaceDataPointer = IntPtr.Zero;
        }

        /// <summary>
        /// Implementation of capturing from the render target of the Direct3D9 Device (or DeviceEx)
        /// </summary>
        /// <param name="device"></param>
        private void DoCaptureRenderTarget(Device device, string hook)
        {
            try
            {
                if (!_surfacesSetup)
                {
                    using (Surface backbuffer = device.GetRenderTarget(0))
                    {
                        _copyData.format = (int)backbuffer.Description.Format;
                        _copyData.width = backbuffer.Description.Width;
                        _copyData.height = backbuffer.Description.Height;
                    }

                    SetupSurfaces(device);
                }

                if (!_surfacesSetup)
                {
                    return;
                }

                HandleCaptureRequest(device);
            }
            catch (Exception e)
            {
                DebugMessage(e.ToString());
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
                if (!_hooksStarted)
                {
                    DebugMessage("EndSceneHook: hooks not started");
                    SetupData(device);
                }
            }
            catch (Exception ex)
            {
                DebugMessage(ex.ToString());
            }
            hresult = _direct3DDeviceEndSceneHook.Original(devicePtr);
            return hresult;
        }

        private void HandleCaptureRequest(Device device)
        {
            try
            {
                if (_killThread)
                    return;

                for (int i = 0; i < BUFFERS; i++)
                {
                    bool tmp;
                    if (_issuedQueries[i] && _queries[i].GetData(out tmp, false))
                    {
                        _issuedQueries[i] = false;
                        var lockedRect = _surfaces[i].LockRectangle(LockFlags.ReadOnly);
                        _surfaceDataPointer = lockedRect.DataPointer;
                        _currentSurface = i;
                        _surfaceLocked[i] = true;

                        _copyReadySignal.Set();
                    }
                }

                if (_captureWaitHandle.WaitOne(0) || _copyWait < BUFFERS - 1)
                {
                    int nextCapture = (_curCapture == BUFFERS - 1) ? 0 : (_curCapture + 1);

                    try
                    {
                        using (var backbuffer = device.GetBackBuffer(0, 0))
                        {
                            var sourceTexture = _copySurfaces[_curCapture];
                            device.StretchRectangle(backbuffer, sourceTexture, TextureFilter.None);
                        }

                        if (_copyWait < BUFFERS - 1)
                        {
                            _copyWait++;
                        }
                        else
                        {
                            var prevSourceTexture = _copySurfaces[nextCapture];
                            var targetTexture = _surfaces[nextCapture];

                            if (_surfaceLocked[nextCapture])
                            {
                                lock (_surfaceLocks[nextCapture])
                                {
                                    _surfaces[nextCapture].UnlockRectangle();
                                    _surfaceLocked[nextCapture] = false;
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

                            _queries[nextCapture].Issue(Issue.End);
                            _issuedQueries[nextCapture] = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        DebugMessage(ex.ToString());
                    }
                    finally
                    {
                        _captureWaitHandle.Reset();
                        _curCapture = nextCapture;
                    }
                }

            }
            catch (Exception e)
            {
                DebugMessage(e.ToString());
            }
        }

        protected override void InterfaceEventProxy_ScreenshotRequested(ScreenshotRequest request)
        {
            request.Dispose();
        }

        private void RetrieveImageDataThread()
        {
            int sharedMemId = 0;
            while (true)
            {
                _copyReadySignal.Wait();
                _copyReadySignal.Reset();

                if (_killThread)
                    break;

                if (_surfaceDataPointer == IntPtr.Zero)
                {
                    continue;
                }

                int nextSharedMemId = sharedMemId == 0 ? 1 : 0;
                try
                {
                    lock (_surfaceLocks[_currentSurface])
                    {
                        int lastRendered = -1;
                        int lastKnown = -1;
                        try
                        {
                            lastKnown = sharedMemId;
                            if (_sharedMemMutexes[sharedMemId].WaitOne(0))
                            {
                                lastRendered = sharedMemId;
                            }
                            else
                            {
                                lastKnown = nextSharedMemId;
                                if (_sharedMemMutexes[nextSharedMemId].WaitOne(0))
                                {
                                    lastRendered = nextSharedMemId;
                                }
                            }
                        }
                        catch (AbandonedMutexException)
                        {
                            _sharedMemMutexes[lastKnown].ReleaseMutex();
                            continue;
                        }

                        if (lastRendered != -1)
                        {
                            var src = _surfaceDataPointer;

                            if (src == IntPtr.Zero)
                            {
                                continue;
                            }

                            if (_killThread)
                                break;

                            var size = _copyData.height * _copyData.pitch;
                            memcpy(new IntPtr(_sharedTexturesPtr[lastRendered]), src, new UIntPtr((uint)size));

                            _copyData.lastRendered = lastRendered;

                            Marshal.StructureToPtr(_copyData, new IntPtr(_copyDataMemPtr), false);
                            _sharedMemMutexes[lastRendered].ReleaseMutex();
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
            int hresult;
            var device = (DeviceEx)devicePtr;
            if (!_hooksStarted)
            {
                hresult = _direct3DDeviceExPresentExHook.Original(devicePtr, pSourceRect, pDestRect, hDestWindowOverride, pDirtyRegion, dwFlags);
                return hresult;
            }

            try
            {
                if (_presentHookRecurse == 0)
                {
                    DoCaptureRenderTarget(device, "PresentEx");
                }
            }
            catch (Exception ex)
            {
                DebugMessage(ex.ToString());
            }
            finally
            {
                _presentHookRecurse++;
                hresult = _direct3DDeviceExPresentExHook.Original(devicePtr, pSourceRect, pDestRect, hDestWindowOverride, pDirtyRegion, dwFlags);
                _presentHookRecurse--;
            }
            return hresult;
        }

        private int PresentHook(
            IntPtr devicePtr,
            Rectangle* pSourceRect,
            Rectangle* pDestRect,
            IntPtr hDestWindowOverride,
            IntPtr pDirtyRegion)
        {
            int hresult;
            var device = (Device)devicePtr;
            if (!_hooksStarted)
            {
                hresult = _direct3DDevicePresentHook.Original(devicePtr, pSourceRect, pDestRect, hDestWindowOverride, pDirtyRegion);
                return hresult;
            }
            try
            {
                if (_presentHookRecurse == 0)
                {
                    DoCaptureRenderTarget(device, "PresentHook");
                }
            }
            catch (Exception ex)
            {
                DebugMessage(ex.ToString());
            }
            finally
            {
                _presentHookRecurse++;
                hresult = _direct3DDevicePresentHook.Original(devicePtr, pSourceRect, pDestRect, hDestWindowOverride, pDirtyRegion);
                _presentHookRecurse--;
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
            try
            {
                if (!_hooksStarted)
                {
                    hresult = _direct3DDeviceExResetExHook.Original(devicePtr, ref presentparameters, displayModeEx);
                    return hresult;
                }

                ClearData();

                hresult = _direct3DDeviceExResetExHook.Original(devicePtr, ref presentparameters, displayModeEx);

                if (_currentDevice != devicePtr)
                {
                    _hooksStarted = false;
                    _currentDevice = devicePtr;
                }
            }
            catch (Exception ex)
            {
                DebugMessage(ex.ToString());
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
            try
            {
                if (!_hooksStarted)
                {
                    hresult = _direct3DDeviceResetHook.Original(devicePtr, ref presentParameters);
                    return hresult;
                }

                ClearData();

                hresult = _direct3DDeviceResetHook.Original(devicePtr, ref presentParameters);

                if (_currentDevice != devicePtr)
                {
                    _hooksStarted = false;
                    _currentDevice = devicePtr;
                }
            }
            catch (Exception ex)
            {
                DebugMessage(ex.ToString());
            }
            return hresult;
        }

        private void SetupData(Device device)
        {
            DebugMessage("SetupData called");

            using (SwapChain swapChain = device.GetSwapChain(0))
            {
                PresentParameters pp = swapChain.PresentParameters;
                _copyData.width = pp.BackBufferWidth;
                _copyData.height = pp.BackBufferHeight;
                _copyData.format = (int)pp.BackBufferFormat;

                DebugMessage(string.Format("D3D9 Setup: w: {0} h: {1} f: {2}", _copyData.width, _copyData.height, pp.BackBufferFormat));
            }

            _hooksStarted = true;
        }

        private void SetupSurfaces(Device device)
        {
            try
            {
                for (int i = 0; i < BUFFERS; i++)
                {
                    _surfaces[i] = Surface.CreateOffscreenPlain(device, _copyData.width, _copyData.height, (Format)_copyData.format, Pool.SystemMemory);
                    var lockedRect = _surfaces[i].LockRectangle(LockFlags.ReadOnly);
                    _copyData.pitch = lockedRect.Pitch;
                    _surfaces[i].UnlockRectangle();

                    _copySurfaces[i] = Surface.CreateRenderTarget(device, _copyData.width, _copyData.height, (Format)_copyData.format, MultisampleType.None, 0, false);
                    _queries[i] = new Query(device, QueryType.Event);

                    _surfaceLocks[i] = new object();
                }

                _copyDataMem = MemoryMappedFile.CreateOrOpen("CaptureHookSharedMemData", Marshal.SizeOf(typeof(CopyData)));
                _copyDataMemAccess = _copyDataMem.CreateViewAccessor(0, Marshal.SizeOf(typeof(CopyData)));
                _copyDataMemAccess.SafeMemoryMappedViewHandle.AcquirePointer(ref _copyDataMemPtr);

                _copyData.textureId = Guid.NewGuid();

                for (int i = 0; i < 2; i++)
                {
                    bool locked = false;
                    try
                    {
                        try
                        {
                            locked = _sharedMemMutexes[i].WaitOne(0);
                        }
                        catch (AbandonedMutexException)
                        {
                            locked = true;
                        }
                        if (!locked)
                        {
                            DebugMessage("shared mem mutex still locked");
                        }

                        _sharedTextures[i] = MemoryMappedFile.CreateOrOpen(
                            _copyData.textureId.ToString() + i,
                            _copyData.pitch * _copyData.height,
                            MemoryMappedFileAccess.ReadWrite);
                        _sharedTexturesAccess[i] = _sharedTextures[i].CreateViewAccessor(0, _copyData.pitch * _copyData.height, MemoryMappedFileAccess.ReadWrite);
                        _sharedTexturesAccess[i].SafeMemoryMappedViewHandle.AcquirePointer(ref _sharedTexturesPtr[i]);
                    }
                    finally
                    {
                        if (locked)
                        {
                            _sharedMemMutexes[i].ReleaseMutex();
                        }
                    }
                }

                _killThread = false;
                _surfacesSetup = true;

                Marshal.StructureToPtr(_copyData, new IntPtr(_copyDataMemPtr), false);

                _retrieveThread = new Thread(RetrieveImageDataThread);
                _retrieveThread.IsBackground = true;
                _retrieveThread.Start();
                _hookReadyWaitHandle.Set();
            }
            catch (Exception ex)
            {
                DebugMessage(ex.ToString());
                ClearData();
            }
        }

        #endregion
    }
}