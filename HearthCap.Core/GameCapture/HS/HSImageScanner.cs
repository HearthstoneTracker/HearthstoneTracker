// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HSImageScanner.cs" company="">
//   
// </copyright>
// <summary>
//   The hs image scanner.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

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

    /// <summary>
    /// The hs image scanner.
    /// </summary>
    [Export(typeof(IImageScanner))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class HSImageScanner : IImageScanner, 
        IHandle<ResetCurrentGame>, 
        IHandle<RequestDeckScreenshot>, 
        IHandle<RequestArenaDeckScreenshot>
    {
        /// <summary>
        /// The current scan state.
        /// </summary>
        private class CurrentScanState
        {
            /// <summary>
            /// Gets or sets a value indicating whether arena leaf detected.
            /// </summary>
            public bool ArenaLeafDetected { get; set; }
        }

        /// <summary>
        /// The detection result.
        /// </summary>
        private struct DetectionResult
        {
            /// <summary>
            /// Gets a value indicating whether found.
            /// </summary>
            public bool Found { get; private set; }

            /// <summary>
            /// Gets the distance.
            /// </summary>
            public int Distance { get; private set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="DetectionResult"/> struct.
            /// </summary>
            /// <param name="found">
            /// The found.
            /// </param>
            /// <param name="distance">
            /// The distance.
            /// </param>
            public DetectionResult(bool found, int distance)
                : this()
            {
                this.Found = found;
                this.Distance = distance;
            }

            /// <summary>
            /// The op_ implicit.
            /// </summary>
            /// <param name="instance">
            /// The instance.
            /// </param>
            /// <returns>
            /// </returns>
            public static implicit operator bool(DetectionResult instance)
            {
                return instance.Found;
            }
        }

        #region Fields

        // private LeastRecentlyUsedCache<string, Tuple<byte[], DetectionResult>> hashCache = new LeastRecentlyUsedCache<string, Tuple<byte[], DetectionResult>>(50);

        /// <summary>
        /// The events.
        /// </summary>
        private readonly IEventAggregator events;

        /// <summary>
        /// The scan area provider.
        /// </summary>
        private readonly IScanAreaProvider scanAreaProvider;

        /// <summary>
        /// The areas.
        /// </summary>
        private ScanAreaDictionary areas;

        /// <summary>
        /// The arena drafting.
        /// </summary>
        private bool arenaDrafting;

        /// <summary>
        /// The arena heroes.
        /// </summary>
        private ScanAreaDictionary arenaHeroes;

        /// <summary>
        /// The coin detected.
        /// </summary>
        private bool coinDetected;

        /// <summary>
        /// The conceded.
        /// </summary>
        private bool conceded;

        /// <summary>
        /// The decks.
        /// </summary>
        private ScanAreaDictionary decks;

        /// <summary>
        /// The end time.
        /// </summary>
        private DateTime endTime = DateTime.MinValue;

        /// <summary>
        /// The game just ended.
        /// </summary>
        private bool gameJustEnded;

        /// <summary>
        /// The game mode.
        /// </summary>
        private GameMode gameMode;

        /// <summary>
        /// The game started.
        /// </summary>
        private bool gameStarted;

        /// <summary>
        /// The game turns.
        /// </summary>
        private int gameTurns;

        /// <summary>
        /// The go first.
        /// </summary>
        private bool goFirst = true;

        /// <summary>
        /// The hero.
        /// </summary>
        private string hero;

        /// <summary>
        /// The heroes.
        /// </summary>
        private ScanAreaDictionary heroes;

        /// <summary>
        /// The image.
        /// </summary>
        private Bitmap image;

        /// <summary>
        /// The image hasher.
        /// </summary>
        private IPerceptualHash imageHasher;

        /// <summary>
        /// The template matcher.
        /// </summary>
        private readonly ITemplateMatcher templateMatcher;

        /// <summary>
        /// The in game counter.
        /// </summary>
        private int inGameCounter;

        /// <summary>
        /// The in menu.
        /// </summary>
        private bool inMenu;

        /// <summary>
        /// The last deck.
        /// </summary>
        private string lastDeck;

        /// <summary>
        /// The last game mode.
        /// </summary>
        private GameMode lastGameMode;

        /// <summary>
        /// The last resolution.
        /// </summary>
        private int lastResolution = 900;

        /// <summary>
        /// The log.
        /// </summary>
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The trace log.
        /// </summary>
        private static readonly TraceLogger TraceLog = new TraceLogger(Log);

        /// <summary>
        /// The myturn.
        /// </summary>
        private bool myturn;

        /// <summary>
        /// The opponent hero.
        /// </summary>
        private string opponentHero;

        /// <summary>
        /// The opponent heroes.
        /// </summary>
        private ScanAreaDictionary opponentHeroes;

        /// <summary>
        /// The round turned.
        /// </summary>
        private bool roundTurned;

        /// <summary>
        /// The scanareas.
        /// </summary>
        private IEnumerable<ScanAreas> scanareas;

        /// <summary>
        /// The start time.
        /// </summary>
        private DateTime startTime = DateTime.MinValue;

        /// <summary>
        /// The victory.
        /// </summary>
        private bool? victory;

        /// <summary>
        /// The arena hero.
        /// </summary>
        private string arenaHero;

        /// <summary>
        /// The arena wins lookup.
        /// </summary>
        private ScanAreaDictionary arenaWinsLookup;

        /// <summary>
        /// The arena losses lookup.
        /// </summary>
        private ScanAreaDictionary arenaLossesLookup;

        /// <summary>
        /// The arena wins.
        /// </summary>
        private int arenaWins = -1;

        /// <summary>
        /// The arena losses.
        /// </summary>
        private int arenaLosses = -1;

        /// <summary>
        /// The arena wins lookup 2.
        /// </summary>
        private ScanAreaImageDictionary arenaWinsLookup2;

        /// <summary>
        /// The game mode change action queue.
        /// </summary>
        private Queue<Action> gameModeChangeActionQueue = new Queue<Action>();

        /// <summary>
        /// The request reset.
        /// </summary>
        private bool requestReset;

        /// <summary>
        /// The pause scanning.
        /// </summary>
        private bool pauseScanning;

        /// <summary>
        /// The found vic.
        /// </summary>
        private int foundVic;

        /// <summary>
        /// The found loss.
        /// </summary>
        private int foundLoss;

        /// <summary>
        /// The found using.
        /// </summary>
        private StringBuilder foundUsing = new StringBuilder();

        /// <summary>
        /// The arena wins scanning.
        /// </summary>
        private bool arenaWinsScanning;

        /// <summary>
        /// The last vic detect.
        /// </summary>
        private string lastVicDetect;

        /// <summary>
        /// The last loss detect.
        /// </summary>
        private string lastLossDetect;

        /// <summary>
        /// The current scan.
        /// </summary>
        private CurrentScanState currentScan;

        /// <summary>
        /// The deck screenshot requested.
        /// </summary>
        private bool deckScreenshotRequested;

        /// <summary>
        /// The take deck screenshot.
        /// </summary>
        private bool takeDeckScreenshot;

        /// <summary>
        /// The deck screenshot requested canceled.
        /// </summary>
        private bool deckScreenshotRequestedCanceled;

        /// <summary>
        /// The cancel deck screenshot.
        /// </summary>
        private CancellationTokenSource cancelDeckScreenshot;

        /// <summary>
        /// The arenadeck screenshot requested.
        /// </summary>
        private bool arenadeckScreenshotRequested;

        /// <summary>
        /// The do arena wins scan.
        /// </summary>
        private bool doArenaWinsScan;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HSImageScanner"/> class.
        /// </summary>
        /// <param name="events">
        /// The events.
        /// </param>
        /// <param name="scanAreaProvider">
        /// The scan area provider.
        /// </param>
        /// <param name="imageHasher">
        /// The image hasher.
        /// </param>
        /// <param name="templateMatcher">
        /// The template matcher.
        /// </param>
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
            this.BasePath = AppDomain.CurrentDomain.BaseDirectory;
            this.LoadScanAreas();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the base path.
        /// </summary>
        public string BasePath { get; set; }

        /// <summary>
        /// Gets or sets the base resolution.
        /// </summary>
        public int BaseResolution { get; set; }

        /// <summary>
        /// Gets or sets the offet override y.
        /// </summary>
        public int OffetOverrideY { get; set; }

        /// <summary>
        /// Gets or sets the offset override x.
        /// </summary>
        public int OffsetOverrideX { get; set; }

        /// <summary>
        /// Gets or sets the thresh hold.
        /// </summary>
        public int ThreshHold { get; set; }

        /// <summary>
        /// Gets or sets the thresh hold for heroes.
        /// </summary>
        public int ThreshHoldForHeroes { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The run.
        /// </summary>
        /// <param name="img">
        /// The img.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
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

        /// <summary>
        /// The stop.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public void Stop(object context)
        {
            this.Reset();
        }

        #endregion

        #region Methods

        // private string DetectBest(ScanAreaDictionary lookup, IDictionary<int, Tuple<ulong, Rectangle>> useArea, string debugkey)
        // {
        // var hashes = new List<ulong>();
        // var keys = new List<string>();

        // var rect = new Rectangle();
        // int theResolution;
        // foreach (var key in lookup.Keys)
        // {
        // var template = lookup[key];
        // ulong matchhash;
        // if (template.ContainsKey(this.lastResolution))
        // {
        // matchhash = template[this.lastResolution].Hash;
        // }
        // else if (template.ContainsKey(this.BaseResolution))
        // {
        // matchhash = template[this.BaseResolution].Hash;
        // }
        // else
        // {
        // Log.Error("No scan data found for requested template: " + debugkey);
        // return null;
        // }
        // hashes.Add(matchhash);
        // keys.Add(key);
        // }

        // if (useArea.ContainsKey(this.lastResolution))
        // {
        // rect = useArea[this.lastResolution].Rect;
        // theResolution = this.lastResolution;
        // }
        // else if (useArea.ContainsKey(this.BaseResolution))
        // {
        // rect = useArea[this.BaseResolution].Rect;
        // theResolution = this.BaseResolution;
        // }
        // else
        // {
        // Log.Error("No scan data found for requested template: " + debugkey);
        // return null;
        // }

        // var source = this.image;
        // using (var roi = source.Clone(ResolutionHelper.CorrectRectangle(source.Size, rect, theResolution), PixelFormat.Format32bppRgb))
        // {
        // var hash = this.imageHasher.Create(roi);
        // var best = PerceptualHash.FindBest(hash, hashes);
        // if (best.Distance <= ThreshHold)
        // {
        // Log.Diag("Detected best hash: '{0}' Distance: {1}", debugkey, best.Distance);
        // return keys[best.Index];                    
        // }
        // }

        // return null;
        // }

        /// <summary>
        /// The detect best.
        /// </summary>
        /// <param name="lookup">
        /// The lookup.
        /// </param>
        /// <param name="debugkey">
        /// The debugkey.
        /// </param>
        /// <param name="threshold">
        /// The threshold.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string DetectBest(ScanAreaDictionary lookup, string debugkey, int? threshold = null)
        {
            threshold = threshold ?? this.ThreshHold;
            var hashes = new List<ulong>();
            var keys = new List<string>();

            var rect = new Rectangle();
            int theResolution = this.BaseResolution;
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

        /// <summary>
        /// The detect.
        /// </summary>
        /// <param name="lookup">
        /// The lookup.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="useArea">
        /// </param>
        /// <param name="threshold">
        /// The threshold.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private DetectionResult Detect(ScanAreaDictionary lookup, string key, IDictionary<int, ScanArea> useArea = null, int threshold = -1)
        {
            threshold = threshold >= 0 ? threshold : this.ThreshHold;
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
                // Tuple<byte[], DetectionResult> cached = null;
                // byte[] roiBytes = roi.GetBytes();
                // if (hashCache.TryGetValue(key, out cached))
                // {
                // if (ImageUtils.AreEqual(cached.Item1, roiBytes))
                // {
                // TraceLog.Log("hash cache hit: {0}, hit: {1} distance: {2}", key, cached.Item2.Found, cached.Item2.Distance);
                // return cached.Item2;
                // }
                // }
                var hash = this.imageHasher.Create(roi.Data);
                distance = PerceptualHash.HammingDistance(hash, area.Hash);
                TraceLog.Log("Detecting '{0}'. Distance: {1}", key, distance);

                if (distance <= threshold)
                {
                    TraceLog.Log("Detected '{0}'. Distance: {1}", key, distance);
                    Mostly mostly;
                    if (!string.IsNullOrEmpty(area.Mostly) && Enum.TryParse(area.Mostly, out mostly))
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

        /// <summary>
        /// The detect best.
        /// </summary>
        /// <param name="lookup">
        /// The lookup.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string DetectBest(ScanAreaImageDictionary lookup)
        {
            string bestKey = null;
            float best = 0;
            foreach (var key in lookup.Keys)
            {
                var detect = this.DetectTemplate(lookup, key);
                if (detect > best)
                {
                    best = detect;
                    bestKey = key;
                }
            }

            return bestKey;
        }

        /// <summary>
        /// The detect template.
        /// </summary>
        /// <param name="lookup">
        /// The lookup.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="float"/>.
        /// </returns>
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

            // var roiRect = ResolutionHelper.CorrectPoints(source.Size, rect, theResolution);
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
                var ismatch = this.templateMatcher.IsMatch(roi, newhash);
                graph.Dispose();
                newhash.Dispose();
                return ismatch;
            }
        }

        /// <summary>
        /// The game ended.
        /// </summary>
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
                new GameEnded {
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

        /// <summary>
        /// The game started.
        /// </summary>
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

        /// <summary>
        /// The init areas.
        /// </summary>
        /// <param name="arrkey">
        /// The arrkey.
        /// </param>
        /// <param name="prefix">
        /// The prefix.
        /// </param>
        /// <param name="scanArea">
        /// The scan area.
        /// </param>
        /// <param name="lookup">
        /// The lookup.
        /// </param>
        /// <param name="a">
        /// The a.
        /// </param>
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

        /// <summary>
        /// The init image areas.
        /// </summary>
        /// <param name="arrkey">
        /// The arrkey.
        /// </param>
        /// <param name="prefix">
        /// The prefix.
        /// </param>
        /// <param name="scanArea">
        /// The scan area.
        /// </param>
        /// <param name="lookup">
        /// The lookup.
        /// </param>
        /// <param name="a">
        /// The a.
        /// </param>
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
                var image = (Bitmap)this.scanAreaProvider.GetImage(a.Value.Image);
                tmp[scanArea.BaseResolution] = new Tuple<Bitmap, ScanArea>(image, a.Value);
            }
        }

        /// <summary>
        /// The load scan areas.
        /// </summary>
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
                    
                        // else if (a.Key.StartsWith("arenahero_"))
                        // {
                        // this.InitAreas(a.Key, "arenahero_", scanArea, this.arenaHeroes, a);
                        // }
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
        /// The reset.
        /// </summary>
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
            this.ResetFoundVictory();

            // this.nothingFoundFired = false;
            // this.gameJustEnded = false; // NOTE: we reset this on entering a game-mode or the menu
        }

        /// <summary>
        /// The scan.
        /// </summary>
        private void Scan()
        {
            this.currentScan = new CurrentScanState();

            if (this.requestReset)
            {
                this.Reset();
            }

            // Scan these before game-modes, to catch victories earlier
            if (!this.gameJustEnded && !this.pauseScanning)
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

            if (this.gameMode == GameMode.Unknown)
            {
                this.ScanDeckScreenshot();
            }

            if (this.gameMode == GameMode.Arena)
            {
                this.ScanArenaDeckScreenshot();
            }

            // Make sure this is AFTER ScanGameModes() !! 
            if (this.pauseScanning)
            {
                return;
            }

            if (this.currentScan.ArenaLeafDetected && this.inGameCounter <= 0)
            {
                if (!this.Detect(this.areas, "arenanohero"))
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

        /// <summary>
        /// The scan arena deck screenshot.
        /// </summary>
        private void ScanArenaDeckScreenshot()
        {
            if (this.arenadeckScreenshotRequested && this.gameMode == GameMode.Arena)
            {
                this.arenadeckScreenshotRequested = false;
                var deckRect = this.areas["deckarea_arena"][this.BaseResolution].Rect;
                deckRect = ResolutionHelper.CorrectRectangle(this.image.Size, deckRect, this.BaseResolution);
                var deck = this.image.Clone(deckRect, this.image.PixelFormat);
                Log.Debug("Arena deck Screenshot Requested. Sending screenshot...");
                this.events.PublishOnBackgroundThread(new ArenaDeckScreenshotTaken(deck));
            }
        }

        /// <summary>
        /// The scan deck screenshot.
        /// </summary>
        private void ScanDeckScreenshot()
        {
            if (this.deckScreenshotRequestedCanceled)
            {
                Log.Debug("Deck Screenshot Requested Canceled.");
                if (this.cancelDeckScreenshot != null)
                {
                    this.cancelDeckScreenshot.Cancel();
                }

                this.deckScreenshotRequestedCanceled = false;
                this.deckScreenshotRequested = false;
                this.takeDeckScreenshot = false;
            }

            if (this.deckScreenshotRequested)
            {
                var detect1 = this.Detect(this.areas, "deckscreen");
                var detect2 = this.Detect(this.areas, "deckscreen2");
                if (detect1 && detect2)
                {
                    this.deckScreenshotRequested = false;
                    if (this.cancelDeckScreenshot != null)
                    {
                        this.cancelDeckScreenshot.Cancel();
                        this.cancelDeckScreenshot.Dispose();
                    }

                    this.cancelDeckScreenshot = new CancellationTokenSource();
                    Log.Debug("Deck Screenshot Requested. Found!");

                    Task.Delay(500, this.cancelDeckScreenshot.Token).ContinueWith(
                        t =>
                        {
                            if (t.IsCanceled) return;
                            this.takeDeckScreenshot = true;
                        });
                }
                else
                {
                    Log.Debug("Deck Screenshot Requested. Not found. deckscreen1: {0}, deckscreen2: {1}", detect1.Distance, detect2.Distance);
                }
            }

            if (this.takeDeckScreenshot)
            {
                this.takeDeckScreenshot = false;
                var deckRect = this.areas["deckarea_cards"][this.BaseResolution].Rect;
                deckRect = ResolutionHelper.CorrectRectangle(this.image.Size, deckRect, this.BaseResolution);
                var deck = this.image.Clone(deckRect, this.image.PixelFormat);
                Log.Debug("Deck Screenshot Requested. Sending screenshot...");
                this.events.PublishOnBackgroundThread(new DeckScreenshotTaken(deck));
            }
        }

        /// <summary>
        /// The scan arena drafting.
        /// </summary>
        private void ScanArenaDrafting()
        {
            if (!this.arenaDrafting)
            {
                if (this.Detect(this.areas, "arena_drafting"))
                {
                    this.arenaDrafting = true;
                    this.Publish(new ArenaDrafting());
                }
            }

            // TODO: enable again

            // if (this.arenaDrafting)
            // {
            // this.ScanArenaDraftingCards();
            // }
        }

        /// <summary>
        /// The scan arena drafting cards.
        /// </summary>
        private void ScanArenaDraftingCards()
        {
            // TODO: scan cards 
            // Detect 3 best cards (by hash) in a given set of card hashes using the 3 card rectangles.
        }

        /// <summary>
        /// The scan arena hero.
        /// </summary>
        private void ScanArenaHero()
        {
            if (this.arenaHero != null || this.arenaWins == 12 || this.arenaLosses == 3) return;

            var best = this.DetectBest(this.arenaHeroes, "arena_hero_*", this.ThreshHoldForHeroes);

            if (best == null)
            {
                return;
            }

            this.arenaHero = best;
            this.Publish(new ArenaHeroDetected(this.arenaHero));
            this.gameModeChangeActionQueue.Enqueue(() => { this.arenaHero = null; });

            // foreach (var k in this.heroes.Keys)
            // {
            // if (this.Detect(this.heroes, k, herobox))
            // {
            // this.arenaHero = k;
            // this.Publish(new ArenaHeroDetected(this.arenaHero));
            // break;
            // }
            // }
        }

        /// <summary>
        /// The scan arena score.
        /// </summary>
        private void ScanArenaScore()
        {
            if (this.arenaWins < 0 && !this.arenaWinsScanning)
            {
                this.arenaWinsScanning = true;
                TraceLog.Log("delay scan score to let key animation finish");
                Task.Delay(1000).ContinueWith(
                    t =>
                        {
                            TraceLog.Log("delay scan score finished now.");
                            this.doArenaWinsScan = true;
                        });
                Task.Delay(5000).ContinueWith(
                    t =>
                        {
                            this.doArenaWinsScan = false;
                            this.arenaWinsScanning = false;
                        });
            }

            if (this.arenaWins < 0 && this.doArenaWinsScan)
            {
                var best = this.DetectBest(this.arenaWinsLookup2);
                if (best != null)
                {
                    this.doArenaWinsScan = false;
                    this.arenaWinsScanning = false;
                    int wins = Convert.ToInt32(best);
                    this.arenaWins = wins;
                    this.Publish(new ArenaWinsDetected(this.arenaWins));
                    this.gameModeChangeActionQueue.Enqueue(() => { this.arenaWins = -1; });
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
                        this.gameModeChangeActionQueue.Enqueue(() => { this.arenaLosses = -1; });
                        break;
                    }
                }

                if (this.arenaLosses < 0)
                {
                    // TODO: refine this, as when we cannot detect 'checked' it could be a false detection
                    this.arenaLosses = 0;
                    this.Publish(new ArenaLossesDetected(this.arenaLosses));
                    this.gameModeChangeActionQueue.Enqueue(() => { this.arenaLosses = -1; });
                }
            }
        }

        /// <summary>
        /// The scan coin.
        /// </summary>
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

        /// <summary>
        /// The scan conceded.
        /// </summary>
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

        /// <summary>
        /// The scan deck.
        /// </summary>
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

        /// <summary>
        /// The scan game modes.
        /// </summary>
        private void ScanGameModes()
        {
            var foundGameMode = this.gameMode;

            bool found = false, 
                detectedGameBoard = false;

            if (!this.pauseScanning && (this.Detect(this.areas, "ingame") || this.Detect(this.areas, "ingame2")))
            {
                if (this.inGameCounter == 0 && !this.gameJustEnded)
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

            if (!found && this.Detect(this.areas, "play_mode") && this.Detect(this.areas, "play_casual"))
            {
                foundGameMode = GameMode.Casual;
                found = true;
            }

            if (!found && this.Detect(this.areas, "play_mode") && this.Detect(this.areas, "play_ranked"))
            {
                foundGameMode = GameMode.Ranked;
                found = true;
            }

            if (!found && this.Detect(this.areas, "practice"))
            {
                foundGameMode = GameMode.Practice;
                found = true;
            }

            if (!found && this.Detect(this.areas, "challenge") && this.Detect(this.areas, "challenge2"))
            {
                foundGameMode = GameMode.Challenge;
                found = true;
            }

            if (!found && (this.Detect(this.areas, "arena_leaf") || (this.Detect(this.areas, "arenanohero") && this.Detect(this.areas, "arena"))))
            {
                this.currentScan.ArenaLeafDetected = true;
                foundGameMode = GameMode.Arena;
                found = true;
            }

            // when in game, you always return to the last game-mode screen, anything else is a false-positive
            if (found && this.inGameCounter >= 2 && (this.lastGameMode != foundGameMode))
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
                    while (this.gameModeChangeActionQueue.Count > 0)
                    {
                        var action = this.gameModeChangeActionQueue.Dequeue();
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

        /// <summary>
        /// The scan heroes.
        /// </summary>
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

            // if (this.hero != null || this.opponentHero != null)
            // {
            // this.GameStarted();
            // }
        }

        /// <summary>
        /// The scan turn.
        /// </summary>
        private void ScanTurn()
        {
            if (this.roundTurned) return;

            if (this.Detect(this.areas, "yourturn") || this.Detect(this.areas, "yourturn2"))
            {
                if (this.gameTurns == 0)
                {
                    this.inGameCounter++;
                }

                if (this.myturn)
                {
                    // we missed enemy turn detection
                    this.myturn = false;
                    this.gameTurns++;
                }

                if (!this.myturn)
                {
                    // to avoid false positives, we reset the victory counter
                    this.ResetFoundVictory();

                    this.gameTurns++;
                    this.roundTurned = true;
                    this.myturn = true;
                    this.GameStarted();
                    Task.Delay(2000).ContinueWith(task => this.roundTurned = false);
                    this.Publish(new NewRound(this.gameTurns, true));
                }
            }
            else if (this.Detect(this.areas, "enemyturn") || this.Detect(this.areas, "enemyturn2"))
            {
                if (this.gameTurns == 0)
                {
                    this.inGameCounter++;
                }

                if (this.myturn)
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

        /// <summary>
        /// The reset found victory.
        /// </summary>
        private void ResetFoundVictory()
        {
            Log.Debug("ResetFoundVictory called. Was: {0}", this.foundUsing);
            this.foundLoss = 0;
            this.foundVic = 0;
            this.foundUsing.Clear();
            this.lastLossDetect = null;
            this.lastVicDetect = null;
        }

        /// <summary>
        /// The scan victory.
        /// </summary>
        private void ScanVictory()
        {
            if (this.gameJustEnded || this.victory != null)
            {
                return;
            }

            if (this.Detect(this.areas, "victory"))
            {
                this.foundVic++;
                this.lastVicDetect = "victory";
                this.foundUsing.Append("victory|");
            }

            if (this.Detect(this.areas, "loss"))
            {
                this.foundLoss++;
                this.lastLossDetect = "loss";
                this.foundUsing.Append("loss|");
            }

            if (this.Detect(this.areas, "victory_explode"))
            {
                this.foundVic++;
                this.lastVicDetect = "victory_explode";
                this.foundUsing.Append("victory_explode|");
            }

            if (this.Detect(this.areas, "victory_explode2"))
            {
                this.foundVic++;
                this.lastVicDetect = "victory_explode2";
                this.foundUsing.Append("victory_explode2|");
            }

            if (this.Detect(this.areas, "victory_explode3"))
            {
                this.foundVic++;
                this.lastVicDetect = "victory_explode3";
                this.foundUsing.Append("victory_explode3|");
            }

            if (this.Detect(this.areas, "victory_explode4"))
            {
                this.foundVic++;
                this.lastVicDetect = "victory_explode4";
                this.foundUsing.Append("victory_explode4|");
            }

            if (this.Detect(this.areas, "victory_explode5"))
            {
                this.foundVic++;
                this.lastVicDetect = "victory_explode5";
                this.foundUsing.Append("victory_explode5|");
            }

            if (this.Detect(this.areas, "loss_explode"))
            {
                this.foundLoss++;
                this.lastLossDetect = "loss_explode";
                this.foundUsing.Append("loss_explode|");
            }

            if (this.Detect(this.areas, "loss_explode2"))
            {
                this.foundLoss++;
                this.lastLossDetect = "loss_explode2";
                this.foundUsing.Append("loss_explode2|");
            }

            if (this.Detect(this.areas, "victory2"))
            {
                this.foundVic++;
                this.lastVicDetect = "victory2";
                this.foundUsing.Append("victory2|");
            }

            if (this.Detect(this.areas, "victory3"))
            {
                this.foundVic++;
                this.lastVicDetect = "victory3";
                this.foundUsing.Append("victory3|");
            }

            if (this.Detect(this.areas, "loss2"))
            {
                this.foundLoss++;
                this.lastLossDetect = "loss2";
                this.foundUsing.Append("loss2|");
            }

            if (this.foundVic >= 3 || this.foundLoss >= 3)
            {
                this.victory = this.foundVic > this.foundLoss;
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
        /// <param name="message">
        /// The message.
        /// </param>
        public void Handle(ResetCurrentGame message)
        {
            Log.Debug("ResetCurrentGame requested");
            this.requestReset = true;
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
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
        /// <param name="message">
        /// The message.
        /// </param>
        public void Handle(RequestArenaDeckScreenshot message)
        {
            this.arenadeckScreenshotRequested = !message.Cancel;
        }
    }

    /// <summary>
    /// The least recently used cache.
    /// </summary>
    /// <typeparam name="TKey">
    /// </typeparam>
    /// <typeparam name="TValue">
    /// </typeparam>
    public class LeastRecentlyUsedCache<TKey, TValue>
    {
        /// <summary>
        /// The entries.
        /// </summary>
        private readonly Dictionary<TKey, Node> entries;

        /// <summary>
        /// The capacity.
        /// </summary>
        private readonly int capacity;

        /// <summary>
        /// The head.
        /// </summary>
        private Node head;

        /// <summary>
        /// The tail.
        /// </summary>
        private Node tail;

        /// <summary>
        /// The node.
        /// </summary>
        private class Node
        {
            /// <summary>
            /// Gets or sets the next.
            /// </summary>
            public Node Next { get; set; }

            /// <summary>
            /// Gets or sets the previous.
            /// </summary>
            public Node Previous { get; set; }

            /// <summary>
            /// Gets or sets the key.
            /// </summary>
            public TKey Key { get; set; }

            /// <summary>
            /// Gets or sets the value.
            /// </summary>
            public TValue Value { get; set; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LeastRecentlyUsedCache{TKey,TValue}"/> class.
        /// </summary>
        /// <param name="capacity">
        /// The capacity.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// </exception>
        public LeastRecentlyUsedCache(int capacity = 16)
        {
            if (capacity <= 0)
                throw new ArgumentOutOfRangeException(
                    "capacity", 
                    "Capacity should be greater than zero");
            this.capacity = capacity;
            this.entries = new Dictionary<TKey, Node>();
            this.head = null;
        }

        /// <summary>
        /// The set.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public void Set(TKey key, TValue value)
        {
            Node entry;
            if (!this.entries.TryGetValue(key, out entry))
            {
                entry = new Node { Key = key, Value = value };
                if (this.entries.Count == this.capacity)
                {
                    this.entries.Remove(this.tail.Key);
                    this.tail = this.tail.Previous;
                    if (this.tail != null) this.tail.Next = null;
                }

                this.entries.Add(key, entry);
            }

            entry.Value = value;
            this.MoveToHead(entry);
            if (this.tail == null) this.tail = this.head;
        }

        /// <summary>
        /// The try get value.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            value = default(TValue);
            Node entry;
            if (!this.entries.TryGetValue(key, out entry)) return false;
            this.MoveToHead(entry);
            value = entry.Value;
            return true;
        }

        /// <summary>
        /// The move to head.
        /// </summary>
        /// <param name="entry">
        /// The entry.
        /// </param>
        private void MoveToHead(Node entry)
        {
            if (entry == this.head || entry == null) return;

            var next = entry.Next;
            var previous = entry.Previous;

            if (next != null) next.Previous = entry.Previous;
            if (previous != null) previous.Next = entry.Next;

            entry.Previous = null;
            entry.Next = this.head;

            if (this.head != null) this.head.Previous = entry;
            this.head = entry;

            if (this.tail == entry) this.tail = previous;
        }
    }
}