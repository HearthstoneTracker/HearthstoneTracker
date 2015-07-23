namespace HearthCap.Features.Core
{
    using System.Threading;

    public static class GlobalLocks
    {
        public static ManualResetEventSlim NewArenaLock = new ManualResetEventSlim(true);
    }
}