namespace HearthCap.Features.GameManager.Events
{
    using HearthCap.Features.Games.Models;

    public class GameResultAdded
    {
        public object Source { get; set; }

        public GameResultModel GameResult { get; set; }

        public GameResultAdded(object source, GameResultModel gameResult)
        {
            this.Source = source;
            this.GameResult = gameResult;
        }
    }
}