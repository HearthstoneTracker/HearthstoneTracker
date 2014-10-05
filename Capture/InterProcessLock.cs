// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InterProcessLock.cs" company="">
//   
// </copyright>
// <summary>
//   The inter process lock.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Capture
{
    using System;
    using System.Security.AccessControl;
    using System.Security.Principal;
    using System.Threading;

    /// <summary>
    /// The inter process lock.
    /// </summary>
    public class InterProcessLock : IDisposable
    {
        /// <summary>
        /// Gets the mutex.
        /// </summary>
        public Mutex Mutex { get; private set; }

        /// <summary>
        /// Gets a value indicating whether is acquired.
        /// </summary>
        public bool IsAcquired { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InterProcessLock"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="timeout">
        /// The timeout.
        /// </param>
        public InterProcessLock(string name, TimeSpan timeout)
        {
            bool created;
            var security = new MutexSecurity();
            security.AddAccessRule(new MutexAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), MutexRights.Synchronize | MutexRights.Modify, AccessControlType.Allow));
            this.Mutex = new Mutex(false, name, out created, security);
            this.IsAcquired = this.Mutex.WaitOne(timeout);
        }

        #region IDisposable Members

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            if (this.IsAcquired)
            {
                this.Mutex.ReleaseMutex();
                this.IsAcquired = false;
            }
        }

        #endregion

        /// <summary>
        /// The try create.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="timeout">
        /// The timeout.
        /// </param>
        /// <param name="thelock">
        /// The thelock.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool TryCreate(string name, TimeSpan timeout, out InterProcessLock thelock)
        {
            thelock = new InterProcessLock(name, timeout);
            return thelock.IsAcquired;
        }

        /// <summary>
        /// The try create.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="timeout">
        /// The timeout.
        /// </param>
        /// <param name="thelock">
        /// The thelock.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool TryCreate(string name, int timeout, out InterProcessLock thelock)
        {
            thelock = new InterProcessLock(name, TimeSpan.FromMilliseconds(timeout));
            return thelock.IsAcquired;
        }
    }
}