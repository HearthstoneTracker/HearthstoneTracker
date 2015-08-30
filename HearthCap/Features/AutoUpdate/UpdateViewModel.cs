using System;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using Caliburn.Micro;
using HearthCap.Framework;
using LogManager = NLog.LogManager;

namespace HearthCap.Features.AutoUpdate
{
    [Export(typeof(UpdateViewModel))]
    public class UpdateViewModel : Screen
    {
        private const string HearthcapUpdaterExe = "HearthCap.Updater.exe";
        private static readonly NLog.Logger _log = LogManager.GetCurrentClassLogger();

        private readonly IEventAggregator _events;

        private bool _updateAvailable;

        private bool _updateCheckDone;

        private bool _checkingForUpdates;

        private bool _downloading;

        private bool _downloadReady;

        private bool _hasLatestVersion;

        private Version _latestVersion;

        private Version _currentVersion;

        private readonly string _updateBaseUrl = "http://hearthstonetracker.com/install/latest/package";
        private readonly string _updateXmlUrl = "http://hearthstonetracker.com/install/latest/package/update.xml";

        private string _error;

        private readonly string _basePath;

        private readonly string _tempPath;
        private UpdateInfo _updateInfo;

        [ImportingConstructor]
        public UpdateViewModel(IEventAggregator events)
        {
            _events = events;
            CurrentVersion = Assembly.GetEntryAssembly().GetName().Version;

            if (!String.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["updatebaseurl"]))
            {
                _updateBaseUrl = ConfigurationManager.AppSettings["updatebaseurl"];
                _updateXmlUrl = _updateBaseUrl + (!_updateBaseUrl.EndsWith("/", StringComparison.Ordinal) ? "/" : "") + "update.xml";
            }
            _basePath = AppDomain.CurrentDomain.BaseDirectory;
            _tempPath = Path.GetTempPath();
            Busy = new BusyWatcher();
        }

        public IBusyWatcher Busy { get; set; }

        public bool CheckingForUpdates
        {
            get { return _checkingForUpdates; }
            set
            {
                if (value.Equals(_checkingForUpdates))
                {
                    return;
                }
                _checkingForUpdates = value;
                NotifyOfPropertyChange(() => CheckingForUpdates);
            }
        }

        public bool UpdateAvailable
        {
            get { return _updateAvailable; }
            set
            {
                if (value.Equals(_updateAvailable))
                {
                    return;
                }
                _updateAvailable = value;
                NotifyOfPropertyChange(() => UpdateAvailable);
            }
        }

        public bool HasLatestVersion
        {
            get { return _hasLatestVersion; }
            set
            {
                if (value.Equals(_hasLatestVersion))
                {
                    return;
                }
                _hasLatestVersion = value;
                NotifyOfPropertyChange(() => HasLatestVersion);
            }
        }

        public bool UpdateCheckDone
        {
            get { return _updateCheckDone; }
            set
            {
                if (value.Equals(_updateCheckDone))
                {
                    return;
                }
                _updateCheckDone = value;
                NotifyOfPropertyChange(() => UpdateCheckDone);
            }
        }

        public bool DownloadReady
        {
            get { return _downloadReady; }
            set
            {
                if (value.Equals(_downloadReady))
                {
                    return;
                }
                _downloadReady = value;
                NotifyOfPropertyChange(() => DownloadReady);
            }
        }

        public bool Downloading
        {
            get { return _downloading; }
            set
            {
                if (value.Equals(_downloading))
                {
                    return;
                }
                _downloading = value;
                NotifyOfPropertyChange(() => Downloading);
            }
        }

        public UpdateInfo UpdateInfo
        {
            get { return _updateInfo; }
            set
            {
                if (Equals(value, _updateInfo))
                {
                    return;
                }
                _updateInfo = value;
                NotifyOfPropertyChange(() => UpdateInfo);
            }
        }

        public async Task CheckForUpdates()
        {
            UpdateAvailable = false;
            DownloadReady = false;
            Downloading = false;
            CheckingForUpdates = false;
            UpdateCheckDone = false;
            HasLatestVersion = false;
            Error = null;

            using (Busy.GetTicket())
            {
                CheckingForUpdates = true;
                // TODO: remove
                // await Task.Delay(1000);
                await DoCheckForUpdates();
                CheckingForUpdates = false;
                UpdateCheckDone = true;
            }
        }

