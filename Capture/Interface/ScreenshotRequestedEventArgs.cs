namespace Capture.Interface
{
    using System;
    using System.Drawing;
    using System.Runtime.Remoting;
    using System.Security.Permissions;

    [Serializable]
    public class ScreenshotRequest : MarshalByRefObject, IDisposable
    {
        private bool _disposed;

        #region Constructors and Destructors

        public ScreenshotRequest(Rectangle region)
            : this(Guid.NewGuid(), region)
        {
        }

        public ScreenshotRequest(Guid requestId, Rectangle region)
        {
            this.RequestId = requestId;
            this.RegionToCapture = region;
        }

        ~ScreenshotRequest()
        {
            this.Dispose(false);
        }

        #endregion

        #region Public Properties

        public Rectangle RegionToCapture { get; set; }

        public Guid RequestId { get; set; }

        #endregion

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.Infrastructure)]
        public override object InitializeLifetimeService()
        {
            return null;
        }

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