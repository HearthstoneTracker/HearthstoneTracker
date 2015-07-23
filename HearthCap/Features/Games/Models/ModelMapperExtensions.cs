namespace HearthCap.Features.Games.Models
{
    using System.Collections.Generic;
    using System.Linq;

    using Caliburn.Micro;

    using HearthCap.Data;
    using HearthCap.Features.Decks;
    using HearthCap.Features.Decks.ModelMappers;

    using Omu.ValueInjecter;

    public static class ModelMapperExtensions
    {
        public static ArenaSessionModel ToModel(this ArenaSession arena, int arenaSessionRecurse = 0)
        {
            if (arena == null) return null;
            ArenaSessionModel model = null;
            model = new ArenaSessionModel();
            model.InjectFrom(arena);
            model.Games.AddRange(arena.Games.Select(x => x.ToModel(arenaSessionRecurse, model)));
            return model;
        }

        public static GameResultModel ToModel(this GameResult game, int arenaSessionRecurse = 0, ArenaSessionModel arenaSessionModel = null)
        {
            if (game == null) return null;
            GameResultModel model = null;
            model = new GameResultModel();
            model.InjectFrom(game);
            if (arenaSessionRecurse <= 1)
            {
                arenaSessionRecurse++;
                model.ArenaSession = arenaSessionModel ?? game.ArenaSession.ToModel(arenaSessionRecurse);
            }
            return model;
        }

        public static IEnumerable<ArenaSessionModel> ToModel(this IEnumerable<ArenaSession> source)
        {
            return source.Select(x => x.ToModel());
        }

        public static IEnumerable<GameResultModel> ToModel(this IEnumerable<GameResult> source)
        {
            return source.Select(x => x.ToModel());
        }

        public static void MapFrom(this ArenaSessionModel target, ArenaSessionModel source)
        {
            target.InjectFrom(source);
            // target.Games.IsNotifying = false;
            target.Games.Clear();
            target.Games.AddRange(source.Games);
            // target.Games.IsNotifying = true;
            // target.Games.Refresh();
        }

        public static void MapFrom(this GameResultModel target, GameResultModel source)
        {
            target.InjectFrom(source);
        }

        public static void MapFrom(this ArenaSessionModel target, ArenaSession source)
        {
            target.InjectFrom(source);
            // target.Games.IsNotifying = false;
            target.Games.Clear();
            target.Games.AddRange(source.Games.Select(g => g.ToModel()));
            // target.Games.IsNotifying = true;
            // target.Games.Refresh();            
        }

        public static void MapFrom(this GameResultModel target, GameResult source)
        {
            target.InjectFrom(source);
            target.ArenaSession = source.ArenaSession.ToModel();
        }

        public static void MapFrom(this Deck target, DeckModel source)
        {
            target.InjectFrom(source);
        }

        public static void MapFrom(this DeckModel target, Deck source)
        {
            target.InjectFrom(source);
        }
    }
}