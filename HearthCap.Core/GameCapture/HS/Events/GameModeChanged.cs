using HearthCap.Data;

namespace HearthCap.Core.GameCapture.HS.Events
{
    public class GameModeChanged : GameEvent
    {
        public GameMode OldGameMode { get; protected set; }

        public GameMode GameMode { get; protected set; }

        public GameModeChanged(GameMode oldGameMode, GameMode gameMode)
            : base(string.Format("Mode changed from {0} to {1}.", oldGameMode, gameMode))
        {
            OldGameMode = oldGameMode;
            GameMode = gameMode;
        }
    }
}
