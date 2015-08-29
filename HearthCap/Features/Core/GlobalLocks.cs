using System.Threading;

namespace HearthCap.Features.Core
{
    public static class GlobalLocks
    {
        public static ManualResetEventSlim NewArenaLock = new ManualResetEventSlim(true);
    }
}
