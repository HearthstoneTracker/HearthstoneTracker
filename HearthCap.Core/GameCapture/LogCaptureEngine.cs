namespace HearthCap.Core.GameCapture
{
    using System;
    using System.ComponentModel.Composition;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting;
    using System.Runtime.Remoting.Channels;
    using System.Runtime.Remoting.Channels.Tcp;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;

    using Caliburn.Micro;

    using HearthCap.Core.GameCapture.EngineEvents;
    using HearthCap.Core.GameCapture.HS.Events;
    using HearthCap.Core.Util;
    using HearthCap.Data;
    using HearthCap.Logger.Interface;

    using NLog;

    using LogManager = NLog.LogManager;

    public interface ILogCaptureEngine : ICaptureEngine
    {

    }

    [Export(typeof(ILogCaptureEngine))]
    public class LogCaptureEngine : ILogCaptureEngine, IDisposable,
        IHandle<RequestDecks>
    {
        private bool running;
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private static readonly TraceLogger TraceLog = new TraceLogger(Log);

        private CaptureInterface _interface;

        private IServerInterface CaptureInterface
        {
            get
            {
                return (IServerInterface)_interface;
            }
        }

        // private ClientCaptureInterfaceEventProxy _clientEventProxy;

        // private IChannel _clientServerChannel;

        private string probe;

        private bool windowFound;

        private bool windowLost;

        private IEventAggregator events;

        // private Regex _loadingScreenRegex = new Regex(@"prevMode=(?<prevMode>.*) nextMode=(?<nextMode>.*)");
        private Regex _playScreenRegex = new Regex(@"prevMode=(?<prevMode>.*) nextMode=(?<nextMode>.*)");
        private Regex _deckRegex = new Regex(@"prev=(?<prev>.*) next=(?<next>.*)");
        private Regex _modeRegex = new Regex(@"prev=(?<prev>.*) next=(?<next>.*)");
        private Regex _gameStateRegex = new Regex(@"prev=(?<prev>.*) next=(?<next>.*)");
        private Regex _turnRegex = new Regex(@"prev=(?<prev>.*) next=(?<next>.*)");
        private Regex _gameStartRegex = new Regex(@"hero=(?<hero>.*) opponenthero=(?<opponenthero>.*) turn=(?<turn>.*) first=(?<first>.*)");
        private Regex _gameOverRegex = new Regex(@"victory=(?<victory>.*)");

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
            this.probe = AppDomain.CurrentDomain.BaseDirectory;
            events.Subscribe(this);
        }

        public int Speed { get; set; }

        public bool PublishCapturedWindow { get; set; }

        public bool IsRunning
        {
            get
            {
                return this.running;
            }
        }

        public CaptureMethod CaptureMethod { get; set; }

        public Task StartAsync()
        {
            return Task.Run(() => this.Start()).ContinueWith(t => this.OnUnhandledException(t.Exception), TaskContinuationOptions.OnlyOnFaulted);
        }

        private void Start()
        {
            if (this.running)
            {
                Log.Warn("already running");
                return;
            }
            this.running = true;

            Log.Debug("Capture method: {0}, Version: {1}", this.CaptureMethod, Assembly.GetEntryAssembly().GetName().Version);

            stopRequested = false;
            while (stopRequested == false)
            {
                if (!injected)
                {
                    IntPtr wnd = IntPtr.Zero;
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
                        continue;
                    }

                    Thread.Sleep(3000);

                    this.InitServerChannel();
                    int injectresult = -1;
                    var injecttask = Task.Run(
                        () =>
                        {
                            injectresult = Inject("hearthstone.exe", "HearthCapLogger.dll", "HearthCapLogger", "Loader", "Load", this.probe);
                        });

                    if (!injecttask.Wait(5000))
                    {

                    }

                    if (injectresult != 0)
                    {
                        Log.Warn("Window found, but could not find hearthstone.exe");
                        OnWindowLost();
                        Thread.Sleep(1000);
                        continue;
                    }

                    Log.Info("Injected into hearthstone.exe");

                    Thread.Sleep(500);
                    injected = true;
                }

                try
                {
                    bool error = false;

                    try
                    {
                        // .NET Remoting exceptions will throw RemotingException
                        if (!this._interface.Ping(1000))
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
            if (_interface != null)
            {
                try
                {
                    this._interface.Disconnect();
                }
                finally
                {
                    RemotingServices.Disconnect(this._interface);
                }
            }
            running = false;
        }

        private void OnWindowLost()
        {
            if (!this.windowLost)
            {
                Log.Debug("Hearthstone window is lost (not running?)");
                this.Publish(new WindowNotFound());
            }

            this.windowFound = false;
            this.windowLost = true;
        }

        protected void Publish(EngineEvent message, bool log = true)
        {
            if (log)
            {
                Log.Info("Event ({0}): {1}", message.GetType().Name, message.Message);
            }
            this.events.PublishOnBackgroundThread(message);
        }

        private void Publish(GameEvent message)
        {
            Log.Info("GameEvent ({0}): {1}", message.GetType().Name, message.Message ?? "(no message)");
            //if (requestReset)
            //{
            //    Log.Info("Ignoring last GameEvent because scanner reset was requested.");
            //    return;
            //}
            this.events.PublishOnBackgroundThread(message);
        }

        private void OnWindowFound()
        {
            if (!this.windowFound)
            {
                Log.Debug("Window found, and capturing.");
                this.Publish(new WindowFound());
            }
            this.windowFound = true;
            this.windowLost = false;
        }

        private void InitServerChannel()
        {
            // Initialise the IPC server (with our instance of _serverInterface)
            try
            {
                string channelName = "hslog";
                //this._interface = IpcUtil.IpcConnectClient<CaptureInterface>(channelName);
                //this._interface.Ping();

                if (this._interface != null)
                {
                    this._interface.Disconnect();
                    RemotingServices.Disconnect(this._interface);
                    this._interface = null;
                }

                this._interface = new CaptureInterface();
                this._server = IpcUtil.IpcCreateServer<CaptureInterface>(
                    ref channelName,
                    WellKnownObjectMode.Singleton,
                    this._interface);

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

                    if (this.lastGameMode != nextMode && nextMode != GameMode.Unknown)
                    {
                        this.Publish(new GameModeChanged(lastGameMode, nextMode));
                        this.lastGameMode = nextMode;
                    }
                }
            }

            if (message.StartsWith("[EXT] InRankedPlay"))
            {
                var match = _playScreenRegex.Match(message);
                if (match.Success)
                {
                    var nextMode = match.Groups["nextMode"].Value.ToLowerInvariant() == "true" ? GameMode.Ranked : GameMode.Casual;

                    if (this.lastGameMode != nextMode && nextMode != GameMode.Unknown)
                    {
                        this.Publish(new GameModeChanged(lastGameMode, nextMode));
                        this.lastGameMode = nextMode;
                    }
                }
            }

            if (message.StartsWith("[EXT] DeckChanged"))
            {
                var match = _deckRegex.Match(message);
                if (match.Success)
                {
                    var nextDeck = match.Groups["next"].Value;

                    if (this.lastDeck != nextDeck)
                    {
                        this.Publish(new DeckDetected(nextDeck));
                        this.lastDeck = nextDeck;
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
                    int.TryParse(turn, out this.turns);
                    this.first = match.Groups["first"].Value == "1";

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
                    int nextturn = 0;
                    if (int.TryParse(next, out nextturn))
                    {
                        if (nextturn != 0 && this.turns != nextturn)
                        {
                            this.Publish(new NewRound(nextturn, first ? nextturn % 2 == 1 : nextturn % 2 == 0));
                            this.turns = nextturn;
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

            if (message.StartsWith("[EXT] GameOver") && gameStarted != null)
            {
                var match = _gameOverRegex.Match(message);
                if (match.Success)
                {
                    var victory = match.Groups["victory"].Value;

                    Publish(new GameEnded()
                                {
                                    Deck = gameStarted.Deck,
                                    EndTime = DateTime.Now,
                                    GameMode = gameStarted.GameMode,
                                    GoFirst = gameStarted.GoFirst,
                                    Hero = gameStarted.Hero,
                                    OpponentHero = gameStarted.OpponentHero,
                                    StartTime = gameStarted.StartTime,
                                    Turns = this.turns,
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
                case "MISSIONDECKPICKER":
                    return GameMode.Mission;
                default:
                    return GameMode.Unknown;
            }
        }

        public void Stop()
        {
            if (!IsRunning)
                return;

            stopRequested = true;
        }

        protected virtual void OnUnhandledException(Exception exception)
        {
            var e = new UnhandledExceptionEventArgs(exception, false);
            var handler = this.UnhandledException;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler<UnhandledExceptionEventArgs> UnhandledException;

        [DllImport("HearthCapLoader.dll", EntryPoint = "Inject", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        static extern int Inject(
            [In, MarshalAs(UnmanagedType.LPWStr)] string target,
            [In, MarshalAs(UnmanagedType.LPWStr)] string dll,
            [In, MarshalAs(UnmanagedType.LPWStr)] string name_space,
            [In, MarshalAs(UnmanagedType.LPWStr)] string class_name,
            [In, MarshalAs(UnmanagedType.LPWStr)] string method_name,
            [In, MarshalAs(UnmanagedType.LPWStr)] string probe);

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(RequestDecks message)
        {
            CaptureInterface.Publish(message);
        }
    }
}