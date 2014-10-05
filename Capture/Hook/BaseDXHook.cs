// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseDXHook.cs" company="">
//   
// </copyright>
// <summary>
//   The base dx hook.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Capture.Hook
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting;
    using System.Threading;

    using Capture.Interface;

    using EasyHook;

    /// <summary>
    /// The base dx hook.
    /// </summary>
    internal abstract class BaseDXHook : IDXHook
    {
        #region Fields

        /// <summary>
        /// The interface event proxy.
        /// </summary>
        protected readonly ClientCaptureInterfaceEventProxy InterfaceEventProxy = new ClientCaptureInterfaceEventProxy();

        /// <summary>
        /// The hooks.
        /// </summary>
        protected List<LocalHook> Hooks = new List<LocalHook>();

        /// <summary>
        /// The _config.
        /// </summary>
        private CaptureConfig _config;

        /// <summary>
        /// The _process id.
        /// </summary>
        private int _processId;

        /// <summary>
        /// The _request.
        /// </summary>
        private ScreenshotRequest _request;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseDXHook"/> class.
        /// </summary>
        /// <param name="ssInterface">
        /// The ss interface.
        /// </param>
        protected BaseDXHook(CaptureInterface ssInterface)
        {
            this.Interface = ssInterface;
            this.Interface.ScreenshotRequested += this.InterfaceEventProxy.ScreenshotRequestedProxyHandler;
            this.InterfaceEventProxy.ScreenshotRequested += this.InterfaceEventProxy_ScreenshotRequested;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="BaseDXHook"/> class. 
        /// </summary>
        ~BaseDXHook()
        {
            this.Dispose(false);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the config.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the interface.
        /// </summary>
        public CaptureInterface Interface { get; set; }

        /// <summary>
        /// Gets or sets the request.
        /// </summary>
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

        /// <summary>
        /// Gets the hook name.
        /// </summary>
        protected virtual string HookName
        {
            get
            {
                return "BaseDXHook";
            }
        }

        /// <summary>
        /// Gets or sets the last capture time.
        /// </summary>
        protected TimeSpan LastCaptureTime { get; set; }

        /// <summary>
        /// Gets the process id.
        /// </summary>
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

        /// <summary>
        /// The cleanup.
        /// </summary>
        public abstract void Cleanup();

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// The hook.
        /// </summary>
        public abstract void Hook();

        #endregion

        #region Methods

        /// <summary>
        /// The debug message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
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

        /// <summary>
        /// The dispose.
        /// </summary>
        /// <param name="disposing">
        /// The disposing.
        /// </param>
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
                            hook.ThreadACL.SetInclusiveACL(new[] { 0 });
                        }

                        Thread.Sleep(100);

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
                    }

                    // Ignore remoting exceptions (host process may have been closed)
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// The get v tbl address.
        /// </summary>
        /// <param name="vTable">
        /// The v table.
        /// </param>
        /// <param name="i">
        /// The i.
        /// </param>
        /// <returns>
        /// The <see cref="IntPtr"/>.
        /// </returns>
        protected IntPtr GetVTblAddress(IntPtr vTable, int i)
        {
            return Marshal.ReadIntPtr(vTable, i * IntPtr.Size); // using IntPtr.Size allows us to support both 32 and 64-bit processes
        }

        /// <summary>
        /// The get v tbl addresses.
        /// </summary>
        /// <param name="pointer">
        /// The pointer.
        /// </param>
        /// <param name="numberOfMethods">
        /// The number of methods.
        /// </param>
        /// <returns>
        /// The <see cref="IntPtr[]"/>.
        /// </returns>
        protected IntPtr[] GetVTblAddresses(IntPtr pointer, int numberOfMethods)
        {
            return this.GetVTblAddresses(pointer, 0, numberOfMethods);
        }

        /// <summary>
        /// The get v tbl addresses.
        /// </summary>
        /// <param name="pointer">
        /// The pointer.
        /// </param>
        /// <param name="startIndex">
        /// The start index.
        /// </param>
        /// <param name="numberOfMethods">
        /// The number of methods.
        /// </param>
        /// <returns>
        /// The <see cref="IntPtr[]"/>.
        /// </returns>
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

        /// <summary>
        /// The interface event proxy_ screenshot requested.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        protected virtual void InterfaceEventProxy_ScreenshotRequested(ScreenshotRequest request)
        {
            if (this.Request != null)
            {
                return;
            }

            this.Request = request;
        }

        /// <summary>
        /// The process capture.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
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

        /// <summary>
        /// The trace message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
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