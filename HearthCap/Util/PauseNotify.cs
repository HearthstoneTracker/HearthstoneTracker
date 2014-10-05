// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PauseNotify.cs" company="">
//   
// </copyright>
// <summary>
//   The pause notify.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Util
{
    using System;
    using System.Collections.Generic;

    using Action = System.Action;

    /// <summary>
    /// The pause notify.
    /// </summary>
    public static class PauseNotify
    {
        /// <summary>
        /// The releasers.
        /// </summary>
        private static readonly IDictionary<object, Releaser> Releasers = new Dictionary<object, Releaser>();

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        public static void Execute<T>(T obj, Action action)
        {
            Caliburn.Micro.Execute.OnUIThread(
                () =>
                    {
                        using (For(obj))
                        {
                            action();
                        }
                    });
        }

        /// <summary>
        /// The for.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="IDisposable"/>.
        /// </returns>
        public static IDisposable For<T>(T obj)
        {
            var rel = new Releaser(obj);
            Releasers[obj] = rel;
            return rel;
        }

        /// <summary>
        /// The is paused.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsPaused(object obj)
        {
            return Releasers.ContainsKey(obj);
        }

        /// <summary>
        /// The releaser.
        /// </summary>
        internal class Releaser : IDisposable
        {
            /// <summary>
            /// The paused object.
            /// </summary>
            private readonly object pausedObject;

            /// <summary>
            /// Initializes a new instance of the <see cref="Releaser"/> class.
            /// </summary>
            /// <param name="pausedObject">
            /// The paused object.
            /// </param>
            public Releaser(object pausedObject)
            {
                this.pausedObject = pausedObject;
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                Releasers.Remove(this.pausedObject);
            }
        }
    }

}