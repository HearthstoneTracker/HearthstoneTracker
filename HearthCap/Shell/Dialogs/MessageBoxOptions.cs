// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageBoxOptions.cs" company="">
//   
// </copyright>
// <summary>
//   The message box options.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Shell.Dialogs
{
    using System;

    /// <summary>
    /// The message box options.
    /// </summary>
    [Flags]
    public enum MessageBoxOptions
    {
        /// <summary>
        /// The ok.
        /// </summary>
        Ok = 2, 

        /// <summary>
        /// The cancel.
        /// </summary>
        Cancel = 4, 

        /// <summary>
        /// The yes.
        /// </summary>
        Yes = 8, 

        /// <summary>
        /// The no.
        /// </summary>
        No = 16, 

        /// <summary>
        /// The ok cancel.
        /// </summary>
        OkCancel = Ok | Cancel, 

        /// <summary>
        /// The yes no.
        /// </summary>
        YesNo = Yes | No, 

        /// <summary>
        /// The yes no cancel.
        /// </summary>
        YesNoCancel = Yes | No | Cancel
    }
}