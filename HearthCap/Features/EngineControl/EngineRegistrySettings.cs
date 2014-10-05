// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EngineRegistrySettings.cs" company="">
//   
// </copyright>
// <summary>
//   The engine registry settings.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.EngineControl
{
    using HearthCap.Core.GameCapture;
    using HearthCap.Shell.UserPreferences;

    /// <summary>
    /// The engine registry settings.
    /// </summary>
    public class EngineRegistrySettings : RegistrySettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EngineRegistrySettings"/> class.
        /// </summary>
        public EngineRegistrySettings()
            : base(@"Software\HearthstoneTracker\")
        {
        }

        /// <summary>
        /// Gets or sets the capture method.
        /// </summary>
        public CaptureMethod CaptureMethod
        {
            get
            {
                return this.GetOrCreate("CaptureMethod", CaptureMethod.AutoDetect);
            }

            set
            {
                this.SetValue("CaptureMethod", value);
            }
        }

        /// <summary>
        /// Gets or sets the speed.
        /// </summary>
        public int Speed
        {
            get
            {
                return this.GetOrCreate("Speed", (int)Speeds.Default);
            }

            set
            {
                this.SetValue("Speed", value);
            }
        }
    }
}