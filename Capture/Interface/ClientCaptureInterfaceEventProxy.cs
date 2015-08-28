namespace Capture.Interface
{
    using System;
    using System.Runtime.Remoting;

    /// <summary>
    /// Client event proxy for marshalling event handlers
    /// </summary>
    public class ClientCaptureInterfaceEventProxy : MarshalByRefObject, IDisposable
    {
        #region Fields

        private bool _disposed;

        #endregion

        #region Constructors and Destructors

        ~ClientCaptureInterfaceEventProxy()
        {
            Dispose(false);
        }

        #endregion

        #region Public Events

        /// <summary>
        /// Client event used to notify the hook to exit
        /// </summary>
        public event DisconnectedEvent Disconnected;

        /// <summary>
        /// Client event used to communicate to the client that it is time to create a screenshot
        /// </summary>
        public event ScreenshotRequestedEvent ScreenshotRequested;

        #endregion

        #region Public Methods and Operators

        public void DisconnectedProxyHandler()
        {
            if (Disconnected != null)
            {
                Disconnected();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public override object InitializeLifetimeService()
        {
            //Returning null holds the object alive
            //until it is explicitly destroyed
            return null;
        }

        public void ScreenshotRequestedProxyHandler(ScreenshotRequest request)
        {
            if (ScreenshotRequested != null)
            {
                ScreenshotRequested(request);
            }
        }

        #endregion

        #region Methods

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    RemotingServices.Disconnect(this);
                }
                _disposed = true;
            }
        }

        #endregion
    }
}