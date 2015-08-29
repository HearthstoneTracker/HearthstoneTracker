using System;
using HearthCap.Data;

namespace HearthCap.Core.GameCapture.HS.Events
{
    public class GameStarted : GameEvent
    {
        public DateTime StartTime { get; protected set; }

        public string Hero { get; protected set; }

        public string OpponentHero { get; protected set; }

        public GameMode GameMode { get; protected set; }

        public bool GoFirst { get; protected set; }

        public string Deck { get; set; }

        public GameStarted(GameMode gameMode, DateTime startTime, string hero, string opponentHero, bool goFirst, string lastDeck)
            : base("Detected started game")
        {
            GameMode = gameMode;
            StartTime = startTime;
            Hero = hero;
            OpponentHero = opponentHero;
            GoFirst = goFirst;
            Deck = lastDeck;
        }
    }
}
