using HearthCap.Features.Games.Models;

namespace HearthCap.Features.GameManager.Events
{
    public class ArenaSessionDeleted
    {
        public ArenaSessionModel ArenaSession { get; set; }

        public ArenaSessionDeleted(ArenaSessionModel arenaSession)
        {
            ArenaSession = arenaSession;
        }
    }
}
