namespace HearthCap.Features.AutoUpdate
{
    using System;
    using System.Threading.Tasks;
    using System.ComponentModel.Composition;
    using System.Configuration;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Net.Cache;
    using System.Reflection;
    using System.Windows;
    using System.Xml;

    using Caliburn.Micro;

    using HearthCap.Framework;

    using NLog;

    [Export(typeof(UpdateViewModel))]
    public class UpdateViewModel : Screen
    {
        private static readonly Logger Log = NLog.LogManager.GetCurrentClassLogger();

        private readonly IEventAggregator events;

        private bool updateAvailable;

        private bool updateCheckDone;

        private bool checkingForUpdates;

        private bool downloading;

        private bool downloadReady;

        private bool hasLatestVersion;

        private string filename;

        private Version latestVersion;

        private Version currentVersion;

        private string updateBaseUrl = "http://hearthstonetracker.com/install";
        private string updateXmlUrl = "http://hearthstonetracker.com/install/update.xml";

        private string updatefileurl;

        private string error;

        private string basePath;

        private string tempPath;

        [ImportingConstructor]
        public UpdateViewModel(IEventAggregator events)
        {
            this.events = events;
            CurrentVersion = Assembly.GetEntryAssembly().GetName().Version;

            if (!String.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["updatebaseurl"]))
            {
                updateBaseUrl = ConfigurationManager.AppSettings["updatebaseurl"];
                updateXmlUrl = updateBaseUrl + (!updateBaseUrl.EndsWith("/") ? "/" : "") + "update.xml";
            }
            this.basePath = AppDomain.CurrentDomain.BaseDirectory;
            this.tempPath = Path.GetTempPath();
            Busy = new BusyWatcher();
        }

        public IBusyWatcher Busy { get; set; }

        public bool CheckingForUpdates
        {
            get
            {
                return this.checkingForUpdates;
            }
            set
            {
                if (value.Equals(this.checkingForUpdates))
                {
                    return;
                }
                this.checkingForUpdates = value;
                this.NotifyOfPropertyChange(() => this.CheckingForUpdates);
            }
        }

        public bool UpdateAvailable
        {
            get
            {
                return this.updateAvailable;
            }
            set
            {
                if (value.Equals(this.updateAvailable))
                {
                    return;
                }
                this.updateAvailable = value;
                this.NotifyOfPropertyChange(() => this.UpdateAvailable);
            }
        }

        public bool HasLatestVersion
        {
            get
            {
                return this.hasLatestVersion;
            }
            set
            {
                if (value.Equals(this.hasLatestVersion))
                {
                    return;
                }
                this.hasLatestVersion = value;
                this.NotifyOfPropertyChange(() => this.HasLatestVersion);
            }
        }

        public bool UpdateCheckDone
        {
            get
            {
                return this.updateCheckDone;
            }
            set
            {
                if (value.Equals(this.updateCheckDone))
                {
                    return;
                }
                this.updateCheckDone = value;
                this.NotifyOfPropertyChange(() => this.UpdateCheckDone);
            }
        }

        public bool DownloadReady
        {
            get
            {
                return this.downloadReady;
            }
            set
            {
                if (value.Equals(this.downloadReady))
                {
                    return;
                }
                this.downloadReady = value;
                this.NotifyOfPropertyChange(() => this.DownloadReady);
            }
        }

        public bool Downloading
        {
            get
            {
                return this.downloading;
            }
            set
            {
                if (value.Equals(this.downloading))
                {
                    return;
                }
                this.downloading = value;
                this.NotifyOfPropertyChange(() => this.Downloading);
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
                                               CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore),
                                           })
                {
                    var xml = await webclient.DownloadStringTaskAsync(updateXmlUrl);
                    xmldoc.LoadXml(xml);
                }

                // TODO: add null checks
                var latestVersion = xmldoc.SelectSingleNode("item/version").InnerText;
                var updatefile = xmldoc.SelectSingleNode("item/file").InnerText;
                var title = xmldoc.SelectSingleNode("item/title").InnerText;
                // TODO: show changelog etc.

                var theversion = new Version(latestVersion);
                if (theversion > CurrentVersion)
                {
                    LatestVersion = theversion;
                    updatefileurl = updateBaseUrl + (!updateBaseUrl.EndsWith("/") ? "/" : "") + updatefile;
                    filename = updatefile.Substring(updatefile.LastIndexOf("/", StringComparison.Ordinal) + 1);
                    UpdateAvailable = true;
                }
                else
                {
                    HasLatestVersion = true;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error getting update status: " + ex);
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
                    var updaterexe_filename = "HearthCap.Updater.exe";
                    var updaterexe_fileurl = updateBaseUrl + (!updateBaseUrl.EndsWith("/") ? "/" : "") + updaterexe_filename;
                    var file = Path.Combine(tempPath, filename);
                    var updaterFile = Path.Combine(tempPath, updaterexe_filename);

                    using (var webclient = new WebClient { CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore) })
                    {
                        await webclient.DownloadFileTaskAsync(updaterexe_fileurl, updaterFile);
                        await webclient.DownloadFileTaskAsync(updatefileurl, file);
                    }
                    if (!File.Exists(file) || !File.Exists(updaterFile))
                    {
                        Error = "Error downloading update. Please try again.";
                        Log.Error("Error downloading update. Please try again (file does not exist).");
                    }
                    else
                    {
                        DownloadReady = true;
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Error downloading update: " + ex);
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
                var start = Path.Combine(tempPath, "HearthCap.Updater.exe");
                var path = this.basePath.EndsWith("\"") ? this.basePath.Substring(0, this.basePath.Length - 1) : this.basePath;
                var pi = new ProcessStartInfo(start, String.Format("{0} \"{1}\"", this.filename, path))
                             {
                                 UseShellExecute = true,
                                 Verb = "runas",
                                 WorkingDirectory = this.basePath,
                             };
                Process.Start(pi);
                Application.Current.Shutdown();
            }
        }

        public Version CurrentVersion
        {
            get
            {
                return this.currentVersion;
            }
            set
            {
                if (Equals(value, this.currentVersion))
                {
                    return;
                }
                this.currentVersion = value;
                this.NotifyOfPropertyChange(() => this.CurrentVersion);
            }
        }

        public Version LatestVersion
        {
            get
            {
                return this.latestVersion;
            }
            set
            {
                if (Equals(value, this.latestVersion))
                {
                    return;
                }
                this.latestVersion = value;
                this.NotifyOfPropertyChange(() => this.LatestVersion);
            }
        }

        public string Error
        {
            get
            {
                return this.error;
            }
            set
            {
                if (value == this.error)
                {
                    return;
                }
                this.error = value;
                this.NotifyOfPropertyChange(() => this.Error);
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
