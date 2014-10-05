// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRepository.cs" company="">
//   
// </copyright>
// <summary>
//   The Repository interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    /// <summary>
    /// The Repository interface.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// The to list.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        List<T> ToList(Func<IQueryable<T>, IQueryable<T>> query);

        /// <summary>
        /// The to list async.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<List<T>> ToListAsync(Func<IQueryable<T>, IQueryable<T>> query);

        /// <summary>
        /// The first or default async.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> query);

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
        Task SaveOrUpdateAsync(T entity, object id);

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task Delete(object id);

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
        Task<TReturn> QueryAsync<TReturn>(Func<IQueryable<T>, Task<TReturn>> query);

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
        TReturn Query<TReturn>(Func<IQueryable<T>, TReturn> query);

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
        TReturn SqlQuery<TReturn>(string sql, params object[] parameters);

        /// <summary>
        /// The first or default.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        T FirstOrDefault(Expression<Func<T, bool>> query);
    }
}