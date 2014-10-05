// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Speeds.cs" company="">
//   
// </copyright>
// <summary>
//   The speeds.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Core.GameCapture
{
    /// <summary>
    /// The speeds.
    /// </summary>
    public enum Speeds 
    {
        /// <summary>
        /// The slow.
        /// </summary>
        Slow = 1000 / 5, // 200 ms

        /// <summary>
        /// The medium.
        /// </summary>
        Medium = 1000 / 10, // 100 ms

        /// <summary>
        /// The fast.
        /// </summary>
        Fast = 1000 / 20, // 50 ms

        /// <summary>
        /// The very fast.
        /// </summary>
        VeryFast = 1000 / 30, // 33.3 ms

        /// <summary>
        /// The insanely fast.
        /// </summary>
        InsanelyFast = 1000 / 60, // 16.6 ms

        /// <summary>
        /// The no delay.
        /// </summary>
        NoDelay = 0, 

        /// <summary>
        /// The default.
        /// </summary>
        Default = VeryFast
    }
}