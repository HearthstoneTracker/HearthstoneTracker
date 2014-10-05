// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScreenshotRequestedEventArgs.cs" company="">
//   
// </copyright>
// <summary>
//   The screenshot request.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Capture.Interface
{
    using System;
    using System.Drawing;
    using System.Runtime.Remoting;
    using System.Security.Permissions;

    /// <summary>
    /// The screenshot request.
    /// </summary>
    [Serializable]
    public class ScreenshotRequest : MarshalByRefObject, IDisposable
    {
        /// <summary>
        /// The _disposed.
        /// </summary>
        private bool _disposed;

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ScreenshotRequest"/> class.
        /// </summary>
        /// <param name="region">
        /// The region.
        /// </param>
        public ScreenshotRequest(Rectangle region)
            : this(Guid.NewGuid(), region)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScreenshotRequest"/> class.
        /// </summary>
        /// <param name="requestId">
        /// The request id.
        /// </param>
        /// <param name="region">
        /// The region.
        /// </param>
        public ScreenshotRequest(Guid requestId, Rectangle region)
        {
            this.RequestId = requestId;
            this.RegionToCapture = region;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="ScreenshotRequest"/> class. 
        /// </summary>
        ~ScreenshotRequest()
        {
            this.Dispose(false);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the region to capture.
        /// </summary>
        public Rectangle RegionToCapture { get; set; }

        /// <summary>
        /// Gets or sets the request id.
        /// </summary>
        public Guid RequestId { get; set; }

        #endregion

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
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.Infrastructure)]
        public override object InitializeLifetimeService()
        {
            return null;
        }

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
    }
}