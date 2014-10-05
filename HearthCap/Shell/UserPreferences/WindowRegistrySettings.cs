// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WindowRegistrySettings.cs" company="">
//   
// </copyright>
// <summary>
//   The window registry settings.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Shell.UserPreferences
{
    using System.Windows;

    /// <summary>
    /// The window registry settings.
    /// </summary>
    public class WindowRegistrySettings : RegistrySettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WindowRegistrySettings"/> class.
        /// </summary>
        public WindowRegistrySettings()
            : base(@"Software\HearthstoneTracker")
        {
        }

        /// <summary>
        /// Gets or sets the window top.
        /// </summary>
        public double WindowTop
        {
            get
            {
                return this.GetOrCreate("WindowTop", (double)-1);
            }

            set
            {
                this.SetValue("WindowTop", value);
            }
        }

        /// <summary>
        /// Gets or sets the window left.
        /// </summary>
        public double WindowLeft
        {
            get
            {
                return this.GetOrCreate("WindowLeft", (double)-1);
            }

            set
            {
                this.SetValue("WindowLeft", value);
            }
        }

        /// <summary>
        /// Gets or sets the window height.
        /// </summary>
        public double WindowHeight
        {
            get
            {
                return this.GetOrCreate("WindowHeight", (double)600);
            }

            set
            {
                this.SetValue("WindowHeight", value);
            }
        }

        /// <summary>
        /// Gets or sets the window width.
        /// </summary>
        public double WindowWidth
        {
            get
            {
                return this.GetOrCreate("WindowWidth", (double)800);
            }

            set
            {
                this.SetValue("WindowWidth", value);
            }
        }

        /// <summary>
        /// Gets or sets the window state.
        /// </summary>
        public WindowState WindowState
        {
            get
            {
                return this.GetOrCreate("WindowState", WindowState.Normal);
            }

            set
            {
                this.SetValue("WindowState", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether start minimized.
        /// </summary>
        public bool StartMinimized
        {
            get
            {
                return this.GetOrCreate("StartMinimized", 0) == 1;
            }

            set
            {
                this.SetValue("StartMinimized", value ? 1 : 0);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether minimize to tray.
        /// </summary>
        public bool MinimizeToTray
        {
            get
            {
                return this.GetOrCreate("MinimizeToTray", 0) == 1;
            }

            set
            {
                this.SetValue("MinimizeToTray", value ? 1 : 0);
            }
        }
    }
}