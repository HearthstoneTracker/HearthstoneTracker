using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using System.Threading;
using Capture.Interface;
using EasyHook;

namespace Capture.Hook
{
    internal abstract class BaseDXHook : IDXHook
    {
        #region Fields

        protected readonly ClientCaptureInterfaceEventProxy InterfaceEventProxy = new ClientCaptureInterfaceEventProxy();

        protected List<LocalHook> Hooks = new List<LocalHook>();

        private int _processId;

        private ScreenshotRequest _request;

        #endregion

        #region Constructors and Destructors

        protected BaseDXHook(CaptureInterface ssInterface)
        {
            Interface = ssInterface;
            Interface.ScreenshotRequested += InterfaceEventProxy.ScreenshotRequestedProxyHandler;
            InterfaceEventProxy.ScreenshotRequested += InterfaceEventProxy_ScreenshotRequested;
        }

        ~BaseDXHook()
        {
            Dispose(false);
        }

        #endregion

        #region Public Properties

        public CaptureConfig Config { get; set; }

        public CaptureInterface Interface { get; set; }

        public ScreenshotRequest Request
        {
            get { return _request; }
            set { Interlocked.Exchange(ref _request, value); }
        }

        #endregion

        #region Properties

        protected virtual string HookName
        {
            get { return "BaseDXHook"; }
        }

        protected TimeSpan LastCaptureTime { get; set; }

        protected int ProcessId
        {
            get
            {
                if (_processId == 0)
                {
                    _processId = RemoteHooking.GetCurrentProcessId();
                }
                return _processId;
            }
        }

        #endregion

        #region Public Methods and Operators

        public abstract void Cleanup();

        public void Dispose()
        {
            Dispose(true);
        }

        public abstract void Hook();

        #endregion

        #region Methods

        protected void DebugMessage(string message)
        {
            // TODO: enable #ifdebug again to avoid to much IPC comms
            try
            {
                Interface.Message(MessageType.Debug, HookName + ": " + message);
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
                    if (Hooks.Count > 0)
                    {
                        // First disable the hook (by excluding all threads) and wait long enough to ensure that all hooks are not active
                        foreach (var hook in Hooks)
                        {
                            // Lets ensure that no threads will be intercepted again
                            hook.ThreadACL.SetInclusiveACL(new[] { 0 });
                        }

                        Thread.Sleep(100);

                        // Now we can dispose of the hooks (which triggers the removal of the hook)
                        foreach (var hook in Hooks)
                        {
                            hook.Dispose();
                        }

                        Hooks.Clear();
                    }

                    try
                    {
                        // Remove the event handlers
                        Interface.ScreenshotRequested -= InterfaceEventProxy.ScreenshotRequestedProxyHandler;
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
            return GetVTblAddresses(pointer, 0, numberOfMethods);
        }

        protected IntPtr[] GetVTblAddresses(IntPtr pointer, int startIndex, int numberOfMethods)
        {
            var vtblAddresses = new List<IntPtr>();

            var vTable = Marshal.ReadIntPtr(pointer);
            for (var i = startIndex; i < startIndex + numberOfMethods; i++)
            {
                vtblAddresses.Add(GetVTblAddress(vTable, i));
            }

            return vtblAddresses.ToArray();
        }

        protected virtual void InterfaceEventProxy_ScreenshotRequested(ScreenshotRequest request)
        {
            if (Request != null)
            {
                return;
            }
            Request = request;
        }

        protected void ProcessCapture(RetrieveImageDataParams data)
        {
            var screenshot = new Screenshot(data.RequestId, data.Data, data.Width, data.Height, data.Pitch);
            try
            {
                Interface.SendScreenshotResponse(screenshot);
            }
            catch (RemotingException ex)
            {
                TraceMessage("RemotingException: " + ex.Message);
                screenshot.Dispose();
                // Ignore remoting exceptions
                // .NET Remoting will throw an exception if the host application is unreachable
            }
            catch (Exception e)
            {
                DebugMessage(e.ToString());
            }
        }

        protected void TraceMessage(string message)
        {
#if DEBUG
            try
            {
                Interface.Message(MessageType.Trace, HookName + ": " + message);
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
