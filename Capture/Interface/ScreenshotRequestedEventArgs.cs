using System;
using System.Drawing;
using System.Runtime.Remoting;
using System.Security.Permissions;

namespace Capture.Interface
{
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
            RequestId = requestId;
            RegionToCapture = region;
        }

        ~ScreenshotRequest()
        {
            Dispose(false);
        }

        #endregion

        #region Public Properties

        public Rectangle RegionToCapture { get; set; }

        public Guid RequestId { get; set; }

        #endregion

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.Infrastructure)]
        public override object InitializeLifetimeService()
        {
            return null;
        }

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
    }
}
