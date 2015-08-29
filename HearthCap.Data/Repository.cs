using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace HearthCap.Data
{
    [Export(typeof(IRepository<>))]
    public class Repository<T> : IRepository<T>
        where T : class
    {
        private readonly Func<HearthStatsDbContext> dbContext;

        [ImportingConstructor]
        public Repository(Func<HearthStatsDbContext> dbContext)
        {
            this.dbContext = dbContext;
        }

        public List<T> ToList(Func<IQueryable<T>, IQueryable<T>> query)
        {
            using (var context = dbContext())
            {
                var q = context.Set<T>().Query();
                var result = query(q);
                return result.ToList();
            }
        }

        public async Task<List<T>> ToListAsync(Func<IQueryable<T>, IQueryable<T>> query)
        {
            using (var context = dbContext())
            {
                var q = context.Set<T>().Query();
                var result = query(q);
                return await result.ToListAsync();
            }
        }

        public TReturn Query<TReturn>(Func<IQueryable<T>, TReturn> query)
        {
            using (var context = dbContext())
            {
                var q = context.Set<T>().Query();
                // var q = context.Set<T>();
                return query(q);
            }
        }

        public async Task<TReturn> QueryAsync<TReturn>(Func<IQueryable<T>, Task<TReturn>> query)
        {
            using (var context = dbContext())
            {
                var q = context.Set<T>().Query();
                // var q = context.Set<T>();
                return await query(q);
            }
        }

        public TReturn SqlQuery<TReturn>(string sql, params object[] parameters)
        {
            using (var context = dbContext())
            {
                return context.Database.SqlQuery<TReturn>(sql, parameters).FirstOrDefault();
            }
        }

        public T FirstOrDefault(Expression<Func<T, bool>> query)
        {
            using (var context = dbContext())
            {
                return context.Set<T>().Query().FirstOrDefault(query);
            }
        }

        public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> query)
        {
            using (var context = dbContext())
            {
                return await context.Set<T>().Query().FirstOrDefaultAsync(query);
            }
        }

        public async Task SaveOrUpdateAsync(T entity, object id)
        {
            using (var context = dbContext())
            {
                var obj = context.Set<T>().Find(id);
                if (obj == null)
                {
                    context.Set<T>().Add(entity);
                }
                else
                {
                    context.Entry(obj).CurrentValues.SetValues(entity);
                }
                await context.SaveChangesAsync();
            }
        }

        public async Task Delete(object id)
        {
            using (var context = dbContext())
            {
                var obj = context.Set<T>().Find(id);
                if (obj != null)
                {
                    context.Set<T>().Remove(obj);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
