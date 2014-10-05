// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueryResultExtensions.cs" company="">
//   
// </copyright>
// <summary>
//   The query result extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Framework
{
    using System.Linq;

    /// <summary>
    /// The query result extensions.
    /// </summary>
    public static class QueryResultExtensions
    {
        /// <summary>
        /// The as result.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        /// <returns>
        /// The <see cref="QueryResult"/>.
        /// </returns>
        public static QueryResult<TEntity> AsResult<TEntity>(this IQueryable<TEntity> query) where TEntity : class
        {
            return new QueryResult<TEntity>(query);
        }        
    }
}