using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Reflection;
using System.Threading;
using System.Windows;
using Caliburn.Micro;
using Microsoft.Win32;

namespace HearthCap.Shell.UserPreferences
{
    [Export(typeof(UserPreferences))]
    public sealed class UserPreferences : PropertyChangedBase, IDisposable
    {
        #region Member Variables

        private double windowTop;
        private double windowLeft;
        private double windowHeight;
        private double windowWidth;
        private WindowState windowState;

        private bool dontSave;

        private Timer timer;

        private readonly object timerLock = new object();

        private bool startMinimized;

        private bool startOnLogon;

        private bool minimizeToTray;

        public double WindowTop
        {
            get { return windowTop; }
            set
            {
                if (value.Equals(windowTop))
                {
                    return;
                }
                windowTop = value;
                NotifyOfPropertyChange(() => WindowTop);
            }
        }

        public double WindowLeft
        {
            get { return windowLeft; }
            set
            {
                if (value.Equals(windowLeft))
                {
                    return;
                }
                windowLeft = value;
                NotifyOfPropertyChange(() => WindowLeft);
            }
        }

        public double WindowHeight
        {
            get { return windowHeight; }
            set
            {
                if (value.Equals(windowHeight))
                {
                    return;
                }
                windowHeight = value;
                NotifyOfPropertyChange(() => WindowHeight);
            }
        }

        public double WindowWidth
        {
            get { return windowWidth; }
            set
            {
                if (value.Equals(windowWidth))
                {
                    return;
                }
                windowWidth = value;
                NotifyOfPropertyChange(() => WindowWidth);
            }
        }

        public WindowState WindowState
        {
            get { return windowState; }
            set
            {
                if (value == windowState)
                {
                    return;
                }
                windowState = value;
                NotifyOfPropertyChange(() => WindowState);
            }
        }

        public bool StartMinimized
        {
            get { return startMinimized; }
            set
            {
                if (value.Equals(startMinimized))
                {
                    return;
                }
                startMinimized = value;
                NotifyOfPropertyChange(() => StartMinimized);
            }
        }

        public bool StartOnLogon
        {
            get { return startOnLogon; }
            set
            {
                if (value.Equals(startOnLogon))
                {
                    return;
                }
                startOnLogon = value;
                NotifyOfPropertyChange(() => StartOnLogon);
            }
        }

        public bool MinimizeToTray
        {
            get { return minimizeToTray; }
            set
            {
                if (value.Equals(minimizeToTray))
                {
                    return;
                }
                minimizeToTray = value;
                NotifyOfPropertyChange(() => MinimizeToTray);
            }
        }

        [ImportingConstructor]
        public UserPreferences()
        {
            Load();
        }

        public void Initialize()
        {
            dontSave = true;
            PropertyChanged += UserPreferences_PropertyChanged;
            SizeToFit();
            MoveIntoView();
            dontSave = false;
            Save();
        }

        private void UserPreferences_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // just catch all as we don't have other properties yet.
            if (timer == null)
            {
                lock (timerLock)
                {
                    if (timer == null)
                    {
                        timer = new Timer(TimerOnElapsed, null, 250, -1);
                    }
                }
            }
            else
            {
                timer.Change(250, -1);
            }
        }

        private void TimerOnElapsed(object sender)
        {
            Save();
            if (timer != null)
            {
                timer.Dispose();
                timer = null;
            }
        }

        public void SizeToFit()
        {
            if (WindowHeight > SystemParameters.VirtualScreenHeight)
            {
                WindowHeight = SystemParameters.VirtualScreenHeight;
            }

            if (WindowWidth > SystemParameters.VirtualScreenWidth)
            {
                WindowWidth = SystemParameters.VirtualScreenWidth;
            }
        }

        public void MoveIntoView()
        {
            var virtualScreenHeight = SystemParameters.VirtualScreenHeight;
            if (WindowTop + WindowHeight / 2 > virtualScreenHeight)
            {
                WindowTop = virtualScreenHeight - windowHeight;
            }

            var virtualScreenWidth = SystemParameters.VirtualScreenWidth;
            if (WindowLeft + WindowWidth / 2 > virtualScreenWidth)
            {
                WindowLeft = virtualScreenWidth - WindowWidth;
            }

            // Center on first load
            if (WindowTop < 0)
            {
                WindowTop = (virtualScreenHeight - WindowHeight) / 2;
            }

            if (WindowLeft < 0)
            {
                WindowLeft = (SystemParameters.PrimaryScreenWidth - WindowWidth) / 2;
            }
        }

        private void Load()
        {
            using (var reg = new WindowRegistrySettings())
            {
                // On first startup
                if (reg.WindowHeight == 0
                    || reg.WindowWidth == 0)
                {
                    Save();
                    return;
                }

                WindowTop = reg.WindowTop;
                WindowLeft = reg.WindowLeft;
                WindowHeight = reg.WindowHeight;
                WindowWidth = reg.WindowWidth;
                WindowState = reg.WindowState;
                StartMinimized = reg.StartMinimized;
                MinimizeToTray = reg.MinimizeToTray;

                using (var section = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
                {
                    if (section != null)
                    {
                        var startupLocation = section.GetValue("HearthstoneTracker");
                        if (startupLocation != null)
                        {
                            StartOnLogon = true;

                            // Verify location still correct:
                            var exeName = Assembly.GetEntryAssembly().Location;
                            var value = String.Format("\"{0}\" -logon", exeName);
                            if (startupLocation.ToString() != value)
                            {
                                section.SetValue("HearthstoneTracker", value);
                            }
                        }
                    }
                }
            }
        }

        public void Save()
        {
            if (WindowState != WindowState.Minimized
                && !dontSave)
            {
                using (var reg = new WindowRegistrySettings())
                {
                    if (WindowState != WindowState.Maximized)
                    {
                        reg.WindowTop = WindowTop;
                        reg.WindowLeft = WindowLeft;
                        reg.WindowHeight = WindowHeight;
                        reg.WindowWidth = WindowWidth;
                    }

                    reg.StartMinimized = StartMinimized;
                    reg.MinimizeToTray = MinimizeToTray;
                    reg.WindowState = WindowState;

                    using (var section = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
                    {
                        if (section != null)
                        {
                            if (StartOnLogon)
                            {
                                var exeName = Assembly.GetEntryAssembly().Location;
                                var value = String.Format("\"{0}\" -logon", exeName);
                                section.SetValue("HearthstoneTracker", value);
                            }
                            else if (section.GetValue("HearthstoneTracker") != null)
                            {
                                section.DeleteValue("HearthstoneTracker");
                            }
                        }
                    }
                }
            }
        }

        #endregion //Functions

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (timer != null)
            {
                timer.Dispose();
            }
        }
    }
}
