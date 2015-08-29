using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using HearthCap.Data;
using HearthCap.Features.GameManager.Events;
using HearthCap.Features.Games.Models;
using Omu.ValueInjecter;
using LogManager = NLog.LogManager;

namespace HearthCap.Features.GameManager
{
    [Export(typeof(GameManager))]
    public class GameManager
    {
        private static readonly NLog.Logger Log = LogManager.GetCurrentClassLogger();

        private readonly Func<HearthStatsDbContext> dbContext;

        private readonly IEventAggregator events;

        [ImportingConstructor]
        public GameManager(Func<HearthStatsDbContext> dbContext, IEventAggregator events)
        {
            this.dbContext = dbContext;
            this.events = events;
        }

        public async Task AddGame(GameResultModel gameModel)
        {
            using (var context = dbContext())
            {
                var game = new GameResult();
                game.InjectFrom(gameModel);

                if (gameModel.Hero != null)
                {
                    game.Hero = context.Heroes.Find(gameModel.Hero.Id);
                }

                if (gameModel.OpponentHero != null)
                {
                    game.OpponentHero = context.Heroes.Find(gameModel.OpponentHero.Id);
                }

                ArenaSessionModel arenaModel = null;
                ArenaSession arena = null;
                if (gameModel.ArenaSession != null)
                {
                    gameModel.Deck = null;
                    game.DeckKey = null;
                    game.Deck = null;

                    arenaModel = gameModel.ArenaSession;
                    arena = context.ArenaSessions.Query().FirstOrDefault(x => x.Id == arenaModel.Id);
                    if (arena == null)
                    {
                        throw new InvalidOperationException("Add arena using gameManager first!");
                    }
                    // context.Entry(arena).CurrentValues.SetValues(arenaModel);

                    AddGameToArena(game, arena);
                    SetEndDateIfNeeded(arena);
                    arena.Modified = DateTime.Now;
                }

                if (gameModel.Deck != null)
                {
                    game.Deck = context.Decks.Find(gameModel.Deck.Id);
                }

                context.Games.Add(game);

                await context.SaveChangesAsync();

                gameModel.InjectFrom(game);
                events.PublishOnBackgroundThread(new GameResultAdded(this, gameModel));
                if (arenaModel != null)
                {
                    arenaModel.InjectFrom(arena);
                    gameModel.ArenaSession = arenaModel;
                    arenaModel.Games.Add(gameModel);
                    var latestId = context.ArenaSessions.OrderByDescending(x => x.StartDate).Select(x => x.Id).FirstOrDefault();
                    events.PublishOnBackgroundThread(new ArenaSessionUpdated(arenaModel.Id, latestId == arenaModel.Id));
                }
            }
        }

        public async Task DeleteGame(Guid id)
        {
            using (var context = dbContext())
            {
                var g = context.Games.Query().FirstOrDefault(x => x.Id == id);
                if (g == null)
                {
                    events.PublishOnBackgroundThread(new GameResultDeleted(id));
                    return;
                    // throw new ArgumentException(string.Format("Game with id '{0}' not found", id), "id");
                }

                // work around old bug
                var deleted = context.DeletedGames.Find(id);
                if (deleted != null)
                {
                    context.DeletedGames.Remove(deleted);
                    context.SaveChanges();
                }

                // move to deleted table
                deleted = new DeletedGameResult();
                deleted.InjectFrom(g);
                deleted.DeletedDate = DateTime.Now;
                context.DeletedGames.Add(deleted);

                var arenaId = g.ArenaSessionId;
                var victory = g.Victory;
                context.Games.Remove(g);
                await context.SaveChangesAsync();

                ArenaSession arena = null;
                if (arenaId != null)
                {
                    arena = context.ArenaSessions.Query().FirstOrDefault(x => x.Id == arenaId);
                    if (arena != null)
                    {
                        //if (victory && arena.IncompleteWins)
                        //{
                        //    arena.Wins--;
                        //}
                        //else if (!victory && arena.IncompleteLosses)
                        //{
                        //    arena.Losses--;
                        //}
                        if (victory)
                        {
                            arena.Wins--;
                        }
                        else
                        {
                            arena.Losses--;
                        }

                        arena.EndDate = null;
                        arena.Modified = DateTime.Now;
                    }
                }

                await context.SaveChangesAsync();
                events.PublishOnBackgroundThread(new GameResultDeleted(id, arenaId));
                if (arena != null)
                {
                    events.PublishOnBackgroundThread(new ArenaSessionUpdated(arena.Id));
                }
            }
        }

