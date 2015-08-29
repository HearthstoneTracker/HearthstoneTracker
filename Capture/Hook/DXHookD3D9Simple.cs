using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using Capture.Interface;
using SharpDX;
using SharpDX.Direct3D9;

namespace Capture.Hook
{
    internal class DXHookD3D9Simple : BaseDXHook
    {
        #region Constants

        private const int D3D9Ex_DEVICE_METHOD_COUNT = 15;

        private const int D3D9_DEVICE_METHOD_COUNT = 119;

        #endregion

        #region Fields

        private readonly ManualResetEventSlim _copyEvent = new ManualResetEventSlim(false);

        private readonly ManualResetEventSlim _copyReadySignal = new ManualResetEventSlim(false);

        private readonly object _endSceneLock = new object();

        private readonly object _renderTargetLock = new object();

        private readonly object _surfaceLock = new object();

        private HookData<Direct3D9DeviceEx_PresentExDelegate> Direct3DDeviceEx_PresentExHook;

        private HookData<Direct3D9DeviceEx_ResetExDelegate> Direct3DDeviceEx_ResetExHook;

        private HookData<Direct3D9Device_EndSceneDelegate> Direct3DDevice_EndSceneHook;

        private HookData<Direct3D9Device_PresentDelegate> Direct3DDevice_PresentHook;

        private HookData<Direct3D9Device_ResetDelegate> Direct3DDevice_ResetHook;

        private Thread _copyThread;

        private IntPtr _currentDevice;

        private Format _format;

        private int _height;

        private bool _hooksStarted;

        private readonly List<IntPtr> _id3DDeviceFunctionAddresses = new List<IntPtr>();

        private bool _killThread;

        private int _pitch;

        private Query _query;

        private bool _queryIssued;

        private Surface _renderTarget;

        private RetrieveImageDataParams? _retrieveParams;

        private Thread _retrieveThread;

        private bool _supportsDirect3DEx;

        private Surface _surface;

        private IntPtr _surfaceDataPointer;

        private bool _surfaceLocked;

        private bool _surfacesSetup;

        private int _width;

        private int _presentHookRecurse;

        private Guid? _lastRequestId;

        #endregion

        #region Constructors and Destructors

        public DXHookD3D9Simple(CaptureInterface ssInterface)
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
        ///     The IDirect3DDevice9.EndScene function definition
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
        ///     The IDirect3DDevice9.Reset function definition
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
            get { return "DXHookD3D9"; }
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
            using (var d3d = new Direct3D())
            {
                DebugMessage("Hook: Direct3D created");
                using (
                    var device = new Device(
                        d3d,
                        0,
                        DeviceType.NullReference,
                        IntPtr.Zero,
                        CreateFlags.HardwareVertexProcessing,
                        new PresentParameters { BackBufferWidth = 1, BackBufferHeight = 1 }))
                {
                    _id3DDeviceFunctionAddresses.AddRange(GetVTblAddresses(device.NativePointer, D3D9_DEVICE_METHOD_COUNT));
                }
            }

