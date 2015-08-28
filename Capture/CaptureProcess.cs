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

    public class CaptureProcess : IDisposable
    {
        #region Fields

        /// <summary>
        /// Must be null to allow a random channel name to be generated
        /// </summary>
        private readonly string _channelName;

        private bool _disposed;

        private IpcServerChannel _screenshotServer;

        private readonly CaptureInterface _serverInterface;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Prepares capturing in the target process. Note that the process must not already be hooked, and must have a <see cref="Process.MainWindowHandle"/>.
        /// </summary>
        /// <param name="process">The process to inject into</param>
        /// <exception cref="ProcessHasNoWindowHandleException">Thrown if the <paramref name="process"/> does not have a window handle. This could mean that the process does not have a UI, or that the process has not yet finished starting.</exception>
        /// <exception cref="ProcessAlreadyHookedException">Thrown if the <paramref name="process"/> is already hooked</exception>
        /// <exception cref="InjectionFailedException">Thrown if the injection failed - see the InnerException for more details.</exception>
        /// <remarks>The target process will have its main window brought to the foreground after successful injection.</remarks>
        public CaptureProcess(Process process, CaptureConfig config, CaptureInterface captureInterface)
        {
            // If the process doesn't have a mainwindowhandle yet, skip it (we need to be able to get the hwnd to set foreground etc)
            //if (process.MainWindowHandle == IntPtr.Zero)
            //{
            //    throw new ProcessHasNoWindowHandleException();
            //}

            // Skip if the process is already hooked (and we want to hook multiple applications)
            if (HookManager.IsHooked(process.Id))
            {
                throw new ProcessAlreadyHookedException();
            }

            captureInterface.ProcessId = process.Id;
            _serverInterface = captureInterface;
            //_serverInterface = new CaptureInterface() { ProcessId = process.Id };

            // Initialise the IPC server (with our instance of _serverInterface)
            _screenshotServer = RemoteHooking.IpcCreateServer<CaptureInterface>(
                ref _channelName,
                WellKnownObjectMode.Singleton,
                _serverInterface);

            try
            {
                var dllName = typeof(CaptureInterface).Assembly.GetName().Name + ".dll";
                var location = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dllName);

                // Inject DLL into target process
                RemoteHooking.Inject(
                    process.Id,
                    InjectionOptions.NoService | InjectionOptions.DoNotRequireStrongName,
                    location,
                    //"Capture.dll", // 32-bit version (the same because AnyCPU) could use different assembly that links to 32-bit C++ helper dll
                    location,
                    //"Capture.dll", // 64-bit version (the same because AnyCPU) could use different assembly that links to 64-bit C++ helper dll
                    // the optional parameter list...
                    _channelName,
                    // The name of the IPC channel for the injected assembly to connect to
                    config
                    );
            }
            catch (Exception e)
            {
                throw new InjectionFailedException(e);
            }

            HookManager.AddHookedProcess(process.Id);

            Process = process;
        }

        ~CaptureProcess()
        {
            Dispose(false);
        }

        #endregion

        #region Public Properties

        public CaptureInterface CaptureInterface
        {
            get
            {
                return _serverInterface;
            }
        }

        public Process Process { get; private set; }

        #endregion

        #region Public Methods and Operators

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Methods

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    CaptureInterface.Disconnect();
                    HookManager.RemoveHookedProcess(Process.Id);
                }
                _disposed = true;
            }
        }

        #endregion
    }
}