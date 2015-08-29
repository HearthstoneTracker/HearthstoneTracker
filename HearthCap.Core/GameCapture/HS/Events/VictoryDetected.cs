namespace HearthCap.Core.GameCapture.HS.Events
{
    public class VictoryDetected : GameEvent
    {
        public bool IsVictory { get; set; }

        public bool Conceded { get; set; }

        public VictoryDetected(bool victory, bool conceded = false)
            : base(string.Format("Victory detected: {0} {1}", victory ? "won" : "lost", conceded ? " (conceded)" : ""))
        {
            IsVictory = victory;
            Conceded = conceded;
        }
    }
}
