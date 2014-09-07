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
            this.Dispose(false);
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
            if (this.Disconnected != null)
            {
                this.Disconnected();
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
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
            if (this.ScreenshotRequested != null)
            {
                this.ScreenshotRequested(request);
            }
        }

        #endregion

        #region Methods

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    RemotingServices.Disconnect(this);
                }
                this._disposed = true;
            }
        }

        #endregion
    }
}