        public async Task MergeArenas(ArenaSessionModel source, ArenaSessionModel target)
        {
            if (source == null
                || target == null
                || source.Hero == null
                || target.Hero == null
                || source.Hero.Id != target.Hero.Id)
            {
                Log.Error("Error merging arenas, null parameters or heroes are not the same.");
                return;
            }

            using (var context = dbContext())
            {
                var sourceArena = context.ArenaSessions.Query().FirstOrDefault(x => x.Id == source.Id);
                var targetArena = context.ArenaSessions.Query().FirstOrDefault(x => x.Id == target.Id);
                if (sourceArena == null
                    || targetArena == null)
                {
                    Log.Error("Error merging arenas, source or target does not exist");
                    return;
                }

                var updatedGames = new List<GameResult>();
                foreach (var sourceGame in sourceArena.Games)
                {
                    AddGameToArena(sourceGame, targetArena);
                    updatedGames.Add(sourceGame);
                }

                for (var i = sourceArena.Games.Count - 1; i >= 0; i--)
                {
                    sourceArena.Games.Remove(sourceArena.Games[i]);
                }

                context.ArenaSessions.Remove(sourceArena);

                SetEndDateIfNeeded(targetArena);

                await context.SaveChangesAsync();

                target.MapFrom(targetArena);
                events.PublishOnBackgroundThread(new ArenaSessionDeleted(source));
                events.PublishOnBackgroundThread(new ArenaSessionUpdated(targetArena.Id));
                foreach (var game in updatedGames)
                {
                    events.PublishOnBackgroundThread(new GameResultUpdated(game.Id, game.ArenaSessionId));
                }
            }
        }

        public async Task UpdateGame(GameResultModel gameModel)
        {
            using (var context = dbContext())
            {
                var game = context.Games.FirstOrDefault(x => x.Id == gameModel.Id);
                if (game == null)
                {
                    throw new ArgumentException("game does not exist", "gameModel");
                }
                var oldVictory = game.Victory;
                if (gameModel.ArenaSession != null)
                {
                    gameModel.Deck = null;
                }

                context.Entry(game).CurrentValues.SetValues(gameModel);
                if (!Equals(gameModel.Hero, game.Hero)
                    && gameModel.Hero != null)
                {
                    game.Hero = context.Heroes.Find(gameModel.Hero.Id);
                }
                if (!Equals(gameModel.OpponentHero, game.OpponentHero)
                    && gameModel.OpponentHero != null)
                {
                    game.OpponentHero = context.Heroes.Find(gameModel.OpponentHero.Id);
                }
                if (!Equals(gameModel.Deck, game.Deck)
                    && gameModel.Deck != null)
                {
                    game.Deck = context.Decks.Find(gameModel.Deck.Id);
                }

                game.Modified = DateTime.Now;
                await context.SaveChangesAsync();

                ArenaSession arena = null;
                if (gameModel.ArenaSession != null)
                {
                    arena = context.ArenaSessions.Query().First(x => x.Id == game.ArenaSessionId);

                    if (game.Victory
                        && !oldVictory)
                    {
                        arena.Wins++;
                        arena.Losses--;
                    }
                    else if (!game.Victory && oldVictory)
                    {
                        arena.Wins--;
                        arena.Losses++;
                    }

                    SetEndDateIfNeeded(arena);

                    gameModel.ArenaSession.InjectFrom(arena);
                    arena.Modified = DateTime.Now;
                    await context.SaveChangesAsync();
                }

                gameModel.InjectFrom(game);
                events.PublishOnBackgroundThread(new GameResultUpdated(gameModel.Id, game.ArenaSessionId));
                if (gameModel.ArenaSession != null)
                {
                    events.PublishOnBackgroundThread(new ArenaSessionUpdated(gameModel.ArenaSession.Id));
                }
            }
        }

