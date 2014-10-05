﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateProgress.cs" company="">
//   
// </copyright>
// <summary>
//   The update progress.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HearthCap.Updater
{
    using System.Diagnostics;
    using System.IO;
    using System.IO.Compression;
    using System.Runtime.InteropServices;
    using System.Threading;

    using NLog;

    /// <summary>
    /// The update progress.
    /// </summary>
    public partial class UpdateProgress : Form
    {
        /// <summary>
        /// The log.
        /// </summary>
        private static Logger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The is testing.
        /// </summary>
        private bool isTesting;

        /// <summary>
        /// The install file.
        /// </summary>
        private readonly string installFile;

        /// <summary>
        /// The install path.
        /// </summary>
        private readonly string installPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateProgress"/> class.
        /// </summary>
        public UpdateProgress()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateProgress"/> class.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        public UpdateProgress(string[] args)
        {
            this.InitializeComponent();
            this.Shown += this.OnShown;

            this.installFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, args[0]);

            if (args.Length >= 2)
            {
                this.installPath = args[1];
                if (this.installPath.EndsWith("\""))
                {
                    this.installPath = args[1].Substring(0, args[1].Length - 1);
                }
            }
            else
            {
                // this.installPath = AppDomain.CurrentDomain.BaseDirectory;
                this.installPath = Directory.GetCurrentDirectory();
            }

            this.isTesting = args.Any(x => x.Contains("--testing"));
        }

        /// <summary>
        /// The on shown.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="eventArgs">
        /// The event args.
        /// </param>
        private void OnShown(object sender, EventArgs eventArgs)
        {
            Task.Run(() => this.DoUpdate());
        }

        /// <summary>
        /// The do update.
        /// </summary>
        private void DoUpdate()
        {
            var success = false;
            
            try
            {
                var retrycount = 0;
                while (retrycount < 5 && IsRunning())
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

                if (this.isTesting)
                {
                    Thread.Sleep(2000);
                    success = true;
                }
                else
                {
                    success = this.DoUpdate(this.installFile);
                    File.Delete(this.installFile);
                }

                Thread.Sleep(500);
                var targs = success ? "-updated" : "-updatefailed";
                var pi = new ProcessStartInfo(Path.Combine(this.installPath, "HearthCap.exe"), targs)
                             {
                                 WorkingDirectory = this.installPath
                             };
                Process.Start(pi);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An unknown error occured:\n" + ex, "Uknown error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Hide();
                Application.Exit();
            }
        }

        /// <summary>
        /// The do update.
        /// </summary>
        /// <param name="zipfile">
        /// The zipfile.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool DoUpdate(string zipfile)
        {
            Log.Info("Starting update... using: {0}", zipfile);
            try
            {
                // Cleanup temp files
                var dir = new DirectoryInfo(this.installPath);
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
                    catch { }
                }

                // HACK: work-around Capture.dll in use.
                // Rename it to .tmp so it will get cleaned on next update
                var fromCapDll = Path.Combine(this.installPath, "Capture.dll");
                var toCapDll = Path.Combine(this.installPath, "Capture.dll.tmp");
                File.Move(fromCapDll, toCapDll);

                using(var file = File.OpenRead(zipfile))
                using (var zip = new ZipArchive(file, ZipArchiveMode.Read))
                {
                    foreach (var entry in zip.Entries)
                    {
                        var targetFile = Path.Combine(this.installPath, entry.FullName);
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
            catch (Exception ex)
            {
                Log.Error(ex);
                MessageBox.Show("An unknown error occured:\n" + ex, "Unknown error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            Log.Info("Update done");
            return true;
        }

        /// <summary>
        /// The is running.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool IsRunning()
        {
            bool createdNew;
            using (new Mutex(false, @"HearthCap", out createdNew))
            {
                return !createdNew;
            }
        }

        /// <summary>
        /// The check not running.
        /// </summary>
        /// <param name="retry">
        /// The retry.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
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

        /// <summary>
        /// The find window.
        /// </summary>
        /// <param name="lpClassName">
        /// The lp class name.
        /// </param>
        /// <param name="lpWindowName">
        /// The lp window name.
        /// </param>
        /// <returns>
        /// The <see cref="IntPtr"/>.
        /// </returns>
        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
    }
}
