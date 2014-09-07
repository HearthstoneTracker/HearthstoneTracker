namespace HearthCap.Core.GameCapture.HS.Events
{
    /// <summary>The game started.</summary>
    public class CoinDetected : GameEvent
    {
        public CoinDetected(bool goFirst)
            : base("Coin detected: " + (goFirst ? "gofirst" : "gosecond"))
        {
            this.GoFirst = goFirst;
        }

        public bool GoFirst { get; protected set; }
    }
}