namespace HearthCap.Features.GameManager.Events
{
    using HearthCap.Features.Games.Models;

    public class ArenaSessionAdded
    {
        public ArenaSessionModel ArenaSession { get; set; }

        public ArenaSessionAdded(ArenaSessionModel arenaSession)
        {
            this.ArenaSession = arenaSession;
        }
    }
}