namespace HearthCap.Core.GameCapture.HS.Events
{
    public class ArenaWinsDetected : GameEvent
    {
        public int Wins { get; protected set; }

        public ArenaWinsDetected(int arenaWins)
            : base("Wins: " + arenaWins)
        {
            this.Wins = arenaWins;
        }
    }
}