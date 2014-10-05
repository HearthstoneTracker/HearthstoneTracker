// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogCaptureEngine.cs" company="">
//   
// </copyright>
// <summary>
//   The LogCaptureEngine interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Core.GameCapture
{
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

    using NLog;

    using LogManager = NLog.LogManager;

    /// <summary>
    /// The LogCaptureEngine interface.
    /// </summary>
    public interface ILogCaptureEngine : ICaptureEngine
    {

    }

    /// <summary>
    /// The log capture engine.
    /// </summary>
    [Export(typeof(ILogCaptureEngine))]
    public class LogCaptureEngine : ILogCaptureEngine, IDisposable, 
        IHandle<RequestDecks>
    {
        /// <summary>
        /// The running.
        /// </summary>
        private bool running;

        /// <summary>
        /// The log.
        /// </summary>
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The trace log.
        /// </summary>
        private static readonly TraceLogger TraceLog = new TraceLogger(Log);

        /// <summary>
        /// The _interface.
        /// </summary>
        private CaptureInterface _interface;

        /// <summary>
        /// Gets the capture interface.
        /// </summary>
        private IServerInterface CaptureInterface
        {
            get
            {
                return this._interface;
            }
        }

        // private ClientCaptureInterfaceEventProxy _clientEventProxy;

        // private IChannel _clientServerChannel;

        /// <summary>
        /// The probe.
        /// </summary>
        private string probe;

        /// <summary>
        /// The window found.
        /// </summary>
        private bool windowFound;

        /// <summary>
        /// The window lost.
        /// </summary>
        private bool windowLost;

        /// <summary>
        /// The window minimized.
        /// </summary>
        private bool windowMinimized;

        /// <summary>
        /// The events.
        /// </summary>
        private IEventAggregator events;

        // private Regex _loadingScreenRegex = new Regex(@"prevMode=(?<prevMode>.*) nextMode=(?<nextMode>.*)");
        /// <summary>
        /// The _play screen regex.
        /// </summary>
        private Regex _playScreenRegex = new Regex(@"prevMode=(?<prevMode>.*) nextMode=(?<nextMode>.*)");

        /// <summary>
        /// The _deck regex.
        /// </summary>
        private Regex _deckRegex = new Regex(@"prev=(?<prev>.*) next=(?<next>.*)");

        /// <summary>
        /// The _mode regex.
        /// </summary>
        private Regex _modeRegex = new Regex(@"prev=(?<prev>.*) next=(?<next>.*)");

        /// <summary>
        /// The _game state regex.
        /// </summary>
        private Regex _gameStateRegex = new Regex(@"prev=(?<prev>.*) next=(?<next>.*)");

        /// <summary>
        /// The _turn regex.
        /// </summary>
        private Regex _turnRegex = new Regex(@"prev=(?<prev>.*) next=(?<next>.*)");

        /// <summary>
        /// The _game start regex.
        /// </summary>
        private Regex _gameStartRegex = new Regex(@"hero=(?<hero>.*) opponenthero=(?<opponenthero>.*) turn=(?<turn>.*) first=(?<first>.*)");

        /// <summary>
        /// The _game over regex.
        /// </summary>
        private Regex _gameOverRegex = new Regex(@"victory=(?<victory>.*)");

        /// <summary>
        /// The last game mode.
        /// </summary>
        private GameMode lastGameMode;

        /// <summary>
        /// The stop requested.
        /// </summary>
        private bool stopRequested;

        /// <summary>
        /// The _server.
        /// </summary>
        private IChannel _server;

        /// <summary>
        /// The last deck.
        /// </summary>
        private string lastDeck;

        /// <summary>
        /// The game started.
        /// </summary>
        private GameStarted gameStarted;

        /// <summary>
        /// The first.
        /// </summary>
        private bool first;

        /// <summary>
        /// The turns.
        /// </summary>
        private int turns;

        /// <summary>
        /// The injected.
        /// </summary>
        private bool injected;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogCaptureEngine"/> class.
        /// </summary>
        /// <param name="events">
        /// The events.
        /// </param>
        [ImportingConstructor]
        public LogCaptureEngine(IEventAggregator events)
        {
            this.events = events;
            this.probe = AppDomain.CurrentDomain.BaseDirectory;
            events.Subscribe(this);
        }

        /// <summary>
        /// Gets or sets the speed.
        /// </summary>
        public int Speed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether publish captured window.
        /// </summary>
        public bool PublishCapturedWindow { get; set; }

        /// <summary>
        /// Gets a value indicating whether is running.
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return this.running;
            }
        }

        /// <summary>
        /// Gets or sets the capture method.
        /// </summary>
        public CaptureMethod CaptureMethod { get; set; }

        /// <summary>
        /// The start async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public Task StartAsync()
        {
            return Task.Run(() => this.Start()).ContinueWith(t => this.OnUnhandledException(t.Exception), TaskContinuationOptions.OnlyOnFaulted);
        }

        /// <summary>
        /// The start.
        /// </summary>
        private void Start()
        {
            if (this.running)
            {
                Log.Warn("already running");
                return;
            }

            this.running = true;

            Log.Debug("Capture method: {0}, Version: {1}", this.CaptureMethod, Assembly.GetEntryAssembly().GetName().Version);

            this.stopRequested = false;
            while (this.stopRequested == false)
            {
                if (!this.injected)
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
                        this.OnWindowLost();
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
                        this.OnWindowLost();
                        Thread.Sleep(1000);
                        continue;
                    }

                    Log.Info("Injected into hearthstone.exe");

                    Thread.Sleep(500);
                    this.injected = true;
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
                        this.injected = false;
                    }
                    else
                    {
                        this.OnWindowFound();
                        Thread.Sleep(1000);
                    }

                }
                catch (Exception e)
                {
                    Log.Error(e.ToString);

                    // this._interface.Message(MessageType.Error, "An unexpected error occured: {0}", e.ToString());
                }
            }

            if (this._interface != null)
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

            this.running = false;
        }

        /// <summary>
        /// The on window lost.
        /// </summary>
        private void OnWindowLost()
        {
            if (!this.windowLost)
            {
                Log.Debug("Hearthstone window is lost (not running?)");
                this.Publish(new WindowNotFound());
            }

            this.windowMinimized = false;
            this.windowFound = false;
            this.windowLost = true;
        }

        /// <summary>
        /// The publish.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="log">
        /// The log.
        /// </param>
        protected void Publish(EngineEvent message, bool log = true)
        {
            if (log)
            {
                Log.Info("Event ({0}): {1}", message.GetType().Name, message.Message);
            }

            this.events.PublishOnBackgroundThread(message);
        }

        /// <summary>
        /// The publish.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        private void Publish(GameEvent message)
        {
            Log.Info("GameEvent ({0}): {1}", message.GetType().Name, message.Message ?? "(no message)");

            // if (requestReset)
            // {
            // Log.Info("Ignoring last GameEvent because scanner reset was requested.");
            // return;
            // }
            this.events.PublishOnBackgroundThread(message);
        }

        /// <summary>
        /// The on window found.
        /// </summary>
        private void OnWindowFound()
        {
            if (!this.windowFound)
            {
                Log.Debug("Window found, and capturing.");
                this.Publish(new WindowFound());
            }

            this.windowFound = true;
            this.windowLost = false;
            this.windowMinimized = false;
        }

        /// <summary>
        /// The init server channel.
        /// </summary>
        private void InitServerChannel()
        {
            // Initialise the IPC server (with our instance of _serverInterface)
            try
            {
                string channelName = "hslog";

                // this._interface = IpcUtil.IpcConnectClient<CaptureInterface>(channelName);
                // this._interface.Ping();
                if (this._interface != null)
                {
                    this._interface.Disconnect();
                    RemotingServices.Disconnect(this._interface);
                    this._interface = null;
                }

                this._interface = new CaptureInterface();
                this._server = IpcUtil.IpcCreateServer<CaptureInterface>(ref channelName, WellKnownObjectMode.Singleton, this._interface);

                //// Attempt to create a IpcServerChannel so that any event handlers on the client will function correctly
                // System.Collections.IDictionary properties = new System.Collections.Hashtable();
                // properties["name"] = channelName + ".rem";
                // properties["port"] = 19191 + 1;
                //// random portName so no conflict with existing channels of channelName

                // var binaryProv = new BinaryServerFormatterSinkProvider
                // {
                // TypeFilterLevel =
                // System.Runtime.Serialization.Formatters.TypeFilterLevel
                // .Full
                // };

                // this._clientServerChannel = new TcpServerChannel(properties, binaryProv);
                // ChannelServices.RegisterChannel(this._clientServerChannel, false);

                // this._clientEventProxy = new ClientCaptureInterfaceEventProxy();
                // this._interface.RemoteMessage += this._clientEventProxy.RemoteMessageHandler;
                // this._clientEventProxy.RemoteMessage += ServerInterfaceOnRemoteMessage;
                this.CaptureInterface.RemoteMessage += this.ServerInterfaceOnRemoteMessage;
                this.CaptureInterface.Published += this.ServerInterfaceOnPublished;
            }
            finally
            {
            }

            Log.Info("Remoting service channel setup.");
        }

        /// <summary>
        /// The server interface on published.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        private void ServerInterfaceOnPublished(PublishEventArgs message)
        {
            // events.PublishOnBackgroundThread(message.Message);
            this.events.PublishOnCurrentThread(message.Message);
        }

        /// <summary>
        /// The server interface on remote message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        private void ServerInterfaceOnRemoteMessage(MessageReceivedEventArgs message)
        {
            this.ParseMessage(message.MessageType, message.Message);
        }

        /// <summary>
        /// The parse message.
        /// </summary>
        /// <param name="messageType">
        /// The message type.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        private void ParseMessage(MessageType messageType, string message)
        {
            Log.Debug("[{0}] {1}", messageType, message);

            // if (message.StartsWith("type:Log condition:"))
            // {
            // var condition = message.Substring(19, message.Length - 19);
            // if (condition.StartsWith("[LoadingScreen] LoadingScreen.OnSceneLoaded()"))
            // {
            // var match = _loadingScreenRegex.Match(condition);
            // if (match.Success)
            // {
            // var nextMode = GetGameMode(match.Groups["nextMode"].Value);
            // if (this.lastGameMode != nextMode && nextMode != GameMode.Unknown)
            // {
            // this.Publish(new GameModeChanged(lastGameMode, nextMode));
            // this.lastGameMode = nextMode;
            // }
            // }
            // }
            // }
            if (message.StartsWith("[EXT] Mode"))
            {
                var match = this._modeRegex.Match(message);
                if (match.Success)
                {
                    var nextMode = this.GetGameMode(match.Groups["next"].Value);

                    if (this.lastGameMode != nextMode && nextMode != GameMode.Unknown)
                    {
                        this.Publish(new GameModeChanged(this.lastGameMode, nextMode));
                        this.lastGameMode = nextMode;
                    }
                }
            }

            if (message.StartsWith("[EXT] InRankedPlay"))
            {
                var match = this._playScreenRegex.Match(message);
                if (match.Success)
                {
                    var nextMode = match.Groups["nextMode"].Value.ToLowerInvariant() == "true" ? GameMode.Ranked : GameMode.Casual;

                    if (this.lastGameMode != nextMode && nextMode != GameMode.Unknown)
                    {
                        this.Publish(new GameModeChanged(this.lastGameMode, nextMode));
                        this.lastGameMode = nextMode;
                    }
                }
            }

            if (message.StartsWith("[EXT] DeckChanged"))
            {
                var match = this._deckRegex.Match(message);
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
                var match = this._gameStartRegex.Match(message);
                if (match.Success)
                {
                    var hero = match.Groups["hero"].Value.ToLowerInvariant();
                    var opponenthero = match.Groups["opponenthero"].Value.ToLowerInvariant();
                    var turn = match.Groups["turn"].Value;
                    int.TryParse(turn, out this.turns);
                    this.first = match.Groups["first"].Value == "1";

                    this.gameStarted = new GameStarted(this.lastGameMode, DateTime.Now, hero, opponenthero, this.first, this.lastDeck);
                    this.Publish(this.gameStarted);
                }
            }

            if (message.StartsWith("[EXT] Turn"))
            {
                var match = this._turnRegex.Match(message);
                if (match.Success)
                {
                    var next = match.Groups["next"].Value;
                    int nextturn = 0;
                    if (int.TryParse(next, out nextturn))
                    {
                        if (nextturn != 0 && this.turns != nextturn)
                        {
                            this.Publish(new NewRound(nextturn, this.first ? nextturn % 2 == 1 : nextturn % 2 == 0));
                            this.turns = nextturn;
                        }
                    }
                }
            }

            // if (message.StartsWith("[EXT] GameState"))
            // {
            // var match = _gameStateRegex.Match(message);
            // if (match.Success)
            // {
            // var hero = match.Groups["hero"].Value;
            // var opponenthero = match.Groups["opponenthero"].Value;
            // var turn = match.Groups["turn"].Value;
            // var first = match.Groups["first"].Value == "1";

            // gameStarted = new GameStarted(lastGameMode, DateTime.Now, hero, opponenthero, first, lastDeck);
            // Publish(gameStarted);
            // }
            // }
            if (message.StartsWith("[EXT] GameOver") && this.gameStarted != null)
            {
                var match = this._gameOverRegex.Match(message);
                if (match.Success)
                {
                    var victory = match.Groups["victory"].Value;

                    this.Publish(new GameEnded {
                                    Deck = this.gameStarted.Deck, 
                                    EndTime = DateTime.Now, 
                                    GameMode = this.gameStarted.GameMode, 
                                    GoFirst = this.gameStarted.GoFirst, 
                                    Hero = this.gameStarted.Hero, 
                                    OpponentHero = this.gameStarted.OpponentHero, 
                                    StartTime = this.gameStarted.StartTime, 
                                    Turns = this.turns, 
                                    Victory = victory == "1"
                                });
                    this.gameStarted = null;
                }
            }
        }

        /// <summary>
        /// The get game mode.
        /// </summary>
        /// <param name="gameMode">
        /// The game mode.
        /// </param>
        /// <returns>
        /// The <see cref="GameMode"/>.
        /// </returns>
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

        /// <summary>
        /// The stop.
        /// </summary>
        public void Stop()
        {
            if (!this.IsRunning)
                return;

            this.stopRequested = true;
        }

        /// <summary>
        /// The on unhandled exception.
        /// </summary>
        /// <param name="exception">
        /// The exception.
        /// </param>
        protected virtual void OnUnhandledException(Exception exception)
        {
            var e = new UnhandledExceptionEventArgs(exception, false);
            var handler = this.UnhandledException;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// The unhandled exception.
        /// </summary>
        public event EventHandler<UnhandledExceptionEventArgs> UnhandledException;

        /// <summary>
        /// The inject.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="dll">
        /// The dll.
        /// </param>
        /// <param name="name_space">
        /// The name_space.
        /// </param>
        /// <param name="class_name">
        /// The class_name.
        /// </param>
        /// <param name="method_name">
        /// The method_name.
        /// </param>
        /// <param name="probe">
        /// The probe.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
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
        /// <param name="message">
        /// The message.
        /// </param>
        public void Handle(RequestDecks message)
        {
            this.CaptureInterface.Publish(message);
        }
    }
}