namespace HearthCap.Util
{
    using System;
    using System.Collections.Generic;

    using Action = System.Action;

    public static class PauseNotify
    {
        private static readonly IDictionary<object, Releaser> Releasers = new Dictionary<object, Releaser>();

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

        public static IDisposable For<T>(T obj)
        {
            var rel = new Releaser(obj);
            Releasers[obj] = rel;
            return rel;
        }

        public static bool IsPaused(object obj)
        {
            return Releasers.ContainsKey(obj);
        }

        internal class Releaser : IDisposable
        {
            private readonly object pausedObject;

            public Releaser(object pausedObject)
            {
                this.pausedObject = pausedObject;
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                PauseNotify.Releasers.Remove(this.pausedObject);
            }
        }
    }

}