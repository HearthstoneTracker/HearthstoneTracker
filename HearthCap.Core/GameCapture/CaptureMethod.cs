// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CaptureMethod.cs" company="">
//   
// </copyright>
// <summary>
//   The capture method.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Core.GameCapture
{
    /// <summary>
    /// The capture method.
    /// </summary>
    public enum CaptureMethod
    {
        /// <summary>
        /// The auto detect.
        /// </summary>
        AutoDetect, 

        /// <summary>
        /// The wdm.
        /// </summary>
        Wdm, 

        /// <summary>
        /// The direct x.
        /// </summary>
        DirectX, 

        /// <summary>
        /// The bit blt.
        /// </summary>
        BitBlt, 

        /// <summary>
        /// The log.
        /// </summary>
        Log
    }
}