namespace HearthCap.Core.GameCapture.HS
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Caliburn.Micro;

    using HearthCap.Core.GameCapture.HS.Commands;
    using HearthCap.Core.GameCapture.HS.Events;
    using HearthCap.Core.Util;
    using HearthCap.Data;

    using NLog;

    using PHash;

    [Export(typeof(IImageScanner))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class HSImageScanner : IImageScanner,
        IHandle<ResetCurrentGame>,
        IHandle<RequestDeckScreenshot>,
        IHandle<RequestArenaDeckScreenshot>
    {
        private class CurrentScanState
        {
            public bool ArenaLeafDetected { get; set; }
        }

        private struct DetectionResult
        {
            public bool Found { get; private set; }

            public int Distance { get; private set; }

            public DetectionResult(bool found, int distance)
                : this()
            {
                this.Found = found;
                this.Distance = distance;
            }

            public static implicit operator bool(DetectionResult instance)
            {
                return instance.Found;
            }
        }

        #region Fields

        // private LeastRecentlyUsedCache<string, Tuple<byte[], DetectionResult>> hashCache = new LeastRecentlyUsedCache<string, Tuple<byte[], DetectionResult>>(50);

        private readonly IEventAggregator events;

        private readonly IScanAreaProvider scanAreaProvider;

        private ScanAreaDictionary areas;

        private bool arenaDrafting;

        private ScanAreaDictionary arenaHeroes;

        private bool coinDetected;

        private bool conceded;

        private ScanAreaDictionary decks;

        private DateTime endTime = DateTime.MinValue;

        private bool gameJustEnded;

        private GameMode gameMode;

        private bool gameStarted;

        private int gameTurns = 0;

        private bool goFirst = true;

        private string hero;

        private ScanAreaDictionary heroes;

        private Bitmap image;

        private IPerceptualHash imageHasher;

        private readonly ITemplateMatcher templateMatcher;

        private int inGameCounter = 0;

        private bool inMenu;

        private string lastDeck;

        private GameMode lastGameMode;

        private int lastResolution = 900;

        private static readonly Logger Log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly TraceLogger TraceLog = new TraceLogger(Log);

        private bool myturn;

        private string opponentHero;

        private ScanAreaDictionary opponentHeroes;

        private bool roundTurned;

        private IEnumerable<ScanAreas> scanareas;

        private DateTime startTime = DateTime.MinValue;

        private bool? victory;

        private string arenaHero;

        private ScanAreaDictionary arenaWinsLookup;

        private ScanAreaDictionary arenaLossesLookup;

        private int arenaWins = -1;

        private int arenaLosses = -1;

        private ScanAreaImageDictionary arenaWinsLookup2;

        private Queue<System.Action> gameModeChangeActionQueue = new Queue<System.Action>();

        private bool requestReset;

        private bool pauseScanning;

        private int foundVic;

        private int foundLoss;

        private StringBuilder foundUsing = new StringBuilder();

        private bool arenaWinsScanning;

        private string lastVicDetect;

        private string lastLossDetect;

        private CurrentScanState currentScan;

        private bool deckScreenshotRequested;

        private bool takeDeckScreenshot;

        private bool deckScreenshotRequestedCanceled;

        private CancellationTokenSource cancelDeckScreenshot;

        private bool arenadeckScreenshotRequested;

        private bool doArenaWinsScan;

        #endregion

        #region Constructors and Destructors

        [ImportingConstructor]
        public HSImageScanner(IEventAggregator events, IScanAreaProvider scanAreaProvider, IPerceptualHash imageHasher, ITemplateMatcher templateMatcher)
        {
            this.events = events;
            this.events.Subscribe(this);
            this.scanAreaProvider = scanAreaProvider;
            this.imageHasher = imageHasher;
            this.templateMatcher = templateMatcher;
            this.ThreshHold = 9;
            this.ThreshHoldForHeroes = 12;
            this.BaseResolution = 900;

            // TODO: fix, this is hacky
            BasePath = AppDomain.CurrentDomain.BaseDirectory;
            this.LoadScanAreas();
        }

        #endregion

        #region Public Properties

        public string BasePath { get; set; }

        public int BaseResolution { get; set; }

        public int OffetOverrideY { get; set; }

        public int OffsetOverrideX { get; set; }

        public int ThreshHold { get; set; }

        public int ThreshHoldForHeroes { get; set; }

        #endregion

        #region Public Methods and Operators

        public void Run(Bitmap img, object context)
        {
            this.image = img;
            if (this.lastResolution != img.Height)
            {
                Log.Info("Base resolution changed from {0} to {1}.", this.lastResolution, img.Height);
                this.lastResolution = img.Height;
                this.LoadScanAreas();
            }

            this.Scan();

            this.image = null;
        }

        public void Stop(object context)
        {
            this.Reset();
        }

        #endregion

        #region Methods

        //private string DetectBest(ScanAreaDictionary lookup, IDictionary<int, Tuple<ulong, Rectangle>> useArea, string debugkey)
        //{
        //    var hashes = new List<ulong>();
        //    var keys = new List<string>();

        //    var rect = new Rectangle();
        //    int theResolution;
        //    foreach (var key in lookup.Keys)
        //    {
        //        var template = lookup[key];
        //        ulong matchhash;
        //        if (template.ContainsKey(this.lastResolution))
        //        {
        //            matchhash = template[this.lastResolution].Hash;
        //        }
        //        else if (template.ContainsKey(this.BaseResolution))
        //        {
        //            matchhash = template[this.BaseResolution].Hash;
        //        }
        //        else
        //        {
        //            Log.Error("No scan data found for requested template: " + debugkey);
        //            return null;
        //        }
        //        hashes.Add(matchhash);
        //        keys.Add(key);
        //    }

        //    if (useArea.ContainsKey(this.lastResolution))
        //    {
        //        rect = useArea[this.lastResolution].Rect;
        //        theResolution = this.lastResolution;
        //    }
        //    else if (useArea.ContainsKey(this.BaseResolution))
        //    {
        //        rect = useArea[this.BaseResolution].Rect;
        //        theResolution = this.BaseResolution;
        //    }
        //    else
        //    {
        //        Log.Error("No scan data found for requested template: " + debugkey);
        //        return null;
        //    }

        //    var source = this.image;
        //    using (var roi = source.Clone(ResolutionHelper.CorrectRectangle(source.Size, rect, theResolution), PixelFormat.Format32bppRgb))
        //    {
        //        var hash = this.imageHasher.Create(roi);
        //        var best = PerceptualHash.FindBest(hash, hashes);
        //        if (best.Distance <= ThreshHold)
        //        {
        //            Log.Diag("Detected best hash: '{0}' Distance: {1}", debugkey, best.Distance);
        //            return keys[best.Index];                    
        //        }
        //    }

        //    return null;
        //}

        private string DetectBest(ScanAreaDictionary lookup, string debugkey, int? threshold = null)
        {
            threshold = threshold ?? ThreshHold;
            var hashes = new List<ulong>();
            var keys = new List<string>();

            var rect = new Rectangle();
            int theResolution = BaseResolution;
            foreach (var key in lookup.Keys)
            {
                var template = lookup[key];
                ulong matchhash;
                if (template.ContainsKey(this.lastResolution))
                {
                    matchhash = template[this.lastResolution].Hash;
                    rect = template[this.lastResolution].Rect;
                    theResolution = this.lastResolution;
                }
                else if (template.ContainsKey(this.BaseResolution))
                {
                    matchhash = template[this.BaseResolution].Hash;
                    rect = template[this.BaseResolution].Rect;
                    theResolution = this.BaseResolution;
                }
                else
                {
                    Log.Error("No scan data found for requested template: " + debugkey);
                    return null;
                }
                hashes.Add(matchhash);
                keys.Add(key);
            }

            var source = this.image;
            using (var roi = source.Lock(ResolutionHelper.CorrectRectangle(source.Size, rect, theResolution), PixelFormat.Format32bppRgb))
            {
                var hash = this.imageHasher.Create(roi.Data);
                var best = PerceptualHash.FindBest(hash, hashes);
                TraceLog.Log("Detected best hash: '{0}' Distance: {1}", debugkey, best.Distance);
                if (best.Distance <= threshold)
                {
                    return keys[best.Index];
                }
            }

            return null;
        }

        /// <summary>The detect.</summary>
        /// <param name="lookup">The lookup.</param>
        /// <param name="key">The key.</param>
        /// <param name="useArea"></param>
        /// <param name="i"></param>
        /// <returns>The <see cref="bool"/>.</returns>
        private DetectionResult Detect(ScanAreaDictionary lookup, string key, IDictionary<int, ScanArea> useArea = null, int threshold = -1)
        {
            threshold = threshold >= 0 ? threshold : ThreshHold;
            if (!lookup.ContainsKey(key))
            {
                Log.Error("No scan data found for requested template: {0}", key);
                return new DetectionResult(false, -1);
            }

            var template = lookup[key];
            // var rect = new Rectangle();
            int theResolution;
            // ulong matchhash;
            ScanArea area = null;
            if (template.ContainsKey(this.lastResolution))
            {
                // rect = template[this.lastResolution].Rect;
                // matchhash = template[this.lastResolution].Hash;
                area = template[this.lastResolution];
                theResolution = this.lastResolution;
            }
            else if (template.ContainsKey(this.BaseResolution))
            {
                // rect = template[this.BaseResolution].Rect;
                // matchhash = template[this.BaseResolution].Hash;
                area = template[this.BaseResolution];
                theResolution = this.BaseResolution;
            }
            else
            {
                Log.Error("No scan data found for requested template: " + key);
                return new DetectionResult(false, -1);
            }
            if (useArea != null)
            {
                if (useArea.ContainsKey(this.lastResolution))
                {
                    // rect = useArea[this.lastResolution].Rect;
                    area = useArea[this.lastResolution];
                    theResolution = this.lastResolution;
                }
                else if (useArea.ContainsKey(this.BaseResolution))
                {
                    // rect = useArea[this.BaseResolution].Rect;
                    area = useArea[this.BaseResolution];
                    theResolution = this.BaseResolution;
                }
                else
                {
                    Log.Error("No scan data found for requested template: " + key);
                    return new DetectionResult(false, -1);
                }
            }
            var source = this.image;
            int distance = -1;
            DetectionResult result = new DetectionResult();
            using (var roi = source.Lock(ResolutionHelper.CorrectRectangle(source.Size, area.Rect, theResolution), source.PixelFormat))
            {
                // var roi = source.Clone(ResolutionHelper.CorrectRectangle(source.Size, area.Rect, theResolution), source.PixelFormat);
                //Tuple<byte[], DetectionResult> cached = null;
                //byte[] roiBytes = roi.GetBytes();
                //if (hashCache.TryGetValue(key, out cached))
                //{
                //    if (ImageUtils.AreEqual(cached.Item1, roiBytes))
                //    {
                //        TraceLog.Log("hash cache hit: {0}, hit: {1} distance: {2}", key, cached.Item2.Found, cached.Item2.Distance);
                //        return cached.Item2;
                //    }
                //}

                var hash = this.imageHasher.Create(roi.Data);
                distance = PerceptualHash.HammingDistance(hash, area.Hash);
                TraceLog.Log("Detecting '{0}'. Distance: {1}", key, distance);

                if (distance <= threshold)
                {
                    TraceLog.Log("Detected '{0}'. Distance: {1}", key, distance);
                    Mostly mostly;
                    if (!String.IsNullOrEmpty(area.Mostly) && Enum.TryParse(area.Mostly, out mostly))
                    {
                        result = new DetectionResult(roi.Data.IsMostly(mostly), distance);
                    }
                    else
                    {
                        result = new DetectionResult(true, distance);
                    }
                }
                else
                {
                    result = new DetectionResult(false, distance);
                }
                // hashCache.Set(key, new Tuple<byte[], DetectionResult>(roiBytes, result));
            }
            return result;
        }

        private string DetectBest(ScanAreaImageDictionary lookup)
        {
            string bestKey = null;
            float best = 0;
            foreach (var key in lookup.Keys)
            {
                var detect = DetectTemplate(lookup, key);
                if (detect > best)
                {
                    best = detect;
                    bestKey = key;
                }
            }
            return bestKey;
        }

        private float DetectTemplate(ScanAreaImageDictionary lookup, string key)
        {
            if (!lookup.ContainsKey(key))
            {
                Log.Error("No scan data found for requested template: {0}", key);
                return -1;
            }

            var template = lookup[key];
            var rect = new Rectangle();
            int theResolution;
            Bitmap matchhash;
            ScanArea area = null;
            if (template.ContainsKey(this.lastResolution))
            {
                area = template[this.lastResolution].Item2;
                // rect = new Rectangle(area.X, area.Y, area.Width, area.Height);
                matchhash = template[this.lastResolution].Item1;
                theResolution = this.lastResolution;
            }
            else if (template.ContainsKey(this.BaseResolution))
            {
                area = template[this.BaseResolution].Item2;
                // rect = new Rectangle(area.X, area.Y, area.Width, area.Height);
                matchhash = template[this.BaseResolution].Item1;
                theResolution = this.BaseResolution;
            }
            else
            {
                Log.Error("No scan data found for requested template: " + key);
                return -1;
            }
            var source = this.image;
            rect = new Rectangle(area.X, area.Y, area.Width, area.Height);
            if (area.BaseResolution > 0)
            {
                // theResolution = area.BaseResolution;
            }
            var templatesize = new Size(matchhash.Width, matchhash.Height);
            var roiRect = ResolutionHelper.CorrectRectangle(source.Size, rect, theResolution);
            //var roiRect = ResolutionHelper.CorrectPoints(source.Size, rect, theResolution);
            templatesize = ResolutionHelper.CorrectSize(source.Size, templatesize, area.BaseResolution > 0 ? area.BaseResolution : theResolution);
            roiRect.Inflate(5, 5);
            // roiRect.X -= 5;
            // roiRect.Y -= 5;
            using (var roi = source.Clone(roiRect, PixelFormat.Format24bppRgb))
            {
                var newhash = new Bitmap(templatesize.Width, templatesize.Height, PixelFormat.Format24bppRgb);
                var graph = Graphics.FromImage(newhash);
                graph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graph.DrawImage(matchhash, 0, 0, templatesize.Width, templatesize.Height);
                var ismatch = templateMatcher.IsMatch(roi, newhash);
                graph.Dispose();
                newhash.Dispose();
                return ismatch;
            }
        }

        private void GameEnded()
        {
            if (this.inGameCounter < 2 ||
                this.hero == null ||
                this.opponentHero == null ||
                this.gameJustEnded)
            {
                return;
            }

            this.gameJustEnded = true;
            this.endTime = DateTime.Now;
            this.victory = this.foundVic > this.foundLoss;

            this.inGameCounter = 0;
            this.pauseScanning = true;
            this.Publish(
                new GameEnded()
                    {
                        GameMode = this.gameMode,
                        StartTime = this.startTime,
                        EndTime = this.endTime,
                        Victory = this.victory,
                        GoFirst = this.goFirst,
                        Hero = this.hero,
                        OpponentHero = this.opponentHero,
                        Turns = this.gameTurns,
                        Conceded = this.conceded,
                        Deck = this.lastDeck
                    });

            // This is done from the ui using an event
            // Task.Delay(2000).ContinueWith((task) => { this.Reset(); });
        }

        private void GameStarted()
        {
            if (this.gameStarted)
            {
                return;
            }

            if (this.inGameCounter >= 2 && this.startTime == DateTime.MinValue)
            {
                this.startTime = DateTime.Now;
            }

            if (this.hero != null &&
                this.opponentHero != null &&
                this.inGameCounter >= 2)
            {
                this.gameStarted = true;
                this.arenaHero = null;
                this.arenaWins = -1;
                this.arenaLosses = -1;
                this.Publish(new GameStarted(this.gameMode, this.startTime, this.hero, this.opponentHero, this.goFirst, this.lastDeck));
            }
        }

        private void InitAreas(string arrkey, string prefix, ScanAreas scanArea, ScanAreaDictionary lookup, KeyValuePair<string, ScanArea> a)
        {
            var key = arrkey.Substring(prefix.Length);
            if (!lookup.ContainsKey(key))
            {
                // lookup[key] = new Tuple<ulong, IDictionary<int, Rectangle>>(a.Value.Hash, new Dictionary<int, Rectangle>());
                lookup[key] = new Dictionary<int, ScanArea>();
            }

            var tmp = lookup[key];
            if (!tmp.ContainsKey(scanArea.BaseResolution))
            {
                tmp[scanArea.BaseResolution] = a.Value;
            }
        }

        private void InitImageAreas(string arrkey, string prefix, ScanAreas scanArea, ScanAreaImageDictionary lookup, KeyValuePair<string, ScanArea> a)
        {
            var key = arrkey.Substring(prefix.Length);
            if (!lookup.ContainsKey(key))
            {
                // lookup[key] = new Tuple<ulong, IDictionary<int, Rectangle>>(a.Value.Hash, new Dictionary<int, Rectangle>());
                lookup[key] = new Dictionary<int, Tuple<Bitmap, ScanArea>>();
            }

            var tmp = lookup[key];
            if (!tmp.ContainsKey(scanArea.BaseResolution))
            {
                // var image = (Bitmap)Image.FromFile(Path.Combine(BasePath, a.Value.Image));
                var image = (Bitmap)scanAreaProvider.GetImage(a.Value.Image);
                tmp[scanArea.BaseResolution] = new Tuple<Bitmap, ScanArea>(image, a.Value);
            }
        }

        private void LoadScanAreas()
        {
            this.areas = new ScanAreaDictionary();
            this.heroes = new ScanAreaDictionary();
            this.opponentHeroes = new ScanAreaDictionary();
            this.decks = new ScanAreaDictionary();
            this.arenaHeroes = new ScanAreaDictionary();
            this.arenaWinsLookup = new ScanAreaDictionary();
            this.arenaWinsLookup2 = new ScanAreaImageDictionary();
            this.arenaLossesLookup = new ScanAreaDictionary();

            this.scanAreaProvider.Load();
            this.scanareas = this.scanAreaProvider.GetScanAreas();

            foreach (var scanArea in this.scanareas)
            {
                var allareas = scanArea.Areas.ToDictionary(x => x.Key, x => x);
                foreach (var a in allareas)
                {
                    if (a.Key.StartsWith("hero_"))
                    {
                        this.InitAreas(a.Key, "hero_", scanArea, this.heroes, a);
                    }
                    else if (a.Key.StartsWith("opphero_"))
                    {
                        this.InitAreas(a.Key, "opphero_", scanArea, this.opponentHeroes, a);
                    }
                    else if (a.Key.StartsWith("deck_"))
                    {
                        this.InitAreas(a.Key, "deck_", scanArea, this.decks, a);
                    }
                    //else if (a.Key.StartsWith("arenahero_"))
                    //{
                    //    this.InitAreas(a.Key, "arenahero_", scanArea, this.arenaHeroes, a);
                    //}
                    else if (a.Key.StartsWith("arenawins_"))
                    {
                        this.InitAreas(a.Key, "arenawins_", scanArea, this.arenaWinsLookup, a);
                        this.InitImageAreas(a.Key, "arenawins_", scanArea, this.arenaWinsLookup2, a);
                    }
                    else if (a.Key.StartsWith("arenaloss_"))
                    {
                        this.InitAreas(a.Key, "arenaloss_", scanArea, this.arenaLossesLookup, a);
                    }
                    else if (a.Key.StartsWith("arena_hero_"))
                    {
                        this.InitAreas(a.Key, "arena_hero_", scanArea, this.arenaHeroes, a);
                    }
                    else
                    {
                        this.InitAreas(a.Key, string.Empty, scanArea, this.areas, a);
                    }
                }
            }
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

        private void Reset()
        {
            // this.lastGameMode = GameMode.Unknown;
            this.hero = null;
            this.opponentHero = null;
            this.goFirst = true;
            this.victory = null;
            // this.gameMode = GameMode.Unknown;
            this.startTime = DateTime.MinValue;
            this.gameStarted = false;
            this.inGameCounter = 0;
            this.gameTurns = 0;
            this.roundTurned = false;
            this.conceded = false;
            this.inMenu = false;
            this.myturn = false;
            this.endTime = DateTime.MinValue;
            // this.lastDeck = null;
            this.foundVic = 0;
            this.foundLoss = 0;
            this.arenaDrafting = false;
            this.arenaHero = null;
            this.arenaWins = -1;
            this.arenaLosses = -1;
            this.coinDetected = false;
            this.requestReset = false;
            this.foundUsing.Clear();
            ResetFoundVictory();
            // this.nothingFoundFired = false;
            // this.gameJustEnded = false; // NOTE: we reset this on entering a game-mode or the menu
        }

        private void Scan()
        {
            this.currentScan = new CurrentScanState();

            if (requestReset)
            {
                this.Reset();
            }

            // Scan these before game-modes, to catch victories earlier
            if (!this.gameJustEnded && !pauseScanning)
            {
                // Scan victory earlier
                if (this.inGameCounter >= 2)
                {
                    this.ScanVictory();
                }

                if (this.inGameCounter >= 1)
                {
                    this.ScanHeroes();
                }

                if (this.inGameCounter >= 1)
                {
                    this.ScanCoin();
                }

                if (this.inGameCounter >= 1)
                {
                    this.ScanTurn();
                    // TODO: enable again sometimee later
                    // this.ScanConceded();
                }
            }

            this.ScanGameModes();

            if (gameMode == GameMode.Unknown)
            {
                ScanDeckScreenshot();
            }

            if (gameMode == GameMode.Arena)
            {
                ScanArenaDeckScreenshot();
            }

            // Make sure this is AFTER ScanGameModes() !! 
            if (this.pauseScanning)
            {
                return;
            }

            if (this.currentScan.ArenaLeafDetected && this.inGameCounter <= 0)
            {
                if (!Detect(this.areas, "arenanohero"))
                {
                    this.ScanArenaHero();

                    if (this.arenaHero != null)
                    {
                        this.ScanArenaScore();
                    }
                }

                if (this.arenaHero == null)
                {
                    this.ScanArenaDrafting();
                }
            }
        }

        private void ScanArenaDeckScreenshot()
        {
            if (arenadeckScreenshotRequested && gameMode == GameMode.Arena)
            {
                arenadeckScreenshotRequested = false;
                var deckRect = this.areas["deckarea_arena"][BaseResolution].Rect;
                deckRect = ResolutionHelper.CorrectRectangle(this.image.Size, deckRect, BaseResolution);
                var deck = this.image.Clone(deckRect, this.image.PixelFormat);
                Log.Debug("Arena deck Screenshot Requested. Sending screenshot...");
                events.PublishOnBackgroundThread(new ArenaDeckScreenshotTaken(deck));
            }
        }

        private void ScanDeckScreenshot()
        {
            if (deckScreenshotRequestedCanceled)
            {
                Log.Debug("Deck Screenshot Requested Canceled.");
                if (cancelDeckScreenshot != null)
                {
                    cancelDeckScreenshot.Cancel();
                }
                deckScreenshotRequestedCanceled = false;
                deckScreenshotRequested = false;
                takeDeckScreenshot = false;
            }

            if (deckScreenshotRequested)
            {
                var detect1 = Detect(this.areas, "deckscreen");
                var detect2 = Detect(this.areas, "deckscreen2", null, 16);
                if (detect1 && detect2)
                {
                    this.deckScreenshotRequested = false;
                    if (cancelDeckScreenshot != null)
                    {
                        cancelDeckScreenshot.Cancel();
                        cancelDeckScreenshot.Dispose();
                    }
                    cancelDeckScreenshot = new CancellationTokenSource();
                    Log.Debug("Deck Screenshot Requested. Found!");

                    Task.Delay(500, this.cancelDeckScreenshot.Token).ContinueWith(
                        (t) =>
                        {
                            if (t.IsCanceled) return;
                            takeDeckScreenshot = true;
                        });
                }
                else
                {
                    Log.Debug("Deck Screenshot Requested. Not found. deckscreen1: {0}, deckscreen2: {1}", detect1.Distance, detect2.Distance);
                }
            }

            if (takeDeckScreenshot)
            {
                takeDeckScreenshot = false;
                var deckRect = this.areas["deckarea_cards"][BaseResolution].Rect;
                deckRect = ResolutionHelper.CorrectRectangle(this.image.Size, deckRect, BaseResolution);
                var deck = this.image.Clone(deckRect, this.image.PixelFormat);
                Log.Debug("Deck Screenshot Requested. Sending screenshot...");
                events.PublishOnBackgroundThread(new DeckScreenshotTaken(deck));
            }
        }

        private void ScanArenaDrafting()
        {
            if (!this.arenaDrafting)
            {
                if (this.Detect(this.areas, "arena_drafting"))
                {
                    this.arenaDrafting = true;
                    Publish(new ArenaDrafting());
                }
            }

            // TODO: enable again

            //if (this.arenaDrafting)
            //{
            //    this.ScanArenaDraftingCards();
            //}
        }

        private void ScanArenaDraftingCards()
        {
            // TODO: scan cards 
            // Detect 3 best cards (by hash) in a given set of card hashes using the 3 card rectangles.
            // 
        }

        private void ScanArenaHero()
        {
            if (this.arenaHero != null || this.arenaWins == 12 || this.arenaLosses == 3) return;

            var best = this.DetectBest(this.arenaHeroes, "arena_hero_*", ThreshHoldForHeroes);

            if (best == null)
            {
                return;
            }

            this.arenaHero = best;
            this.Publish(new ArenaHeroDetected(this.arenaHero));
            gameModeChangeActionQueue.Enqueue(() => { this.arenaHero = null; });

            //foreach (var k in this.heroes.Keys)
            //{
            //    if (this.Detect(this.heroes, k, herobox))
            //    {
            //        this.arenaHero = k;
            //        this.Publish(new ArenaHeroDetected(this.arenaHero));
            //        break;
            //    }
            //}
        }

        private void ScanArenaScore()
        {
            if (this.arenaWins < 0 && !arenaWinsScanning)
            {
                arenaWinsScanning = true;
                TraceLog.Log("delay scan score to let key animation finish");
                Task.Delay(1000).ContinueWith(
                    (t) =>
                        {
                            TraceLog.Log("delay scan score finished now.");
                            doArenaWinsScan = true;
                        });
                Task.Delay(5000).ContinueWith(
                    (t) =>
                        {
                            doArenaWinsScan = false;
                            arenaWinsScanning = false;
                        });
            }
            if (this.arenaWins < 0 && doArenaWinsScan)
            {
                var best = DetectBest(this.arenaWinsLookup2);
                if (best != null)
                {
                    doArenaWinsScan = false;
                    arenaWinsScanning = false;
                    int wins = Convert.ToInt32(best);
                    this.arenaWins = wins;
                    this.Publish(new ArenaWinsDetected(this.arenaWins));
                    gameModeChangeActionQueue.Enqueue(() => { this.arenaWins = -1; });
                }
            }
            if (this.arenaLosses < 0)
            {
                // var detected = false;
                foreach (var k in this.arenaLossesLookup.Keys.OrderByDescending(k => k))
                {
                    if (this.Detect(this.arenaLossesLookup, k))
                    {
                        int losses = Convert.ToInt32(k);
                        this.arenaLosses = losses;
                        this.Publish(new ArenaLossesDetected(this.arenaLosses));
                        gameModeChangeActionQueue.Enqueue(() => { this.arenaLosses = -1; });
                        break;
                    }
                }
                if (this.arenaLosses < 0)
                {
                    // TODO: refine this, as when we cannot detect 'checked' it could be a false detection
                    this.arenaLosses = 0;
                    this.Publish(new ArenaLossesDetected(this.arenaLosses));
                    gameModeChangeActionQueue.Enqueue(() => { this.arenaLosses = -1; });
                }
            }
        }

        private void ScanCoin()
        {
            if (this.coinDetected)
            {
                return;
            }

            if (this.gameTurns > 0)
            {
                return;
            }

            var detected = false;
            // this the THE THE coin !
            if (this.Detect(this.areas, "gosecond"))
            {
                detected = true;
            }

            if (!detected && this.Detect(this.areas, "gosecond2"))
            {
                detected = true;
            }

            if (detected)
            {
                this.coinDetected = true;
                this.goFirst = false;

                this.inGameCounter++;
                this.gameTurns++;
                this.Publish(new NewRound(this.gameTurns, false));
                this.Publish(new CoinDetected(false) { });
                this.GameStarted();
            }
        }

        private void ScanConceded()
        {
            if (this.conceded)
            {
                return;
            }

            if (this.Detect(this.areas, "conceded"))
            {
                this.conceded = true;
                this.victory = false;
                Log.Debug("concede detected, calling GameEnded()");
                this.Publish(new VictoryDetected(false, true));
                this.GameEnded();
            }
        }

        private void ScanDeck()
        {
            foreach (var deck in this.decks)
            {
                if (this.Detect(this.decks, deck.Key, null, 10))
                {
                    if (this.lastDeck == deck.Key)
                    {
                        return;
                    }

                    this.lastDeck = deck.Key;
                    this.Publish(new DeckDetected(deck.Key));
                }
            }
        }

        private void ScanGameModes()
        {
            var foundGameMode = this.gameMode;

            bool found = false,
                detectedGameBoard = false;

            if (!pauseScanning && (this.Detect(this.areas, "ingame") || this.Detect(this.areas, "ingame2")))
            {
                if (inGameCounter == 0 && !this.gameJustEnded)
                {
                    this.inGameCounter++;
                    Log.Debug("Detected gameboard");
                }
                detectedGameBoard = true;
                found = true;
            }

            if (!found && this.Detect(this.areas, "quest"))
            {
                foundGameMode = GameMode.Unknown;
                this.inMenu = true;
                found = true;
            }

            if (!found && Detect(this.areas, "play_mode") && Detect(this.areas, "play_casual"))
            {
                foundGameMode = GameMode.Casual;
                found = true;
            }

            if (!found && Detect(this.areas, "play_mode") && Detect(this.areas, "play_ranked"))
            {
                foundGameMode = GameMode.Ranked;
                found = true;
            }

            if (!found && this.Detect(this.areas, "practice"))
            {
                foundGameMode = GameMode.Practice;
                found = true;
            }

            if (!found && Detect(this.areas, "challenge") && Detect(this.areas, "challenge2"))
            {
                foundGameMode = GameMode.Challenge;
                found = true;
            }

            if (!found && (this.Detect(this.areas, "arena_leaf") || (Detect(this.areas, "arenanohero") && Detect(this.areas, "arena"))))
            {
                this.currentScan.ArenaLeafDetected = true;
                foundGameMode = GameMode.Arena;
                found = true;
            }

            // when in game, you always return to the last game-mode screen, anything else is a false-positive
            if (found && inGameCounter >= 2 && (lastGameMode != foundGameMode))
            {
                // log.Debug("gamemode detected ({0}), but ignoring (inGame && last != found).", foundGameMode);
                return;
            }

            if (found && !detectedGameBoard)
            {
                if (this.inGameCounter >= 2)
                {
                    Log.Debug("gamemode detected ({0}) and was in game, calling GameEnded()", foundGameMode);
                    Log.Debug("victory/loss information at this point: {0}", this.foundUsing);
                    this.GameEnded();
                }

                this.pauseScanning = false;
                this.gameJustEnded = false; // NOTE: this is to not trigger new game detection when a game just finished

                this.inGameCounter = 0;
                this.gameMode = foundGameMode;

                if (this.lastGameMode != this.gameMode)
                {
                    while (gameModeChangeActionQueue.Count > 0)
                    {
                        var action = gameModeChangeActionQueue.Dequeue();
                        action();
                    }

                    this.Publish(new GameModeChanged(this.lastGameMode, this.gameMode));
                    this.lastGameMode = this.gameMode;
                }

                if (this.inGameCounter <= 0 &&
                    (this.gameMode == GameMode.Casual
                    || this.gameMode == GameMode.Ranked
                    || this.gameMode == GameMode.Challenge
                    || this.gameMode == GameMode.Practice))
                {
                    this.ScanDeck();
                }
            }
        }

        private void ScanHeroes()
        {
            if (this.hero != null && this.opponentHero != null)
            {
                return;
            }

            if (this.hero == null)
            {
                var best = this.DetectBest(this.heroes, "hero_*");
                if (best != null)
                {
                    this.hero = best;
                    this.inGameCounter++;
                    this.Publish(new HeroDetected(this.hero));
                }
            }

            if (this.opponentHero == null)
            {
                var best = this.DetectBest(this.opponentHeroes, "opphero_*");
                if (best != null)
                {
                    this.opponentHero = best;
                    this.inGameCounter++;
                    this.Publish(new OpponentHeroDetected(this.opponentHero));
                }
            }

            // removed, we wait until first turn now (to catch the coin for the webapi)

            //if (this.hero != null || this.opponentHero != null)
            //{
            //    this.GameStarted();
            //}
        }

        private void ScanTurn()
        {
            if (roundTurned) return;

            if (Detect(this.areas, "yourturn") || Detect(this.areas, "yourturn2"))
            {
                if (this.gameTurns == 0)
                {
                    this.inGameCounter++;
                }

                if (myturn)
                {
                    // we missed enemy turn detection
                    this.myturn = false;
                    gameTurns++;
                }
                if (!myturn)
                {
                    // to avoid false positives, we reset the victory counter
                    ResetFoundVictory();

                    this.gameTurns++;
                    this.roundTurned = true;
                    this.myturn = true;
                    this.GameStarted();
                    Task.Delay(2000).ContinueWith(task => this.roundTurned = false);
                    this.Publish(new NewRound(this.gameTurns, true));
                }
            }
            else if (Detect(this.areas, "enemyturn") || Detect(this.areas, "enemyturn2"))
            {
                if (this.gameTurns == 0)
                {
                    this.inGameCounter++;
                }

                if (myturn)
                {
                    this.gameTurns++;
                    this.roundTurned = true;
                    this.myturn = false;
                    this.GameStarted();
                    Task.Delay(2000).ContinueWith(task => this.roundTurned = false);
                    this.Publish(new NewRound(this.gameTurns));
                }
            }
        }

        private void ResetFoundVictory()
        {
            Log.Debug("ResetFoundVictory called. Was: {0}", this.foundUsing);
            this.foundLoss = 0;
            this.foundVic = 0;
            this.foundUsing.Clear();
            this.lastLossDetect = null;
            this.lastVicDetect = null;
        }

        private void ScanVictory()
        {
            if (gameJustEnded || this.victory != null)
            {
                return;
            }

            if (this.Detect(this.areas, "victory"))
            {
                this.foundVic++;
                this.lastVicDetect = "victory";
                foundUsing.Append("victory|");
            }

            if (this.Detect(this.areas, "loss"))
            {
                this.foundLoss++;
                this.lastLossDetect = "loss";
                foundUsing.Append("loss|");
            }

            if (this.Detect(this.areas, "victory_explode"))
            {
                this.foundVic++;
                this.lastVicDetect = "victory_explode";
                foundUsing.Append("victory_explode|");
            }

            if (this.Detect(this.areas, "victory_explode2"))
            {
                this.foundVic++;
                this.lastVicDetect = "victory_explode2";
                foundUsing.Append("victory_explode2|");
            }

            if (this.Detect(this.areas, "victory_explode3"))
            {
                this.foundVic++;
                this.lastVicDetect = "victory_explode3";
                foundUsing.Append("victory_explode3|");
            }

            if (this.Detect(this.areas, "victory_explode4"))
            {
                this.foundVic++;
                this.lastVicDetect = "victory_explode4";
                foundUsing.Append("victory_explode4|");
            }

            if (this.Detect(this.areas, "victory_explode5"))
            {
                this.foundVic++;
                this.lastVicDetect = "victory_explode5";
                foundUsing.Append("victory_explode5|");
            }

            if (this.Detect(this.areas, "loss_explode"))
            {
                this.foundLoss++;
                this.lastLossDetect = "loss_explode";
                foundUsing.Append("loss_explode|");
            }

            if (this.Detect(this.areas, "loss_explode2"))
            {
                this.foundLoss++;
                this.lastLossDetect = "loss_explode2";
                foundUsing.Append("loss_explode2|");
            }

            if (this.Detect(this.areas, "victory2"))
            {
                this.foundVic++;
                this.lastVicDetect = "victory2";
                foundUsing.Append("victory2|");
            }

            if (this.Detect(this.areas, "victory3"))
            {
                this.foundVic++;
                this.lastVicDetect = "victory3";
                foundUsing.Append("victory3|");
            }

            if (this.Detect(this.areas, "loss2"))
            {
                this.foundLoss++;
                this.lastLossDetect = "loss2";
                foundUsing.Append("loss2|");
            }

            if (this.foundVic >= 3 || this.foundLoss >= 3)
            {
                this.victory = foundVic > foundLoss;
                this.Publish(new VictoryDetected(this.victory.Value));
                Log.Info("found victory/loss (debug info: {0})", this.foundUsing);
                this.foundUsing.Clear();
                this.GameEnded();
            }
        }

        #endregion

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(ResetCurrentGame message)
        {
            Log.Debug("ResetCurrentGame requested");
            this.requestReset = true;
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(RequestDeckScreenshot message)
        {
            this.deckScreenshotRequested = !message.Cancel;
            if (message.Cancel)
            {
                this.deckScreenshotRequestedCanceled = true;
            }
        }


        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(RequestArenaDeckScreenshot message)
        {
            this.arenadeckScreenshotRequested = !message.Cancel;
        }
    }

    public class LeastRecentlyUsedCache<TKey, TValue>
    {
        private readonly Dictionary<TKey, Node> entries;
        private readonly int capacity;
        private Node head;
        private Node tail;

        private class Node
        {
            public Node Next { get; set; }
            public Node Previous { get; set; }
            public TKey Key { get; set; }
            public TValue Value { get; set; }
        }

        public LeastRecentlyUsedCache(int capacity = 16)
        {
            if (capacity <= 0)
                throw new ArgumentOutOfRangeException(
                    "capacity",
                    "Capacity should be greater than zero");
            this.capacity = capacity;
            entries = new Dictionary<TKey, Node>();
            head = null;
        }

        public void Set(TKey key, TValue value)
        {
            Node entry;
            if (!entries.TryGetValue(key, out entry))
            {
                entry = new Node { Key = key, Value = value };
                if (entries.Count == capacity)
                {
                    entries.Remove(tail.Key);
                    tail = tail.Previous;
                    if (tail != null) tail.Next = null;
                }
                entries.Add(key, entry);
            }

            entry.Value = value;
            MoveToHead(entry);
            if (tail == null) tail = head;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            value = default(TValue);
            Node entry;
            if (!entries.TryGetValue(key, out entry)) return false;
            MoveToHead(entry);
            value = entry.Value;
            return true;
        }

        private void MoveToHead(Node entry)
        {
            if (entry == head || entry == null) return;

            var next = entry.Next;
            var previous = entry.Previous;

            if (next != null) next.Previous = entry.Previous;
            if (previous != null) previous.Next = entry.Next;

            entry.Previous = null;
            entry.Next = head;

            if (head != null) head.Previous = entry;
            head = entry;

            if (tail == entry) tail = previous;
        }
    }
}