        public async Task<ArenaSessionModel> AddArenaSession(ArenaSessionModel arenaModel)
        {
            using (var context = dbContext())
            {
                var arena = new ArenaSession();
                arena.InjectFrom(arenaModel);
                if (arenaModel.Hero != null)
                {
                    arena.Hero = context.Heroes.Find(arenaModel.Hero.Id);
                }
                if (arenaModel.Image1 != null)
                {
                    var img = context.ArenaDeckImages.Find(arenaModel.Image1.Id);
                    if (img == null)
                    {
                        img = arenaModel.Image1;
                        context.ArenaDeckImages.Add(img);
                    }
                    context.Entry(img).CurrentValues.SetValues(arenaModel.Image1);
                    arena.Image1 = img;
                }
                if (arenaModel.Image2 != null)
                {
                    var img = context.ArenaDeckImages.Find(arenaModel.Image2.Id);
                    if (img == null)
                    {
                        img = arenaModel.Image2;
                        context.ArenaDeckImages.Add(img);
                    }
                    context.Entry(img).CurrentValues.SetValues(arenaModel.Image2);
                    arena.Image2 = img;
                }
                context.ArenaSessions.Add(arena);
                await context.SaveChangesAsync();
                arenaModel.InjectFrom(arena);
            }
            events.PublishOnBackgroundThread(new ArenaSessionAdded(arenaModel));
            return arenaModel;
        }

        public async Task DeleteArenaSession(Guid arenaId)
        {
            using (var context = dbContext())
            {
                var arena = context.ArenaSessions.Query().FirstOrDefault(x => x.Id == arenaId);
                if (arena == null)
                {
                    return;
                }
                var arenaModel = arena.ToModel();

                var deletedArena = new DeletedArenaSession();
                deletedArena.InjectFrom(arena);
                deletedArena.DeletedDate = DateTime.Now;
                foreach (var game in arena.Games)
                {
                    var deletedGame = new DeletedGameResult();
                    deletedGame.InjectFrom(game);
                    deletedGame.DeletedDate = DateTime.Now;
                    deletedArena.Games.Add(deletedGame);
                }

                context.Games.RemoveRange(arena.Games);
                context.DeletedArenaSessions.Add(deletedArena);

                if (arena.Image1 != null)
                {
                    var img = context.ArenaDeckImages.Find(arena.Image1.Id);
                    context.ArenaDeckImages.Remove(img);
                }
                if (arena.Image2 != null)
                {
                    var img = context.ArenaDeckImages.Find(arena.Image2.Id);
                    context.ArenaDeckImages.Remove(img);
                }
                context.ArenaSessions.Remove(arena);
                await context.SaveChangesAsync();
                events.PublishOnBackgroundThread(new ArenaSessionDeleted(arenaModel));
                foreach (var deletedGame in deletedArena.Games)
                {
                    events.PublishOnBackgroundThread(new GameResultDeleted(deletedGame.Id, deletedGame.ArenaSessionId));
                }
            }
        }

        public async Task UpdateArenaSession(ArenaSessionModel arenaModel)
        {
            using (var context = dbContext())
            {
                var arena = context.ArenaSessions.Query().FirstOrDefault(x => x.Id == arenaModel.Id);
                if (arena == null)
                {
                    throw new ArgumentException("arena does not exist", "arenaModel");
                }
                if (!Equals(arenaModel.Hero, arena.Hero))
                {
                    arena.Hero = context.Heroes.Find(arenaModel.Hero.Id);
                }
                if (arenaModel.Image1 != null)
                {
                    var img = context.ArenaDeckImages.Find(arenaModel.Image1.Id);
                    if (img == null)
                    {
                        img = arenaModel.Image1;
                        context.ArenaDeckImages.Add(img);
                    }
                    context.Entry(img).CurrentValues.SetValues(arenaModel.Image1);
                    arena.Image1 = img;
                }
                if (arenaModel.Image2 != null)
                {
                    var img = context.ArenaDeckImages.Find(arenaModel.Image2.Id);
                    if (img == null)
                    {
                        img = arenaModel.Image2;
                        context.ArenaDeckImages.Add(img);
                    }
                    context.Entry(img).CurrentValues.SetValues(arenaModel.Image2);
                    arena.Image2 = img;
                }

                context.Entry(arena).CurrentValues.SetValues(arenaModel);
                SetEndDateIfNeeded(arena);
                arena.Modified = DateTime.Now;
                await context.SaveChangesAsync();

                arena = context.ArenaSessions.Query().First(x => x.Id == arena.Id);
                var latestId = context.ArenaSessions.OrderByDescending(x => x.StartDate).Select(x => x.Id).FirstOrDefault();
                arenaModel.InjectFrom(arena);
                events.PublishOnBackgroundThread(new ArenaSessionUpdated(arenaModel.Id, latestId == arena.Id));
            }
        }

