namespace Capture.Hook
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Threading;

    using Capture.Interface;

    using SharpDX;
    using SharpDX.Direct3D9;

    internal class DXHookD3D9 : BaseDXHook
    {
        #region Constants

        private const int D3D9Ex_DEVICE_METHOD_COUNT = 15;

        private const int D3D9_DEVICE_METHOD_COUNT = 119;

        #endregion

        #region Fields

        // private readonly ManualResetEventSlim copyEvent = new ManualResetEventSlim(false);

        private readonly ManualResetEventSlim _copyReadySignal = new ManualResetEventSlim(false);

        private readonly object[] _surfaceLocks = new object[BUFFERS];

        private HookData<Direct3D9DeviceEx_PresentExDelegate> _direct3DDeviceExPresentExHook;

        private HookData<Direct3D9DeviceEx_ResetExDelegate> _direct3DDeviceExResetExHook;

        private HookData<Direct3D9Device_EndSceneDelegate> _direct3DDeviceEndSceneHook;

        private HookData<Direct3D9Device_PresentDelegate> _direct3DDevicePresentHook;

        private HookData<Direct3D9Device_ResetDelegate> _direct3DDeviceResetHook;

        // private Thread copyThread;

        private IntPtr _currentDevice;

        private Format _format;

        private int _height;

        private bool _hooksStarted;

        private readonly List<IntPtr> _id3DDeviceFunctionAddresses = new List<IntPtr>();

        private bool _killThread;

        private int _pitch;

        private readonly Query[] _queries = new Query[BUFFERS];

        private readonly bool[] _issuedQueries = new bool[BUFFERS];

        private readonly Surface[] _copySurfaces = new Surface[BUFFERS];

        private Thread _retrieveThread;

        private bool _supportsDirect3DEx;

        private readonly Surface[] _surfaces = new Surface[BUFFERS];

        private IntPtr _surfaceDataPointer;

        private readonly bool[] _surfaceLocked = new bool[BUFFERS];

        private bool _surfacesSetup;

        private int _width;

        private int _presentHookRecurse;

        private Guid? _lastRequestId;

        private int _curCapture;

        private int _copyWait;

        private int _currentSurface;

        private const int BUFFERS = 2;

        private Guid? _previousRequestId;

        #endregion

        #region Constructors and Destructors

        public DXHookD3D9(CaptureInterface ssInterface)
            : base(ssInterface)
        {
        }

        #endregion

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
                // ReSharper disable once EmptyGeneralCatchClause
                // ReSharper disable once CatchAllClause
                catch
                {
                    // Don't care
                }
            }
            base.Dispose(disposing);
        }

        private void ClearData()
        {
            DebugMessage("ClearData called");

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
                if (Monitor.TryEnter(_surfaceLocks[i]))
                {
                    Monitor.Exit(_surfaceLocks[i]);
                    _surfaceLocks[i] = null;
                }
                _issuedQueries[i] = false;
            }

            _width = 0;
            _height = 0;
            _pitch = 0;
            _hooksStarted = false;
            _surfacesSetup = false;
            _copyWait = 0;
            _curCapture = 0;
            _currentSurface = 0;
            _lastRequestId = null;
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
                        _format = backbuffer.Description.Format;
                        _width = backbuffer.Description.Width;
                        _height = backbuffer.Description.Height;
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

            var hresult = _direct3DDeviceEndSceneHook.Original(devicePtr);
            return hresult;
        }

        private void HandleCaptureRequest(Device device)
        {
            try
            {
                for (int i = 0; i < BUFFERS; i++)
                {
                    bool tmp;
                    if (_issuedQueries[i] && _queries[i].GetData(out tmp, false))
                    {
                        _issuedQueries[i] = false;
                        try
                        {
                            var lockedRect = _surfaces[i].LockRectangle(LockFlags.ReadOnly);
                            _surfaceDataPointer = lockedRect.DataPointer;
                            _currentSurface = i;
                            _surfaceLocked[i] = true;
                        }
                        catch (Exception ex)
                        {
                            DebugMessage(ex.ToString());
                        }
                    }
                }

                if (_previousRequestId != null || _lastRequestId != null)
                {
                    _previousRequestId = null;
                    int nextCapture = (_curCapture == BUFFERS - 1) ? 0 : (_curCapture + 1);

                    var sourceTexture = _copySurfaces[_curCapture];
                    try
                    {
                        using (var backbuffer = device.GetBackBuffer(0, 0))
                        {
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

                    _curCapture = nextCapture;
                }

            }
            catch (Exception e)
            {
                DebugMessage(e.ToString());
            }
        }

        protected override void InterfaceEventProxy_ScreenshotRequested(ScreenshotRequest request)
        {
            if (_surfacesSetup)
            {
                _lastRequestId = request.RequestId;
                _copyReadySignal.Set();
            }
            request.Dispose();
        }

        private void RetrieveImageDataThread()
        {
            while (true)
            {
                _copyReadySignal.Wait();
                _copyReadySignal.Reset();

                if (_killThread)
                    break;

                if (_lastRequestId == null)
                {
                    continue;
                }

                if (_surfaceDataPointer == IntPtr.Zero)
                {
                    continue;
                }

                var requestId = _lastRequestId.Value;
                _previousRequestId = _lastRequestId.Value;
                _lastRequestId = null;
                try
                {
                    var size = _height * _pitch;
                    var bdata = new byte[size];
                    lock (_surfaceLocks[_currentSurface])
                    {
                        Marshal.Copy(_surfaceDataPointer, bdata, 0, size);
                    }
                    ProcessCapture(
                        new RetrieveImageDataParams()
                        {
                            RequestId = requestId,
                            Data = bdata,
                            Width = _width,
                            Height = _height,
                            Pitch = _pitch
                        });
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

        private unsafe int PresentHook(
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
                _width = pp.BackBufferWidth;
                _height = pp.BackBufferHeight;
                _format = pp.BackBufferFormat;

                DebugMessage(string.Format("D3D9 Setup: w: {0} h: {1} f: {2}", _width, _height, _format));
            }

            _hooksStarted = true;
        }

        private void SetupSurfaces(Device device)
        {
            try
            {
                for (int i = 0; i < BUFFERS; i++)
                {
                    _surfaces[i] = Surface.CreateOffscreenPlain(device, _width, _height, _format, Pool.SystemMemory);
                    var lockedRect = _surfaces[i].LockRectangle(LockFlags.ReadOnly);
                    _pitch = lockedRect.Pitch;
                    _surfaces[i].UnlockRectangle();

                    _copySurfaces[i] = Surface.CreateRenderTarget(device, _width, _height, _format, MultisampleType.None, 0, false);
                    _queries[i] = new Query(device, QueryType.Event);

                    _surfaceLocks[i] = new object();
                }

                _killThread = false;
                _retrieveThread = new Thread(RetrieveImageDataThread);
                _retrieveThread.IsBackground = true;
                _retrieveThread.Start();

                _surfacesSetup = true;
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