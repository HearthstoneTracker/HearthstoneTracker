using HearthCap.Features.Games.Models;

namespace HearthCap.Features.Games
{
    public class SelectedGameChanged
    {
        public object Source { get; protected set; }

        public GameResultModel Game { get; protected set; }

        public SelectedGameChanged(object source, GameResultModel game)
        {
            Source = source;
            Game = game;
        }
    }
}
