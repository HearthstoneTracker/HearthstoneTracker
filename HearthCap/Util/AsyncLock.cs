// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsyncLock.cs" company="">
//   
// </copyright>
// <summary>
//   The async lock.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Util
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// The async lock.
    /// </summary>
    public sealed class AsyncLock
    {
        /// <summary>
        /// The semaphore.
        /// </summary>
        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        /// <summary>
        /// The releaser.
        /// </summary>
        private readonly Task<IDisposable> releaser;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncLock"/> class.
        /// </summary>
        public AsyncLock()
        {
            this.releaser = Task.FromResult((IDisposable)new Releaser(this));
        }

        /// <summary>
        /// The lock async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public Task<IDisposable> LockAsync()
        {
            var wait = this.semaphore.WaitAsync();
            return wait.IsCompleted
                       ? this.releaser
                       : wait.ContinueWith(
                           (_, state) => (IDisposable)state, 
                           this.releaser.Result, 
                           CancellationToken.None, 
                           TaskContinuationOptions.ExecuteSynchronously, 
                           TaskScheduler.Default);
        }

        /// <summary>
        /// The releaser.
        /// </summary>
        private sealed class Releaser : IDisposable
        {
            /// <summary>
            /// The lock to release.
            /// </summary>
            private readonly AsyncLock lockToRelease;

            /// <summary>
            /// Initializes a new instance of the <see cref="Releaser"/> class.
            /// </summary>
            /// <param name="toRelease">
            /// The to release.
            /// </param>
            internal Releaser(AsyncLock toRelease)
            {
                this.lockToRelease = toRelease;
            }

            /// <summary>
            /// The dispose.
            /// </summary>
            public void Dispose()
            {
                this.lockToRelease.semaphore.Release();
            }
        }
    }
}