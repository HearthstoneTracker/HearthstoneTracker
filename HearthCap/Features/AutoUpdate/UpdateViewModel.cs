// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The update view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.AutoUpdate
{
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

    using NLog;

    using LogManager = NLog.LogManager;

    /// <summary>
    /// The update view model.
    /// </summary>
    [Export(typeof(UpdateViewModel))]
    public class UpdateViewModel : Screen
    {
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The events.
        /// </summary>
        private readonly IEventAggregator events;

        /// <summary>
        /// The update available.
        /// </summary>
        private bool updateAvailable;

        /// <summary>
        /// The update check done.
        /// </summary>
        private bool updateCheckDone;

        /// <summary>
        /// The checking for updates.
        /// </summary>
        private bool checkingForUpdates;

        /// <summary>
        /// The downloading.
        /// </summary>
        private bool downloading;

        /// <summary>
        /// The download ready.
        /// </summary>
        private bool downloadReady;

        /// <summary>
        /// The has latest version.
        /// </summary>
        private bool hasLatestVersion;

        /// <summary>
        /// The filename.
        /// </summary>
        private string filename;

        /// <summary>
        /// The latest version.
        /// </summary>
        private Version latestVersion;

        /// <summary>
        /// The current version.
        /// </summary>
        private Version currentVersion;

        /// <summary>
        /// The update base url.
        /// </summary>
        private string updateBaseUrl = "http://hearthstonetracker.com/install";

        /// <summary>
        /// The update xml url.
        /// </summary>
        private string updateXmlUrl = "http://hearthstonetracker.com/install/update.xml";

        /// <summary>
        /// The updatefileurl.
        /// </summary>
        private string updatefileurl;

        /// <summary>
        /// The error.
        /// </summary>
        private string error;

        /// <summary>
        /// The base path.
        /// </summary>
        private string basePath;

        /// <summary>
        /// The temp path.
        /// </summary>
        private string tempPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateViewModel"/> class.
        /// </summary>
        /// <param name="events">
        /// The events.
        /// </param>
        [ImportingConstructor]
        public UpdateViewModel(IEventAggregator events)
        {
            this.events = events;
            this.CurrentVersion = Assembly.GetEntryAssembly().GetName().Version;

            if (!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["updatebaseurl"]))
            {
                this.updateBaseUrl = ConfigurationManager.AppSettings["updatebaseurl"];
                this.updateXmlUrl = this.updateBaseUrl + (!this.updateBaseUrl.EndsWith("/") ? "/" : string.Empty) + "update.xml";
            }

            this.basePath = AppDomain.CurrentDomain.BaseDirectory;
            this.tempPath = Path.GetTempPath();
            this.Busy = new BusyWatcher();
        }

        /// <summary>
        /// Gets or sets the busy.
        /// </summary>
        public IBusyWatcher Busy { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether checking for updates.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether update available.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether has latest version.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether update check done.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether download ready.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether downloading.
        /// </summary>
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

        /// <summary>
        /// The check for updates.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task CheckForUpdates()
        {
            this.UpdateAvailable = false;
            this.DownloadReady = false;
            this.Downloading = false;
            this.CheckingForUpdates = false;
            this.UpdateCheckDone = false;
            this.HasLatestVersion = false;
            this.Error = null;

            using (this.Busy.GetTicket())
            {
                this.CheckingForUpdates = true;

                // TODO: remove
                // await Task.Delay(1000);
                await this.DoCheckForUpdates();
                this.CheckingForUpdates = false;
                this.UpdateCheckDone = true;
            }
        }

        /// <summary>
        /// The do check for updates.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task DoCheckForUpdates()
        {
            try
            {
                var xmldoc = new XmlDocument();
                using (var webclient = new WebClient { CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore), })
                {
                    var xml = await webclient.DownloadStringTaskAsync(this.updateXmlUrl);
                    xmldoc.LoadXml(xml);
                }

                // TODO: add null checks
                var latestVersion = xmldoc.SelectSingleNode("item/version").InnerText;
                var updatefile = xmldoc.SelectSingleNode("item/file").InnerText;
                var title = xmldoc.SelectSingleNode("item/title").InnerText;

                // TODO: show changelog etc.
                var theversion = new Version(latestVersion);
                if (theversion > this.CurrentVersion)
                {
                    this.LatestVersion = theversion;
                    this.updatefileurl = this.updateBaseUrl + (!this.updateBaseUrl.EndsWith("/") ? "/" : string.Empty) + updatefile;
                    this.filename = updatefile.Substring(updatefile.LastIndexOf("/", StringComparison.Ordinal) + 1);
                    this.UpdateAvailable = true;
                }
                else
                {
                    this.HasLatestVersion = true;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error getting update status: " + ex);
                this.Error = ex.Message;
            }
        }

        /// <summary>
        /// The download.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task Download()
        {
            using (this.Busy.GetTicket())
            {
                try
                {
                    this.UpdateAvailable = false;
                    this.Downloading = true;
                    var updaterexe_filename = "HearthCap.Updater.exe";
                    var updaterexe_fileurl = this.updateBaseUrl + (!this.updateBaseUrl.EndsWith("/") ? "/" : string.Empty) + updaterexe_filename;
                    var file = Path.Combine(this.tempPath, this.filename);
                    var updaterFile = Path.Combine(this.tempPath, updaterexe_filename);

                    using (var webclient = new WebClient { CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore) })
                    {
                        await webclient.DownloadFileTaskAsync(updaterexe_fileurl, updaterFile);
                        await webclient.DownloadFileTaskAsync(this.updatefileurl, file);
                    }

                    if (!File.Exists(file) || !File.Exists(updaterFile))
                    {
                        this.Error = "Error downloading update. Please try again.";
                        Log.Error("Error downloading update. Please try again (file does not exist).");
                    }
                    else
                    {
                        this.DownloadReady = true;
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Error downloading update: " + ex);
                    this.Error = ex.Message;
                }
                finally
                {
                    this.Downloading = false;
                }
            }
        }

        /// <summary>
        /// The install.
        /// </summary>
        public void Install()
        {
            using (this.Busy.GetTicket())
            {
                this.DownloadReady = false;
                var start = Path.Combine(this.tempPath, "HearthCap.Updater.exe");
                var path = this.basePath.EndsWith("\"") ? this.basePath.Substring(0, this.basePath.Length - 1) : this.basePath;
                var pi = new ProcessStartInfo(start, string.Format("{0} \"{1}\"", this.filename, path))
                             {
                                 UseShellExecute = true, 
                                 Verb = "runas", 
                                 WorkingDirectory = this.basePath, 
                             };
                Process.Start(pi);
                Application.Current.Shutdown();
            }
        }

        /// <summary>
        /// Gets or sets the current version.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the latest version.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the error.
        /// </summary>
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

        /// <summary>
        /// The close.
        /// </summary>
        public void Close()
        {
            // UpdateAvailable = false;
            // DownloadReady = false;
            // Downloading = false;
            // CheckingForUpdates = false;
            // UpdateCheckDone = false;
            ((IDeactivate)this).Deactivate(true);
        }
    }
}
