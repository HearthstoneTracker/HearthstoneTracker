namespace Capture.Hook
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting;
    using System.Threading;

    using Capture.Interface;

    using EasyHook;

    internal abstract class BaseDXHook : IDXHook
    {
        #region Fields

        protected readonly ClientCaptureInterfaceEventProxy InterfaceEventProxy = new ClientCaptureInterfaceEventProxy();

        protected List<LocalHook> Hooks = new List<LocalHook>();

        private CaptureConfig _config;

        private int _processId = 0;

        private ScreenshotRequest _request;

        #endregion

        #region Constructors and Destructors

        protected BaseDXHook(CaptureInterface ssInterface)
        {
            this.Interface = ssInterface;
            this.Interface.ScreenshotRequested += this.InterfaceEventProxy.ScreenshotRequestedProxyHandler;
            this.InterfaceEventProxy.ScreenshotRequested += this.InterfaceEventProxy_ScreenshotRequested;
        }

        ~BaseDXHook()
        {
            this.Dispose(false);
        }

        #endregion

        #region Public Properties

        public CaptureConfig Config
        {
            get
            {
                return this._config;
            }
            set
            {
                this._config = value;
            }
        }

        public CaptureInterface Interface { get; set; }

        public ScreenshotRequest Request
        {
            get
            {
                return this._request;
            }
            set
            {
                Interlocked.Exchange(ref this._request, value);
            }
        }

        #endregion

        #region Properties

        protected virtual string HookName
        {
            get
            {
                return "BaseDXHook";
            }
        }

        protected TimeSpan LastCaptureTime { get; set; }

        protected int ProcessId
        {
            get
            {
                if (this._processId == 0)
                {
                    this._processId = RemoteHooking.GetCurrentProcessId();
                }
                return this._processId;
            }
        }

        #endregion

        #region Public Methods and Operators

        public abstract void Cleanup();

        public void Dispose()
        {
            this.Dispose(true);
        }

        public abstract void Hook();

        #endregion

        #region Methods

        protected void DebugMessage(string message)
        {
            // TODO: enable #ifdebug again to avoid to much IPC comms
            try
            {
                this.Interface.Message(MessageType.Debug, this.HookName + ": " + message);
            }
            catch (RemotingException)
            {
                // Ignore remoting exceptions
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            // Only clean up managed objects if disposing (i.e. not called from destructor)
            if (disposing)
            {
                try
                {
                    // Uninstall Hooks
                    if (this.Hooks.Count > 0)
                    {
                        // First disable the hook (by excluding all threads) and wait long enough to ensure that all hooks are not active
                        foreach (var hook in this.Hooks)
                        {
                            // Lets ensure that no threads will be intercepted again
                            hook.ThreadACL.SetInclusiveACL(new int[] { 0 });
                        }

                        System.Threading.Thread.Sleep(100);

                        // Now we can dispose of the hooks (which triggers the removal of the hook)
                        foreach (var hook in this.Hooks)
                        {
                            hook.Dispose();
                        }

                        this.Hooks.Clear();
                    }

                    try
                    {
                        // Remove the event handlers
                        this.Interface.ScreenshotRequested -= this.InterfaceEventProxy.ScreenshotRequestedProxyHandler;
                    }
                    catch (RemotingException)
                    {
                    } // Ignore remoting exceptions (host process may have been closed)
                }
                catch
                {
                }
            }
        }

        protected IntPtr GetVTblAddress(IntPtr vTable, int i)
        {
            return Marshal.ReadIntPtr(vTable, i * IntPtr.Size); // using IntPtr.Size allows us to support both 32 and 64-bit processes
        }

        protected IntPtr[] GetVTblAddresses(IntPtr pointer, int numberOfMethods)
        {
            return this.GetVTblAddresses(pointer, 0, numberOfMethods);
        }

        protected IntPtr[] GetVTblAddresses(IntPtr pointer, int startIndex, int numberOfMethods)
        {
            List<IntPtr> vtblAddresses = new List<IntPtr>();

            IntPtr vTable = Marshal.ReadIntPtr(pointer);
            for (int i = startIndex; i < startIndex + numberOfMethods; i++)
            {
                vtblAddresses.Add(this.GetVTblAddress(vTable, i));
            }

            return vtblAddresses.ToArray();
        }

        protected virtual void InterfaceEventProxy_ScreenshotRequested(ScreenshotRequest request)
        {
            if (Request != null)
            {
                return;
            }
            this.Request = request;
        }

        protected void ProcessCapture(RetrieveImageDataParams data)
        {
            var screenshot = new Screenshot(data.RequestId, data.Data, data.Width, data.Height, data.Pitch);
            try
            {
                this.Interface.SendScreenshotResponse(screenshot);
            }
            catch (RemotingException ex)
            {
                this.TraceMessage("RemotingException: " + ex.Message);
                screenshot.Dispose();
                // Ignore remoting exceptions
                // .NET Remoting will throw an exception if the host application is unreachable
            }
            catch (Exception e)
            {
                this.DebugMessage(e.ToString());
            }
            finally
            {
            }
        }

        protected void TraceMessage(string message)
        {
#if DEBUG
            try
            {
                this.Interface.Message(MessageType.Trace, this.HookName + ": " + message);
            }
            catch (RemotingException)
            {
                // Ignore remoting exceptions
            }
#endif
        }

        #endregion
    }
}