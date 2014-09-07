namespace HearthCap.Core.GameCapture.HS.Events
{
    public class DeckDetected : GameEvent
    {
        public string Key { get; protected set; }

        public DeckDetected(string key)
            :base("Deck detected: " + key)
        {
            this.Key = key;
        }
    }
}