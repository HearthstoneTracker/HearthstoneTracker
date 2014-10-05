// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Repository.cs" company="">
//   
// </copyright>
// <summary>
//   The repository.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Data.Entity;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    /// <summary>
    /// The repository.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    [Export(typeof(IRepository<>))]
    public class Repository<T> : IRepository<T>
        where T : class
    {
        /// <summary>
        /// The db context.
        /// </summary>
        private readonly Func<HearthStatsDbContext> dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{T}"/> class.
        /// </summary>
        /// <param name="dbContext">
        /// The db context.
        /// </param>
        [ImportingConstructor]
        public Repository(Func<HearthStatsDbContext> dbContext)
        {
            this.dbContext = dbContext;
        }

        /// <summary>
        /// The to list.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<T> ToList(Func<IQueryable<T>, IQueryable<T>> query)
        {
            using (var context = this.dbContext())
            {
                var q = context.Set<T>().Query();
                var result = query(q);
                return result.ToList();
            }
        }

        /// <summary>
        /// The to list async.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<List<T>> ToListAsync(Func<IQueryable<T>, IQueryable<T>> query)
        {
            using (var context = this.dbContext())
            {
                var q = context.Set<T>().Query();
                var result = query(q);
                return await result.ToListAsync();
            }
        }

        /// <summary>
        /// The query.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <typeparam name="TReturn">
        /// </typeparam>
        /// <returns>
        /// The <see cref="TReturn"/>.
        /// </returns>
        public TReturn Query<TReturn>(Func<IQueryable<T>, TReturn> query)
        {
            using (var context = this.dbContext())
            {
                var q = context.Set<T>().Query();

                // var q = context.Set<T>();
                return query(q);
            }
        }

        /// <summary>
        /// The query async.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <typeparam name="TReturn">
        /// </typeparam>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<TReturn> QueryAsync<TReturn>(Func<IQueryable<T>, Task<TReturn>> query)
        {
            using (var context = this.dbContext())
            {
                var q = context.Set<T>().Query();

                // var q = context.Set<T>();
                return await query(q);
            }
        }

        /// <summary>
        /// The sql query.
        /// </summary>
        /// <param name="sql">
        /// The sql.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <typeparam name="TReturn">
        /// </typeparam>
        /// <returns>
        /// The <see cref="TReturn"/>.
        /// </returns>
        public TReturn SqlQuery<TReturn>(string sql, params object[] parameters)
        {
            using (var context = this.dbContext())
            {
                return context.Database.SqlQuery<TReturn>(sql, parameters).FirstOrDefault();
            }
        }

        /// <summary>
        /// The first or default.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public T FirstOrDefault(Expression<Func<T, bool>> query)
        {
            using (var context = this.dbContext())
            {
                return context.Set<T>().Query().FirstOrDefault(query);
            }
        }

        /// <summary>
        /// The first or default async.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> query)
        {
            using (var context = this.dbContext())
            {
                return await context.Set<T>().Query().FirstOrDefaultAsync(query);
            }
        }

        /// <summary>
        /// The save or update async.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task SaveOrUpdateAsync(T entity, object id)
        {
            using (var context = this.dbContext())
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

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task Delete(object id)
        {
            using (var context = this.dbContext())
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