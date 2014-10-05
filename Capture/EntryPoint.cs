// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntryPoint.cs" company="">
//   
// </copyright>
// <summary>
//   The entry point.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Capture
{
    using System;
    using System.Collections;
    using System.Runtime.Remoting;
    using System.Runtime.Remoting.Channels;
    using System.Runtime.Remoting.Channels.Ipc;
    using System.Runtime.Serialization.Formatters;
    using System.Threading;
    using System.Threading.Tasks;

    using Capture.Hook;
    using Capture.Interface;

    using EasyHook;

    /// <summary>
    /// The entry point.
    /// </summary>
    public class EntryPoint : IEntryPoint
    {
        #region Fields

        /// <summary>
        /// The _check alive.
        /// </summary>
        private Task _checkAlive;

        /// <summary>
        /// The _client event proxy.
        /// </summary>
        private ClientCaptureInterfaceEventProxy _clientEventProxy = new ClientCaptureInterfaceEventProxy();

        /// <summary>
        /// The _client server channel.
        /// </summary>
        private IpcServerChannel _clientServerChannel = null;

        /// <summary>
        /// The _direct x hook.
        /// </summary>
        private IDXHook _directXHook;

        /// <summary>
        /// The _interface.
        /// </summary>
        private CaptureInterface _interface;

        /// <summary>
        /// The _run wait.
        /// </summary>
        private ManualResetEvent _runWait;

        /// <summary>
        /// The _stop check alive.
        /// </summary>
        private long _stopCheckAlive;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EntryPoint"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="channelName">
        /// The channel name.
        /// </param>
        /// <param name="config">
        /// The config.
        /// </param>
        public EntryPoint(
            RemoteHooking.IContext context, 
            string channelName, 
            CaptureConfig config)
        {
            // Get reference to IPC to host application
            // Note: any methods called or events triggered against _interface will execute in the host process.
            this._interface = RemoteHooking.IpcConnectClient<CaptureInterface>(channelName);

            // We try to ping immediately, if it fails then injection fails
            this._interface.Ping();

            // Attempt to create a IpcServerChannel so that any event handlers on the client will function correctly
            IDictionary properties = new Hashtable();
            properties["name"] = channelName;
            properties["portName"] = channelName + Guid.NewGuid().ToString("N");

            // random portName so no conflict with existing channels of channelName
            BinaryServerFormatterSinkProvider binaryProv = new BinaryServerFormatterSinkProvider();
            binaryProv.TypeFilterLevel = TypeFilterLevel.Full;

            IpcServerChannel _clientServerChannel = new IpcServerChannel(properties, binaryProv);
            ChannelServices.RegisterChannel(_clientServerChannel, false);
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The run.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="channelName">
        /// The channel name.
        /// </param>
        /// <param name="config">
        /// The config.
        /// </param>
        public void Run(
            RemoteHooking.IContext context, 
            string channelName, 
            CaptureConfig config)
        {
            // When not using GAC there can be issues with remoting assemblies resolving correctly
            // this is a workaround that ensures that the current assembly is correctly associated
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve +=
                (sender, args) => { return this.GetType().Assembly.FullName == args.Name ? this.GetType().Assembly : null; };

            // NOTE: This is running in the target process
            this._interface.Message(MessageType.Information, "Injected into process Id:{0}.", RemoteHooking.GetCurrentProcessId());

            this._runWait = new ManualResetEvent(false);
            this._runWait.Reset();
            try
            {
                // Initialise the Hook
                if (!this.InitialiseDirectXHook(config))
                {
                    return;
                }

                this._interface.Disconnected += this._clientEventProxy.DisconnectedProxyHandler;

                // Important Note:
                // accessing the _interface from within a _clientEventProxy event handler must always 
                // be done on a different thread otherwise it will cause a deadlock
                this._clientEventProxy.Disconnected += () =>
                    {
                        // We can now signal the exit of the Run method
                        this._runWait.Set();
                    };

                // We start a thread here to periodically check if the host is still running
                // If the host process stops then we will automatically uninstall the hooks
                this.StartCheckHostIsAliveThread();

                // Wait until signaled for exit either when a Disconnect message from the host 
                // or if the the check is alive has failed to Ping the host.
                this._runWait.WaitOne();

                // we need to tell the check host thread to exit (if it hasn't already)
                this.StopCheckHostIsAliveThread();

                // Dispose of the DXHook so any installed hooks are removed correctly
                this.DisposeDirectXHook();
            }
            catch (Exception e)
            {
                this._interface.Message(MessageType.Error, "An unexpected error occured: {0}", e.ToString());
            }
            finally
            {
                try
                {
                    this._interface.Message(MessageType.Information, "Disconnecting from process {0}", RemoteHooking.GetCurrentProcessId());
                }
                catch
                {
                }

                // Remove the client server channel (that allows client event handlers)
                ChannelServices.UnregisterChannel(this._clientServerChannel);

                // Always sleep long enough for any remaining messages to complete sending
                Thread.Sleep(100);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The dispose direct x hook.
        /// </summary>
        private void DisposeDirectXHook()
        {
            if (this._directXHook != null)
            {
                try
                {
                    this._interface.Message(MessageType.Debug, "Disposing of hooks...");
                }
                catch (RemotingException)
                {
                }

                // Ignore channel remoting errors

                // Dispose of the hooks so they are removed
                this._directXHook.Dispose();
            }
        }

        /// <summary>
        /// The initialise direct x hook.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool InitialiseDirectXHook(CaptureConfig config)
        {
            Direct3DVersion version = config.Direct3DVersion;

            bool isX64Process = RemoteHooking.IsX64Process(RemoteHooking.GetCurrentProcessId());
            this._interface.Message(MessageType.Information, "Remote process is a {0}-bit process.", isX64Process ? "64" : "32");

            try
            {
                if (version == Direct3DVersion.AutoDetect)
                {
                    // Attempt to determine the correct version based on loaded module.
                    // In most cases this will work fine, however it is perfectly ok for an application to use a D3D10 device along with D3D11 devices
                    // so the version might matched might not be the one you want to use
                    IntPtr d3D9Loaded = IntPtr.Zero;
                    IntPtr d3D10Loaded = IntPtr.Zero;
                    IntPtr d3D10_1Loaded = IntPtr.Zero;
                    IntPtr d3D11Loaded = IntPtr.Zero;
                    IntPtr d3D11_1Loaded = IntPtr.Zero;

                    int delayTime = 100;
                    int retryCount = 0;
                    while (d3D9Loaded == IntPtr.Zero && d3D10Loaded == IntPtr.Zero && d3D10_1Loaded == IntPtr.Zero && d3D11Loaded == IntPtr.Zero
                           && d3D11_1Loaded == IntPtr.Zero)
                    {
                        retryCount++;
                        d3D9Loaded = NativeMethods.GetModuleHandle("d3d9.dll");
                        d3D10Loaded = NativeMethods.GetModuleHandle("d3d10.dll");
                        d3D10_1Loaded = NativeMethods.GetModuleHandle("d3d10_1.dll");
                        d3D11Loaded = NativeMethods.GetModuleHandle("d3d11.dll");
                        d3D11_1Loaded = NativeMethods.GetModuleHandle("d3d11_1.dll");
                        Thread.Sleep(delayTime);

                        if (retryCount * delayTime > 5000)
                        {
                            this._interface.Message(MessageType.Error, "Unsupported Direct3D version, or Direct3D DLL not loaded within 5 seconds.");
                            return false;
                        }
                    }

                    version = Direct3DVersion.Unknown;
                    if (d3D11_1Loaded != IntPtr.Zero)
                    {
                        this._interface.Message(MessageType.Debug, "Autodetect found Direct3D 11.1");
                        version = Direct3DVersion.Direct3D11_1;
                    }
                    else if (d3D11Loaded != IntPtr.Zero)
                    {
                        this._interface.Message(MessageType.Debug, "Autodetect found Direct3D 11");
                        version = Direct3DVersion.Direct3D11;
                    }
                    else if (d3D10_1Loaded != IntPtr.Zero)
                    {
                        this._interface.Message(MessageType.Debug, "Autodetect found Direct3D 10.1");
                        version = Direct3DVersion.Direct3D10_1;
                    }
                    else if (d3D10Loaded != IntPtr.Zero)
                    {
                        this._interface.Message(MessageType.Debug, "Autodetect found Direct3D 10");
                        version = Direct3DVersion.Direct3D10;
                    }
                    else if (d3D9Loaded != IntPtr.Zero)
                    {
                        this._interface.Message(MessageType.Debug, "Autodetect found Direct3D 9");
                        version = Direct3DVersion.Direct3D9;
                    }
                }

                switch (version)
                {
                    case Direct3DVersion.Direct3D9:
                        this._directXHook = new DXHookD3D9(this._interface);
                        break;
                    case Direct3DVersion.Direct3D9Simple:
                        this._directXHook = new DXHookD3D9Simple(this._interface);
                        break; // case Direct3DVersion.Direct3D9Obs:
                    case Direct3DVersion.Direct3D9SharedMem:
                        this._directXHook = new DXHookD3D9SharedMem(this._interface);
                        break;

                        // case Direct3DVersion.Direct3D9Obs:
                        // _directXHook = new DXHookD3D9Obs(_interface);
                        // break;
                        // case Direct3DVersion.Direct3D10:
                        // _directXHook = new DXHookD3D10(_interface);
                        // break;
                        // case Direct3DVersion.Direct3D10_1:
                        // _directXHook = new DXHookD3D10_1(_interface);
                        // break;
                        // case Direct3DVersion.Direct3D11:
                        // _directXHook = new DXHookD3D11(_interface);
                        // break;
                        // case Direct3DVersion.Direct3D11_1:
                        // _directXHook = new DXHookD3D11_1(_interface);
                        // return;
                    default:
                        this._interface.Message(MessageType.Error, "Unsupported Direct3D version: {0}", version);
                        return false;
                }

                this._directXHook.Config = config;
                this._directXHook.Hook();

                return true;
            }
            catch (Exception e)
            {
                // Notify the host/server application about this error
                this._interface.Message(MessageType.Error, "Error in InitialiseHook: {0}", e.ToString());
                return false;
            }
        }

        /// <summary>
        /// Begin a background thread to check periodically that the host process is still accessible on its IPC channel
        /// </summary>
        private void StartCheckHostIsAliveThread()
        {
            this._checkAlive = new Task(
                () =>
                {
                    try
                    {
                        while (Interlocked.Read(ref this._stopCheckAlive) == 0)
                        {
                            Thread.Sleep(1000);

                            // .NET Remoting exceptions will throw RemotingException
                            this._interface.Ping();
                        }
                    }
                    catch
                    {
                        // We will assume that any exception means that the hooks need to be removed. 
                        // Signal the Run method so that it can exit
                        this._runWait.Set();
                    }
                });

            this._checkAlive.Start();
        }

        /// <summary>
        /// Tell the _checkAlive thread that it can exit if it hasn't already
        /// </summary>
        private void StopCheckHostIsAliveThread()
        {
            Interlocked.Increment(ref this._stopCheckAlive);
        }

        #endregion
    }
}