// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogLevel.cs" company="">
//   
// </copyright>
// <summary>
//   The log level.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Core.GameCapture.Logging
{
    using System;

    /// <summary>
    /// The log level.
    /// </summary>
    [Flags]
    public enum LogLevel
    {
        /// <summary>
        /// The none.
        /// </summary>
        None = 0, 

        /// <summary>
        /// The error.
        /// </summary>
        Error = 1, 

        /// <summary>
        /// The warn.
        /// </summary>
        Warn = 2, 

        /// <summary>
        /// The info.
        /// </summary>
        Info = 4, 

        /// <summary>
        /// The diag.
        /// </summary>
        Diag = 8, 

        /// <summary>
        /// The debug.
        /// </summary>
        Debug = 16, 

        /// <summary>
        /// The all.
        /// </summary>
        All = Error | Warn | Info | Diag | Debug, 
    }
}