using System;

namespace HearthCap.Features.GameManager.Events
{
    public class GameResultDeleted
    {
        public Guid GameId { get; set; }
        public Guid? ArenaId { get; set; }

        public GameResultDeleted(Guid gameId, Guid? arenaId = null)
        {
            GameId = gameId;
            ArenaId = arenaId;
        }
    }
}
