namespace HearthCap.Core.GameCapture.HS.Events
{
    public class NewRound : GameEvent
    {
        public int Current { get; protected set; }

        public bool MyTurn { get; set; }

        public NewRound(int current, bool myTurn = false)
            : base(string.Format("New game round (current: {0})", current))
        {
            this.Current = current;
            this.MyTurn = myTurn;
        }
    }
}