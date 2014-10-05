// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CaptureProcess.cs" company="">
//   
// </copyright>
// <summary>
//   The capture process.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Capture
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.Remoting;
    using System.Runtime.Remoting.Channels.Ipc;

    using Capture.Hook;
    using Capture.Interface;

    using EasyHook;

    /// <summary>
    /// The capture process.
    /// </summary>
    public class CaptureProcess : IDisposable
    {
        #region Fields

        /// <summary>
        /// Must be null to allow a random channel name to be generated
        /// </summary>
        private string _channelName;

        /// <summary>
        /// The _disposed.
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// The _screenshot server.
        /// </summary>
        private IpcServerChannel _screenshotServer;

        /// <summary>
        /// The _server interface.
        /// </summary>
        private CaptureInterface _serverInterface;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CaptureProcess"/> class. 
        /// Prepares capturing in the target process. Note that the process must not already be hooked, and must have a <see cref="Process.MainWindowHandle"/>.
        /// </summary>
        /// <param name="process">
        /// The process to inject into
        /// </param>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <param name="captureInterface">
        /// The capture Interface.
        /// </param>
        /// <exception cref="ProcessHasNoWindowHandleException">
        /// Thrown if the <paramref name="process"/> does not have a window handle. This could mean that the process does not have a UI, or that the process has not yet finished starting.
        /// </exception>
        /// <exception cref="ProcessAlreadyHookedException">
        /// Thrown if the <paramref name="process"/> is already hooked
        /// </exception>
        /// <exception cref="InjectionFailedException">
        /// Thrown if the injection failed - see the InnerException for more details.
        /// </exception>
        /// <remarks>
        /// The target process will have its main window brought to the foreground after successful injection.
        /// </remarks>
        public CaptureProcess(Process process, CaptureConfig config, CaptureInterface captureInterface)
        {
            // If the process doesn't have a mainwindowhandle yet, skip it (we need to be able to get the hwnd to set foreground etc)
            // if (process.MainWindowHandle == IntPtr.Zero)
            // {
            // throw new ProcessHasNoWindowHandleException();
            // }

            // Skip if the process is already hooked (and we want to hook multiple applications)
            if (HookManager.IsHooked(process.Id))
            {
                throw new ProcessAlreadyHookedException();
            }

            captureInterface.ProcessId = process.Id;
            this._serverInterface = captureInterface;

            // _serverInterface = new CaptureInterface() { ProcessId = process.Id };

            // Initialise the IPC server (with our instance of _serverInterface)
            this._screenshotServer = RemoteHooking.IpcCreateServer<CaptureInterface>(
                ref this._channelName, 
                WellKnownObjectMode.Singleton, 
                this._serverInterface);

            try
            {
                var dllName = typeof(CaptureInterface).Assembly.GetName().Name + ".dll";
                var location = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dllName);

                // Inject DLL into target process
                RemoteHooking.Inject(
                    process.Id, 
                    InjectionOptions.NoService | InjectionOptions.DoNotRequireStrongName, 
                    location, 
                    // "Capture.dll", // 32-bit version (the same because AnyCPU) could use different assembly that links to 32-bit C++ helper dll
                    location, 
                    // "Capture.dll", // 64-bit version (the same because AnyCPU) could use different assembly that links to 64-bit C++ helper dll
                    // the optional parameter list...
                    this._channelName, 
                    // The name of the IPC channel for the injected assembly to connect to
                    config);
            }
            catch (Exception e)
            {
                throw new InjectionFailedException(e);
            }

            HookManager.AddHookedProcess(process.Id);

            this.Process = process;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="CaptureProcess"/> class. 
        /// </summary>
        ~CaptureProcess()
        {
            this.Dispose(false);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the capture interface.
        /// </summary>
        public CaptureInterface CaptureInterface
        {
            get
            {
                return this._serverInterface;
            }
        }

        /// <summary>
        /// Gets the process.
        /// </summary>
        public Process Process { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The dispose.
        /// </summary>
        /// <param name="disposing">
        /// The disposing.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    this.CaptureInterface.Disconnect();
                    HookManager.RemoveHookedProcess(this.Process.Id);
                }

                this._disposed = true;
            }
        }

        #endregion
    }
}