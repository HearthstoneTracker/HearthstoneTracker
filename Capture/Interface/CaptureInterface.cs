namespace Capture.Interface
{
    using System;
    using System.Drawing;
    using System.Threading;

    [Serializable]
    public delegate void MessageReceivedEvent(MessageReceivedEventArgs message);

    [Serializable]
    public delegate void DisconnectedEvent();

    [Serializable]
    public delegate void ScreenshotRequestedEvent(ScreenshotRequest request);

    [Serializable]
    public class CaptureInterface : MarshalByRefObject
    {
        #region Fields

        private Action<Screenshot> _completeScreenshot = null;

        private bool _disposed;

        private object _lock = new object();

        private Guid? _requestId = null;

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
        public Screenshot GetScreenshot()
        {
            return this.GetScreenshot(Rectangle.Empty, new TimeSpan(0, 0, 2));
        }

        /// <summary>
        /// Get a screenshot of the specified region
        /// </summary>
        /// <param name="region">the region to capture (x=0,y=0 is top left corner)</param>
        /// <param name="timeout">maximum time to wait for the screenshot</param>
        public Screenshot GetScreenshot(Rectangle region, TimeSpan timeout)
        {
            lock (this._lock)
            {
                Screenshot result = null;
                this._requestId = Guid.NewGuid();
                this._wait.Reset();

                this.SafeInvokeScreenshotRequested(new ScreenshotRequest(this._requestId.Value, region));

                this._completeScreenshot = (sc) =>
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
        /// <param name="messageType"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void Message(MessageType messageType, string format, params object[] args)
        {
            this.Message(messageType, String.Format(format, args));
        }

        public void Message(MessageType messageType, string message)
        {
            this.SafeInvokeMessageRecevied(new MessageReceivedEventArgs(messageType, message));
        }

        public void Ping()
        {
        }

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

        private void SafeInvokeDisconnected()
        {
            if (this.Disconnected == null)
            {
                return; //No Listeners
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
                    //Could not reach the destination, so remove it
                    //from the list
                    this.Disconnected -= listener;
                }
            }
        }

        private void SafeInvokeMessageRecevied(MessageReceivedEventArgs eventArgs)
        {
            if (this.RemoteMessage == null)
            {
                return; //No Listeners
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
                    //Could not reach the destination, so remove it
                    //from the list
                    this.RemoteMessage -= listener;
                }
            }
        }

        private void SafeInvokeScreenshotRequested(ScreenshotRequest eventArgs)
        {
            if (this.ScreenshotRequested == null)
            {
                return; //No Listeners
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
                    //Could not reach the destination, so remove it
                    //from the list
                    this.ScreenshotRequested -= listener;
                }
            }
        }

        #endregion
    }
}