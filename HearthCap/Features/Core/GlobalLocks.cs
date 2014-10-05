// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GlobalLocks.cs" company="">
//   
// </copyright>
// <summary>
//   The global locks.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Core
{
    using System.Threading;

    /// <summary>
    /// The global locks.
    /// </summary>
    public static class GlobalLocks
    {
        /// <summary>
        /// The new arena lock.
        /// </summary>
        public static ManualResetEventSlim NewArenaLock = new ManualResetEventSlim(true);
    }
}