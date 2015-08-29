using System;

namespace HearthCap.Features.GameManager.Events
{
    public class ArenaSessionUpdated
    {
        public Guid ArenaSessionId { get; set; }

        public bool IsLatest { get; set; }

        public ArenaSessionUpdated(Guid arenaSessionId, bool isLatest = false)
        {
            ArenaSessionId = arenaSessionId;
            IsLatest = isLatest;
        }
    }
}
