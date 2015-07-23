namespace HearthCap.Features.GameManager.Events
{
    using System;

    public class GameResultDeleted
    {
        public Guid GameId { get; set; }
        public Guid? ArenaId { get; set; }

        public GameResultDeleted(Guid gameId, Guid? arenaId = null)
        {
            this.GameId = gameId;
            this.ArenaId = arenaId;
        }
    }
}