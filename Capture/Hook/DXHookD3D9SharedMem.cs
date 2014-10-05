// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DXHookD3D9SharedMem.cs" company="">
//   
// </copyright>
// <summary>
//   The dx hook d 3 d 9 shared mem.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

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

    /// <summary>
    /// The dx hook d 3 d 9 shared mem.
    /// </summary>
    internal unsafe class DXHookD3D9SharedMem : BaseDXHook
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
        /// The copy data.
        /// </summary>
        private CopyData copyData;

        /// <summary>
        /// The copy data size.
        /// </summary>
        private readonly int copyDataSize = Marshal.SizeOf(typeof(CopyData));

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
        /// The copy data mem.
        /// </summary>
        private MemoryMappedFile copyDataMem;

        /// <summary>
        /// The copy data mem access.
        /// </summary>
        private MemoryMappedViewAccessor copyDataMemAccess;

        /// <summary>
        /// The copy data mem ptr.
        /// </summary>
        private byte* copyDataMemPtr = (byte*)0;

        /// <summary>
        /// The copy ready signal.
        /// </summary>
        private readonly ManualResetEventSlim copyReadySignal = new ManualResetEventSlim(false);

        /// <summary>
        /// The surface locks.
        /// </summary>
        private readonly object[] surfaceLocks = new object[BUFFERS];

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
        /// The current device.
        /// </summary>
        private IntPtr currentDevice;

        /// <summary>
        /// The hooks started.
        /// </summary>
        private bool hooksStarted;

        /// <summary>
        /// The id 3 d device function addresses.
        /// </summary>
        private List<IntPtr> id3dDeviceFunctionAddresses = new List<IntPtr>();

        /// <summary>
        /// The kill thread.
        /// </summary>
        private bool killThread;

        /// <summary>
        /// The queries.
        /// </summary>
        private Query[] queries = new Query[BUFFERS];

        /// <summary>
        /// The issued queries.
        /// </summary>
        private bool[] issuedQueries = new bool[BUFFERS];

        /// <summary>
        /// The copy surfaces.
        /// </summary>
        private Surface[] copySurfaces = new Surface[BUFFERS];

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
        /// The surfaces.
        /// </summary>
        private Surface[] surfaces = new Surface[BUFFERS];

        /// <summary>
        /// The surface data pointer.
        /// </summary>
        private IntPtr surfaceDataPointer;

        /// <summary>
        /// The surface locked.
        /// </summary>
        private bool[] surfaceLocked = new bool[BUFFERS];

        /// <summary>
        /// The shared mem mutexes.
        /// </summary>
        private Mutex[] sharedMemMutexes;

        /// <summary>
        /// The surfaces setup.
        /// </summary>
        private bool surfacesSetup;

        /// <summary>
        /// The present hook recurse.
        /// </summary>
        private int presentHookRecurse;

        /// <summary>
        /// The cur capture.
        /// </summary>
        private int curCapture;

        /// <summary>
        /// The copy wait.
        /// </summary>
        private int copyWait;

        /// <summary>
        /// The current surface.
        /// </summary>
        private int currentSurface;

        /// <summary>
        /// The buffers.
        /// </summary>
        private const int BUFFERS = 3;

        /// <summary>
        /// The capture wait handle.
        /// </summary>
        private EventWaitHandle captureWaitHandle;

        /// <summary>
        /// The hook ready wait handle.
        /// </summary>
        private EventWaitHandle hookReadyWaitHandle;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DXHookD3D9SharedMem"/> class.
        /// </summary>
        /// <param name="ssInterface">
        /// The ss interface.
        /// </param>
        public DXHookD3D9SharedMem(CaptureInterface ssInterface)
            : base(ssInterface)
        {
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
            this.captureWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset, "Global\\DXHookD3D9Capture", out created, ewsecurity);
            this.hookReadyWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset, "Global\\DXHookD3D9CaptureReady", out created, ewsecurity);
        }

        #endregion

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
            this.hookReadyWaitHandle.Reset();

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

                // if (Monitor.TryEnter(this.surfaceLocks[i]))
                // {
                // Monitor.Exit(this.surfaceLocks[i]);
                // this.surfaceLocks[i] = null;
                // }
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

            if (this.copyDataMemAccess != null)
            {
                this.copyDataMemAccess.Write(0, ref this.copyData);
                this.copyDataMemAccess.Flush();

                this.copyDataMemAccess.SafeMemoryMappedViewHandle.ReleasePointer();
                this.copyDataMemPtr = (byte*)0;
                this.copyDataMemAccess.Dispose();
                this.copyDataMemAccess = null;

                // copyDataMemPtr = (byte*)0;
            }

            if (this.copyDataMem != null)
            {
                this.copyDataMem.Dispose();
                this.copyDataMem = null;
            }

            this.captureWaitHandle.Reset();
            this.hooksStarted = false;
            this.surfacesSetup = false;
            this.copyWait = 0;
            this.curCapture = 0;
            this.currentSurface = 0;
            this.surfaceDataPointer = IntPtr.Zero;
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
                        this.copyData.format = (int)backbuffer.Description.Format;
                        this.copyData.width = backbuffer.Description.Width;
                        this.copyData.height = backbuffer.Description.Height;
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
                if (this.killThread)
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

                if (this.captureWaitHandle.WaitOne(0) || this.copyWait < BUFFERS - 1)
                {
                    int nextCapture = (this.curCapture == BUFFERS - 1) ? 0 : (this.curCapture + 1);

                    try
                    {
                        using (var backbuffer = device.GetBackBuffer(0, 0))
                        {
                            var sourceTexture = this.copySurfaces[this.curCapture];
                            device.StretchRectangle(backbuffer, sourceTexture, TextureFilter.None);
                        }

                        if (this.copyWait < BUFFERS - 1)
                        {
                            this.copyWait++;
                        }
                        else
                        {
                            var prevSourceTexture = this.copySurfaces[nextCapture];
                            var targetTexture = this.surfaces[nextCapture];

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
                                this.DebugMessage(ex.ToString());
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
                        this.captureWaitHandle.Reset();
                        this.curCapture = nextCapture;                        
                    }
                }

            }
            catch (Exception e)
            {
                this.DebugMessage(e.ToString());
            }
        }

        /// <summary>
        /// The interface event proxy_ screenshot requested.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        protected override void InterfaceEventProxy_ScreenshotRequested(ScreenshotRequest request)
        {
            request.Dispose();
        }

        /// <summary>
        /// The retrieve image data thread.
        /// </summary>
        private unsafe void RetrieveImageDataThread()
        {
            int sharedMemId = 0;
            while (true)
            {
                this.copyReadySignal.Wait();
                this.copyReadySignal.Reset();

                if (this.killThread)
                    break;

                if (this.surfaceDataPointer == IntPtr.Zero)
                {
                    continue;
                }

                int nextSharedMemId = sharedMemId == 0 ? 1 : 0;
                try
                {
                    lock (this.surfaceLocks[this.currentSurface])
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
                        catch (AbandonedMutexException ex)
                        {
                            this.sharedMemMutexes[lastKnown].ReleaseMutex();
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
                            memcpy(new IntPtr(this.sharedTexturesPtr[lastRendered]), src, new UIntPtr((uint)size));

                            this.copyData.lastRendered = lastRendered;
                            
                            Marshal.StructureToPtr(this.copyData, new IntPtr(this.copyDataMemPtr), false);
                            this.sharedMemMutexes[lastRendered].ReleaseMutex();
                            sharedMemId = nextSharedMemId;
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.DebugMessage(ex.ToString());
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
                this.copyData.width = pp.BackBufferWidth;
                this.copyData.height = pp.BackBufferHeight;
                this.copyData.format = (int)pp.BackBufferFormat;

                this.DebugMessage(string.Format("D3D9 Setup: w: {0} h: {1} f: {2}", this.copyData.width, this.copyData.height, pp.BackBufferFormat));
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
                this.copyDataMemAccess.SafeMemoryMappedViewHandle.AcquirePointer(ref this.copyDataMemPtr);

                this.copyData.textureId = Guid.NewGuid();

                for (int i = 0; i < 2; i++)
                {
                    bool locked = false;
                    try
                    {
                        try
                        {
                            locked = this.sharedMemMutexes[i].WaitOne(0);
                        }
                        catch (AbandonedMutexException ex)
                        {
                            locked = true;
                        }

                        if (!locked)
                        {
                            this.DebugMessage("shared mem mutex still locked");
                        }

                        this.sharedTextures[i] = MemoryMappedFile.CreateOrOpen(
                            this.copyData.textureId.ToString() + i, 
                            this.copyData.pitch * this.copyData.height, 
                            MemoryMappedFileAccess.ReadWrite);
                        this.sharedTexturesAccess[i] = this.sharedTextures[i].CreateViewAccessor(0, this.copyData.pitch * this.copyData.height, MemoryMappedFileAccess.ReadWrite);
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

                this.killThread = false;
                this.surfacesSetup = true;

                Marshal.StructureToPtr(this.copyData, new IntPtr(this.copyDataMemPtr), false);

                this.retrieveThread = new Thread(this.RetrieveImageDataThread);
                this.retrieveThread.Start();
                this.hookReadyWaitHandle.Set();
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