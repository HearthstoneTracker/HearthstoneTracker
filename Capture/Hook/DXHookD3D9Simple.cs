// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DXHookD3D9Simple.cs" company="">
//   
// </copyright>
// <summary>
//   The dx hook d 3 d 9 simple.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Capture.Hook
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Threading;

    using Capture.Interface;

    using SharpDX;
    using SharpDX.Direct3D9;

    /// <summary>
    /// The dx hook d 3 d 9 simple.
    /// </summary>
    internal class DXHookD3D9Simple : BaseDXHook
    {
        #region Constants

        /// <summary>
        /// The d 3 d 9 ex_ devic e_ metho d_ count.
        /// </summary>
        private const int D3D9Ex_DEVICE_METHOD_COUNT = 15;

        /// <summary>
        /// The d 3 d 9_ devic e_ metho d_ count.
        /// </summary>
        private const int D3D9_DEVICE_METHOD_COUNT = 119;

        #endregion

        #region Fields

        /// <summary>
        /// The copy event.
        /// </summary>
        private readonly ManualResetEventSlim copyEvent = new ManualResetEventSlim(false);

        /// <summary>
        /// The copy ready signal.
        /// </summary>
        private readonly ManualResetEventSlim copyReadySignal = new ManualResetEventSlim(false);

        /// <summary>
        /// The end scene lock.
        /// </summary>
        private readonly object endSceneLock = new object();

        /// <summary>
        /// The render target lock.
        /// </summary>
        private readonly object renderTargetLock = new object();

        /// <summary>
        /// The surface lock.
        /// </summary>
        private readonly object surfaceLock = new object();

        /// <summary>
        /// The direct 3 d device ex_ present ex hook.
        /// </summary>
        private HookData<Direct3D9DeviceEx_PresentExDelegate> Direct3DDeviceEx_PresentExHook;

        /// <summary>
        /// The direct 3 d device ex_ reset ex hook.
        /// </summary>
        private HookData<Direct3D9DeviceEx_ResetExDelegate> Direct3DDeviceEx_ResetExHook;

        /// <summary>
        /// The direct 3 d device_ end scene hook.
        /// </summary>
        private HookData<Direct3D9Device_EndSceneDelegate> Direct3DDevice_EndSceneHook;

        /// <summary>
        /// The direct 3 d device_ present hook.
        /// </summary>
        private HookData<Direct3D9Device_PresentDelegate> Direct3DDevice_PresentHook;

        /// <summary>
        /// The direct 3 d device_ reset hook.
        /// </summary>
        private HookData<Direct3D9Device_ResetDelegate> Direct3DDevice_ResetHook;

        /// <summary>
        /// The copy thread.
        /// </summary>
        private Thread copyThread;

        /// <summary>
        /// The current device.
        /// </summary>
        private IntPtr currentDevice;

        /// <summary>
        /// The format.
        /// </summary>
        private Format format;

        /// <summary>
        /// The height.
        /// </summary>
        private int height;

        /// <summary>
        /// The hooks started.
        /// </summary>
        private bool hooksStarted;

        /// <summary>
        /// The id 3 d device function addresses.
        /// </summary>
        private List<IntPtr> id3dDeviceFunctionAddresses = new List<IntPtr>();

        /// <summary>
        /// The is using present.
        /// </summary>
        private bool isUsingPresent;

        /// <summary>
        /// The kill thread.
        /// </summary>
        private bool killThread;

        /// <summary>
        /// The pitch.
        /// </summary>
        private int pitch;

        /// <summary>
        /// The query.
        /// </summary>
        private Query query;

        /// <summary>
        /// The query issued.
        /// </summary>
        private bool queryIssued;

        /// <summary>
        /// The render target.
        /// </summary>
        private Surface renderTarget;

        /// <summary>
        /// The retrieve params.
        /// </summary>
        private RetrieveImageDataParams? retrieveParams;

        /// <summary>
        /// The retrieve thread.
        /// </summary>
        private Thread retrieveThread;

        /// <summary>
        /// The supports direct 3 d ex.
        /// </summary>
        private bool supportsDirect3DEx;

        /// <summary>
        /// The surface.
        /// </summary>
        private Surface surface;

        /// <summary>
        /// The surface data pointer.
        /// </summary>
        private IntPtr surfaceDataPointer;

        /// <summary>
        /// The surface locked.
        /// </summary>
        private bool surfaceLocked;

        /// <summary>
        /// The surfaces setup.
        /// </summary>
        private bool surfacesSetup;

        /// <summary>
        /// The width.
        /// </summary>
        private int width;

        /// <summary>
        /// The present hook recurse.
        /// </summary>
        private int presentHookRecurse;

        /// <summary>
        /// The last request id.
        /// </summary>
        private Guid? lastRequestId;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DXHookD3D9Simple"/> class.
        /// </summary>
        /// <param name="ssInterface">
        /// The ss interface.
        /// </param>
        public DXHookD3D9Simple(CaptureInterface ssInterface)
            : base(ssInterface)
        {
        }

        #endregion

        #region Delegates

        /// <summary>
        /// The direct 3 d 9 device ex_ present ex delegate.
        /// </summary>
        /// <param name="devicePtr">
        /// The device ptr.
        /// </param>
        /// <param name="pSourceRect">
        /// The p source rect.
        /// </param>
        /// <param name="pDestRect">
        /// The p dest rect.
        /// </param>
        /// <param name="hDestWindowOverride">
        /// The h dest window override.
        /// </param>
        /// <param name="pDirtyRegion">
        /// The p dirty region.
        /// </param>
        /// <param name="dwFlags">
        /// The dw flags.
        /// </param>
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        private unsafe delegate int Direct3D9DeviceEx_PresentExDelegate(
            IntPtr devicePtr, 
            Rectangle* pSourceRect, 
            Rectangle* pDestRect, 
            IntPtr hDestWindowOverride, 
            IntPtr pDirtyRegion, 
            Present dwFlags);

        /// <summary>
        /// The direct 3 d 9 device ex_ reset ex delegate.
        /// </summary>
        /// <param name="devicePtr">
        /// The device ptr.
        /// </param>
        /// <param name="presentParameters">
        /// The present parameters.
        /// </param>
        /// <param name="displayModeEx">
        /// The display mode ex.
        /// </param>
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate int Direct3D9DeviceEx_ResetExDelegate(IntPtr devicePtr, ref PresentParameters presentParameters, DisplayModeEx displayModeEx);

        /// <summary>
        /// The IDirect3DDevice9.EndScene function definition
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate int Direct3D9Device_EndSceneDelegate(IntPtr device);

        /// <summary>
        /// The direct 3 d 9 device_ present delegate.
        /// </summary>
        /// <param name="devicePtr">
        /// The device ptr.
        /// </param>
        /// <param name="pSourceRect">
        /// The p source rect.
        /// </param>
        /// <param name="pDestRect">
        /// The p dest rect.
        /// </param>
        /// <param name="hDestWindowOverride">
        /// The h dest window override.
        /// </param>
        /// <param name="pDirtyRegion">
        /// The p dirty region.
        /// </param>
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

        /// <summary>
        /// Gets the hook name.
        /// </summary>
        protected override string HookName
        {
            get
            {
                return "DXHookD3D9";
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The cleanup.
        /// </summary>
        public override void Cleanup()
        {
            // ClearData();
        }

        /// <summary>
        /// The hook.
        /// </summary>
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
                        new PresentParameters { BackBufferWidth = 1, BackBufferHeight = 1 }))
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
                            new PresentParameters { BackBufferWidth = 1, BackBufferHeight = 1 }, 
                            new DisplayModeEx { Width = 800, Height = 600 }))
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

        /// <summary>
        /// The dispose.
        /// </summary>
        /// <param name="disposing">
        /// The disposing.
        /// </param>
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

        /// <summary>
        /// The clear data.
        /// </summary>
        private void ClearData()
        {
            this.DebugMessage("ClearData called");

            if (this.copyThread != null)
            {
                this.killThread = true;
                this.copyEvent.Set();

                if (!this.copyThread.Join(500))
                {
                    this.copyThread.Abort();
                }

                this.copyEvent.Reset();
                this.copyThread = null;
            }

            if (this.retrieveThread != null)
            {
                this.killThread = true;
                this.copyReadySignal.Set();

                if (this.retrieveThread.Join(500))
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

            this.width = 0;
            this.height = 0;
            this.pitch = 0;
            if (this.surfaceLocked)
            {
                lock (this.surfaceLock)
                {
                    this.surface.UnlockRectangle();
                    this.surfaceLocked = false;
                }
            }

            if (this.surface != null)
            {
                this.surface.Dispose();
                this.surface = null;
            }

            if (this.renderTarget != null)
            {
                this.renderTarget.Dispose();
                this.renderTarget = null;
            }

            if (this.query != null)
            {
                this.query.Dispose();
                this.query = null;
                this.queryIssued = false;
            }

            this.hooksStarted = false;
            this.surfacesSetup = false;
        }

        /// <summary>
        /// Implementation of capturing from the render target of the Direct3D9 Device (or DeviceEx)
        /// </summary>
        /// <param name="device">
        /// </param>
        /// <param name="hook">
        /// The hook.
        /// </param>
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

                if (this.Request != null)
                {
                    try
                    {
                        this.lastRequestId = this.Request.RequestId;
                        this.HandleCaptureRequest(device);
                    }
                    finally
                    {
                        this.Request.Dispose();
                        this.Request = null;
                    }
                }
            }
            catch (Exception e)
            {
                this.DebugMessage(e.ToString());
            }
        }

        /// <summary>
        /// Hook for IDirect3DDevice9.EndScene
        /// </summary>
        /// <param name="devicePtr">
        /// Pointer to the IDirect3DDevice9 instance. Note: object member functions always pass "this" as the first parameter.
        /// </param>
        /// <returns>
        /// The HRESULT of the original EndScene
        /// </returns>
        /// <remarks>
        /// Remember that this is called many times a second by the Direct3D application - be mindful of memory and performance!
        /// </remarks>
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

        /// <summary>
        /// The handle capture request.
        /// </summary>
        /// <param name="device">
        /// The device.
        /// </param>
        private void HandleCaptureRequest(Device device)
        {
            try
            {
                bool tmp;
                if (this.queryIssued && this.query.GetData(out tmp, false))
                {
                    this.queryIssued = false;
                    var lockedRect = this.surface.LockRectangle(LockFlags.ReadOnly);
                    this.surfaceDataPointer = lockedRect.DataPointer;
                    this.surfaceLocked = true;

                    this.copyEvent.Set();
                }

                using (var backbuffer = device.GetBackBuffer(0, 0))
                {
                    device.StretchRectangle(backbuffer, this.renderTarget, TextureFilter.None);
                }

                if (this.surfaceLocked)
                {
                    lock (this.renderTargetLock)
                    {
                        if (this.surfaceLocked)
                        {
                            this.surface.UnlockRectangle();
                            this.surfaceLocked = false;
                        }
                    }
                }

                try
                {
                    var cooplevel = device.TestCooperativeLevel();
                    if (cooplevel.Code == ResultCode.Success.Code)
                    {
                        device.GetRenderTargetData(this.renderTarget, this.surface);
                        this.query.Issue(Issue.End);
                        this.queryIssued = true;
                    }
                    else
                    {
                        this.DebugMessage(string.Format("DirectX Error: TestCooperativeLevel = {0}", cooplevel.Code));
                    }
                }
                catch (Exception ex)
                {
                    this.DebugMessage(ex.ToString());
                }
            }
            catch (Exception e)
            {
                this.DebugMessage(e.ToString());
            }
        }

        /// <summary>
        /// The handle capture request thread.
        /// </summary>
        private void HandleCaptureRequestThread()
        {
            while (true)
            {
                this.copyEvent.Wait();
                this.copyEvent.Reset();

                if (this.killThread)
                    break;

                var requestId = this.lastRequestId;
                if (requestId == null || this.surfaceDataPointer == IntPtr.Zero)
                {
                    continue;
                }

                try
                {
                    lock (this.renderTargetLock)
                    {
                        if (this.surfaceDataPointer == IntPtr.Zero)
                        {
                            continue;
                        }

                        var size = this.height * this.pitch;
                        var bdata = new byte[size];
                        Marshal.Copy(this.surfaceDataPointer, bdata, 0, size);

                        // Marshal.FreeHGlobal(this.surfaceDataPointer);
                        this.retrieveParams = new RetrieveImageDataParams {
                                                      RequestId = requestId.Value, 
                                                      Data = bdata, 
                                                      Width = this.width, 
                                                      Height = this.height, 
                                                      Pitch = this.pitch
                                                  };

                        this.copyReadySignal.Set();
                    }
                }
                catch (Exception ex)
                {
                    this.DebugMessage(ex.ToString());
                }
                finally
                {
                }
            }
        }

        /// <summary>
        /// The retrieve image data thread.
        /// </summary>
        private void RetrieveImageDataThread()
        {
            while (true)
            {
                this.copyReadySignal.Wait();
                this.copyReadySignal.Reset();

                if (this.killThread)
                    break;

                if (this.retrieveParams == null)
                {
                    continue;
                }

                try
                {
                    this.ProcessCapture(this.retrieveParams.Value);
                }
                finally
                {
                    this.retrieveParams = null;
                }
            }
        }

        /// <summary>
        /// The present ex hook.
        /// </summary>
        /// <param name="devicePtr">
        /// The device ptr.
        /// </param>
        /// <param name="pSourceRect">
        /// The p source rect.
        /// </param>
        /// <param name="pDestRect">
        /// The p dest rect.
        /// </param>
        /// <param name="hDestWindowOverride">
        /// The h dest window override.
        /// </param>
        /// <param name="pDirtyRegion">
        /// The p dirty region.
        /// </param>
        /// <param name="dwFlags">
        /// The dw flags.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
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
                if (this.presentHookRecurse == 0)
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
                this.presentHookRecurse--;
            }

            return hresult;
        }

        /// <summary>
        /// The present hook.
        /// </summary>
        /// <param name="devicePtr">
        /// The device ptr.
        /// </param>
        /// <param name="pSourceRect">
        /// The p source rect.
        /// </param>
        /// <param name="pDestRect">
        /// The p dest rect.
        /// </param>
        /// <param name="hDestWindowOverride">
        /// The h dest window override.
        /// </param>
        /// <param name="pDirtyRegion">
        /// The p dirty region.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
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
                if (this.presentHookRecurse == 0)
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

        /// <summary>
        /// The reset ex hook.
        /// </summary>
        /// <param name="devicePtr">
        /// The device ptr.
        /// </param>
        /// <param name="presentparameters">
        /// The presentparameters.
        /// </param>
        /// <param name="displayModeEx">
        /// The display mode ex.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
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
        /// <param name="devicePtr">
        /// </param>
        /// <param name="presentParameters">
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
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

        /// <summary>
        /// The setup data.
        /// </summary>
        /// <param name="device">
        /// The device.
        /// </param>
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

        /// <summary>
        /// The setup surfaces.
        /// </summary>
        /// <param name="device">
        /// The device.
        /// </param>
        private void SetupSurfaces(Device device)
        {
            try
            {
                this.surface = Surface.CreateOffscreenPlain(device, this.width, this.height, this.format, Pool.SystemMemory);
                var lockedRect = this.surface.LockRectangle(LockFlags.ReadOnly);
                this.pitch = lockedRect.Pitch;
                this.surface.UnlockRectangle();
                this.renderTarget = Surface.CreateRenderTarget(device, this.width, this.height, this.format, MultisampleType.None, 0, false);
                this.query = new Query(device, QueryType.Event);

                this.killThread = false;
                this.copyThread = new Thread(this.HandleCaptureRequestThread);
                this.copyThread.Start();

                this.retrieveThread = new Thread(this.RetrieveImageDataThread);
                this.retrieveThread.Start();

                this.surfacesSetup = true;
            }
            catch (Exception ex)
            {
                this.DebugMessage(ex.ToString());
                this.ClearData();
            }
        }

        #endregion
    }
}