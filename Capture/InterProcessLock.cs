namespace Capture
{
    using System;
    using System.Security.AccessControl;
    using System.Security.Principal;
    using System.Threading;

    public class InterProcessLock : IDisposable
    {
        public Mutex Mutex { get; private set; }

        public bool IsAcquired { get; private set; }

        public InterProcessLock(string name, TimeSpan timeout)
        {
            bool created;
            var security = new MutexSecurity();
            security.AddAccessRule(new MutexAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), MutexRights.Synchronize | MutexRights.Modify, AccessControlType.Allow));
            this.Mutex = new Mutex(false, name, out created, security);
            this.IsAcquired = this.Mutex.WaitOne(timeout);
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (this.IsAcquired)
            {
                this.Mutex.ReleaseMutex();
                this.IsAcquired = false;
            }
        }

        #endregion

        public static bool TryCreate(string name, TimeSpan timeout, out InterProcessLock thelock)
        {
            thelock = new InterProcessLock(name, timeout);
            return thelock.IsAcquired;
        }

        public static bool TryCreate(string name, int timeout, out InterProcessLock thelock)
        {
            thelock = new InterProcessLock(name, TimeSpan.FromMilliseconds(timeout));
            return thelock.IsAcquired;
        }
    }
}