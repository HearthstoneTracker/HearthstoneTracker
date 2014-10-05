// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAppLogManager.cs" company="">
//   
// </copyright>
// <summary>
//   The AppLogManager interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Logging
{
    using System;

    /// <summary>
    /// The AppLogManager interface.
    /// </summary>
    public interface IAppLogManager : IDisposable
    {
        /// <summary>
        /// The flush.
        /// </summary>
        void Flush();

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="logFilesDirectory">
        /// The log files directory.
        /// </param>
        void Initialize(string logFilesDirectory);
    }
}