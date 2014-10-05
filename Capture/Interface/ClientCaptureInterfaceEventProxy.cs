// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientCaptureInterfaceEventProxy.cs" company="">
//   
// </copyright>
// <summary>
//   Client event proxy for marshalling event handlers
// </summary>
// --------------------------------------------------------------------------------------------------------------------

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

        /// <summary>
        /// The _disposed.
        /// </summary>
        private bool _disposed;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Finalizes an instance of the <see cref="ClientCaptureInterfaceEventProxy"/> class. 
        /// </summary>
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

        /// <summary>
        /// The disconnected proxy handler.
        /// </summary>
        public void DisconnectedProxyHandler()
        {
            if (this.Disconnected != null)
            {
                this.Disconnected();
            }
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// The initialize lifetime service.
        /// </summary>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public override object InitializeLifetimeService()
        {
            // Returning null holds the object alive
            // until it is explicitly destroyed
            return null;
        }

        /// <summary>
        /// The screenshot requested proxy handler.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        public void ScreenshotRequestedProxyHandler(ScreenshotRequest request)
        {
            if (this.ScreenshotRequested != null)
            {
                this.ScreenshotRequested(request);
            }
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
                    RemotingServices.Disconnect(this);
                }

                this._disposed = true;
            }
        }

        #endregion
    }
}