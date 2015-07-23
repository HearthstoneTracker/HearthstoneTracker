namespace HearthCap.Features.GameManager.Events
{
    using HearthCap.Features.Games.Models;

    public class ArenaSessionDeleted
    {
        public ArenaSessionModel ArenaSession { get; set; }

        public ArenaSessionDeleted(ArenaSessionModel arenaSession)
        {
            this.ArenaSession = arenaSession;
        }
    }
}