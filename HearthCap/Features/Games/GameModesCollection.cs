using Caliburn.Micro;
using HearthCap.Data;

namespace HearthCap.Features.Games
{
    public class GameModesCollection : BindableCollection<GameMode>
    {
        public GameModesCollection()
            : base(new[]
                {
                    GameMode.Unknown,
                    GameMode.Arena,
                    GameMode.Casual,
                    GameMode.Challenge,
                    GameMode.Practice,
                    GameMode.Ranked,
                    GameMode.Mission,
                    GameMode.TavernBrawl
                })
        {
        }
    }
}