        private async Task DoCheckForUpdates()
        {
            try
            {
                var xmldoc = new XmlDocument();
                using (var webclient = new WebClient
                {
                    CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore)
                })
                {
                    var xml = await webclient.DownloadStringTaskAsync(_updateXmlUrl);
                    xmldoc.LoadXml(xml);
                }

                // TODO: add null checks
                UpdateInfo = new UpdateInfo()
                {
                    Version = new Version(xmldoc.SelectSingleNode("item/version").InnerText),
                    File = xmldoc.SelectSingleNode("item/file").InnerText,
                    Updater = xmldoc.SelectSingleNode("item/updater").InnerText,
                    Title = xmldoc.SelectSingleNode("item/title").InnerText
                };
                // TODO: show changelog etc.

                if (UpdateInfo.Version > CurrentVersion)
                {
                    LatestVersion = UpdateInfo.Version;
                    UpdateAvailable = true;
                }
                else
                {
                    HasLatestVersion = true;
                }
            }
            catch (Exception ex)
            {
                _log.Error("Error getting update status: " + ex);
                Error = ex.Message;
            }
        }

        public async Task Download()
        {
            using (Busy.GetTicket())
            {
                try
                {
                    UpdateAvailable = false;
                    Downloading = true;

                    // Updater
                    string updaterFileUrl = UpdateInfo.Updater;
                    if (String.IsNullOrEmpty(UpdateInfo.Updater))
                    {
                        updaterFileUrl = HearthcapUpdaterExe;
                    }

                    if (!updaterFileUrl.StartsWith("http://", StringComparison.Ordinal)
                        && !updaterFileUrl.StartsWith("https://", StringComparison.Ordinal))
                    {
                        updaterFileUrl = _updateBaseUrl + (!_updateBaseUrl.EndsWith("/", StringComparison.Ordinal) ? "/" : "") + updaterFileUrl;
                    }

                    // File package
                    string filePackageUrl = UpdateInfo.File;
                    if (!filePackageUrl.StartsWith("http://", StringComparison.Ordinal)
                        && !filePackageUrl.StartsWith("https://", StringComparison.Ordinal))
                    {
                        filePackageUrl = _updateBaseUrl + (!_updateBaseUrl.EndsWith("/", StringComparison.Ordinal) ? "/" : "") + filePackageUrl;
                    }

                    string filename = UpdateInfo.File;
                    filename = filename.Substring(filename.LastIndexOf("/", StringComparison.Ordinal) + 1);

                    var file = Path.Combine(_tempPath, filename);
                    var updaterFile = Path.Combine(_tempPath, HearthcapUpdaterExe);

                    // Download
                    using (var wcFilePackage = new WebClient { CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore) })
                    using (var wcUpdater = new WebClient { CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore) })
                    {
                        await Task.WhenAll(
                            wcUpdater.DownloadFileTaskAsync(updaterFileUrl, updaterFile),
                            wcFilePackage.DownloadFileTaskAsync(filePackageUrl, file)
                            );
                    }

                    if (!File.Exists(file)
                        || !File.Exists(updaterFile))
                    {
                        Error = "Error downloading update. Please try again.";
                        _log.Error("Error downloading update. Please try again (file does not exist).");
                    }
                    else
                    {
                        DownloadReady = true;
                    }
                }
                catch (Exception ex)
                {
                    _log.Error("Error downloading update: " + ex);
                    Error = ex.Message;
                }
                finally
                {
                    Downloading = false;
                }
            }
        }

        public void Install()
        {
            using (Busy.GetTicket())
            {
                DownloadReady = false;
                string filename = UpdateInfo.File;
                filename = filename.Substring(filename.LastIndexOf("/", StringComparison.Ordinal) + 1);

                var file = Path.Combine(_tempPath, filename);
                var updaterFile = Path.Combine(_tempPath, HearthcapUpdaterExe);
                var path = _basePath.EndsWith("\"", StringComparison.Ordinal) ? _basePath.Substring(0, _basePath.Length - 1) : _basePath;
                var pi = new ProcessStartInfo(updaterFile, String.Format("{0} \"{1}\"", file, path))
                {
                    UseShellExecute = true,
                    Verb = "runas",
                    WorkingDirectory = _basePath
                };
                Process.Start(pi);
                Application.Current.Shutdown();
            }
        }

        public Version CurrentVersion
        {
            get { return _currentVersion; }
            set
            {
                if (Equals(value, _currentVersion))
                {
                    return;
                }
                _currentVersion = value;
                NotifyOfPropertyChange(() => CurrentVersion);
            }
        }

        public Version LatestVersion
        {
            get { return _latestVersion; }
            set
            {
                if (Equals(value, _latestVersion))
                {
                    return;
                }
                _latestVersion = value;
                NotifyOfPropertyChange(() => LatestVersion);
            }
        }

        public string Error
        {
            get { return _error; }
            set
            {
                if (value == _error)
                {
                    return;
                }
                _error = value;
                NotifyOfPropertyChange(() => Error);
            }
        }

        public void Close()
        {
            //UpdateAvailable = false;
            //DownloadReady = false;
            //Downloading = false;
            //CheckingForUpdates = false;
            //UpdateCheckDone = false;

            ((IDeactivate)this).Deactivate(true);
        }
    }
}
