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

        private readonly ManualResetEventSlim copyReadySignal = new ManualResetEventSlim(false);

        private readonly object[] surfaceLocks = new object[BUFFERS];

        private HookData<Direct3D9DeviceEx_PresentExDelegate> Direct3DDeviceEx_PresentExHook = null;

        private HookData<Direct3D9DeviceEx_ResetExDelegate> Direct3DDeviceEx_ResetExHook = null;

        private HookData<Direct3D9Device_EndSceneDelegate> Direct3DDevice_EndSceneHook = null;

        private HookData<Direct3D9Device_PresentDelegate> Direct3DDevice_PresentHook = null;

        private HookData<Direct3D9Device_ResetDelegate> Direct3DDevice_ResetHook = null;

        // private Thread copyThread;

        private IntPtr currentDevice;

        private Format format;

        private int height;

        private bool hooksStarted;

        private List<IntPtr> id3dDeviceFunctionAddresses = new List<IntPtr>();

        private bool killThread;

        private int pitch;

        private Query[] queries = new Query[BUFFERS];

        private bool[] issuedQueries = new bool[BUFFERS];

        private Surface[] copySurfaces = new Surface[BUFFERS];

        private Thread retrieveThread;

        private bool supportsDirect3DEx = false;

        private Surface[] surfaces = new Surface[BUFFERS];

        private IntPtr surfaceDataPointer;

        private bool[] surfaceLocked = new bool[BUFFERS];

        private bool surfacesSetup;

        private int width;

        private int presentHookRecurse;

        private Guid? lastRequestId;

        private int curCapture;

        private int copyWait;

        private int currentSurface;

        private const int BUFFERS = 2;

        private Guid? previousRequestId;

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
                if (Monitor.TryEnter(this.surfaceLocks[i]))
                {
                    Monitor.Exit(this.surfaceLocks[i]);
                    this.surfaceLocks[i] = null;
                }
                this.issuedQueries[i] = false;
            }

            this.width = 0;
            this.height = 0;
            this.pitch = 0;
            this.hooksStarted = false;
            this.surfacesSetup = false;
            this.copyWait = 0;
            this.curCapture = 0;
            this.currentSurface = 0;
            lastRequestId = null;
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
                        this.format = backbuffer.Description.Format;
                        this.width = backbuffer.Description.Width;
                        this.height = backbuffer.Description.Height;
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
                for (int i = 0; i < BUFFERS; i++)
                {
                    bool tmp;
                    if (this.issuedQueries[i] && this.queries[i].GetData(out tmp, false))
                    {
                        this.issuedQueries[i] = false;
                        try
                        {
                            var lockedRect = this.surfaces[i].LockRectangle(LockFlags.ReadOnly);
                            this.surfaceDataPointer = lockedRect.DataPointer;
                            this.currentSurface = i;
                            this.surfaceLocked[i] = true;
                        }
                        catch (Exception ex)
                        {
                            DebugMessage(ex.ToString());
                        }
                    }
                }

                if (previousRequestId != null || lastRequestId != null)
                {
                    this.previousRequestId = null;
                    int nextCapture = (curCapture == BUFFERS - 1) ? 0 : (curCapture + 1);

                    var sourceTexture = copySurfaces[curCapture];
                    try
                    {
                        using (var backbuffer = device.GetBackBuffer(0, 0))
                        {
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

                    curCapture = nextCapture;
                }

            }
            catch (Exception e)
            {
                this.DebugMessage(e.ToString());
            }
        }

        protected override void InterfaceEventProxy_ScreenshotRequested(ScreenshotRequest request)
        {
            if (surfacesSetup)
            {
                this.lastRequestId = request.RequestId;
                this.copyReadySignal.Set();                
            }
            request.Dispose();
        }

        private void RetrieveImageDataThread()
        {
            while (true)
            {
                this.copyReadySignal.Wait();
                this.copyReadySignal.Reset();

                if (killThread)
                    break;

                if (this.lastRequestId == null)
                {
                    continue;
                }

                if (this.surfaceDataPointer == IntPtr.Zero)
                {
                    continue;
                }
                
                var requestId = lastRequestId.Value;
                this.previousRequestId = this.lastRequestId.Value;
                this.lastRequestId = null;
                try
                {
                    var size = this.height * this.pitch;
                    var bdata = new byte[size];
                    lock (this.surfaceLocks[currentSurface])
                    {
                        Marshal.Copy(this.surfaceDataPointer, bdata, 0, size);
                    }
                    this.ProcessCapture(
                        new RetrieveImageDataParams()
                            {
                                RequestId = requestId,
                                Data = bdata,
                                Width = this.width,
                                Height = this.height,
                                Pitch = this.pitch
                            });
                }
                catch(Exception ex)
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
                this.width = pp.BackBufferWidth;
                this.height = pp.BackBufferHeight;
                this.format = pp.BackBufferFormat;

                this.DebugMessage(string.Format("D3D9 Setup: w: {0} h: {1} f: {2}", this.width, this.height, this.format));
            }

            this.hooksStarted = true;
        }

        private void SetupSurfaces(Device device)
        {
            try
            {
                for (int i = 0; i < BUFFERS; i++)
                {
                    this.surfaces[i] = Surface.CreateOffscreenPlain(device, this.width, this.height, (Format)this.format, Pool.SystemMemory);
                    var lockedRect = this.surfaces[i].LockRectangle(LockFlags.ReadOnly);
                    this.pitch = lockedRect.Pitch;
                    this.surfaces[i].UnlockRectangle();

                    this.copySurfaces[i] = Surface.CreateRenderTarget(device, this.width, this.height, this.format, MultisampleType.None, 0, false);
                    this.queries[i] = new Query(device, QueryType.Event);

                    this.surfaceLocks[i] = new object();
                }

                killThread = false;
                this.retrieveThread = new Thread(this.RetrieveImageDataThread);
                this.retrieveThread.IsBackground = true;
                this.retrieveThread.Start();

                this.surfacesSetup = true;
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