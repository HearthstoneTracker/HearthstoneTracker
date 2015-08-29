using System;

namespace HearthCap.Features.GameManager.Events
{
    public class GameResultUpdated
    {
        public Guid GameResultId { get; set; }

        public Guid? ArenaSessionId { get; set; }

        public GameResultUpdated(Guid gameResultId, Guid? arenaSessionId)
        {
            GameResultId = gameResultId;
            ArenaSessionId = arenaSessionId;
        }
    }
}
