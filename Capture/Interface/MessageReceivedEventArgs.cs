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
            MessageType = messageType;
            Message = message;
        }

        ~MessageReceivedEventArgs()
        {
            Dispose(false);
        }

        #endregion

        #region Public Properties

        public string Message { get; set; }

        public MessageType MessageType { get; set; }

        #endregion

        #region Public Methods and Operators

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public override string ToString()
        {
            return String.Format("{0}: {1}", MessageType, Message);
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