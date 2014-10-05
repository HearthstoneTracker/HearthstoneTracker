// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageReceivedEventArgs.cs" company="">
//   
// </copyright>
// <summary>
//   The message received event args.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Capture.Interface
{
    using System;
    using System.Runtime.Remoting;

    /// <summary>
    /// The message received event args.
    /// </summary>
    [Serializable]
    public class MessageReceivedEventArgs : MarshalByRefObject, IDisposable
    {
        #region Fields

        /// <summary>
        /// The _disposed.
        /// </summary>
        private bool _disposed;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageReceivedEventArgs"/> class.
        /// </summary>
        /// <param name="messageType">
        /// The message type.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        public MessageReceivedEventArgs(MessageType messageType, string message)
        {
            this.MessageType = messageType;
            this.Message = message;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="MessageReceivedEventArgs"/> class. 
        /// </summary>
        ~MessageReceivedEventArgs()
        {
            this.Dispose(false);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the message type.
        /// </summary>
        public MessageType MessageType { get; set; }

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
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0}: {1}", this.MessageType, this.Message);
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