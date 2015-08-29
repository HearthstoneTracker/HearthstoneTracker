using System;
using System.Drawing;
using System.Threading;

namespace Capture.Interface
{
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

        private Action<Screenshot> _completeScreenshot;

        private readonly object _lock = new object();

        private Guid? _requestId;

        private readonly ManualResetEvent _wait = new ManualResetEvent(false);

        #endregion

        #region Public Events

        /// <summary>
        ///     Client event used to notify the hook to exit
        /// </summary>
        public event DisconnectedEvent Disconnected;

        /// <summary>
        ///     Server event for sending debug and error information from the client to server
        /// </summary>
        public event MessageReceivedEvent RemoteMessage;

        /// <summary>
        ///     Client event used to communicate to the client that it is time to create a screenshot
        /// </summary>
        public event ScreenshotRequestedEvent ScreenshotRequested;

        #endregion

        #region Public Properties

        /// <summary>
        ///     The client process Id
        /// </summary>
        public int ProcessId { get; set; }

        #endregion

        #region Public Methods and Operators

        public IAsyncResult BeginGetScreenshot(Rectangle region, TimeSpan timeout, AsyncCallback callback = null)
        {
            Func<Rectangle, TimeSpan, Screenshot> getScreenshot = GetScreenshot;

            return getScreenshot.BeginInvoke(region, timeout, callback, getScreenshot);
        }

        /// <summary>
        ///     Tell the client process to disconnect
        /// </summary>
        public void Disconnect()
        {
            SafeInvokeDisconnected();
        }

        public Screenshot EndGetScreenshot(IAsyncResult result)
        {
            var getScreenshot = result.AsyncState as Func<Rectangle, TimeSpan, Screenshot>;
            if (getScreenshot != null)
            {
                return getScreenshot.EndInvoke(result);
            }
            return null;
        }

        /// <summary>
        ///     Get a fullscreen screenshot with the default timeout of 2 seconds
        /// </summary>
        public Screenshot GetScreenshot()
        {
            return GetScreenshot(Rectangle.Empty, new TimeSpan(0, 0, 2));
        }

        /// <summary>
        ///     Get a screenshot of the specified region
        /// </summary>
        /// <param name="region">the region to capture (x=0,y=0 is top left corner)</param>
        /// <param name="timeout">maximum time to wait for the screenshot</param>
        public Screenshot GetScreenshot(Rectangle region, TimeSpan timeout)
        {
            lock (_lock)
            {
                Screenshot result = null;
                _requestId = Guid.NewGuid();
                _wait.Reset();

                SafeInvokeScreenshotRequested(new ScreenshotRequest(_requestId.Value, region));

                _completeScreenshot = sc =>
                    {
                        try
                        {
                            Interlocked.Exchange(ref result, sc);
                        }
                        catch
                        {
                        }
                        _wait.Set();
                    };

                _wait.WaitOne(timeout);
                _completeScreenshot = null;
                return result;
            }
        }

        /// <summary>
        ///     Send a message to all handlers of <see cref="CaptureInterface.RemoteMessage" />.
        /// </summary>
        /// <param name="messageType"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void Message(MessageType messageType, string format, params object[] args)
        {
            Message(messageType, String.Format(format, args));
        }

        public void Message(MessageType messageType, string message)
        {
            SafeInvokeMessageRecevied(new MessageReceivedEventArgs(messageType, message));
        }

        public void Ping()
        {
        }

        public void SendScreenshotResponse(Screenshot screenshot)
        {
            if (_requestId != null
                && screenshot != null
                && screenshot.RequestId == _requestId.Value)
            {
                if (_completeScreenshot != null)
                {
                    _completeScreenshot(screenshot);
                }
            }
        }

        #endregion

        #region Methods

        private void SafeInvokeDisconnected()
        {
            if (Disconnected == null)
            {
                return; //No Listeners
            }

            DisconnectedEvent listener = null;
            var dels = Disconnected.GetInvocationList();

            foreach (var del in dels)
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
                    Disconnected -= listener;
                }
            }
        }

        private void SafeInvokeMessageRecevied(MessageReceivedEventArgs eventArgs)
        {
            if (RemoteMessage == null)
            {
                return; //No Listeners
            }

            MessageReceivedEvent listener = null;
            var dels = RemoteMessage.GetInvocationList();

            foreach (var del in dels)
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
                    RemoteMessage -= listener;
                }
            }
        }

        private void SafeInvokeScreenshotRequested(ScreenshotRequest eventArgs)
        {
            if (ScreenshotRequested == null)
            {
                return; //No Listeners
            }

            ScreenshotRequestedEvent listener = null;
            var dels = ScreenshotRequested.GetInvocationList();

            foreach (var del in dels)
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
                    ScreenshotRequested -= listener;
                }
            }
        }

        #endregion
    }
}
