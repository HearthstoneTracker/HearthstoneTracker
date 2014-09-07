namespace HearthCap.Features.GameManager.Events
{
    using System;

    public class GameResultUpdated
    {
        public Guid GameResultId { get; set; }
        
        public Guid? ArenaSessionId { get; set; }

        public GameResultUpdated(Guid gameResultId, Guid? arenaSessionId)
        {
            this.GameResultId = gameResultId;
            this.ArenaSessionId = arenaSessionId;
        }
    }
}