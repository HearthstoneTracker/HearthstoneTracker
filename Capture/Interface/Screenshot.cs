// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Screenshot.cs" company="">
//   
// </copyright>
// <summary>
//   The screenshot.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Capture.Interface
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Runtime.Remoting;
    using System.Security.Permissions;

    /// <summary>
    /// The screenshot.
    /// </summary>
    [Serializable]
    public class Screenshot : MarshalByRefObject, IDisposable
    {
        #region Fields

        /// <summary>
        /// The _captured bitmap.
        /// </summary>
        private byte[] _capturedBitmap;

        /// <summary>
        /// The _disposed.
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// The _request id.
        /// </summary>
        private Guid _requestId;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Screenshot"/> class.
        /// </summary>
        /// <param name="requestId">
        /// The request id.
        /// </param>
        /// <param name="capturedBitmap">
        /// The captured bitmap.
        /// </param>
        public Screenshot(Guid requestId, byte[] capturedBitmap)
        {
            this._requestId = requestId;
            this._capturedBitmap = capturedBitmap;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Screenshot"/> class.
        /// </summary>
        /// <param name="requestId">
        /// The request id.
        /// </param>
        /// <param name="capturedBitmap">
        /// The captured bitmap.
        /// </param>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        /// <param name="pitch">
        /// The pitch.
        /// </param>
        public Screenshot(Guid requestId, byte[] capturedBitmap, int width, int height, int pitch)
        {
            this.Width = width;
            this.Height = height;
            this.Pitch = pitch;
            this._requestId = requestId;
            this._capturedBitmap = capturedBitmap;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="Screenshot"/> class. 
        /// </summary>
        ~Screenshot()
        {
            this.Dispose(false);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the captured bitmap.
        /// </summary>
        public byte[] CapturedBitmap
        {
            get
            {
                return this._capturedBitmap;
            }
        }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        public int Height { get; protected set; }

        /// <summary>
        /// Gets or sets the pitch.
        /// </summary>
        public int Pitch { get; protected set; }

        /// <summary>
        /// Gets the request id.
        /// </summary>
        public Guid RequestId
        {
            get
            {
                return this._requestId;
            }
        }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        public int Width { get; protected set; }

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

        /// <summary>
        /// The initialize lifetime service.
        /// </summary>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.Infrastructure)]
        public override object InitializeLifetimeService()
        {
            // ILease lease = (ILease)base.InitializeLifetimeService();
            // if (lease.CurrentState == LeaseState.Initial)
            // {
            // lease.InitialLeaseTime = TimeSpan.FromSeconds(10);
            // lease.SponsorshipTimeout = TimeSpan.FromSeconds(10);
            // lease.RenewOnCallTime = TimeSpan.FromSeconds(10);
            // }

            // return lease;            
            // Returning null designates an infinite non-expiring lease.
            // We must therefore ensure that RemotingServices.Disconnect() is called when
            // it's no longer needed otherwise there will be a memory leak.
            return null;

            // var lease = (ILease)base.InitializeLifetimeService();
            // if (lease.CurrentState == LeaseState.Initial)
            // {
            // lease.InitialLeaseTime = TimeSpan.FromSeconds(2);
            // lease.SponsorshipTimeout = TimeSpan.FromSeconds(5);
            // lease.RenewOnCallTime = TimeSpan.FromSeconds(2);
            // }
            // return lease;
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
                    this.Disconnect();
                }

                this._disposed = true;
            }
        }

        /// <summary>
        /// Disconnects the remoting channel(s) of this object and all nested objects.
        /// </summary>
        private void Disconnect()
        {
            RemotingServices.Disconnect(this);
        }

        #endregion
    }

    /// <summary>
    /// The bitmap extension.
    /// </summary>
    public static class BitmapExtension
    {
        #region Public Methods and Operators

        /// <summary>
        /// The to bitmap.
        /// </summary>
        /// <param name="imageBytes">
        /// The image bytes.
        /// </param>
        /// <returns>
        /// The <see cref="Bitmap"/>.
        /// </returns>
        public static Bitmap ToBitmap(this byte[] imageBytes)
        {
            using (MemoryStream ms = new MemoryStream(imageBytes))
            {
                try
                {
                    Bitmap image = (Bitmap)Image.FromStream(ms);
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