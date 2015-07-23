namespace Capture.Interface
{
    using System;
    using System.Runtime.Remoting;

    [Serializable]
    public class MessageReceivedEventArgs : MarshalByRefObject, IDisposable
    {
        #region Fields

        private bool _disposed;

        #endregion

        #region Constructors and Destructors

        public MessageReceivedEventArgs(MessageType messageType, string message)
        {
            this.MessageType = messageType;
            this.Message = message;
        }

        ~MessageReceivedEventArgs()
        {
            this.Dispose(false);
        }

        #endregion

        #region Public Properties

        public string Message { get; set; }

        public MessageType MessageType { get; set; }

        #endregion

        #region Public Methods and Operators

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public override string ToString()
        {
            return String.Format("{0}: {1}", this.MessageType, this.Message);
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