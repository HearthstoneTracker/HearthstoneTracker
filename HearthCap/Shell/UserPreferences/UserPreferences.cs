// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserPreferences.cs" company="">
//   
// </copyright>
// <summary>
//   The user preferences.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Shell.UserPreferences
{
    using System.ComponentModel;
    using System.ComponentModel.Composition;
    using System.Reflection;
    using System.Threading;
    using System.Windows;

    using Caliburn.Micro;

    using Microsoft.Win32;

    /// <summary>
    /// The user preferences.
    /// </summary>
    [Export(typeof(UserPreferences))]
    public class UserPreferences : PropertyChangedBase
    {
        #region Member Variables

        /// <summary>
        /// The window top.
        /// </summary>
        private double windowTop;

        /// <summary>
        /// The window left.
        /// </summary>
        private double windowLeft;

        /// <summary>
        /// The window height.
        /// </summary>
        private double windowHeight;

        /// <summary>
        /// The window width.
        /// </summary>
        private double windowWidth;

        /// <summary>
        /// The window state.
        /// </summary>
        private WindowState windowState;

        /// <summary>
        /// The dont save.
        /// </summary>
        private bool dontSave;

        /// <summary>
        /// The timer.
        /// </summary>
        private Timer timer;

        /// <summary>
        /// The timer lock.
        /// </summary>
        private readonly object timerLock = new object();

        /// <summary>
        /// The start minimized.
        /// </summary>
        private bool startMinimized;

        /// <summary>
        /// The start on logon.
        /// </summary>
        private bool startOnLogon;

        /// <summary>
        /// The minimize to tray.
        /// </summary>
        private bool minimizeToTray;

        /// <summary>
        /// Gets or sets the window top.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the window left.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the window height.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the window width.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the window state.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether start minimized.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether start on logon.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether minimize to tray.
        /// </summary>
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
                this.NotifyOfPropertyChange(() => this.MinimizeToTray);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserPreferences"/> class.
        /// </summary>
        [ImportingConstructor]
        public UserPreferences()
        {
            this.Load();
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        public void Initialize()
        {
            this.dontSave = true;
            this.PropertyChanged += this.UserPreferences_PropertyChanged;
            this.SizeToFit();
            this.MoveIntoView();
            this.dontSave = false;
            this.Save();
        }

        /// <summary>
        /// The user preferences_ property changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        void UserPreferences_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // just catch all as we don't have other properties yet.
            if (this.timer == null)
            {
                lock (this.timerLock)
                {
                    if (this.timer == null)
                    {
                        this.timer = new Timer(this.TimerOnElapsed, null, 250, -1);
                    }
                }
            }
            else
            {
                this.timer.Change(250, -1);
            }
        }

        /// <summary>
        /// The timer on elapsed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        private void TimerOnElapsed(object sender)
        {
            this.Save();
            if (this.timer != null)
            {
                this.timer.Dispose();
                this.timer = null;                
            }
        }

        /// <summary>
        /// The size to fit.
        /// </summary>
        public void SizeToFit()
        {
            if (this.WindowHeight > SystemParameters.VirtualScreenHeight)
            {
                this.WindowHeight = SystemParameters.VirtualScreenHeight;
            }

            if (this.WindowWidth > SystemParameters.VirtualScreenWidth)
            {
                this.WindowWidth = SystemParameters.VirtualScreenWidth;
            }
        }

        /// <summary>
        /// The move into view.
        /// </summary>
        public void MoveIntoView()
        {
            var virtualScreenHeight = SystemParameters.VirtualScreenHeight;
            if (this.WindowTop + this.WindowHeight / 2 > virtualScreenHeight)
            {
                this.WindowTop = virtualScreenHeight - this.windowHeight;
            }

            var virtualScreenWidth = SystemParameters.VirtualScreenWidth;
            if (this.WindowLeft + this.WindowWidth / 2 > virtualScreenWidth)
            {
                this.WindowLeft = virtualScreenWidth - this.WindowWidth;
            }

            // Center on first load
            if (this.WindowTop < 0)
            {
                this.WindowTop = (virtualScreenHeight - this.WindowHeight) / 2;
            }

            if (this.WindowLeft < 0)
            {
                this.WindowLeft = (SystemParameters.PrimaryScreenWidth - this.WindowWidth) / 2;
            }
        }

        /// <summary>
        /// The load.
        /// </summary>
        private void Load()
        {
            using (var reg = new WindowRegistrySettings())
            {
                // On first startup
                if (reg.WindowHeight == 0 || reg.WindowWidth == 0)
                {
                    this.Save();
                    return;
                }

                this.WindowTop = reg.WindowTop;
                this.WindowLeft = reg.WindowLeft;
                this.WindowHeight = reg.WindowHeight;
                this.WindowWidth = reg.WindowWidth;
                this.WindowState = reg.WindowState;
                this.StartMinimized = reg.StartMinimized;
                this.MinimizeToTray = reg.MinimizeToTray;

                using (var section = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
                {
                    if (section != null)
                    {
                        var startupLocation = section.GetValue("HearthstoneTracker");
                        if (startupLocation != null)
                        {
                            this.StartOnLogon = true;

                            // Verify location still correct:
                            var exeName = Assembly.GetEntryAssembly().Location;
                            var value = string.Format("\"{0}\" -logon", exeName);
                            if (startupLocation.ToString() != value)
                            {
                                section.SetValue("HearthstoneTracker", value);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The save.
        /// </summary>
        public void Save()
        {
            if (this.WindowState != WindowState.Minimized && !this.dontSave)
            {
                using (var reg = new WindowRegistrySettings())
                {
                    if (this.WindowState != WindowState.Maximized)
                    {
                        reg.WindowTop = this.WindowTop;
                        reg.WindowLeft = this.WindowLeft;
                        reg.WindowHeight = this.WindowHeight;
                        reg.WindowWidth = this.WindowWidth;
                    }

                    reg.StartMinimized = this.StartMinimized;
                    reg.MinimizeToTray = this.MinimizeToTray;
                    reg.WindowState = this.WindowState;

                    using (var section = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
                    {
                        if (section != null)
                        {
                            if (this.StartOnLogon)
                            {
                                var exeName = Assembly.GetEntryAssembly().Location;
                                var value = string.Format("\"{0}\" -logon", exeName);
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

    }
}