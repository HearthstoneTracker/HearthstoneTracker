using System;
using System.Drawing;
using System.IO;
using System.Runtime.Remoting;
using System.Security.Permissions;

namespace Capture.Interface
{
    [Serializable]
    public class Screenshot : MarshalByRefObject, IDisposable
    {
        #region Fields

        private readonly byte[] _capturedBitmap;

        private bool _disposed;

        private readonly Guid _requestId;

        #endregion

        #region Constructors and Destructors

        public Screenshot(Guid requestId, byte[] capturedBitmap)
        {
            _requestId = requestId;
            _capturedBitmap = capturedBitmap;
        }

        public Screenshot(Guid requestId, byte[] capturedBitmap, int width, int height, int pitch)
        {
            Width = width;
            Height = height;
            Pitch = pitch;
            _requestId = requestId;
            _capturedBitmap = capturedBitmap;
        }

        ~Screenshot()
        {
            Dispose(false);
        }

        #endregion

        #region Public Properties

        public byte[] CapturedBitmap
        {
            get { return _capturedBitmap; }
        }

        public int Height { get; protected set; }

        public int Pitch { get; protected set; }

        public Guid RequestId
        {
            get { return _requestId; }
        }

        public int Width { get; protected set; }

        #endregion

        #region Public Methods and Operators

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.Infrastructure)]
        public override object InitializeLifetimeService()
        {
            //ILease lease = (ILease)base.InitializeLifetimeService();
            //if (lease.CurrentState == LeaseState.Initial)
            //{
            //    lease.InitialLeaseTime = TimeSpan.FromSeconds(10);
            //    lease.SponsorshipTimeout = TimeSpan.FromSeconds(10);
            //    lease.RenewOnCallTime = TimeSpan.FromSeconds(10);
            //}

            //return lease;            
            //
            // Returning null designates an infinite non-expiring lease.
            // We must therefore ensure that RemotingServices.Disconnect() is called when
            // it's no longer needed otherwise there will be a memory leak.
            //
            return null;

            //var lease = (ILease)base.InitializeLifetimeService();
            //if (lease.CurrentState == LeaseState.Initial)
            //{
            //    lease.InitialLeaseTime = TimeSpan.FromSeconds(2);
            //    lease.SponsorshipTimeout = TimeSpan.FromSeconds(5);
            //    lease.RenewOnCallTime = TimeSpan.FromSeconds(2);
            //}
            //return lease;
        }

        #endregion

        #region Methods

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Disconnect();
                }
                _disposed = true;
            }
        }

        /// <summary>
        ///     Disconnects the remoting channel(s) of this object and all nested objects.
        /// </summary>
        private void Disconnect()
        {
            RemotingServices.Disconnect(this);
        }

        #endregion
    }

    public static class BitmapExtension
    {
        #region Public Methods and Operators

        public static Bitmap ToBitmap(this byte[] imageBytes)
        {
            using (var ms = new MemoryStream(imageBytes))
            {
                try
                {
                    var image = (Bitmap)Image.FromStream(ms);
                    return image;
                }
                catch
                {
                    return null;
                }
            }
        }

        #endregion
    }
}
