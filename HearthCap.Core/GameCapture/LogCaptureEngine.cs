using System;
using System.ComponentModel.Composition;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using HearthCap.Core.GameCapture.EngineEvents;
using HearthCap.Core.GameCapture.HS.Events;
using HearthCap.Core.Util;
using HearthCap.Data;
using HearthCap.Logger.Interface;
using LogManager = NLog.LogManager;

namespace HearthCap.Core.GameCapture
{
    public interface ILogCaptureEngine : ICaptureEngine
    {
    }

    [Export(typeof(ILogCaptureEngine))]
    public class LogCaptureEngine : ILogCaptureEngine, IDisposable,
        IHandle<RequestDecks>
    {
        private static readonly NLog.Logger Log = LogManager.GetCurrentClassLogger();

        private static readonly TraceLogger TraceLog = new TraceLogger(Log);

        private CaptureInterface _interface;

        private IServerInterface CaptureInterface
        {
            get { return _interface; }
        }

        // private ClientCaptureInterfaceEventProxy _clientEventProxy;

        // private IChannel _clientServerChannel;

        private readonly string probe;

        private bool windowFound;

        private bool windowLost;

        private readonly IEventAggregator events;

        // private Regex _loadingScreenRegex = new Regex(@"prevMode=(?<prevMode>.*) nextMode=(?<nextMode>.*)");
        private readonly Regex _playScreenRegex = new Regex(@"prevMode=(?<prevMode>.*) nextMode=(?<nextMode>.*)");
        private readonly Regex _deckRegex = new Regex(@"prev=(?<prev>.*) next=(?<next>.*)");
        private readonly Regex _modeRegex = new Regex(@"prev=(?<prev>.*) next=(?<next>.*)");
        private Regex _gameStateRegex = new Regex(@"prev=(?<prev>.*) next=(?<next>.*)");
        private readonly Regex _turnRegex = new Regex(@"prev=(?<prev>.*) next=(?<next>.*)");
        private readonly Regex _gameStartRegex = new Regex(@"hero=(?<hero>.*) opponenthero=(?<opponenthero>.*) turn=(?<turn>.*) first=(?<first>.*)");
        private readonly Regex _gameOverRegex = new Regex(@"victory=(?<victory>.*)");

        private GameMode lastGameMode;

        private bool stopRequested;

        private IChannel _server;

        private string lastDeck;

        private GameStarted gameStarted;

        private bool first;
        private int turns;

        private bool injected;

        [ImportingConstructor]
        public LogCaptureEngine(IEventAggregator events)
        {
            this.events = events;
            probe = AppDomain.CurrentDomain.BaseDirectory;
            events.Subscribe(this);
        }

        public event EventHandler<EventArgs> Started;

        public event EventHandler<EventArgs> Stopped;

        public int Speed { get; set; }

        public bool PublishCapturedWindow { get; set; }

        public bool IsRunning { get; private set; }

        public CaptureMethod CaptureMethod { get; set; }

        public Task StartAsync()
        {
            return Task.Run(() => Start()).ContinueWith(t => OnUnhandledException(t.Exception), TaskContinuationOptions.OnlyOnFaulted);
        }

        private void Start()
        {
            if (IsRunning)
            {
                Log.Warn("already running");
                return;
            }

            IsRunning = true;
            try
            {
                Log.Debug("Capture method: {0}, Version: {1}", CaptureMethod, Assembly.GetEntryAssembly().GetName().Version);
                stopRequested = false;

                OnStarted();
                CaptureLoop();
                while (stopRequested == false)
                {
                    CaptureLoop();
                }
            }
            finally
            {
                if (_interface != null)
                {
                    try
                    {
                        _interface.Disconnect();
                    }
                    finally
                    {
                        RemotingServices.Disconnect(_interface);
                    }
                }

                IsRunning = false;
            }
        }

        private void CaptureLoop()
        {
            if (!injected)
            {
                var wnd = IntPtr.Zero;
                try
                {
                    wnd = HearthstoneHelper.GetHearthstoneWindow();
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }

                // Verify window exists
                if (wnd == IntPtr.Zero)
                {
                    OnWindowLost();
                    Thread.Sleep(1000);
                    return;
                }

                Thread.Sleep(3000);

                InitServerChannel();
                var injectresult = -1;
                var injecttask = Task.Run(
                    () => { injectresult = Inject("hearthstone.exe", "HearthCapLogger.dll", "HearthCapLogger", "Loader", "Load", probe); });

                if (!injecttask.Wait(5000))
                {
                }

                if (injectresult != 0)
                {
                    Log.Warn("Window found, but could not find hearthstone.exe");
                    OnWindowLost();
                    Thread.Sleep(1000);
                    return;
                }

                Log.Info("Injected into hearthstone.exe");

                Thread.Sleep(500);
                injected = true;
            }

            try
            {
                var error = false;

                try
                {
                    // .NET Remoting exceptions will throw RemotingException
                    if (!_interface.Ping(1000))
                    {
                        Log.Error("Failed to ping log engine.");
                        error = true;
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                    error = true;
                }

                if (error)
                {
                    injected = false;
                }
                else
                {
                    OnWindowFound();
                    Thread.Sleep(1000);
                }
            }
            catch (Exception e)
            {
                Log.Error(e.ToString);
                // this._interface.Message(MessageType.Error, "An unexpected error occured: {0}", e.ToString());
            }
        }

        private void OnWindowLost()
        {
            if (!windowLost)
            {
                Log.Debug("Hearthstone window is lost (not running?)");
                Publish(new WindowNotFound());
            }

            windowFound = false;
            windowLost = true;
        }

        protected void Publish(EngineEvent message, bool log = true)
        {
            if (log)
            {
                Log.Info("Event ({0}): {1}", message.GetType().Name, message.Message);
            }
            events.PublishOnBackgroundThread(message);
        }

        private void Publish(GameEvent message)
        {
            Log.Info("GameEvent ({0}): {1}", message.GetType().Name, message.Message ?? "(no message)");
            //if (requestReset)
            //{
            //    Log.Info("Ignoring last GameEvent because scanner reset was requested.");
            //    return;
            //}
            events.PublishOnBackgroundThread(message);
        }

        private void OnWindowFound()
        {
            if (!windowFound)
            {
                Log.Debug("Window found, and capturing.");
                Publish(new WindowFound());
            }
            windowFound = true;
            windowLost = false;
        }

        private void InitServerChannel()
        {
            // Initialise the IPC server (with our instance of _serverInterface)
            try
            {
                var channelName = "hslog";
                //this._interface = IpcUtil.IpcConnectClient<CaptureInterface>(channelName);
                //this._interface.Ping();

                if (_interface != null)
                {
                    _interface.Disconnect();
                    RemotingServices.Disconnect(_interface);
                    _interface = null;
                }

                _interface = new CaptureInterface();
                _server = IpcUtil.IpcCreateServer(
                    ref channelName,
                    WellKnownObjectMode.Singleton,
                    _interface);

                //// Attempt to create a IpcServerChannel so that any event handlers on the client will function correctly
                //System.Collections.IDictionary properties = new System.Collections.Hashtable();
                //properties["name"] = channelName + ".rem";
                //properties["port"] = 19191 + 1;
                //// random portName so no conflict with existing channels of channelName

                //var binaryProv = new BinaryServerFormatterSinkProvider
                //                     {
                //                         TypeFilterLevel =
                //                             System.Runtime.Serialization.Formatters.TypeFilterLevel
                //                             .Full
                //                     };

                //this._clientServerChannel = new TcpServerChannel(properties, binaryProv);
                //ChannelServices.RegisterChannel(this._clientServerChannel, false);

                //this._clientEventProxy = new ClientCaptureInterfaceEventProxy();
                //this._interface.RemoteMessage += this._clientEventProxy.RemoteMessageHandler;
                //this._clientEventProxy.RemoteMessage += ServerInterfaceOnRemoteMessage;
                CaptureInterface.RemoteMessage += ServerInterfaceOnRemoteMessage;
                CaptureInterface.Published += ServerInterfaceOnPublished;
            }
            finally
            {
            }

            Log.Info("Remoting service channel setup.");
        }

        private void ServerInterfaceOnPublished(PublishEventArgs message)
        {
            // events.PublishOnBackgroundThread(message.Message);
            events.PublishOnCurrentThread(message.Message);
        }

        private void ServerInterfaceOnRemoteMessage(MessageReceivedEventArgs message)
        {
            ParseMessage(message.MessageType, message.Message);
        }

        private void ParseMessage(MessageType messageType, string message)
        {
            Log.Debug("[{0}] {1}", messageType, message);

            //if (message.StartsWith("type:Log condition:"))
            //{
            //    var condition = message.Substring(19, message.Length - 19);
            //    if (condition.StartsWith("[LoadingScreen] LoadingScreen.OnSceneLoaded()"))
            //    {
            //        var match = _loadingScreenRegex.Match(condition);
            //        if (match.Success)
            //        {
            //            var nextMode = GetGameMode(match.Groups["nextMode"].Value);
            //            if (this.lastGameMode != nextMode && nextMode != GameMode.Unknown)
            //            {
            //                this.Publish(new GameModeChanged(lastGameMode, nextMode));
            //                this.lastGameMode = nextMode;
            //            }
            //        }
            //    }
            //}

            if (message.StartsWith("[EXT] Mode"))
            {
                var match = _modeRegex.Match(message);
                if (match.Success)
                {
                    var nextMode = GetGameMode(match.Groups["next"].Value);

                    if (lastGameMode != nextMode
                        && nextMode != GameMode.Unknown)
                    {
                        Publish(new GameModeChanged(lastGameMode, nextMode));
                        lastGameMode = nextMode;
                    }
                }
            }

            if (message.StartsWith("[EXT] InRankedPlay"))
            {
                var match = _playScreenRegex.Match(message);
                if (match.Success)
                {
                    var nextMode = match.Groups["nextMode"].Value.ToLowerInvariant() == "true" ? GameMode.Ranked : GameMode.Casual;

                    if (lastGameMode != nextMode
                        && nextMode != GameMode.Unknown)
                    {
                        Publish(new GameModeChanged(lastGameMode, nextMode));
                        lastGameMode = nextMode;
                    }
                }
            }

            if (message.StartsWith("[EXT] DeckChanged"))
            {
                var match = _deckRegex.Match(message);
                if (match.Success)
                {
                    var nextDeck = match.Groups["next"].Value;

                    if (lastDeck != nextDeck)
                    {
                        Publish(new DeckDetected(nextDeck));
                        lastDeck = nextDeck;
                    }
                }
            }

            // Games

            if (message.StartsWith("[EXT] GameStart"))
            {
                var match = _gameStartRegex.Match(message);
                if (match.Success)
                {
                    var hero = match.Groups["hero"].Value.ToLowerInvariant();
                    var opponenthero = match.Groups["opponenthero"].Value.ToLowerInvariant();
                    var turn = match.Groups["turn"].Value;
                    int.TryParse(turn, out turns);
                    first = match.Groups["first"].Value == "1";

                    gameStarted = new GameStarted(lastGameMode, DateTime.Now, hero, opponenthero, first, lastDeck);
                    Publish(gameStarted);
                }
            }

            if (message.StartsWith("[EXT] Turn"))
            {
                var match = _turnRegex.Match(message);
                if (match.Success)
                {
                    var next = match.Groups["next"].Value;
                    var nextturn = 0;
                    if (int.TryParse(next, out nextturn))
                    {
                        if (nextturn != 0
                            && turns != nextturn)
                        {
                            Publish(new NewRound(nextturn, first ? nextturn % 2 == 1 : nextturn % 2 == 0));
                            turns = nextturn;
                        }
                    }
                }
            }

            //if (message.StartsWith("[EXT] GameState"))
            //{
            //    var match = _gameStateRegex.Match(message);
            //    if (match.Success)
            //    {
            //        var hero = match.Groups["hero"].Value;
            //        var opponenthero = match.Groups["opponenthero"].Value;
            //        var turn = match.Groups["turn"].Value;
            //        var first = match.Groups["first"].Value == "1";

            //        gameStarted = new GameStarted(lastGameMode, DateTime.Now, hero, opponenthero, first, lastDeck);
            //        Publish(gameStarted);
            //    }
            //}

            if (message.StartsWith("[EXT] GameOver")
                && gameStarted != null)
            {
                var match = _gameOverRegex.Match(message);
                if (match.Success)
                {
                    var victory = match.Groups["victory"].Value;

                    Publish(new GameEnded
                        {
                            Deck = gameStarted.Deck,
                            EndTime = DateTime.Now,
                            GameMode = gameStarted.GameMode,
                            GoFirst = gameStarted.GoFirst,
                            Hero = gameStarted.Hero,
                            OpponentHero = gameStarted.OpponentHero,
                            StartTime = gameStarted.StartTime,
                            Turns = turns,
                            Victory = victory == "1"
                        });
                    gameStarted = null;
                }
            }
        }

        private GameMode GetGameMode(string gameMode)
        {
            if (gameMode == null)
            {
                return GameMode.Unknown;
            }

            switch (gameMode.ToUpperInvariant())
            {
                case "PRACTICE":
                    return GameMode.Practice;
                case "DRAFT":
                    return GameMode.Arena;
                case "FRIENDLY":
                    return GameMode.Challenge;
                case "HUB":
                    return GameMode.Menu;
                case "TAVERN_BRAWL":
                    return GameMode.TavernBrawl;
                case "ADVENTURE":
                    return GameMode.Mission;
                case "MISSIONDISPLAY":
                    return GameMode.Mission;
                case "CLASSCHALLENGE":
                    return GameMode.Mission;
                default:
                    return GameMode.Unknown;
            }
        }

        public void Stop()
        {
            if (!IsRunning)
            {
                return;
            }

            stopRequested = true;
        }

        protected virtual void OnUnhandledException(Exception exception)
        {
            var e = new UnhandledExceptionEventArgs(exception, false);
            var handler = UnhandledException;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler<UnhandledExceptionEventArgs> UnhandledException;

        [DllImport("HearthCapLoader.dll", EntryPoint = "Inject", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Inject(
            [In] [MarshalAs(UnmanagedType.LPWStr)] string target,
            [In] [MarshalAs(UnmanagedType.LPWStr)] string dll,
            [In] [MarshalAs(UnmanagedType.LPWStr)] string name_space,
            [In] [MarshalAs(UnmanagedType.LPWStr)] string class_name,
            [In] [MarshalAs(UnmanagedType.LPWStr)] string method_name,
            [In] [MarshalAs(UnmanagedType.LPWStr)] string probe);

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        ///     Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(RequestDecks message)
        {
            CaptureInterface.Publish(message);
        }

        protected virtual void OnStarted()
        {
            var handler = Started;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        protected virtual void OnStopped()
        {
            var handler = Stopped;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}
