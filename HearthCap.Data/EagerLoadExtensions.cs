using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;

namespace HearthCap.Data
{
    public static class EagerLoadExtensions
    {
        private static readonly IDictionary<Type, MethodInfo> methodInfos = new Dictionary<Type, MethodInfo>();

        public static IQueryable<GameResult> Query(this DbSet<GameResult> set)
        {
            return set
                .Include(x => x.Hero)
                .Include(x => x.OpponentHero)
                .Include(x => x.ArenaSession)
                .Include(x => x.ArenaSession.Hero)
                .Include(x => x.ArenaSession.Image1)
                .Include(x => x.ArenaSession.Image2)
                .Include(x => x.Deck);
        }

        public static IQueryable<ArenaSession> Query(this DbSet<ArenaSession> set)
        {
            return set
                .Include(x => x.Hero)
                .Include(x => x.Games)
                .Include(x => x.Image1)
                .Include(x => x.Image2)
                .Include(x => x.Games.Select(g => g.ArenaSession))
                .Include(x => x.Games.Select(g => g.Hero))
                .Include(x => x.Games.Select(g => g.OpponentHero));
        }

        public static IQueryable<Deck> Query(this DbSet<Deck> set)
        {
            return set
                .Include(x => x.Image);
        }

        public static IQueryable<T> Query<T>(this DbSet<T> set) where T : class
        {
            MethodInfo methodInfo;
            if (methodInfos.ContainsKey(typeof(T)))
            {
                methodInfo = methodInfos[typeof(T)];
            }
            else
            {
                var argumentType = typeof(DbSet<>).MakeGenericType(typeof(T));
                methodInfo = typeof(EagerLoadExtensions).GetMethod(
                    "Query",
                    BindingFlags.Public | BindingFlags.Static,
                    null,
                    new[] { argumentType },
                    null);

                if (methodInfo != null)
                {
                    methodInfos[typeof(T)] = methodInfo;
                }
            }

            if (methodInfo != null)
            {
                return (IQueryable<T>)methodInfo.Invoke(null, new object[] { set });
            }

            // todo fail hard?
            return set;
        }
    }
}
