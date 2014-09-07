namespace HearthCap.Features.GameManager.Events
{
    using System;

    public class ArenaSessionUpdated
    {
        public Guid ArenaSessionId { get; set; }

        public bool IsLatest { get; set; }

        public ArenaSessionUpdated(Guid arenaSessionId, bool isLatest = false)
        {
            this.ArenaSessionId = arenaSessionId;
            this.IsLatest = isLatest;
        }
    }
}