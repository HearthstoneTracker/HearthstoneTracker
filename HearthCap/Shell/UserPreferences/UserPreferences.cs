namespace HearthCap.Shell.UserPreferences
{
    using System;
    using System.ComponentModel.Composition;
    using System.Windows;
    using Caliburn.Micro;

    using Microsoft.Win32;

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

        private System.Threading.Timer timer;

        private readonly object timerLock = new object();

        private bool startMinimized;

        private bool startOnLogon;

        private bool minimizeToTray;

        public double WindowTop
        {
            get { return this.windowTop; }
            set
            {
                if (value.Equals(this.windowTop)) return;
                this.windowTop = value;
                this.NotifyOfPropertyChange(() => this.WindowTop);
            }
        }

        public double WindowLeft
        {
            get { return this.windowLeft; }
            set
            {
                if (value.Equals(this.windowLeft)) return;
                this.windowLeft = value;
                this.NotifyOfPropertyChange(() => this.WindowLeft);
            }
        }

        public double WindowHeight
        {
            get { return this.windowHeight; }
            set
            {
                if (value.Equals(this.windowHeight)) return;
                this.windowHeight = value;
                this.NotifyOfPropertyChange(() => this.WindowHeight);
            }
        }

        public double WindowWidth
        {
            get { return this.windowWidth; }
            set
            {
                if (value.Equals(this.windowWidth)) return;
                this.windowWidth = value;
                this.NotifyOfPropertyChange(() => this.WindowWidth);
            }
        }

        public WindowState WindowState
        {
            get { return this.windowState; }
            set
            {
                if (value == this.windowState) return;
                this.windowState = value;
                this.NotifyOfPropertyChange(() => this.WindowState);
            }
        }

        public bool StartMinimized
        {
            get
            {
                return this.startMinimized;
            }
            set
            {
                if (value.Equals(this.startMinimized))
                {
                    return;
                }
                this.startMinimized = value;
                this.NotifyOfPropertyChange(() => this.StartMinimized);
            }
        }

        public bool StartOnLogon
        {
            get
            {
                return this.startOnLogon;
            }
            set
            {
                if (value.Equals(this.startOnLogon))
                {
                    return;
                }
                this.startOnLogon = value;
                this.NotifyOfPropertyChange(() => this.StartOnLogon);
            }
        }

        public bool MinimizeToTray
        {
            get
            {
                return this.minimizeToTray;
            }
            set
            {
                if (value.Equals(this.minimizeToTray)) return;
                this.minimizeToTray = value;
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
            this.dontSave = true;
            this.PropertyChanged += UserPreferences_PropertyChanged;
            SizeToFit();
            MoveIntoView();
            this.dontSave = false;
            Save();
        }

        void UserPreferences_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // just catch all as we don't have other properties yet.
            if (this.timer == null)
            {
                lock (timerLock)
                {
                    if (this.timer == null)
                    {
                        timer = new System.Threading.Timer(TimerOnElapsed, null, 250, -1);
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
                this.timer.Dispose();
                this.timer = null;                
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
                WindowTop = virtualScreenHeight - this.windowHeight;
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
                if (reg.WindowHeight == 0 || reg.WindowWidth == 0)
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
                            this.StartOnLogon = true;

                            // Verify location still correct:
                            var exeName = System.Reflection.Assembly.GetEntryAssembly().Location;
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
            if (WindowState != WindowState.Minimized && !dontSave)
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
                                var exeName = System.Reflection.Assembly.GetEntryAssembly().Location;
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
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
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