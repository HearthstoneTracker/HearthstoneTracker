namespace HearthCap.Core.GameCapture.HS.Events
{
    public class ArenaLossesDetected : GameEvent
    {
        public int Losses { get; set; }

        public ArenaLossesDetected(int losses)
            : base("Losses: " + losses)

        {
            Losses = losses;
        }
    }
}
