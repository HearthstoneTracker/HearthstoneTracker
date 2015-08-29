using HearthCap.Features.Games.Models;

namespace HearthCap.Features.GameManager.Events
{
    public class GameResultAdded
    {
        public object Source { get; set; }

        public GameResultModel GameResult { get; set; }

        public GameResultAdded(object source, GameResultModel gameResult)
        {
            Source = source;
            GameResult = gameResult;
        }
    }
}