        private void SetEndDateIfNeeded(ArenaSession arena)
        {
            if (arena.IsEnded
                && arena.EndDate == null)
            {
                arena.EndDate = DateTime.Now;
            }

            if (!arena.IsEnded
                && arena.EndDate != null)
            {
                arena.EndDate = null;
            }
        }

        protected void AddGameToArena(GameResult game, ArenaSession arena)
        {
            game.ArenaSessionId = arena.Id;
            game.ArenaSession = arena;
            arena.Games.Add(game);

            if (game.Victory)
            {
                arena.Wins++;
            }
            else
            {
                arena.Losses++;
            }
        }

        public async Task AssignGameToArena(GameResultModel gameModel, ArenaSessionModel arenaModel)
        {
            using (var context = dbContext())
            {
                // var lastGame = context.Games.Where(x => x.ArenaSessionId == arenaModel.Id).OrderByDescending(x => x.ArenaGameNo).FirstOrDefault();
                var arena = context.ArenaSessions.Query().First(x => x.Id == arenaModel.Id);
                var game = context.Games.Query().First(x => x.Id == gameModel.Id);
                // game.ArenaGameNo = lastGame == null ? 1 : lastGame.ArenaGameNo + 1;

                AddGameToArena(game, arena);
                SetEndDateIfNeeded(arena);
                arena.Modified = DateTime.Now;
                game.Modified = DateTime.Now;
                await context.SaveChangesAsync();

                gameModel.InjectFrom(game);
                arenaModel.InjectFrom(arena);

                gameModel.ArenaSession = arenaModel;
                arenaModel.Games.Add(gameModel);
            }
        }

        public async Task Retire(ArenaSessionModel arenaModel)
        {
            using (var context = dbContext())
            {
                var arena = context.ArenaSessions.Query().First(x => x.Id == arenaModel.Id);
                arena.Retired = true;
                if (arena.EndDate == null)
                {
                    arena.EndDate = DateTime.Now;
                }
                arena.Modified = DateTime.Now;
                await context.SaveChangesAsync();
                arenaModel.InjectFrom(arena);
            }
        }

        public async Task MoveGameToArena(GameResultModel gameModel, ArenaSessionModel arenaModel)
        {
            using (var context = dbContext())
            {
                var sourceArena = context.ArenaSessions.Query().First(x => x.Id == gameModel.ArenaSessionId);
                var targetarena = context.ArenaSessions.Query().First(x => x.Id == arenaModel.Id);
                var game = context.Games.Query().First(x => x.Id == gameModel.Id);

                if (sourceArena.Games.Contains(game))
                {
                    sourceArena.Games.Remove(game);
                    if (game.Victory)
                    {
                        sourceArena.Wins--;
                    }
                    else
                    {
                        sourceArena.Losses--;
                    }
                }
                if (!(Equals(targetarena.Hero, game.Hero)))
                {
                    game.Hero = targetarena.Hero;
                }
                AddGameToArena(game, targetarena);
                SetEndDateIfNeeded(targetarena);

                await context.SaveChangesAsync();
                var latestId = context.ArenaSessions.OrderByDescending(x => x.StartDate).Select(x => x.Id).FirstOrDefault();
                events.PublishOnBackgroundThread(new ArenaSessionUpdated(sourceArena.Id, latestId == sourceArena.Id));
                events.PublishOnBackgroundThread(new ArenaSessionUpdated(targetarena.Id, latestId == targetarena.Id));
                events.PublishOnBackgroundThread(new GameResultUpdated(gameModel.Id, game.ArenaSessionId));
            }
        }
    }
}