            try
            {
                using (var d3dEx = new Direct3DEx())
                {
                    DebugMessage("Hook: Try Direct3DEx...");
                    using (
                        var deviceEx = new DeviceEx(
                            d3dEx,
                            0,
                            DeviceType.NullReference,
                            IntPtr.Zero,
                            CreateFlags.HardwareVertexProcessing,
                            new PresentParameters { BackBufferWidth = 1, BackBufferHeight = 1 },
                            new DisplayModeEx { Width = 800, Height = 600 }))
                    {
                        _id3DDeviceFunctionAddresses.AddRange(
                            GetVTblAddresses(deviceEx.NativePointer, D3D9_DEVICE_METHOD_COUNT, D3D9Ex_DEVICE_METHOD_COUNT));
                        _supportsDirect3DEx = true;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            DebugMessage("Setting up Direct3D hooks...");
            Direct3DDevice_EndSceneHook =
                new HookData<Direct3D9Device_EndSceneDelegate>(
                    _id3DDeviceFunctionAddresses[(int)Direct3DDevice9FunctionOrdinals.EndScene],
                    new Direct3D9Device_EndSceneDelegate(EndSceneHook),
                    this);

            Direct3DDevice_EndSceneHook.ReHook();
            Hooks.Add(Direct3DDevice_EndSceneHook.Hook);

            Direct3DDevice_PresentHook =
                new HookData<Direct3D9Device_PresentDelegate>(
                    _id3DDeviceFunctionAddresses[(int)Direct3DDevice9FunctionOrdinals.Present],
                    new Direct3D9Device_PresentDelegate(PresentHook),
                    this);

            Direct3DDevice_ResetHook =
                new HookData<Direct3D9Device_ResetDelegate>(
                    _id3DDeviceFunctionAddresses[(int)Direct3DDevice9FunctionOrdinals.Reset],
                    new Direct3D9Device_ResetDelegate(ResetHook),
                    this);

            if (_supportsDirect3DEx)
            {
                DebugMessage("Setting up Direct3DEx hooks...");
                Direct3DDeviceEx_PresentExHook =
                    new HookData<Direct3D9DeviceEx_PresentExDelegate>(
                        _id3DDeviceFunctionAddresses[(int)Direct3DDevice9ExFunctionOrdinals.PresentEx],
                        new Direct3D9DeviceEx_PresentExDelegate(PresentExHook),
                        this);

                Direct3DDeviceEx_ResetExHook =
                    new HookData<Direct3D9DeviceEx_ResetExDelegate>(
                        _id3DDeviceFunctionAddresses[(int)Direct3DDevice9ExFunctionOrdinals.ResetEx],
                        new Direct3D9DeviceEx_ResetExDelegate(ResetExHook),
                        this);
            }

            Direct3DDevice_ResetHook.ReHook();
            Hooks.Add(Direct3DDevice_ResetHook.Hook);

            Direct3DDevice_PresentHook.ReHook();
            Hooks.Add(Direct3DDevice_PresentHook.Hook);

            if (_supportsDirect3DEx)
            {
                Direct3DDeviceEx_PresentExHook.ReHook();
                Hooks.Add(Direct3DDeviceEx_PresentExHook.Hook);

                Direct3DDeviceEx_ResetExHook.ReHook();
                Hooks.Add(Direct3DDeviceEx_ResetExHook.Hook);
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
                catch
                {
                }
            }
            base.Dispose(disposing);
        }

        private void ClearData()
        {
            DebugMessage("ClearData called");

            if (_copyThread != null)
            {
                _killThread = true;
                _copyEvent.Set();

                if (!_copyThread.Join(500))
                {
                    _copyThread.Abort();
                }

                _copyEvent.Reset();
                _copyThread = null;
            }

            if (_retrieveThread != null)
            {
                _killThread = true;
                _copyReadySignal.Set();

                if (_retrieveThread.Join(500))
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

            _width = 0;
            _height = 0;
            _pitch = 0;
            if (_surfaceLocked)
            {
                lock (_surfaceLock)
                {
                    _surface.UnlockRectangle();
                    _surfaceLocked = false;
                }
            }

            if (_surface != null)
            {
                _surface.Dispose();
                _surface = null;
            }
            if (_renderTarget != null)
            {
                _renderTarget.Dispose();
                _renderTarget = null;
            }
            if (_query != null)
            {
                _query.Dispose();
                _query = null;
                _queryIssued = false;
            }
            _hooksStarted = false;
            _surfacesSetup = false;
        }

        /// <summary>
        ///     Implementation of capturing from the render target of the Direct3D9 Device (or DeviceEx)
        /// </summary>
        /// <param name="device"></param>
        private void DoCaptureRenderTarget(Device device, string hook)
        {
            try
            {
                if (!_surfacesSetup)
                {
                    using (var backbuffer = device.GetRenderTarget(0))
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

                if (Request != null)
                {
                    try
                    {
                        _lastRequestId = Request.RequestId;
                        HandleCaptureRequest(device);
                    }
                    finally
                    {
                        Request.Dispose();
                        Request = null;
                    }
                }
            }
            catch (Exception e)
            {
                DebugMessage(e.ToString());
            }
        }

        /// <summary>
        ///     Hook for IDirect3DDevice9.EndScene
        /// </summary>
        /// <param name="devicePtr">
        ///     Pointer to the IDirect3DDevice9 instance. Note: object member functions always pass "this" as
        ///     the first parameter.
        /// </param>
        /// <returns>The HRESULT of the original EndScene</returns>
        /// <remarks>
        ///     Remember that this is called many times a second by the Direct3D application - be mindful of memory and
        ///     performance!
        /// </remarks>
        private int EndSceneHook(IntPtr devicePtr)
        {
            var hresult = Result.Ok.Code;
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
            hresult = Direct3DDevice_EndSceneHook.Original(devicePtr);
            return hresult;
        }

        private void HandleCaptureRequest(Device device)
        {
            try
            {
                bool tmp;
                if (_queryIssued && _query.GetData(out tmp, false))
                {
                    _queryIssued = false;
                    var lockedRect = _surface.LockRectangle(LockFlags.ReadOnly);
                    _surfaceDataPointer = lockedRect.DataPointer;
                    _surfaceLocked = true;

                    _copyEvent.Set();
                }

                using (var backbuffer = device.GetBackBuffer(0, 0))
                {
                    device.StretchRectangle(backbuffer, _renderTarget, TextureFilter.None);
                }

                if (_surfaceLocked)
                {
                    lock (_renderTargetLock)
                    {
                        if (_surfaceLocked)
                        {
                            _surface.UnlockRectangle();
                            _surfaceLocked = false;
                        }
                    }
                }

                try
                {
                    var cooplevel = device.TestCooperativeLevel();
                    if (cooplevel.Code == ResultCode.Success.Code)
                    {
                        device.GetRenderTargetData(_renderTarget, _surface);
                        _query.Issue(Issue.End);
                        _queryIssued = true;
                    }
                    else
                    {
                        DebugMessage(string.Format("DirectX Error: TestCooperativeLevel = {0}", cooplevel.Code));
                    }
                }
                catch (Exception ex)
                {
                    DebugMessage(ex.ToString());
                }
            }
            catch (Exception e)
            {
                DebugMessage(e.ToString());
            }
        }

        private void HandleCaptureRequestThread()
        {
            while (true)
            {
                _copyEvent.Wait();
                _copyEvent.Reset();

                if (_killThread)
                {
                    break;
                }

                var requestId = _lastRequestId;
                if (requestId == null
                    || _surfaceDataPointer == IntPtr.Zero)
                {
                    continue;
                }

                try
                {
                    lock (_renderTargetLock)
                    {
                        if (_surfaceDataPointer == IntPtr.Zero)
                        {
                            continue;
                        }

                        var size = _height * _pitch;
                        var bdata = new byte[size];
                        Marshal.Copy(_surfaceDataPointer, bdata, 0, size);
                        // Marshal.FreeHGlobal(this.surfaceDataPointer);

                        _retrieveParams = new RetrieveImageDataParams
                            {
                                RequestId = requestId.Value,
                                Data = bdata,
                                Width = _width,
                                Height = _height,
                                Pitch = _pitch
                            };

                        _copyReadySignal.Set();
                    }
                }
                catch (Exception ex)
                {
                    DebugMessage(ex.ToString());
                }
                finally
                {
                }
            }
        }

        private void RetrieveImageDataThread()
        {
            while (true)
            {
                _copyReadySignal.Wait();
                _copyReadySignal.Reset();

                if (_killThread)
                {
                    break;
                }

                if (_retrieveParams == null)
                {
                    continue;
                }
                try
                {
                    ProcessCapture(_retrieveParams.Value);
                }
                finally
                {
                    _retrieveParams = null;
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
            var hresult = Result.Ok.Code;
            var device = (DeviceEx)devicePtr;
            if (!_hooksStarted)
            {
                hresult = Direct3DDeviceEx_PresentExHook.Original(devicePtr, pSourceRect, pDestRect, hDestWindowOverride, pDirtyRegion, dwFlags);
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
                hresult = Direct3DDeviceEx_PresentExHook.Original(devicePtr, pSourceRect, pDestRect, hDestWindowOverride, pDirtyRegion, dwFlags);
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
                hresult = Direct3DDevice_PresentHook.Original(devicePtr, pSourceRect, pDestRect, hDestWindowOverride, pDirtyRegion);
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
                hresult = Direct3DDevice_PresentHook.Original(devicePtr, pSourceRect, pDestRect, hDestWindowOverride, pDirtyRegion);
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
            var hresult = Result.Ok.Code;
            var device = (DeviceEx)devicePtr;
            try
            {
                if (!_hooksStarted)
                {
                    hresult = Direct3DDeviceEx_ResetExHook.Original(devicePtr, ref presentparameters, displayModeEx);
                    return hresult;
                }

                ClearData();

                hresult = Direct3DDeviceEx_ResetExHook.Original(devicePtr, ref presentparameters, displayModeEx);

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
        ///     Reset the _renderTarget so that we are sure it will have the correct presentation parameters (required to support
        ///     working across changes to windowed/fullscreen or resolution changes)
        /// </summary>
        /// <param name="devicePtr"></param>
        /// <param name="presentParameters"></param>
        /// <returns></returns>
        private int ResetHook(IntPtr devicePtr, ref PresentParameters presentParameters)
        {
            var hresult = Result.Ok.Code;
            var device = (Device)devicePtr;
            try
            {
                if (!_hooksStarted)
                {
                    hresult = Direct3DDevice_ResetHook.Original(devicePtr, ref presentParameters);
                    return hresult;
                }

                ClearData();

                hresult = Direct3DDevice_ResetHook.Original(devicePtr, ref presentParameters);

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

            using (var swapChain = device.GetSwapChain(0))
            {
                var pp = swapChain.PresentParameters;
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
                _surface = Surface.CreateOffscreenPlain(device, _width, _height, _format, Pool.SystemMemory);
                var lockedRect = _surface.LockRectangle(LockFlags.ReadOnly);
                _pitch = lockedRect.Pitch;
                _surface.UnlockRectangle();
                _renderTarget = Surface.CreateRenderTarget(device, _width, _height, _format, MultisampleType.None, 0, false);
                _query = new Query(device, QueryType.Event);

                _killThread = false;
                _copyThread = new Thread(HandleCaptureRequestThread);
                _copyThread.IsBackground = true;
                _copyThread.Start();

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
