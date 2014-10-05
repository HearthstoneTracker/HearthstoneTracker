// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommonSettings.cs" company="">
//   
// </copyright>
// <summary>
//   The common settings.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.SlowOpponent
{
    using HearthCap.Shell.UserPreferences;

    /// <summary>
    /// The common settings.
    /// </summary>
    public class CommonSettings : RegistrySettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommonSettings"/> class.
        /// </summary>
        public CommonSettings()
            : this(string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommonSettings"/> class.
        /// </summary>
        /// <param name="subsection">
        /// The subsection.
        /// </param>
        public CommonSettings(string subsection)
            : base(@"Software\HearthstoneTracker" + (subsection.EndsWith("\\") ? subsection : subsection + "\\"))
        {
        }
    }
}