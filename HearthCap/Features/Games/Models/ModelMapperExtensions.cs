﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelMapperExtensions.cs" company="">
//   
// </copyright>
// <summary>
//   The model mapper extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Games.Models
{
    using System.Collections.Generic;
    using System.Linq;

    using HearthCap.Data;
    using HearthCap.Features.Decks;

    using Omu.ValueInjecter;

    /// <summary>
    /// The model mapper extensions.
    /// </summary>
    public static class ModelMapperExtensions
    {
        /// <summary>
        /// The to model.
        /// </summary>
        /// <param name="arena">
        /// The arena.
        /// </param>
        /// <param name="arenaSessionRecurse">
        /// The arena session recurse.
        /// </param>
        /// <returns>
        /// The <see cref="ArenaSessionModel"/>.
        /// </returns>
        public static ArenaSessionModel ToModel(this ArenaSession arena, int arenaSessionRecurse = 0)
        {
            if (arena == null) return null;
            ArenaSessionModel model = null;
            model = new ArenaSessionModel();
            model.InjectFrom(arena);
            model.Games.AddRange(arena.Games.Select(x => x.ToModel(arenaSessionRecurse, model)));
            return model;
        }

        /// <summary>
        /// The to model.
        /// </summary>
        /// <param name="game">
        /// The game.
        /// </param>
        /// <param name="arenaSessionRecurse">
        /// The arena session recurse.
        /// </param>
        /// <param name="arenaSessionModel">
        /// The arena session model.
        /// </param>
        /// <returns>
        /// The <see cref="GameResultModel"/>.
        /// </returns>
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

        /// <summary>
        /// The to model.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public static IEnumerable<ArenaSessionModel> ToModel(this IEnumerable<ArenaSession> source)
        {
            return source.Select(x => x.ToModel());
        }

        /// <summary>
        /// The to model.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public static IEnumerable<GameResultModel> ToModel(this IEnumerable<GameResult> source)
        {
            return source.Select(x => x.ToModel());
        }

        /// <summary>
        /// The map from.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="source">
        /// The source.
        /// </param>
        public static void MapFrom(this ArenaSessionModel target, ArenaSessionModel source)
        {
            target.InjectFrom(source);

            // target.Games.IsNotifying = false;
            target.Games.Clear();
            target.Games.AddRange(source.Games);

            // target.Games.IsNotifying = true;
            // target.Games.Refresh();
        }

        /// <summary>
        /// The map from.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="source">
        /// The source.
        /// </param>
        public static void MapFrom(this GameResultModel target, GameResultModel source)
        {
            target.InjectFrom(source);
        }

        /// <summary>
        /// The map from.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="source">
        /// The source.
        /// </param>
        public static void MapFrom(this ArenaSessionModel target, ArenaSession source)
        {
            target.InjectFrom(source);

            // target.Games.IsNotifying = false;
            target.Games.Clear();
            target.Games.AddRange(source.Games.Select(g => g.ToModel()));

            // target.Games.IsNotifying = true;
            // target.Games.Refresh();            
        }

        /// <summary>
        /// The map from.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="source">
        /// The source.
        /// </param>
        public static void MapFrom(this GameResultModel target, GameResult source)
        {
            target.InjectFrom(source);
            target.ArenaSession = source.ArenaSession.ToModel();
        }

        /// <summary>
        /// The map from.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="source">
        /// The source.
        /// </param>
        public static void MapFrom(this Deck target, DeckModel source)
        {
            target.InjectFrom(source);
        }

        /// <summary>
        /// The map from.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="source">
        /// The source.
        /// </param>
        public static void MapFrom(this DeckModel target, Deck source)
        {
            target.InjectFrom(source);
        }
    }
}