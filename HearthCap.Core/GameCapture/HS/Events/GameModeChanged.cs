namespace HearthCap.Core.GameCapture.HS.Events
{
    using HearthCap.Data;

    public class GameModeChanged : GameEvent
    {
        public GameMode OldGameMode { get; protected set; }

        public GameMode GameMode { get; protected set; }

        public GameModeChanged(GameMode oldGameMode, GameMode gameMode)
            : base(string.Format("Mode changed from {0} to {1}.", oldGameMode, gameMode))
        {
            this.OldGameMode = oldGameMode;
            this.GameMode = gameMode;
        }
    }
}