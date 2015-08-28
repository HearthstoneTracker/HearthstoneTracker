namespace HearthCap.Core.GameCapture.HS
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.Linq;
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

    using Action = System.Action;
    using LogManager = NLog.LogManager;

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
                Found = found;
                Distance = distance;
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

        private int gameTurns;

        private bool goFirst = true;

        private string hero;

        private ScanAreaDictionary heroes;

        private Bitmap image;

        private IPerceptualHash imageHasher;

        private readonly ITemplateMatcher templateMatcher;

        private int inGameCounter;

        private string lastDeck;

        private GameMode lastGameMode;

        private int lastResolution = 900;

        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
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

        private Queue<Action> gameModeChangeActionQueue = new Queue<Action>();

        private bool requestReset;

        private bool pauseScanning;

        private int foundVic;

        private int foundLoss;

        private StringBuilder foundUsing = new StringBuilder();

        private bool arenaWinsScanning;

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
            ThreshHold = 9;
            ThreshHoldForHeroes = 12;
            BaseResolution = 900;

            // TODO: fix, this is hacky
            BasePath = AppDomain.CurrentDomain.BaseDirectory;
            LoadScanAreas();
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
            image = img;
            if (lastResolution != img.Height)
            {
                Log.Info("Base resolution changed from {0} to {1}.", lastResolution, img.Height);
                lastResolution = img.Height;
                LoadScanAreas();
            }

            Scan();

            image = null;
        }

        public void Stop(object context)
        {
            Reset();
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
                if (template.ContainsKey(lastResolution))
                {
                    matchhash = template[lastResolution].Hash;
                    rect = template[lastResolution].Rectangle;
                    theResolution = lastResolution;
                }
                else if (template.ContainsKey(BaseResolution))
                {
                    matchhash = template[BaseResolution].Hash;
                    rect = template[BaseResolution].Rectangle;
                    theResolution = BaseResolution;
                }
                else
                {
                    Log.Error("No scan data found for requested template: " + debugkey);
                    return null;
                }
                hashes.Add(matchhash);
                keys.Add(key);
            }

            var source = image;
            using (var roi = source.Lock(ResolutionHelper.CorrectRectangle(source.Size, rect, theResolution), PixelFormat.Format32bppRgb))
            {
                var hash = imageHasher.Create(roi.Data);
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
            if (template.ContainsKey(lastResolution))
            {
                // rect = template[this.lastResolution].Rect;
                // matchhash = template[this.lastResolution].Hash;
                area = template[lastResolution];
                theResolution = lastResolution;
            }
            else if (template.ContainsKey(BaseResolution))
            {
                // rect = template[this.BaseResolution].Rect;
                // matchhash = template[this.BaseResolution].Hash;
                area = template[BaseResolution];
                theResolution = BaseResolution;
            }
            else
            {
                Log.Error("No scan data found for requested template: " + key);
                return new DetectionResult(false, -1);
            }
            if (useArea != null)
            {
                if (useArea.ContainsKey(lastResolution))
                {
                    // rect = useArea[this.lastResolution].Rect;
                    area = useArea[lastResolution];
                    theResolution = lastResolution;
                }
                else if (useArea.ContainsKey(BaseResolution))
                {
                    // rect = useArea[this.BaseResolution].Rect;
                    area = useArea[BaseResolution];
                    theResolution = BaseResolution;
                }
                else
                {
                    Log.Error("No scan data found for requested template: " + key);
                    return new DetectionResult(false, -1);
                }
            }
            var source = image;
            int distance = -1;
            DetectionResult result = new DetectionResult();
            using (var roi = source.Lock(ResolutionHelper.CorrectRectangle(source.Size, area.Rectangle, theResolution), source.PixelFormat))
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

                var hash = imageHasher.Create(roi.Data);
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
            if (template.ContainsKey(lastResolution))
            {
                area = template[lastResolution].Item2;
                // rect = new Rectangle(area.X, area.Y, area.Width, area.Height);
                matchhash = template[lastResolution].Item1;
                theResolution = lastResolution;
            }
            else if (template.ContainsKey(BaseResolution))
            {
                area = template[BaseResolution].Item2;
                // rect = new Rectangle(area.X, area.Y, area.Width, area.Height);
                matchhash = template[BaseResolution].Item1;
                theResolution = BaseResolution;
            }
            else
            {
                Log.Error("No scan data found for requested template: " + key);
                return -1;
            }
            var source = image;
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
            if (inGameCounter < 2 ||
                hero == null ||
                opponentHero == null ||
                gameJustEnded)
            {
                return;
            }

            gameJustEnded = true;
            endTime = DateTime.Now;
            victory = foundVic > foundLoss;

            inGameCounter = 0;
            pauseScanning = true;
            Publish(
                new GameEnded
                    {
                        GameMode = gameMode,
                        StartTime = startTime,
                        EndTime = endTime,
                        Victory = victory,
                        GoFirst = goFirst,
                        Hero = hero,
                        OpponentHero = opponentHero,
                        Turns = gameTurns,
                        Conceded = conceded,
                        Deck = lastDeck
                    });

            // This is done from the ui using an event
            // Task.Delay(2000).ContinueWith((task) => { this.Reset(); });
        }

        private void GameStarted()
        {
            if (gameStarted)
            {
                return;
            }

            if (inGameCounter >= 2 && startTime == DateTime.MinValue)
            {
                startTime = DateTime.Now;
            }

            if (hero != null &&
                opponentHero != null &&
                inGameCounter >= 2)
            {
                gameStarted = true;
                arenaHero = null;
                arenaWins = -1;
                arenaLosses = -1;
                Publish(new GameStarted(gameMode, startTime, hero, opponentHero, goFirst, lastDeck));
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
            areas = new ScanAreaDictionary();
            heroes = new ScanAreaDictionary();
            opponentHeroes = new ScanAreaDictionary();
            decks = new ScanAreaDictionary();
            arenaHeroes = new ScanAreaDictionary();
            arenaWinsLookup = new ScanAreaDictionary();
            arenaWinsLookup2 = new ScanAreaImageDictionary();
            arenaLossesLookup = new ScanAreaDictionary();

            scanAreaProvider.Load();
            scanareas = scanAreaProvider.GetScanAreas();

            foreach (var scanArea in scanareas)
            {
                var allareas = scanArea.Areas.ToDictionary(x => x.Key, x => x);
                foreach (var a in allareas)
                {
                    if (a.Key.StartsWith("hero_"))
                    {
                        InitAreas(a.Key, "hero_", scanArea, heroes, a);
                    }
                    else if (a.Key.StartsWith("opphero_"))
                    {
                        InitAreas(a.Key, "opphero_", scanArea, opponentHeroes, a);
                    }
                    else if (a.Key.StartsWith("deck_"))
                    {
                        InitAreas(a.Key, "deck_", scanArea, decks, a);
                    }
                    //else if (a.Key.StartsWith("arenahero_"))
                    //{
                    //    this.InitAreas(a.Key, "arenahero_", scanArea, this.arenaHeroes, a);
                    //}
                    else if (a.Key.StartsWith("arenawins_"))
                    {
                        InitAreas(a.Key, "arenawins_", scanArea, arenaWinsLookup, a);
                        InitImageAreas(a.Key, "arenawins_", scanArea, arenaWinsLookup2, a);
                    }
                    else if (a.Key.StartsWith("arenaloss_"))
                    {
                        InitAreas(a.Key, "arenaloss_", scanArea, arenaLossesLookup, a);
                    }
                    else if (a.Key.StartsWith("arena_hero_"))
                    {
                        InitAreas(a.Key, "arena_hero_", scanArea, arenaHeroes, a);
                    }
                    else
                    {
                        InitAreas(a.Key, string.Empty, scanArea, areas, a);
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
            events.PublishOnBackgroundThread(message);
        }

        private void Reset()
        {
            // this.lastGameMode = GameMode.Unknown;
            hero = null;
            opponentHero = null;
            goFirst = true;
            victory = null;
            // this.gameMode = GameMode.Unknown;
            startTime = DateTime.MinValue;
            gameStarted = false;
            inGameCounter = 0;
            gameTurns = 0;
            roundTurned = false;
            conceded = false;
            myturn = false;
            endTime = DateTime.MinValue;
            // this.lastDeck = null;
            foundVic = 0;
            foundLoss = 0;
            arenaDrafting = false;
            arenaHero = null;
            arenaWins = -1;
            arenaLosses = -1;
            coinDetected = false;
            requestReset = false;
            foundUsing.Clear();
            ResetFoundVictory();
            // this.nothingFoundFired = false;
            // this.gameJustEnded = false; // NOTE: we reset this on entering a game-mode or the menu
        }

        private void Scan()
        {
            currentScan = new CurrentScanState();

            if (requestReset)
            {
                Reset();
            }

            // Scan these before game-modes, to catch victories earlier
            if (!gameJustEnded && !pauseScanning)
            {
                // Scan victory earlier
                if (inGameCounter >= 2)
                {
                    ScanVictory();
                }

                if (inGameCounter >= 1)
                {
                    ScanHeroes();
                }

                if (inGameCounter >= 1)
                {
                    ScanCoin();
                }

                if (inGameCounter >= 1)
                {
                    ScanTurn();
                    // TODO: enable again sometimee later
                    // this.ScanConceded();
                }
            }

            ScanGameModes();

            if (gameMode == GameMode.Unknown)
            {
                ScanDeckScreenshot();
            }

            if (gameMode == GameMode.Arena)
            {
                ScanArenaDeckScreenshot();
            }

            // Make sure this is AFTER ScanGameModes() !! 
            if (pauseScanning)
            {
                return;
            }

            if (currentScan.ArenaLeafDetected && inGameCounter <= 0)
            {
                if (!Detect(areas, "arenanohero"))
                {
                    ScanArenaHero();

                    if (arenaHero != null)
                    {
                        ScanArenaScore();
                    }
                }

                if (arenaHero == null)
                {
                    ScanArenaDrafting();
                }
            }
        }

        private void ScanArenaDeckScreenshot()
        {
            if (arenadeckScreenshotRequested && gameMode == GameMode.Arena)
            {
                arenadeckScreenshotRequested = false;
                var deckRect = areas["deckarea_arena"][BaseResolution].Rectangle;
                deckRect = ResolutionHelper.CorrectRectangle(image.Size, deckRect, BaseResolution);
                var deck = image.Clone(deckRect, image.PixelFormat);
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
                var detect1 = Detect(areas, "deckscreen");
                var detect2 = Detect(areas, "deckscreen2");
                if (detect1 && detect2)
                {
                    deckScreenshotRequested = false;
                    if (cancelDeckScreenshot != null)
                    {
                        cancelDeckScreenshot.Cancel();
                        cancelDeckScreenshot.Dispose();
                    }
                    cancelDeckScreenshot = new CancellationTokenSource();
                    Log.Debug("Deck Screenshot Requested. Found!");

                    Task.Delay(500, cancelDeckScreenshot.Token).ContinueWith(
                        t =>
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
                var deckRect = areas["deckarea_cards"][BaseResolution].Rectangle;
                deckRect = ResolutionHelper.CorrectRectangle(image.Size, deckRect, BaseResolution);
                var deck = image.Clone(deckRect, image.PixelFormat);
                Log.Debug("Deck Screenshot Requested. Sending screenshot...");
                events.PublishOnBackgroundThread(new DeckScreenshotTaken(deck));
            }
        }

        private void ScanArenaDrafting()
        {
            if (!arenaDrafting)
            {
                if (Detect(areas, "arena_drafting"))
                {
                    arenaDrafting = true;
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
            if (arenaHero != null || arenaWins == 12 || arenaLosses == 3) return;

            var best = DetectBest(arenaHeroes, "arena_hero_*", ThreshHoldForHeroes);

            if (best == null)
            {
                return;
            }

            arenaHero = best;
            Publish(new ArenaHeroDetected(arenaHero));
            gameModeChangeActionQueue.Enqueue(() => { arenaHero = null; });

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
            if (arenaWins < 0 && !arenaWinsScanning)
            {
                arenaWinsScanning = true;
                TraceLog.Log("delay scan score to let key animation finish");
                Task.Delay(1000).ContinueWith(
                    t =>
                        {
                            TraceLog.Log("delay scan score finished now.");
                            doArenaWinsScan = true;
                        });
                Task.Delay(5000).ContinueWith(
                    t =>
                        {
                            doArenaWinsScan = false;
                            arenaWinsScanning = false;
                        });
            }
            if (arenaWins < 0 && doArenaWinsScan)
            {
                var best = DetectBest(arenaWinsLookup2);
                if (best != null)
                {
                    doArenaWinsScan = false;
                    arenaWinsScanning = false;
                    int wins = Convert.ToInt32(best);
                    arenaWins = wins;
                    Publish(new ArenaWinsDetected(arenaWins));
                    gameModeChangeActionQueue.Enqueue(() => { arenaWins = -1; });
                }
            }
            if (arenaLosses < 0)
            {
                // var detected = false;
                foreach (var k in arenaLossesLookup.Keys.OrderByDescending(k => k))
                {
                    if (Detect(arenaLossesLookup, k))
                    {
                        int losses = Convert.ToInt32(k);
                        arenaLosses = losses;
                        Publish(new ArenaLossesDetected(arenaLosses));
                        gameModeChangeActionQueue.Enqueue(() => { arenaLosses = -1; });
                        break;
                    }
                }
                if (arenaLosses < 0)
                {
                    // TODO: refine this, as when we cannot detect 'checked' it could be a false detection
                    arenaLosses = 0;
                    Publish(new ArenaLossesDetected(arenaLosses));
                    gameModeChangeActionQueue.Enqueue(() => { arenaLosses = -1; });
                }
            }
        }

        private void ScanCoin()
        {
            if (coinDetected)
            {
                return;
            }

            if (gameTurns > 0)
            {
                return;
            }

            var detected = false;
            // this the THE THE coin !
            if (Detect(areas, "gosecond"))
            {
                detected = true;
            }

            if (!detected && Detect(areas, "gosecond2"))
            {
                detected = true;
            }

            if (detected)
            {
                coinDetected = true;
                goFirst = false;

                inGameCounter++;
                gameTurns++;
                Publish(new NewRound(gameTurns, false));
                Publish(new CoinDetected(false));
                GameStarted();
            }
        }

        private void ScanConceded()
        {
            if (conceded)
            {
                return;
            }

            if (Detect(areas, "conceded"))
            {
                conceded = true;
                victory = false;
                Log.Debug("concede detected, calling GameEnded()");
                Publish(new VictoryDetected(false, true));
                GameEnded();
            }
        }

        private void ScanDeck()
        {
            foreach (var deck in decks)
            {
                if (Detect(decks, deck.Key, null, 10))
                {
                    if (lastDeck == deck.Key)
                    {
                        return;
                    }

                    lastDeck = deck.Key;
                    Publish(new DeckDetected(deck.Key));
                }
            }
        }

        private void ScanGameModes()
        {
            var foundGameMode = gameMode;

            bool found = false,
                detectedGameBoard = false;

            if (!pauseScanning && (Detect(areas, "ingame") || Detect(areas, "ingame2")))
            {
                if (inGameCounter == 0 && !gameJustEnded)
                {
                    inGameCounter++;
                    Log.Debug("Detected gameboard");
                }
                detectedGameBoard = true;
                found = true;
            }

            if (!found && Detect(areas, "quest"))
            {
                foundGameMode = GameMode.Unknown;
                found = true;
            }

            if (!found && Detect(areas, "play_mode") && Detect(areas, "play_casual"))
            {
                foundGameMode = GameMode.Casual;
                found = true;
            }

            if (!found && Detect(areas, "play_mode") && Detect(areas, "play_ranked"))
            {
                foundGameMode = GameMode.Ranked;
                found = true;
            }

            if (!found && Detect(areas, "practice"))
            {
                foundGameMode = GameMode.Practice;
                found = true;
            }

            if (!found && Detect(areas, "challenge") && Detect(areas, "challenge2"))
            {
                foundGameMode = GameMode.Challenge;
                found = true;
            }

            if (!found && (Detect(areas, "arena_leaf") || (Detect(areas, "arenanohero") && Detect(areas, "arena"))))
            {
                currentScan.ArenaLeafDetected = true;
                foundGameMode = GameMode.Arena;
                found = true;
            }

            if (!found && Detect(areas, "mode_brawl"))
            {
                foundGameMode = GameMode.TavernBrawl;
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
                if (inGameCounter >= 2)
                {
                    Log.Debug("gamemode detected ({0}) and was in game, calling GameEnded()", foundGameMode);
                    Log.Debug("victory/loss information at this point: {0}", foundUsing);
                    GameEnded();
                }

                pauseScanning = false;
                gameJustEnded = false; // NOTE: this is to not trigger new game detection when a game just finished

                inGameCounter = 0;
                gameMode = foundGameMode;

                if (lastGameMode != gameMode)
                {
                    while (gameModeChangeActionQueue.Count > 0)
                    {
                        var action = gameModeChangeActionQueue.Dequeue();
                        action();
                    }

                    Publish(new GameModeChanged(lastGameMode, gameMode));
                    lastGameMode = gameMode;
                }

                if (inGameCounter <= 0 &&
                    (gameMode == GameMode.Casual
                    || gameMode == GameMode.Ranked
                    || gameMode == GameMode.Challenge
                    || gameMode == GameMode.Practice))
                {
                    ScanDeck();
                }
            }
        }

        private void ScanHeroes()
        {
            if (hero != null && opponentHero != null)
            {
                return;
            }

            if (hero == null)
            {
                var best = DetectBest(heroes, "hero_*");
                if (best != null)
                {
                    hero = best;
                    inGameCounter++;
                    Publish(new HeroDetected(hero));
                }
            }

            if (opponentHero == null)
            {
                var best = DetectBest(opponentHeroes, "opphero_*");
                if (best != null)
                {
                    opponentHero = best;
                    inGameCounter++;
                    Publish(new OpponentHeroDetected(opponentHero));
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

            if (Detect(areas, "yourturn") || Detect(areas, "yourturn2"))
            {
                if (gameTurns == 0)
                {
                    inGameCounter++;
                }

                if (myturn)
                {
                    // we missed enemy turn detection
                    myturn = false;
                    gameTurns++;
                }
                if (!myturn)
                {
                    // to avoid false positives, we reset the victory counter
                    ResetFoundVictory();

                    gameTurns++;
                    roundTurned = true;
                    myturn = true;
                    GameStarted();
                    Task.Delay(2000).ContinueWith(task => roundTurned = false);
                    Publish(new NewRound(gameTurns, true));
                }
            }
            else if (Detect(areas, "enemyturn") || Detect(areas, "enemyturn2"))
            {
                if (gameTurns == 0)
                {
                    inGameCounter++;
                }

                if (myturn)
                {
                    gameTurns++;
                    roundTurned = true;
                    myturn = false;
                    GameStarted();
                    Task.Delay(2000).ContinueWith(task => roundTurned = false);
                    Publish(new NewRound(gameTurns));
                }
            }
        }

        private void ResetFoundVictory()
        {
            Log.Debug("ResetFoundVictory called. Was: {0}", foundUsing);
            foundLoss = 0;
            foundVic = 0;
            foundUsing.Clear();           
        }

        private void ScanVictory()
        {
            if (gameJustEnded || victory != null)
            {
                return;
            }

            if (Detect(areas, "victory"))
            {
                foundVic++;
                foundUsing.Append("victory|");
            }

            if (Detect(areas, "loss"))
            {
                foundLoss++;
                foundUsing.Append("loss|");
            }

            if (Detect(areas, "victory_explode"))
            {
                foundVic++;
                foundUsing.Append("victory_explode|");
            }

            if (Detect(areas, "victory_explode2"))
            {
                foundVic++;
                foundUsing.Append("victory_explode2|");
            }

            if (Detect(areas, "victory_explode3"))
            {
                foundVic++;
                foundUsing.Append("victory_explode3|");
            }

            if (Detect(areas, "victory_explode4"))
            {
                foundVic++;
                foundUsing.Append("victory_explode4|");
            }

            if (Detect(areas, "victory_explode5"))
            {
                foundVic++;
                foundUsing.Append("victory_explode5|");
            }

            if (Detect(areas, "loss_explode"))
            {
                foundLoss++;
                foundUsing.Append("loss_explode|");
            }

            if (Detect(areas, "loss_explode2"))
            {
                foundLoss++;
                foundUsing.Append("loss_explode2|");
            }

            if (Detect(areas, "victory2"))
            {
                foundVic++;
                foundUsing.Append("victory2|");
            }

            if (Detect(areas, "victory3"))
            {
                foundVic++;
                foundUsing.Append("victory3|");
            }

            if (Detect(areas, "loss2"))
            {
                foundLoss++;
                foundUsing.Append("loss2|");
            }

            if (foundVic >= 3 || foundLoss >= 3)
            {
                victory = foundVic > foundLoss;
                Publish(new VictoryDetected(victory.Value));
                Log.Info("found victory/loss (debug info: {0})", foundUsing);
                foundUsing.Clear();
                GameEnded();
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
            requestReset = true;
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(RequestDeckScreenshot message)
        {
            deckScreenshotRequested = !message.Cancel;
            if (message.Cancel)
            {
                deckScreenshotRequestedCanceled = true;
            }
        }


        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(RequestArenaDeckScreenshot message)
        {
            arenadeckScreenshotRequested = !message.Cancel;
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