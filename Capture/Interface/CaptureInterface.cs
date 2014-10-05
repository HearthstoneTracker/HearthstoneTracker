// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CaptureInterface.cs" company="">
//   
// </copyright>
// <summary>
//   The message received event.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Capture.Interface
{
    using System;
    using System.Drawing;
    using System.Threading;

    /// <summary>
    /// The message received event.
    /// </summary>
    /// <param name="message">
    /// The message.
    /// </param>
    [Serializable]
    public delegate void MessageReceivedEvent(MessageReceivedEventArgs message);

    /// <summary>
    /// The disconnected event.
    /// </summary>
    [Serializable]
    public delegate void DisconnectedEvent();

    /// <summary>
    /// The screenshot requested event.
    /// </summary>
    /// <param name="request">
    /// The request.
    /// </param>
    [Serializable]
    public delegate void ScreenshotRequestedEvent(ScreenshotRequest request);

    /// <summary>
    /// The capture interface.
    /// </summary>
    [Serializable]
    public class CaptureInterface : MarshalByRefObject
    {
        #region Fields

        /// <summary>
        /// The _complete screenshot.
        /// </summary>
        private Action<Screenshot> _completeScreenshot;

        /// <summary>
        /// The _disposed.
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// The _lock.
        /// </summary>
        private object _lock = new object();

        /// <summary>
        /// The _request id.
        /// </summary>
        private Guid? _requestId;

        /// <summary>
        /// The _wait.
        /// </summary>
        private ManualResetEvent _wait = new ManualResetEvent(false);

        #endregion

        #region Public Events

        /// <summary>
        /// Client event used to notify the hook to exit
        /// </summary>
        public event DisconnectedEvent Disconnected;

        /// <summary>
        /// Server event for sending debug and error information from the client to server
        /// </summary>
        public event MessageReceivedEvent RemoteMessage;

        /// <summary>
        /// Client event used to communicate to the client that it is time to create a screenshot
        /// </summary>
        public event ScreenshotRequestedEvent ScreenshotRequested;

        #endregion

        #region Public Properties

        /// <summary>
        /// The client process Id
        /// </summary>
        public int ProcessId { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The begin get screenshot.
        /// </summary>
        /// <param name="region">
        /// The region.
        /// </param>
        /// <param name="timeout">
        /// The timeout.
        /// </param>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <returns>
        /// The <see cref="IAsyncResult"/>.
        /// </returns>
        public IAsyncResult BeginGetScreenshot(Rectangle region, TimeSpan timeout, AsyncCallback callback = null)
        {
            Func<Rectangle, TimeSpan, Screenshot> getScreenshot = this.GetScreenshot;

            return getScreenshot.BeginInvoke(region, timeout, callback, getScreenshot);
        }

        /// <summary>
        /// Tell the client process to disconnect
        /// </summary>
        public void Disconnect()
        {
            this.SafeInvokeDisconnected();
        }

        /// <summary>
        /// The end get screenshot.
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <returns>
        /// The <see cref="Screenshot"/>.
        /// </returns>
        public Screenshot EndGetScreenshot(IAsyncResult result)
        {
            Func<Rectangle, TimeSpan, Screenshot> getScreenshot = result.AsyncState as Func<Rectangle, TimeSpan, Screenshot>;
            if (getScreenshot != null)
            {
                return getScreenshot.EndInvoke(result);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Get a fullscreen screenshot with the default timeout of 2 seconds
        /// </summary>
        /// <returns>
        /// The <see cref="Screenshot"/>.
        /// </returns>
        public Screenshot GetScreenshot()
        {
            return this.GetScreenshot(Rectangle.Empty, new TimeSpan(0, 0, 2));
        }

        /// <summary>
        /// Get a screenshot of the specified region
        /// </summary>
        /// <param name="region">
        /// the region to capture (x=0,y=0 is top left corner)
        /// </param>
        /// <param name="timeout">
        /// maximum time to wait for the screenshot
        /// </param>
        /// <returns>
        /// The <see cref="Screenshot"/>.
        /// </returns>
        public Screenshot GetScreenshot(Rectangle region, TimeSpan timeout)
        {
            lock (this._lock)
            {
                Screenshot result = null;
                this._requestId = Guid.NewGuid();
                this._wait.Reset();

                this.SafeInvokeScreenshotRequested(new ScreenshotRequest(this._requestId.Value, region));

                this._completeScreenshot = sc =>
                    {
                        try
                        {
                            Interlocked.Exchange(ref result, sc);
                        }
                        catch
                        {
                        }

                        this._wait.Set();
                    };

                this._wait.WaitOne(timeout);
                this._completeScreenshot = null;
                return result;
            }
        }

        /// <summary>
        /// Send a message to all handlers of <see cref="CaptureInterface.RemoteMessage"/>.
        /// </summary>
        /// <param name="messageType">
        /// </param>
        /// <param name="format">
        /// </param>
        /// <param name="args">
        /// </param>
        public void Message(MessageType messageType, string format, params object[] args)
        {
            this.Message(messageType, string.Format(format, args));
        }

        /// <summary>
        /// The message.
        /// </summary>
        /// <param name="messageType">
        /// The message type.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Message(MessageType messageType, string message)
        {
            this.SafeInvokeMessageRecevied(new MessageReceivedEventArgs(messageType, message));
        }

        /// <summary>
        /// The ping.
        /// </summary>
        public void Ping()
        {
        }

        /// <summary>
        /// The send screenshot response.
        /// </summary>
        /// <param name="screenshot">
        /// The screenshot.
        /// </param>
        public void SendScreenshotResponse(Screenshot screenshot)
        {
            if (this._requestId != null && screenshot != null && screenshot.RequestId == this._requestId.Value)
            {
                if (this._completeScreenshot != null)
                {
                    this._completeScreenshot(screenshot);
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The safe invoke disconnected.
        /// </summary>
        private void SafeInvokeDisconnected()
        {
            if (this.Disconnected == null)
            {
                return; // No Listeners
            }

            DisconnectedEvent listener = null;
            Delegate[] dels = this.Disconnected.GetInvocationList();

            foreach (Delegate del in dels)
            {
                try
                {
                    listener = (DisconnectedEvent)del;
                    listener.Invoke();
                }
                catch (Exception)
                {
                    // Could not reach the destination, so remove it
                    // from the list
                    this.Disconnected -= listener;
                }
            }
        }

        /// <summary>
        /// The safe invoke message recevied.
        /// </summary>
        /// <param name="eventArgs">
        /// The event args.
        /// </param>
        private void SafeInvokeMessageRecevied(MessageReceivedEventArgs eventArgs)
        {
            if (this.RemoteMessage == null)
            {
                return; // No Listeners
            }

            MessageReceivedEvent listener = null;
            Delegate[] dels = this.RemoteMessage.GetInvocationList();

            foreach (Delegate del in dels)
            {
                try
                {
                    listener = (MessageReceivedEvent)del;
                    listener.Invoke(eventArgs);
                }
                catch (Exception)
                {
                    // Could not reach the destination, so remove it
                    // from the list
                    this.RemoteMessage -= listener;
                }
            }
        }

        /// <summary>
        /// The safe invoke screenshot requested.
        /// </summary>
        /// <param name="eventArgs">
        /// The event args.
        /// </param>
        private void SafeInvokeScreenshotRequested(ScreenshotRequest eventArgs)
        {
            if (this.ScreenshotRequested == null)
            {
                return; // No Listeners
            }

            ScreenshotRequestedEvent listener = null;
            Delegate[] dels = this.ScreenshotRequested.GetInvocationList();

            foreach (Delegate del in dels)
            {
                try
                {
                    listener = (ScreenshotRequestedEvent)del;
                    listener.Invoke(eventArgs);
                }
                catch (Exception)
                {
                    // Could not reach the destination, so remove it
                    // from the list
                    this.ScreenshotRequested -= listener;
                }
            }
        }

        #endregion
    }
}