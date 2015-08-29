using HearthCap.Features.Games.Models;

namespace HearthCap.Features.GameManager.Events
{
    public class ArenaSessionAdded
    {
        public ArenaSessionModel ArenaSession { get; set; }

        public ArenaSessionAdded(ArenaSessionModel arenaSession)
        {
            ArenaSession = arenaSession;
        }
    }
}
