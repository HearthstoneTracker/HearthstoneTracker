using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using NLog;

namespace HearthCap.Updater
{
    public partial class UpdateProgress : Form
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private readonly bool isTesting;

        private readonly string installFile;

        private readonly string installPath;

        public UpdateProgress()
        {
        }

        public UpdateProgress(string[] args)
        {
            InitializeComponent();
            Shown += OnShown;

            installFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, args[0]);

            if (args.Length >= 2)
            {
                installPath = args[1];
                if (installPath.EndsWith("\""))
                {
                    installPath = args[1].Substring(0, args[1].Length - 1);
                }
            }
            else
            {
                // this.installPath = AppDomain.CurrentDomain.BaseDirectory;
                installPath = Directory.GetCurrentDirectory();
            }

            isTesting = args.Any(x => x.Contains("--testing"));
        }

        private void OnShown(object sender, EventArgs eventArgs)
        {
            Task.Run(() => DoUpdate());
        }

        private void DoUpdate()
        {
            var success = false;

            try
            {
                var retrycount = 0;
                while (retrycount < 5
                       && IsRunning())
                {
                    Thread.Sleep(1000);
                    retrycount++;
                }

                bool retry;
                bool notRunning;
                do
                {
                    notRunning = CheckNotRunning(out retry);
                }
                while (retry && !notRunning);

                if (isTesting)
                {
                    Thread.Sleep(2000);
                    success = true;
                }
                else
                {
                    success = DoUpdate(installFile);
                    File.Delete(installFile);
                }

                Thread.Sleep(500);
                var targs = success ? "-updated" : "-updatefailed";
                var pi = new ProcessStartInfo(Path.Combine(installPath, "HearthCap.exe"), targs)
                    {
                        WorkingDirectory = installPath
                    };
                Process.Start(pi);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An unknown error occured:\n" + ex, "Uknown error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Hide();
                Application.Exit();
            }
        }

        private bool DoUpdate(string zipfile)
        {
            Log.Info("Starting update... using: {0}", zipfile);
            try
            {
                // Cleanup temp files
                var dir = new DirectoryInfo(installPath);
                var tempFiles = dir.GetFiles().Where(
                    x =>
                        x.Extension.ToLower().EndsWith(".pendingoverwrite") ||
                        x.Extension.ToLower().EndsWith(".tmp")).ToList();
                foreach (var tempFile in tempFiles)
                {
                    try
                    {
                        Log.Warn("Deleting tempory file: " + tempFile.Name);
                        File.Delete(tempFile.FullName);
                    }
                    catch
                    {
                    }
                }

                // HACK: work-around Capture.dll in use.
                // Rename it to .tmp so it will get cleaned on next update
                var fromCapDll = Path.Combine(installPath, "Capture.dll");
                var toCapDll = Path.Combine(installPath, "Capture.dll.tmp");
                File.Move(fromCapDll, toCapDll);

                using (var file = File.OpenRead(zipfile))
                {
                    using (var zip = new ZipArchive(file, ZipArchiveMode.Read))
                    {
                        foreach (var entry in zip.Entries)
                        {
                            var targetFile = Path.Combine(installPath, entry.FullName);
                            Log.Debug("Extracting: " + entry.FullName);
                            try
                            {
                                entry.ExtractToFile(targetFile, true);
                            }
                            catch (IOException ex)
                            {
                                Log.Error(ex);
                                Log.Info("Renaming inaccesable file: " + targetFile);
                                var tmpTargetFile = targetFile + ".tmp";
                                File.Move(targetFile, tmpTargetFile);
                                Log.Debug("Extracting: " + entry.FullName);
                                entry.ExtractToFile(targetFile, true);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                MessageBox.Show("An unknown error occured:\n" + ex, "Unknown error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            Log.Info("Update done");
            return true;
        }

        private static bool IsRunning()
        {
            bool createdNew;
            using (new Mutex(false, @"HearthCap", out createdNew))
            {
                return !createdNew;
            }
        }

        private static bool CheckNotRunning(out bool retry)
        {
            retry = false;
            if (IsRunning())
            {
                var result = MessageBox.Show("Cannot update while Hearthstone Tracker is running. Try again?", "Already running", MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning);
                if (result == DialogResult.Retry)
                {
                    retry = true;
                }
                return false;
            }

            var wnd = FindWindow("UnityWndClass", "Hearthstone");
            if (wnd != IntPtr.Zero)
            {
                var result = MessageBox.Show("It is recommended to close Hearthstone before updating.", "Already running", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Warning);
                if (result == DialogResult.Retry)
                {
                    retry = true;
                }
                else if (result == DialogResult.Ignore)
                {
                    return true;
                }

                return false;
            }

            return true;
        }

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
    }
}
